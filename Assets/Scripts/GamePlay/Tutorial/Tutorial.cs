using UnityEngine;
using System.Collections;

/** Tutorial Controller
 * 
 *  Basically a clone of PlayerController, consider extending
 *  said class instead of cloning if added logic can be injected as such.
 * 
 */ 
public class Tutorial : MonoBehaviour {
	
	[HideInInspector]
	// Cached off screen position for Panels, accomodates all aspect ratios
	public Vector3 leftOffScreenPos = new Vector3( -800f, 0f, 0f );
	
	[HideInInspector]
	public Vector3 rightOffScreenPos = new Vector3( 800f, 0f, 0f );
	
	public UIPanel calibratePanel;
	public UIPanel tutorialPanel;
	
	public TutorialGrid grid;
	
	private Matrix4x4 calibrationMatrix;		// Cached calibration zero point for user device tilt
	
	public float sensitivityX = 10F;
    public float sensitivityY = 10F;
	
	public float rotationX = 0F;
    public float rotationY = 0F;
	
	public enum RotationAxes { None = 0, MouseXAndY, MouseX, MouseY }
    public RotationAxes axes = RotationAxes.None;

    Quaternion originalRotation;
	
	private float rotationMinY = -90F;			// Left to Right FOV Limit
	private float rotationMaxY = 90f;			
	
	private float rotationMinX = -35F;			// Up to Down FOV Limit
	private float rotationMaxX = 35F;
	
	private float speed = 6.0f; 				// Adjustable speed from Inspector in Unity Editor
	
	public Vector3 localRotation;				// Temp vector3 to apply to transform.eulerAngles
	
	public LayerMask rayCastmask;
	public LayerMask tutorialMask;
	
	public GameObject gunModel;
	
	public Camera UICam;
	
	public Camera UIcam3D;
	
	public int shotCount = 0;
	
	public CameraFade cameraFade;
	
	public AudioSource[] audioSources;
	
	public LayerMask UIMask3D;
	
	// Used to handle flow and when to allow touches / firing
	public enum _TutorialState
	{
		None,
		InTransition,
		NoControl,
		MissionOne,
		MissionTwo,
		MissionTwoSecond,
		MissionThree,
		MissionFour
	}
	public _TutorialState TutorialState = _TutorialState.InTransition;
	
	public delegate void OnPlayerHitEnemyHandler( GameObject hit );
	public static event OnPlayerHitEnemyHandler onHitByRayCast;
	
	public delegate void OnTutorialUpdateHandler( GameObject clicked, Gun gun );
	public static event OnTutorialUpdateHandler onTutorialUpdate;
	
	IEnumerator Start () 
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		originalRotation = transform.localRotation;
#elif UNITY_IPHONE || UNITY_ANDROID
		localRotation = transform.eulerAngles;
#endif		
		cameraFade.SetScreenOverlayColor( Color.black );
		cameraFade.StartFade( Color.clear, 2f );
		
		yield return new WaitForSeconds(2f);
		iTween.MoveTo( tutorialPanel.gameObject, iTween.Hash(
			"position", Vector3.zero,
			"islocal", true,
			"time", 0.4f,
			"delay", 0.2f,
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
		
		// Load gun
		DBAccess.instance.userPrefs.userGun.capacity = DBAccess.instance.userPrefs.userGun.compatibleAmmo[0].magSize;
	}
	
	void OnEnable() 
	{
		QuitSprite.onQuit += onQuit; 
		Gun.onReloadStart += onReloadStart;
	}
	
	void OnDisable() 
	{
		QuitSprite.onQuit -= onQuit; 
		Gun.onReloadStart -= onReloadStart;
	}
	
	void Update () 
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX = ClampAngle (rotationX, rotationMinY, rotationMaxY);
            rotationY = ClampAngle (rotationY, rotationMinX, rotationMaxX);

            Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = ClampAngle (rotationX, rotationMinY, rotationMaxY);

            Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
			
			// When User is told to Tilt Device Left or Right
			if( TutorialState == _TutorialState.MissionOne )
			{
				if( rotationX < -60 && grid.curPos < 5 )
				{
					axes = RotationAxes.None;
					grid.moveGrid(_TutorialState.None);
				}
				else if( rotationX > 60 && grid.curPos == 5 )
				{
					axes = RotationAxes.None;
					grid.moveGrid(_TutorialState.None);
				}
			}
        }
        else if( axes == RotationAxes.MouseY )
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = ClampAngle (rotationY, rotationMinX, rotationMaxX);

