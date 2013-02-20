using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** Enemy controller script
 *  -- Primarily used for non-AI based movement behaviors such as shooting
 *  -- routines that are not in direct relation to the use of A*.  Put any additional
 *  -- behaviours for the enemy in here and leave AStarEnemy for movement based
 *  -- tracking 
 */ 
public class EnemyBase : MonoBehaviour {
	
	// Enemy info object populated by DB
    public EnemyStats stats;
	
	public int hitCount = 0;
	
	public Material mat;

    public GUIStyle tempStyle;
	
	/** Gameobject's that need to be cached for animation calls.  These
	 *  objects can be found in the same hierarchy / children of this GameObject,
	 *  as of this point named simply Enemy
	 */ 
	public GameObject enemyActual;
	public GameObject gunActual;
	
	private const string GUN_NODE_TRANSFORM_PATH		= "/Bone/Bone Pelvis/Bone Spine/Bone R Clavicle/Bone R UpperArm/Bone R Forearm/Bone R Hand/R Hand/";
	
	public enum _EnemyState { None, Alive, Dead }
	public _EnemyState EnemyState = _EnemyState.Alive;
	
	public delegate void OnEnemyStatsChangeHandler ( EnemyStats stats, _EnemyState state );
	public static event OnEnemyStatsChangeHandler onEnemyStatsChange;
	
	// Set DB ready to false to allow objects time to read from DB
	public virtual void Awake() { DBAccess.instance.isDbInit = false; }
	
    public virtual void Start()
    {
		if( Application.loadedLevel == 1 )
		{
			stats = new EnemyStats(DBAccess.instance.getEnemyStats(1));
		}
		
		if( Application.loadedLevel == 2 )
		{
			stats = DBAccess.instance.enemyStats;

			transform.position = new Vector3( 0, 0, 13 );
			transform.eulerAngles = new Vector3( 0, 180, 0 );
			enemyActual.animation.Play("Character_Idle");
			
			if( DBAccess.instance.enemyStats.isTwoHanded )
				enemyActual.animation.Blend("Character_THW");	
		}
	}
	
	public virtual void OnEnable() 
	{ 
		Gun.onHitByRayCast += hitByRayCast;
		
		PlayerController.onCalibratingStop += onCalibratingStop;
		PlayerController.onUIUpdate += onUIUpdate;
		
		if( Application.loadedLevel == 1 )
		{
			Tutorial.onHitByRayCast += hitByRayCast;
			TutorialGrid.onTutorialUpdate += onTutorialUpdate;
		}
	}
	
	public virtual void OnDisable()
	{
		Gun.onHitByRayCast -= hitByRayCast;
		
		PlayerController.onCalibratingStop -= onCalibratingStop;
		PlayerController.onUIUpdate -= onUIUpdate;
		
		if( Application.loadedLevel == 1 )
		{
			Tutorial.onHitByRayCast -= hitByRayCast;
			TutorialGrid.onTutorialUpdate -= onTutorialUpdate;
		}
		
		// Clean up all coroutines just in case
		StopAllCoroutines();
	}
	
	void onCalibratingStop( PlayerController c )
	{
		if( c.isCalibrating == false )
		{
			StartCoroutine(canShootRoutine());
		}
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		if( state == PlayerController._GameState.Dead )
			StopAllCoroutines();
	}
	
