using UnityEngine;
using System.Collections;

public class EnemySelectPopulator : MonoBehaviour {

	public GameObject[] enemyGroupings;
	
	private const string ENEMY_RESOURCES_PATH 			= "Models/Character/Prefab/";
	private const string GUN_RESOURCES_PATH				= "Models/Guns/";
	private const string GUN_NODE_TRANSFORM_PATH		= "Bone/Bone Pelvis/Bone Spine/Bone R Clavicle/Bone R UpperArm/Bone R Forearm/Bone R Hand/R Hand";
	
	private Vector3 OFF_SCREEN_POS = new Vector3( 8.5f, 0f, 0f );
		
	private int _characterIndex = 0;
	
	public int characterIndex
	{
		get{ return _characterIndex; }
		set
		{
			if( value > 4 )
				_characterIndex = 4;
			else if( value < 0 )
				_characterIndex = 0;
			else
				_characterIndex = value;
		}
	}
	
	void Start()
	{
		for( int i = 0; i < enemyGroupings.Length; i++ )
		{
			switch( i )
			{
			case 0:
				Transform[] enemyRoots0 = enemyGroupings[i].GetComponentsInChildren<Transform>();
				
				populateEnemy(enemyRoots0, 1, 5);
				break;
			case 1:		
				Transform[] enemyRoots1 = enemyGroupings[i].GetComponentsInChildren<Transform>();
				
				populateEnemy(enemyRoots1, 5, 9 );
				break;
			case 2:
				Transform[] enemyRoots2 = enemyGroupings[i].GetComponentsInChildren<Transform>();
				
				populateEnemy(enemyRoots2, 9, 14 );
				break;
			case 3:
				Transform[] enemyRoots3 = enemyGroupings[i].GetComponentsInChildren<Transform>();
				
				populateEnemy(enemyRoots3, 14, 17 );
				break;
			case 4:
				Transform[] enemyRoots4 = enemyGroupings[i].GetComponentsInChildren<Transform>();
				
				populateEnemy(enemyRoots4, 17, 21 );
				break;
			default:
				break;
			}
		}
	}
	
	void OnEnable()
	{
		PlayCrate.onCharacterSelectStart += onCharacterSelectStart;
		MainMenuController.onCharacterArrow += onCharacterArrow;
		OptionsOmniCrate.onOptionsBack += onOptionsBack;
	}
	
	void OnDisable()
	{
		PlayCrate.onCharacterSelectStart -= onCharacterSelectStart;
		MainMenuController.onCharacterArrow -= onCharacterArrow;
		OptionsOmniCrate.onOptionsBack -= onOptionsBack;
	}
	
	void populateEnemy(Transform[] roots, int levelMin, int levelMax)
	{
		for( int j = 1; j < roots.Length; j++ )
		{
			GameObject g = (GameObject)Instantiate( Resources.Load(string.Format( ENEMY_RESOURCES_PATH + "Character{0}/Character{0}", Random.Range(1,7) ) ) );
			g.transform.parent = roots[j];
			g.transform.localPosition = Vector3.zero;
			
			g.AddComponent<EnemySelectStats>();
			
			EnemySelectStats e = g.GetComponent<EnemySelectStats>();
			e.stats = new EnemyStats( DBAccess.instance.getEnemyStats( Random.Range(levelMin, levelMax) ) );
			
			GameObject eGun;
			
			int randRange = Random.Range( 0, 101 );
	
			if( randRange < 33 )
			{
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "Colt_1911/FBX/Colt1911Enemy" ) );
				
				eGun.transform.parent = g.transform.FindChild(GUN_NODE_TRANSFORM_PATH);
				eGun.transform.localPosition = Vector3.zero;
				eGun.transform.localEulerAngles = Vector3.zero;
				
				g.animation.Play("Character_Run");
				
				e.stats.enemyGunModel = "Colt1911Enemy";
				e.stats.isTwoHanded = false;
			}
			else if( randRange < 66 && randRange > 32 )
			{
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "Winchester_1912/FBX/EnemyWinchester" ) );
				
				eGun.transform.parent = g.transform.FindChild(GUN_NODE_TRANSFORM_PATH);
				eGun.transform.localPosition = Vector3.zero;
				
				eGun.transform.localEulerAngles = Vector3.zero;
				
				e.stats.enemyGunModel = "EnemyWinchester";
				e.stats.isTwoHanded = true;
				
				g.animation.Play("Character_Run");
				g.animation.Blend("Character_THW");
			} else {
				eGun = (GameObject)Instantiate( Resources.Load( GUN_RESOURCES_PATH + "AK-47/FBX/Ak47Enemy" ) );
				
				eGun.transform.parent = g.transform.FindChild(GUN_NODE_TRANSFORM_PATH);
				eGun.transform.localPosition = Vector3.zero;
				
				eGun.transform.localEulerAngles = Vector3.zero;
				
				e.stats.enemyGunModel = "Ak47Enemy";
				e.stats.isTwoHanded = true;
				
				g.animation.Play("Character_Run");
				g.animation.Blend("Character_THW");
			}
			
			System.Object[,] name = DBAccess.instance.getRandomEnemyName();
					
			e.labels = g.GetComponentsInChildren<UILabel>();
			e.labels[0].text = (string)name[0,0] + " " + (string)name[1,0];
			e.labels[1].text = "L - " + e.stats.level;
		}
	}
	
	void onCharacterSelectStart()
	{
		transform.position = OFF_SCREEN_POS;
		
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", 0,
			"time", 1f,
			"delay", 1.8f,
			"easetype", iTween.EaseType.easeOutSine,
			"oncompletetarget", gameObject
			)
		);
	}
	
	void onCharacterArrow(bool isRight)
	{			
		float moveAmount = (isRight) ? 8.5f : -8.5f;
		
		if( isRight )
			characterIndex++;
		else
			characterIndex--;
		
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", transform.position.x - moveAmount,
			"time", 1.4f,
			"easetype", iTween.EaseType.easeOutSine,
			"oncompletetarget", gameObject,
			"oncomplete", "revertState"
			)
		);
	}
	
	void revertState() { MainMenuController.instance.menuState = MainMenuController.MenuState.Character; }
	
	void onOptionsBack()
	{
		for( int i = 0; i < enemyGroupings.Length; i++ )
		{
			if( i != characterIndex )
			{
				Vector3 offset = enemyGroupings[i].transform.localPosition;
				offset.y = 20f;
				
				enemyGroupings[i].transform.localPosition = offset;
			}
		}
		
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", 8.5f,
			"time", 1.4f,
			"easetype", iTween.EaseType.easeInSine,
			"oncompletetarget", gameObject,
			"oncomplete", "resetEnemyGroupings"
			)
		);
	}
	
	void resetEnemyGroupings()
	{
		for( int i = 0; i < enemyGroupings.Length; i++ )
		{
			if( i != characterIndex )
			{
				Vector3 reset = enemyGroupings[i].transform.localPosition;
				reset.y = 0;
				
				enemyGroupings[i].transform.localPosition = reset;
			}
		}
		
		characterIndex = 0;
	}
}
