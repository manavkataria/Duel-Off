using UnityEngine;
using System.Collections;

/** Player Controls / Base
 * -- Consider renaming file
 * 
 * -- Attempted to use Preprocessor directives to minimize variable usage
 * -- between PC and Mobile, but iOS was complaining about SIGBRT issues
 * -- and not being able to correctly compile this script.
 */ 
public class PlayerController : MonoBehaviour {
	
	private Matrix4x4 calibrationMatrix;		// Cached calibration zero point for user device tilt

	private bool _isCalibrating	= true;			// Wait for user to hold device steady
	
	public bool isCalibrating
	{
		get { return _isCalibrating; }
		set 
		{
			if( value == false )
			{
				_isCalibrating = false;
				onCalibratingStop(this);
			}
			else
				_isCalibrating = true;
		}
	}
	
	public float sensitivityX = 10F;
    public float sensitivityY = 10F;
	
	float rotationX = 0F;
    float rotationY = 0F;
	
	public enum RotationAxes { None, MouseXAndY = 0, MouseX, MouseY }
    public RotationAxes axes = RotationAxes.MouseXAndY;
	private RotationAxes _cachedAxes = RotationAxes.MouseXAndY;
	
	public enum _GameState { None, Reload, IsReloading, Paused, Active, Dead, Results }
	public _GameState GameState = _GameState.Active;
	private _GameState _cachedGameState = _GameState.Active;

    Quaternion originalRotation;
	
	private const float ROTATION_MIN_Y = -90F;			// Left to Right FOV Limit
	private const float ROTATION_MAX_Y = 90f;			
	
	private const float ROTATION_MIN_X = -45F;			// Up to Down FOV Limit
	private const float ROTATION_MAX_X = 45F;
	
	private float speed = 8f; 				// Adjustable speed from Inspector in Unity Editor
	
	private Vector3 localRotation;				// Temp vector3 to apply to transform.eulerAngles
	
	public LayerMask rayCastmask;
	
	public LayerMask capsulCastMask;
	
	public GameObject gunModel;
	
	public BloodSplatter splatCam;
	
	public UICamera UICam;
	
	public UICamera UICam2D;
	
	public LayerMask UIMask3D;
	
	public CameraFade[] cameraFades;
	
	public GameObject cameraAnimNode;
	
	public AudioSource[] audioSources;
	
	private GameObject lastAmmoHit;
	
	public UISprite crosshair;
	
	public delegate void CalibrationHandler ( PlayerController c );
	public static event CalibrationHandler onCalibratingStop;
	
	public delegate void UIUpdateHandler( Gun gun, _GameState gameState, GameObject clicked, EnemyStats stats );
	public static event UIUpdateHandler onUIUpdate;
	
	public delegate void OnHitByEnemyHandler( float dmgAmount );
	public static event OnHitByEnemyHandler onHitByEnemy;
		
	IEnumerator Start () 
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		originalRotation = transform.localRotation;
#elif UNITY_IPHONE || UNITY_ANDROID
		localRotation = transform.eulerAngles;
#endif
		
		if( DBAccess.instance.userPrefs.isMusicOn )
		{
			int randRange = Random.Range(0,2);
			
			if( randRange == 0 )
				audioSources[10].Play();
			else
				audioSources[11].Play();
		}
		
		if( DBAccess.instance.userPrefs.userGun.model == "Winchester_1912" )
		{
			crosshair.transform.localScale = new Vector3( 50f, 50f, 1f );
			crosshair.spriteName = "shotgunCrosshair";
		}
		
		for( int i = 0; i < 3; i++ )
		{
			cameraFades[i].enabled = true;
			cameraFades[i].SetScreenOverlayColor( Color.black );
			cameraFades[i].StartFade( Color.clear, 2f );
		}

		// Set ammo capacity of gun before gameplay start
		DBAccess.instance.userPrefs.userGun.capacity = DBAccess.instance.userPrefs.userGun.compatibleAmmo[0].magSize;
		
		// Load user picked gun model and set it's position to Player
		Debug.Log(DBAccess.instance.userPrefs.userGun.model );
		gunModel = (GameObject)Instantiate( Resources.Load("Models/Guns/" + DBAccess.instance.userPrefs.userGun.model + "/Prefab/" + DBAccess.instance.userPrefs.userGun.model) );
		gunModel.transform.parent = Camera.mainCamera.transform;
		gunModel = gunModel.transform.FindChild(DBAccess.instance.userPrefs.userGun.model).gameObject;
		
		// Set recoil force
		GetComponentInChildren<CameraRecoil>().recoilForce = DBAccess.instance.userPrefs.userGun.recoil;
		