	/** Start shooting in Cluster of shots
	 *  -- CoRoutine to have the enemy shoot in a set amount of shots, in a set interval.  Clusters of shots
	 *  -- are seperated by CoRoutine canShootRoutine() by yielding return of this routine. 
	 */
    public IEnumerator StartClusterShot()
    {
        //Debug.Log("ENEMY: ShootAtPlayer");
		
		// For how long the cluster of shots is to be
		float clusterTime = Random.Range(1f,4f);
		
		// For when the Time.time offset for stopping the cluster shots should be
		float endTime = Time.time + clusterTime;
		
		// For how long the enemy should wait in between shots during clusterTime
		float shootTime = Time.time + 0.4f;
		
		Vector3 playerPos = new Vector3(0f,1.5f,0f);
		
		// While in clusterTime...
		while(Time.time < endTime)
		{
			float rightAngle = Vector3.Angle( transform.right, ( Vector3.zero - transform.position ) );
			float lookAngle = Vector3.Angle( transform.forward, ( Vector3.zero - transform.position ) );
			
			/** Logic for which way the animation should turn upper torso to shoot while lower body is
			 *  oriented to the path he is running. */
			if( rightAngle < 90 )
			{
				if( !stats.isTwoHanded )
				{
					enemyActual.animation.Blend( "Character_Aim0", 1f, 0.2f );
					enemyActual.animation.Blend( "Character_Aim180", 0f, 0.2f);
					enemyActual.animation.Blend( "Character_Aim-180", ( lookAngle / 180 ), 0.2f);
				} else {
					enemyActual.animation.Blend( "Character_THW_Aim0", 1f, 0.2f );
					enemyActual.animation.Blend( "Character_THW_Aim180", 0f, 0.2f );
					enemyActual.animation.Blend( "Character_THW_Aim-180", ( lookAngle / 180 ), 0.2f );
				}
			} else {
				if( !stats.isTwoHanded )
				{
					enemyActual.animation.Blend( "Character_Aim0", 1f, 0.2f );
					enemyActual.animation.Blend( "Character_Aim-180", 0f, 0.2f );
					enemyActual.animation.Blend( "Character_Aim180", ( lookAngle / 180 ), 0.2f );
				} else {
					enemyActual.animation.Blend( "Character_THW_Aim0", 1f, 0.2f );
					enemyActual.animation.Blend( "Character_THW_Aim-180", 0f, 0.2f );
					enemyActual.animation.Blend( "Character_THW_Aim180", (lookAngle / 180 ), 0.2f );
				}
			}
			
			// Is it time to shoot?
			if( Time.time > shootTime )
			{
				RaycastHit hit;
		        if (Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, 100f))
		        {	
		            if (hit.collider.name == "Player")
		            {
						gunActual.animation.Play ("Shoot");
						audio.Play();
						
						/** Logic for the upper torso of animations for the Enemy, similar to above string
						 *  of animations.  EXPAND ON THIS LOGIC TO FIX ANIMATION HICCUPS DURING FIRING
						 *  AND ORIENTING OF PATH.
						 */
						if( rightAngle < 90 )
						{
							if( !stats.isTwoHanded )
							{
								enemyActual.animation.Blend("Character_Aim180_Shoot",0f,0f);
								enemyActual.animation.Blend("Character_Aim-180_Shoot",(lookAngle / 180),0f);
								enemyActual.animation.Blend("Character_Aim0_Shoot",(1 - (lookAngle / 180)), 0f);
							} else {
								enemyActual.animation.Blend("Character_THW_Aim180_Shoot",0f,0f);
								enemyActual.animation.Blend("Character_THW_Aim-180_Shoot",(lookAngle / 180),0f);
								enemyActual.animation.Blend("Character_THW_Aim0_Shoot",(1 - (lookAngle / 180)), 0f);
							}
						} else {
							if( !stats.isTwoHanded )
							{
								enemyActual.animation.Blend("Character_Aim-180_Shoot",0f,0f);
								enemyActual.animation.Blend("Character_Aim180_Shoot",(lookAngle / 180), 0f);
								enemyActual.animation.Blend("Character_Aim0_Shoot",(1 - (lookAngle / 180)), 0f);
							} else {
								enemyActual.animation.Blend("Character_THW_Aim-180_Shoot",0f,0f);
								enemyActual.animation.Blend("Character_THW_Aim180_Shoot",(lookAngle / 180), 0f);
								enemyActual.animation.Blend("Character_THW_Aim0_Shoot",(1 - (lookAngle / 180)), 0f);
							}
						}
						shootTime = Time.time + 0.4f;
						
						//Debug.Log( hit.collider.name );
		                if (Random.Range(0f, 1f) <= stats.hitRate)
		                {	
		                    //Debug.Log("ENEMY: I Hit the Player");
		                    hit.collider.SendMessage("hitByEnemy", stats, SendMessageOptions.DontRequireReceiver);
		                }//else 
							//Debug.Log("ENEMY: I MISSED DAMNIT!");
		            }//else
		                //Debug.Log( "ENEMY: I hit an obstacle" );
		        }
			}
			// null return required within while loop to prevent CoRoutine crashes
			yield return null;
		}
		
