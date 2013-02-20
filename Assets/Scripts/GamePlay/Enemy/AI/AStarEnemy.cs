using UnityEngine;
using System.Collections;

using Pathfinding;

/** A* Movement for Enemy
 *  -- Attached to Enemy object in hierarchy. Primarily use this object for the
 *  -- Enemy's movement based logic, and supply other logic inside EnemyBase
 */ 
public class AStarEnemy : MonoBehaviour {
	
	/** This A* Seeker script
	 *  Used to interface to A* singleton */
    private Seeker seeker;
	
	/** A* utility class for custom searches
	 *  tag based search in this case */
	private NNConstraint constraint = new NNConstraint();
	
	public GameObject enemyActual;
	
	private Transform thisTransform;
	private EnemyBase enemy;
	
	private bool isPathing = false;
	
    void Start () 
	{
        seeker = GetComponent<Seeker>(); 
		
		enemy = GetComponent<EnemyBase>();
		thisTransform = transform;
		
		constraint.tags = 1 << 2;
		constraint.constrainWalkability = true;
    }
	
	void OnEnable() 
	{ 
		TutorialGrid.onTutorialUpdate += onTutorialUpdate;
		EnemyBase.onEnemyStatsChange += onEnemyStatsChange;
		PlayerController.onCalibratingStop += onCalibratingStop;
		PlayerController.onUIUpdate += onUIUpdate;
	}
	
	void OnDisable() 
	{ 
		TutorialGrid.onTutorialUpdate -= onTutorialUpdate; 
		EnemyBase.onEnemyStatsChange -= onEnemyStatsChange;
		PlayerController.onCalibratingStop -= onCalibratingStop;
		PlayerController.onUIUpdate -= onUIUpdate;
	} 
	
	void onCalibratingStop( PlayerController c )
	{
		if( c.isCalibrating == false )
		{
			if( enemy != null )
			{
				if( Application.loadedLevel == 2 )
				{
					if( enemy.GetType() == typeof(EnemyBase) )
						StartCoroutine( randomPathing() );
					else
						return;
				}
			}
		}
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		if( state == PlayerController._GameState.Dead )
			StopAllCoroutines();
	}
	
	public IEnumerator randomPathing()
	{
        while (!DBAccess.instance.isDbInit) { yield return null; }

		float waitTime; 

		while(true)
		{
			isPathing = true;
			waitTime = Random.Range( 1f, 3f );
			
			if( Random.Range( 0f, 1f ) <= enemy.stats.hideRate )
			{
				//Debug.Log( "Enemy is headed to hide" );
				seeker.StartPath( 
					thisTransform.position, 
					findNearestHide( thisTransform.position ), 
					OnPathComplete
				);
			} else {
				seeker.StartPath( 
					thisTransform.position, 
					findRandomEndPoint( thisTransform.position ), 
					OnPathComplete 
				);
			}
			
			while( isPathing ) { yield return null; }
			
			iTween.LookTo( gameObject, iTween.Hash(
				"looktarget", Vector3.zero,
				"looktime", 0.1f,
				"time", 0.2f,
				"easetype", iTween.EaseType.linear
				)
			);
						
			yield return new WaitForSeconds( waitTime );
		}
	}
	
	/** Find Nearest Hiding Spot
	 * 
	 *  Recursively search to prevent returning present hiding spot 
	 * 
	 */
	public Vector3 findNearestHide( Vector3 startPos )
	{
		Vector3 returnPos;
		NNInfo info = AstarPath.active.GetNearest( startPos, constraint );
		returnPos = (Vector3)info.node.position;
		return Vector3.Distance( returnPos, startPos ) < 2 ? findNearestHide( findRandomEndPoint( startPos ) ) : returnPos;
	}
	
	/** Find random end point for path
	 * 
	 *  Rudimentary logic to prevent 'edging' at bounds
	 *  edging - returning a point too close to present position at boundaries 
	 * 
	 */
	public Vector3 findRandomEndPoint( Vector3 startPos )
	{
		float xMax = 15f;
		float xMin = -15f;
		float zMax = 12f;
		float zMin = 6f;
		
		Vector3 endPos = Vector3.up;
		
		if( startPos.z > zMax )			endPos.z = Random.Range( 0f, 10f );
		else if( startPos.z < zMin )	endPos.z = Random.Range( 7f, 15f );
		else 							endPos.z = Random.Range( 0f, 15f );
		
		if( startPos.x > xMax ) 		endPos.x = Random.Range( -17f, 13f );
		else if( startPos.x < xMin ) 	endPos.x = Random.Range( -13f, 17f );
		else 							endPos.x = Random.Range( -17f, 17f );

		return endPos;
	}
	
	/** When A* returns a path, then do
	 *  -- Apply iTween based movement once a walkable path is returned */
	public void OnPathComplete (Path p) 
	{
        //Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
        if (!p.error) 
		{
			enemyActual.animation.CrossFade( "Character_Run", 0.2f );
			
            iTween.MoveTo( gameObject, iTween.Hash
			( 
				"path", p.vectorPath, 
				"speed", 2, 
				"orienttopath", true,
				"looktime", 0.2f,
				"lookahead", 1f,
				"oncompletetarget", gameObject,
				"oncomplete", "stopPathing",
				"easetype", 
				iTween.EaseType.linear 
			) );
        }
    }

    void stopPathing() 
	{ 
		isPathing = false; 
		enemyActual.animation.CrossFade( "Character_Idle", 0.2f );
		
		if( DBAccess.instance.enemyStats != null )
			if( !DBAccess.instance.enemyStats.isTwoHanded )
				enemyActual.animation.CrossFade( "Character_THW", 0.2f );
	}
	
	void onTutorialUpdate( int curPos )
	{
		if( curPos == 10 )
		{
			seeker.StartPath(
				thisTransform.position, 
				new Vector3( 0, 0, 8.3f ),
				OnPathComplete
			);
			return;
		}
		if( curPos == 11 || curPos == 14)
		{
			Debug.Log("randompathing");
			StartCoroutine(randomPathing());
			return;
		}
	}
	
	void onEnemyStatsChange( EnemyStats stats, EnemyBase._EnemyState state )
	{
		if( stats.health == 0 && state == EnemyBase._EnemyState.Dead )
		{
			StopAllCoroutines();
		}
	}
} 