            Quaternion yQuaternion = Quaternion.AngleAxis (-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
		
#elif UNITY_IPHONE || UNITY_ANDROID
		if( axes == RotationAxes.MouseXAndY )
		{
			// Store movement into temporary Vector3 to be applied to transform
			localRotation.y -= Input.acceleration.y * speed;
			localRotation.x += FixAcceleration( Input.acceleration ).x * speed;
			
			// Clamp rotation within set field of view
			localRotation.y = ClampAngle( localRotation.y, rotationMinY, rotationMaxY );
			localRotation.x = ClampAngle( localRotation.x, rotationMinX, rotationMaxX );
			
			// Apply new cached rotation to transform rotation
			transform.localEulerAngles = localRotation;
		}
		else if( axes == RotationAxes.MouseX )
		{
			localRotation.y -= Input.acceleration.y * speed;
			localRotation.y = ClampAngle( localRotation.y, rotationMinY, rotationMaxY );

			transform.localEulerAngles = localRotation;
			
			// When User is told to Tilt Device Left or Right
			if( TutorialState == _TutorialState.MissionOne )
			{
				if( localRotation.y < -60 && grid.curPos < 5 )
				{
					axes = RotationAxes.None;
					grid.moveGrid(_TutorialState.None);
				}
				else if( localRotation.y > 60 && grid.curPos == 5 )
				{
					axes = RotationAxes.None;
					grid.moveGrid(_TutorialState.None);
				}
			}
		}
		else if( axes == RotationAxes.MouseY )
		{
			localRotation.x += FixAcceleration( Input.acceleration ).x * speed;
			localRotation.x = ClampAngle( localRotation.x, rotationMinX, rotationMaxX );
			
			transform.localEulerAngles = localRotation;
		}
#endif
		if( Input.GetMouseButtonUp(0) )
		{
			// When user is able to Shoot
			if( (int)TutorialState >= (int)_TutorialState.MissionTwo )
			{
				// If user hits ammo sprite, reload instead of shooting
				Ray ray = UIcam3D.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if( Physics.Raycast( ray, out hit, 100f, UIMask3D ) )
				{
					if( onTutorialUpdate != null )
						onTutorialUpdate( hit.collider.gameObject, DBAccess.instance.userPrefs.userGun );
					
					if( TutorialState == _TutorialState.MissionTwoSecond )
					{
						axes = RotationAxes.None;
						TutorialState = _TutorialState.InTransition;
						grid.moveGrid(_TutorialState.None);
					}
				}
				else if( DBAccess.instance.userPrefs.userGun.capacity > 0 )
				{
					// If not in reload or pause, shoot gun.  Gun will take care of bullets remaining logic.
					DBAccess.instance.userPrefs.userGun.Shoot(transform,rayCastmask,gunModel);
					
					onTutorialUpdate( null, DBAccess.instance.userPrefs.userGun );
					
					BroadcastMessage( "applyRecoil" );
					
					audioSources[0].Play();
				}
				else
					audioSources[3].Play();
				
				// When user is told to Shoot Anywhere
				if( TutorialState == _TutorialState.MissionTwo )
				{
					Debug.Log( DBAccess.instance.userPrefs.userGun.capacity );
					if( shotCount < 4 )
						shotCount++;
					else
					{
						TutorialState = _TutorialState.None;
						axes = RotationAxes.None;
						grid.moveGrid(_TutorialState.None);
					}
				}
			}
			// When user needs to click on the Tutorial Window
			else if( TutorialState != _TutorialState.InTransition )
			{
				RaycastHit hit;
				if( Physics.Raycast(transform.position,UICam.transform.forward,out hit,100f,tutorialMask) )
				{
					if( TutorialState != _TutorialState.MissionOne )
					{
						grid.moveGrid(TutorialState);
						TutorialState = _TutorialState.InTransition;
					}
					if( hit.collider.name == "WindowBG(Calibrate)" )
					{
						CalibrateAccelerometer();
					}
				}
			}
			
			Ray ray2 = UIcam3D.camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit2;
			
			if( Physics.Raycast( ray2, out hit2, 100f ) )
				hit2.collider.SendMessage( "onHitByRayCast", hit2.collider.gameObject, SendMessageOptions.DontRequireReceiver );
		}
	}
	
	// Method for when hit by Enemy bullet or projectile
    private void hitByEnemy()
    {
        //Debug.Log("Player: OUCH, I GOT HIT");

        iTween.ShakePosition(gameObject, iTween.Hash(
            "amount", new Vector3( 0.4f, 0.4f, 0.4f ),
            "time", 0.5f,
            "oncompletetarget", gameObject,
            "oncomplete", "resetPos"
            ));
		
		audioSources[Random.Range(1,2)].Play();
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
	
	void onQuit()
	{
		cameraFade.SetScreenOverlayColor( Color.clear );
		cameraFade.StartFade( Color.black, 2f );
		
		DBAccess.instance.userPrefs.playCount++;
		DBAccess.instance.userPrefs.CommitToDB(false);
		
		StartCoroutine( cameraFadeAndChangeLevel() );
	}
	
	void onReloadStart(float timeToReload, Gun gun)
	{
		audioSources[4].Play();
	}
	
	IEnumerator cameraFadeAndChangeLevel()
	{
		yield return new WaitForSeconds(2f);
		Application.LoadLevel (0);
	}
	
	public void changeLevelInTutorial()
	{
		cameraFade.SetScreenOverlayColor( Color.clear );
		cameraFade.StartFade( Color.black, 2f );
		
		DBAccess.instance.userPrefs.playCount++;
		DBAccess.instance.userPrefs.CommitToDB(false);
		
		StartCoroutine( cameraFadeAndChangeLevel() );
	}
}