		DBAccess.instance.userPrefs.health = 100f;
		DBAccess.instance.userPrefs.triggerHealthPackUpdate();
	
		if( onUIUpdate != null )
			onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
		
		yield return new WaitForSeconds(2f);
		
		for( int i = 0; i < 3; i++ )
			cameraFades[i].enabled = false;
	}
	
	void OnEnable() 
	{ 
		Gun.onReloadStart += onReloadStart;
		EnemyBase.onEnemyStatsChange += onEnemyStatsChange;
		QuitSprite.onQuit += onQuit;
		PauseSprite.onPause += onPause;
	}
	
	void OnDisable() 
	{ 
		Gun.onReloadStart -= onReloadStart; 
		EnemyBase.onEnemyStatsChange -= onEnemyStatsChange;
		QuitSprite.onQuit -= onQuit;
		PauseSprite.onPause -= onPause;
		
		if( DBAccess.instance.userPrefs != null )
			DBAccess.instance.userPrefs.CommitToDB(false);
	}
	
	void Update () 
	{
		if( isCalibrating )
		{
			if( Input.GetMouseButtonUp(0) )
			{
				Ray ray = UICam2D.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if( Physics.Raycast( ray, out hit, 100f ) )
				{
					if( hit.collider.name == "WindowBG(Calibrate)" )
					{
						hit.collider.SendMessage( "onHitByRayCast", this, SendMessageOptions.DontRequireReceiver );
						CalibrateAccelerometer();
					}
				}
			}
		}
		if( !isCalibrating )
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			if( GameState != _GameState.Dead && GameState != _GameState.Results && GameState != _GameState.None && GameState != _GameState.Paused)
			{
		        if (axes == RotationAxes.MouseXAndY)
		        {
		            // Read the mouse input axis
		            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		
		            rotationX = ClampAngle (rotationX, ROTATION_MIN_Y, ROTATION_MAX_Y);
		            rotationY = ClampAngle (rotationY, ROTATION_MIN_X, ROTATION_MAX_X);
		
		            Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		            Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
		
		            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		        }
		        else if (axes == RotationAxes.MouseX)
		        {
		            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		            rotationX = ClampAngle (rotationX, ROTATION_MIN_Y, ROTATION_MAX_Y);
		
		            Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		            transform.localRotation = originalRotation * xQuaternion;
		        }
		        else
		        {
		            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		            rotationY = ClampAngle (rotationY, ROTATION_MIN_X, ROTATION_MAX_X);
		
		            Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
		            transform.localRotation = originalRotation * yQuaternion;
		        }
				if( rotationY < -25f && GameState != _GameState.Reload && GameState != _GameState.IsReloading )
				{
					GameState = _GameState.Reload;
					
					lastAmmoHit = null;
					
					if( onUIUpdate != null )
						onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
					
					gunModel.animation.Stop();
					gunModel.animation.Play("ReloadStart");
				}
				if( rotationY > -25f && ( GameState == _GameState.Reload || GameState == _GameState.IsReloading ) )
				{
					if( !DBAccess.instance.userPrefs.userGun.isReloading )
					{
						GameState = _GameState.Active;
						
						if( onUIUpdate != null )
							onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );

						if( lastAmmoHit == null )
							gunModel.animation.Play("ReloadEnd");
						else if( lastAmmoHit != null && lastAmmoHit.name != "ActiveAmmo" )
							gunModel.animation.Play("ReloadEnd");
					} else {
						if( onUIUpdate != null )
							onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
					}
				}
			}