		// If clusterTime is over, reset the upper torso to orient to path or orient to player
		if( !stats.isTwoHanded )
		{
			enemyActual.animation.Blend( "Character_Aim0", 0f, 0.3f );
			enemyActual.animation.Blend( "Character_Aim180", 0f, 0.3f);
			enemyActual.animation.Blend( "Character_Aim-180", 0f, 0.3f);
		} else {
			enemyActual.animation.Blend( "Character_THW_Aim0", 0f, 0.3f );
			enemyActual.animation.Blend( "Character_THW_Aim180", 0f, 0.3f );
			enemyActual.animation.Blend( "Character_THW_Aim-180", 0f, 0.3f );
		}
		
		yield return null;
    }

    void hitByRayCast(GameObject hit) 
	{ 
		//Debug.Log("hitByRayCast");
		
		if( hit.layer == gameObject.layer )
		{
			StartCoroutine(damagedAnim()); 
			
			if( Application.loadedLevel == 2 )
				stats.health -= DBAccess.instance.userPrefs.userGun.modifiedPower;
			
			if( stats.health > 0 )
			{				
				if( onEnemyStatsChange != null )
					onEnemyStatsChange(stats, EnemyState);
			} else {
				if( Application.platform == RuntimePlatform.IPhonePlayer )
					ChartBoostBinding.trackEvent( "Player Win" );
				
				EnemyState = _EnemyState.Dead;
				
				StopAllCoroutines();
				
				mat.color = Color.white;
				
				iTween.Stop(gameObject);
				iTween.Stop(enemyActual);
				
				enemyActual.animation.Stop();
				enemyActual.animation.Play("Character_Death");
				
				if( onEnemyStatsChange != null )
					onEnemyStatsChange(stats, EnemyState);
				
				GameObject.Find("Crosshair").GetComponent<UISprite>().enabled = false;
			}
			
			// If in Tutorial Level
			if( Application.loadedLevel == 1 )
			{
				// Reset state of tutorial to progress forward
				Tutorial t = GameObject.Find("Player").GetComponent<Tutorial>();
				
				if( hitCount < 4 && (int)t.TutorialState < (int)Tutorial._TutorialState.MissionFour )
					hitCount++;
				else if( t.TutorialState == Tutorial._TutorialState.MissionFour )
				{
					hitCount++;
					
					stats.health -= 10;
					
					if( stats.health > 0 )
					{				
						if( onEnemyStatsChange != null )
							onEnemyStatsChange(stats, EnemyState);
					} else {
						EnemyState = _EnemyState.Dead;
						
						StopAllCoroutines();
						
						mat.color = Color.white;
						
						iTween.Stop(gameObject);
						iTween.Stop(enemyActual);
						
						enemyActual.animation.Stop();
						enemyActual.animation.Play("Character_Death");
						
						TutorialGrid tg = GameObject.FindObjectOfType( typeof( TutorialGrid ) ) as TutorialGrid;
						tg.moveGrid(Tutorial._TutorialState.None);
						
						if( onEnemyStatsChange != null )
							onEnemyStatsChange(stats, EnemyState);
					}
				}
				else
				{
					// Reset hitCount for reuse through tutorial
					hitCount = 0;
					
					t.axes = Tutorial.RotationAxes.None;
					t.grid.moveGrid(Tutorial._TutorialState.None);
					
					// Stop AI pathing
					GetComponent<AStarEnemy>().StopAllCoroutines();
				}
			}
		}
	}
	
	/** Wait time for seperation of clustered shots dependent on values taken from the
	 *  DB for the specific level of the enemy character. */
    IEnumerator canShootRoutine()
    {
        //Debug.Log("Starting shoot routine");
		// While this script is true / existing
		
        while (true)
        {
            float waitTime = Random.Range( stats.minShotRate, stats.maxShotRate );
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine( StartClusterShot() );
        }
    }

	// Temporary animation for when the enemy takes damage
    IEnumerator damagedAnim()
    {
		//Debug.Log("ENEMY: I GOT HIT!");
		for( int i = 0; i < 10; i++ )
		{
			mat.color = Color.red;
			yield return new WaitForSeconds(0.025f);
			mat.color = Color.white;
			yield return new WaitForSeconds(0.025f);
		}
    }
	
	// Used in Tutorial Mission Four for final battle to start enemy shooting routine
	void onTutorialUpdate(int curPos) { if( curPos == 14 ) StartCoroutine(canShootRoutine()); }
}
