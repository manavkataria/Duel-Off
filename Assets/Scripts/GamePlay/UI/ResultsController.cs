using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResultsController : MonoBehaviour {
	
	public UILabel expLabel;
	public UILabel goldLabel;
	
	public UILabel lvlLabel;
	public UILabel unlockLabel;
	
	public UILabel windowLabel;
	
	public UISprite unlockedSprite;
	
	public UIPanel windowPanel;
	
	public UIPanel clipPanel;
	
	public UIDragPanelContents[] items;
	
	public UIGrid grid;
	
	public int curPos = 0;
	
	private Vector3 panelPosCache;
	
	private Vector4 cRange;
	private Vector3 cPos;
	
	private Vector3 RESET_POS = new Vector3(0,0,250);
	private Vector3 RED_SCREEN_POS = new Vector3(0,0,0.1f);
	
	private bool canPress = true;
	
	public GameObject redScreen;
	
	public GameObject playerHealthBar;
	
	public GameObject ammoCount;
	
	public PlayerController player;
	
	public delegate void OnResultsStartHandler();
	public static event OnResultsStartHandler onResultsStart;
	
	public delegate void OnResultsFinishHandler();
	public static event OnResultsFinishHandler onResultsFinished;
	
	void Awake()
	{
		panelPosCache = clipPanel.transform.position;
		player = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	void OnEnable()
	{
		PlayerController.onUIUpdate += onUIUpdate;
	}
	
	void OnDisable()
	{
		PlayerController.onUIUpdate -= onUIUpdate;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		if( state == PlayerController._GameState.Results && stats != null && canPress )
		{
			canPress = false;
			
			expLabel.text = "0";
			goldLabel.text = DBAccess.instance.userPrefs.Gold.ToString();
			
			iTween.MoveTo( windowPanel.gameObject, iTween.Hash(
				"position", RESET_POS,
				"islocal", true,
				"time", 0.4f,
				"delay", 2f,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
			
			float expEarned = ( ( (float)stats.level / (float)DBAccess.instance.userPrefs.Level ) * (float)DBAccess.instance.userPrefs.Level ) * 3;
			float goldEarned = expEarned / 2f;
			
			
			
			iTween.ValueTo( gameObject, iTween.Hash(
				"from", 0,
				"to", expEarned,
				"delay", 3f,
				"time", 1f,
				"onupdatetarget", gameObject,
				"onupdate", "expValueTo",
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
			
			iTween.ValueTo( gameObject, iTween.Hash(
				"from", DBAccess.instance.userPrefs.Gold,
				"to", goldEarned,
				"delay", 3f,
				"time", 1f,
				"onupdatetarget", gameObject,
				"onupdate", "goldValueTo",
				"oncompletetarget", gameObject,
				"oncomplete", "setExpAndGold",
				"oncompleteparams", expEarned,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
		}
		else if( state == PlayerController._GameState.Dead )
			StartCoroutine( deathRoutine(stats) );
		
		if( clicked != null && state != PlayerController._GameState.Dead ) 
		{
			if( clicked == gameObject )
			{
				moveGridTo(curPos+1);
			}
		}
	}
	
	void expValueTo( float newVal ) { expLabel.text = ((int)newVal).ToString(); }
	
	void goldValueTo( float newVal ) 
	{ 
		goldLabel.text = ((int)newVal).ToString(); 
		if( ( (int)newVal % 3 ) == 0 )
			player.audioSources[8].Play();
	}
	
	void setExpAndGold( float exp )
	{
		Debug.Log( "EXP : " + exp );
		DBAccess.instance.userPrefs.Exp += (int)exp;
		DBAccess.instance.userPrefs.Gold += (int)(exp/2);
		
		Debug.Log( "EXPE : " + DBAccess.instance.userPrefs.Exp );
		canPress = true;
		
		player.audioSources[8].Play();
	}
	
	void setExpAndGoldDeath( float exp )
	{
		DBAccess.instance.userPrefs.Exp += (int)exp;
		DBAccess.instance.userPrefs.Gold += (int)(exp/2);
	}
	
	protected void moveGridTo( int index )
	{
		if( index < 3 )
		{
			canPress = false;
			
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", clipPanel.clipRange.x,
	            "to", items[curPos+1].transform.localPosition.x,
	            "time", 0.4f,
	            "onupdatetarget", gameObject,
	            "onupdate", "moveCenter",
	            "easetype", iTween.EaseType.easeOutExpo
	            )
	        );
	        iTween.ValueTo(windowPanel.gameObject, iTween.Hash(
	            "from", clipPanel.transform.localPosition.x,
	            "to", panelPosCache.x - ( grid.cellWidth * (float)index ),
	            "time", 0.4f,
	            "onupdatetarget", gameObject,
	            "onupdate", "movePosition",
	            "oncompletetarget", gameObject,
	            "oncomplete", "updateCurPosMoveTo",
	            "oncompleteparams", index,
	            "easetype", iTween.EaseType.easeOutExpo
	            )
	        );
		}
	}
	
    // Update callback for iTween ValueTo
    protected void moveCenter(float newValue)
    {
        cRange = clipPanel.clipRange;
        cRange.x = newValue;

        clipPanel.clipRange = cRange;
    }

    // Update callback for iTween ValueTo
    protected void movePosition(float newValue)
    {
        cPos = clipPanel.transform.localPosition;
        cPos.x = newValue;
        clipPanel.transform.localPosition = cPos;
    }
	
	void updateCurPosMoveTo(int index)
	{
		canPress = true;
		
		curPos = index;
		
		if( index == 1 )
		{
			int lvl = DBAccess.instance.userPrefs.Level;
			int exp = DBAccess.instance.userPrefs.Exp;
			
			for( int i = lvl + 1; i < 26; i++ )
			{
				if( exp > DBAccess.instance.userPrefs.expRequired[i] )
					continue;
				else if( exp == DBAccess.instance.userPrefs.expRequired[i] )
				{
					DBAccess.instance.userPrefs.Level = i;
					lvlLabel.text = DBAccess.instance.userPrefs.Level.ToString();
				}
				else
				{
					DBAccess.instance.userPrefs.Level = i-1;
					lvlLabel.text = DBAccess.instance.userPrefs.Level.ToString();
					break;
				}
			}
			
			Vector3 originalScale = lvlLabel.transform.localScale;
			
			iTween.ScaleTo( lvlLabel.gameObject, iTween.Hash(
				"scale", originalScale * 1.3f,
				"islocal", true,
				"time", 0.15f,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
			
			iTween.ScaleTo( lvlLabel.gameObject, iTween.Hash(
				"scale", originalScale,
				"islocal", true,
				"time", 0.15f,
				"delay", 0.15f,
				"easetype", iTween.EaseType.easeInQuad
				)
			);
		}
		if( index == 2 )
		{
			int lvl = DBAccess.instance.userPrefs.Level;
			
			List<string[,]> infoList = new List<string[,]>();
			
			for( int i = 0; i < 5; i++ )
			{
				if( i < 3 )
				{
					if( lvl >= DBAccess.instance.userPrefs.allGuns[i].unlockLevel && !DBAccess.instance.userPrefs.allGuns[i].isPurchased )
					{
						string[,] firstItem = new string[1,2];
						
						firstItem[0,0] = DBAccess.instance.userPrefs.allGuns[i].model + "_Icon";
						firstItem[0,1] = DBAccess.instance.userPrefs.allGuns[i].model;
						
						infoList.Add( firstItem );

						DBAccess.instance.userPrefs.allGuns[i].isPurchased = true;
						
						for( int j = 1; j < 4; j++ )
						{
							if( lvl >= DBAccess.instance.userPrefs.allGuns[i].compatibleAmmo[j].unlockLevel && !DBAccess.instance.userPrefs.allGuns[i].compatibleAmmo[j].isPurchased )
							{
								string[,] secondItem = new string[1,2];
								
								secondItem[0,0] = DBAccess.instance.userPrefs.allGuns[i].compatibleAmmo[j].spriteName;
								secondItem[0,1] = DBAccess.instance.userPrefs.allGuns[i].compatibleAmmo[j].ammoName;
								
								infoList.Add( secondItem );
								
								DBAccess.instance.userPrefs.allGuns[i].compatibleAmmo[j].isPurchased = true;
							}
						}
					}
				}
				if( lvl >= DBAccess.instance.userPrefs.allHPacks[i].unlockLevel && !DBAccess.instance.userPrefs.allHPacks[i].isPurchased )
				{
					string[,] thirdItem = new string[1,2];
					
					thirdItem[0,0] = "HealthIcon";
					thirdItem[0,1] = DBAccess.instance.userPrefs.allHPacks[i].model;
					
					infoList.Add( thirdItem );
					
					DBAccess.instance.userPrefs.allHPacks[i].isPurchased = true;
				}
			}
			
			StartCoroutine( showUnlocks( infoList ) );
		}
	}
	
	IEnumerator showUnlocks( List<string[,]> items )
	{
		UnityEngine.Debug.Log("ResultsController.cs: UNlock Screen");
		
		for( int i = 0; i < items.Count; i++ )
		{
			unlockedSprite.spriteName = items[i][0,0];
			unlockLabel.text = items[i][0,1];
			TweenAlpha.Begin( unlockedSprite.gameObject, 1f, 1f );
			TweenAlpha.Begin( unlockLabel.gameObject, 1f, 1f );
			yield return new WaitForSeconds(1.5f);
			TweenAlpha.Begin( unlockedSprite.gameObject, 1f, 0f );
			TweenAlpha.Begin( unlockLabel.gameObject, 1f, 0f );
			yield return new WaitForSeconds(1.5f);
		}
		
		if( onResultsFinished != null ) {
			UnityEngine.Debug.Log("ResultsController.cs: UNlock Screen - Finished!");
			onResultsFinished();
		} else {
			UnityEngine.Debug.Log("ResultsController.cs: UNlock Screen - Skipped onResultsFinished() - mkfadeToMainMenu");
			StartCoroutine( mkfadeToMainMenu() );
		}
			
		
	}
	
	IEnumerator mkfadeToMainMenu()
	{
		yield return new WaitForSeconds(2f);
		Application.LoadLevel(0);
	}
	
	IEnumerator deathRoutine(EnemyStats stats)
	{
		UnityEngine.Debug.Log("ResultsController.cs: Death Routine");

		if( canPress )
		{
			onResultsStart();
			
			yield return new WaitForSeconds(4.533f);
					
			redScreen.transform.localPosition = RED_SCREEN_POS;
			
			windowLabel.text = "You Lose!";
			
			iTween.MoveTo( windowPanel.gameObject, iTween.Hash(
				"x", 0,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
			
			canPress = false;
				
			expLabel.text = "0";
			goldLabel.text = DBAccess.instance.userPrefs.Gold.ToString();
			
			float expEarned = ( ( ( (float)stats.level / (float)DBAccess.instance.userPrefs.Level ) * (float)DBAccess.instance.userPrefs.Level ) * 3 ) / 2;
			float goldEarned = expEarned / 2f;
			
			iTween.ValueTo( gameObject, iTween.Hash(
				"from", 0,
				"to", expEarned,
				"delay", 1f,
				"time", 1f,
				"onupdatetarget", gameObject,
				"onupdate", "expValueTo",
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
			
			iTween.ValueTo( gameObject, iTween.Hash(
				"from", DBAccess.instance.userPrefs.Gold,
				"to", goldEarned,
				"delay", 1f,
				"time", 1f,
				"onupdatetarget", gameObject,
				"onupdate", "goldValueTo",
				"oncompletetarget", gameObject,
				"oncomplete", "setExpAndGoldDeath",
				"oncompleteparams", expEarned,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
		
			onResultsFinished();
		}
	}
}