#elif UNITY_IPHONE || UNITY_ANDROID
			if( GameState != _GameState.Dead && GameState != _GameState.Results && GameState != _GameState.None && GameState != _GameState.Paused )
			{
				if( axes == RotationAxes.MouseXAndY )
				{
					// Store movement into temporary Vector3 to be applied to transform
					localRotation.y -= Input.acceleration.y * speed;
					localRotation.x += FixAcceleration( Input.acceleration ).x * speed;
					
					// Clamp rotation within set field of view
					localRotation.y = ClampAngle( localRotation.y, ROTATION_MIN_Y, ROTATION_MAX_Y );
					localRotation.x = ClampAngle( localRotation.x, ROTATION_MIN_X, ROTATION_MAX_X );
					
					//Debug.Log( "Y : " + localRotation.y );
					//Debug.Log( "X : " + localRotation.x );
					
					// Apply new cached rotation to transform rotation
					transform.localEulerAngles = localRotation;
				}
				else if( axes == RotationAxes.MouseX )
				{
					localRotation.y -= Input.acceleration.y * speed;
					localRotation.y = ClampAngle( localRotation.y, ROTATION_MIN_Y, ROTATION_MAX_Y );
		
					transform.localEulerAngles = localRotation;
				}
				else if( axes == RotationAxes.MouseY )
				{
					localRotation.x += FixAcceleration( Input.acceleration ).x * speed;
					localRotation.x = ClampAngle( localRotation.x, ROTATION_MIN_X, ROTATION_MAX_X );
					
					transform.localEulerAngles = localRotation;
				}
				if( localRotation.x > 25f && GameState != _GameState.Reload && GameState != _GameState.IsReloading )
				{
					GameState = _GameState.Reload;
					
					lastAmmoHit = null;
					
					if( onUIUpdate != null )
						onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
					
					gunModel.animation.Play("ReloadStart");
				}
				if( localRotation.x < 25f && ( GameState == _GameState.Reload || GameState == _GameState.IsReloading ) )
				{
					if( !DBAccess.instance.userPrefs.userGun.isReloading )
					{
						GameState = _GameState.Active;
						
						if( onUIUpdate != null )
							onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
						
						if( lastAmmoHit == null )
							gunModel.animation.Play("ReloadEnd");
						else if( lastAmmoHit != null && lastAmmoHit.name != "ActiveAmmo" )
							gunModel.animation.Play("ReloadEnd");
					} else {
						if( onUIUpdate != null )
							onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, null );
					}
				}
			}
#endif
			if( Input.GetMouseButtonUp(0) )
			{
				if( GameState == _GameState.Active || GameState == _GameState.Paused )
				{	
					// If user hits ammo sprite, reload instead of shooting
					Ray ray = UICam.camera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if( Physics.Raycast( ray, out hit, 100f, UIMask3D ) )
					{
						onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, hit.collider.gameObject, null );
						lastAmmoHit = hit.collider.gameObject;
					}
					else if( DBAccess.instance.userPrefs.userGun.capacity > 0 && GameState == _GameState.Active )
					{
						// If not in reload or pause, shoot gun.  Gun will take care of bullets remaining logic.
						if( DBAccess.instance.userPrefs.userGun.model == "Winchester_1912")
							DBAccess.instance.userPrefs.userGun.ShootShotgun( transform, capsulCastMask, gunModel );
						else
							DBAccess.instance.userPrefs.userGun.Shoot(transform,rayCastmask,gunModel);
						
						onUIUpdate(DBAccess.instance.userPrefs.userGun, GameState, null, null);
						
						BroadcastMessage( "applyRecoil" );
						
						if( DBAccess.instance.userPrefs.userGun.gunID == 1 )
							audioSources[0].Play();
						if( DBAccess.instance.userPrefs.userGun.gunID == 2 )
							audioSources[1].Play();
						if( DBAccess.instance.userPrefs.userGun.gunID == 3 )
							audioSources[2].Play();
					}
					else if( GameState == _GameState.Active )
						audioSources[6].Play();
				}
				if( GameState == _GameState.Reload || GameState == _GameState.Results || GameState == _GameState.Dead || GameState == _GameState.None )
				{
					if( Application.platform == RuntimePlatform.IPhonePlayer )
						ChartBoostBinding.trackEvent( "Secondary Reload Screen Use" );
					
					Ray ray = UICam.camera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if( Physics.Raycast( ray, out hit, 100f, UIMask3D ) )
					{
						onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, hit.collider.gameObject, null );
						lastAmmoHit = hit.collider.gameObject;
					}
				}
				if( GameState == _GameState.Dead || GameState == _GameState.Results )
				{
					Ray ray = UICam2D.camera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					if( Physics.Raycast( ray, out hit, 100f ) )
					{
						onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, hit.collider.gameObject, null );
					}
				}
			}
		}
	}

    // Method for when hit by Enemy bullet or projectile
    private void hitByEnemy( EnemyStats stats )
    {
        //Debug.Log("Player: OUCH, I GOT HIT");

        iTween.ShakePosition(gameObject, iTween.Hash(
            "amount", new Vector3( 0.4f, 0.4f, 0.4f ),
            "time", 0.5f,
            "oncompletetarget", gameObject,
            "oncomplete", "resetPos"
            ));
		
		DBAccess.instance.userPrefs.health -= stats.damage;
		
		audioSources[Random.Range(3,5)].Play();
		
		if( DBAccess.instance.userPrefs.health > 0 )
		{
			if( onHitByEnemy != null )
				onHitByEnemy( DBAccess.instance.userPrefs.health );
		}
		else
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "Player Loss" );
			
			if( onHitByEnemy != null )
				onHitByEnemy( DBAccess.instance.userPrefs.health );
			
			GameState = _GameState.Dead;
			
			cameraAnimNode.animation.Play("CameraDeath");
			
			// Death Sound
			audioSources[7].Play();
			
			// Stop Heartbeat
			audioSources[9].Stop();
			
			// Fade Music
			if( audioSources[10].isPlaying )
				iTween.AudioTo( gameObject, iTween.Hash(
					"audiosource", audioSources[10],
					"volume", 0f,
					"time", 1f,
					"easetype", iTween.EaseType.linear
					)
				);
			
			if( audioSources[11].isPlaying )
				iTween.AudioTo( gameObject, iTween.Hash(
					"audiosource", audioSources[11],
					"volume", 0,
					"time", 1f,
					"easetype", iTween.EaseType.linear
					)
				);
			
			GameObject.Find("Crosshair").GetComponent<UISprite>().enabled = false;
			
			StartCoroutine( deathRoutine() );
						
			onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, stats );
		}
		
		int randRange = Random.Range( 0, 101 );
		
		if( randRange < 33 )
			splatCam.onHitByRayCast();
    }
	
	void onEnemyStatsChange( EnemyStats stats, EnemyBase._EnemyState state )
	{
		if( state == EnemyBase._EnemyState.Dead )
		{
			GameState = _GameState.Results;
			
			onUIUpdate( DBAccess.instance.userPrefs.userGun, GameState, null, stats );
			
			DBAccess.instance.userPrefs.wins++;
			
			if( GameCenterBinding.isPlayerAuthenticated() )
				GameCenterBinding.reportScore( (long)DBAccess.instance.userPrefs.wins, "Leaderboard_Win");
		}
	}
	
	void onReloadStart( float timeToReload, Gun gun ) 
	{ 
		GameState = _GameState.IsReloading;
		if( DBAccess.instance.userPrefs.userGun.model != "Winchester_1912")
			audioSources[5].Play();
		else 
			audioSources[12].Play();
	}

    void resetPos() { transform.position = new Vector3(0f, 1.5f, 0f); }
	
	/** Calibrate the accelerometer
	 *	Used to calibrate the initial Input.acceleration */
	private void CalibrateAccelerometer() 
	{
		Vector3 accelerationSnapshot = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation( new Vector3( 0, 0, -1f ), accelerationSnapshot );
		
		// Create identity matrix - rotate our matrix to match with down vector
		Matrix4x4 matrix = Matrix4x4.TRS( Vector3.zero, rotateQuaternion, Vector3.one );
		
		calibrationMatrix = matrix.inverse;
	}
	
	// Get the calibrated value from User Input
	private Vector3 FixAcceleration( Vector3 accelerator )
	{
		Vector3 fixedAcceleration = calibrationMatrix.MultiplyVector(accelerator);
		return fixedAcceleration;
	}
	
    public static float ClampAngle (float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp (angle, min, max);
    }
	
	void onQuit() {
		UnityEngine.Debug.Log("MK - PlayerController.cs onQuit()");			
		StartCoroutine( fadeToMainMenu() ); 
	}
	
	void onPause()
	{
		if( Time.timeScale == 1 )
		{
			_cachedAxes = axes;
			_cachedGameState = GameState;
			
			axes = RotationAxes.None;
			GameState = _GameState.Paused;
			
			audioSources[10].mute = true;
			audioSources[11].mute = true;
			
			Time.timeScale = 0;
			
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "On Pause Event" );
			
		} else {
			axes = _cachedAxes;
			GameState = _cachedGameState;
			
			audioSources[10].mute = false;
			audioSources[11].mute = false;
			
			Time.timeScale = 1;
		}
	}
	
	IEnumerator fadeToMainMenu()
	{
		for( int i = 0; i < 3; i++ )
		{
			cameraFades[i].enabled = true;
			cameraFades[i].SetScreenOverlayColor( Color.clear );
			cameraFades[i].StartFade( Color.black, 2f );
		}
		yield return new WaitForSeconds(2f);
		Application.LoadLevel(0);
	}
	
	IEnumerator deathRoutine()
	{
		yield return new WaitForSeconds( cameraAnimNode.animation["CameraDeath"].length );
	
		cameraFades[0].enabled = true;
		cameraFades[0].SetScreenOverlayColor( Color.clear );
		cameraFades[0].StartFade( new Color( 0.725f, 0f, 0f, 1f ), 2f );
		
		yield return new WaitForSeconds(2f);
		
		cameraFades[0].enabled = false;
		
		DBAccess.instance.userPrefs.losses++;
		
		if( GameCenterBinding.isPlayerAuthenticated() )
			GameCenterBinding.reportScore( (long)DBAccess.instance.userPrefs.losses, "Leaderboard_Loss");
	}
}
