using UnityEngine;
using System.Collections;

public class PauseSprite : MonoBehaviour {
	
	private const float RESET_POS = 300f;
	private const float TWEEN_POS = 0f;
	
	private GameObject PARENT;
	
	public delegate void OnPauseHandler();
	public static event OnPauseHandler onPause;
	
	void Awake() { PARENT = transform.parent.gameObject; }
	
	void OnEnable() 
	{ 
		PlayerController.onUIUpdate += onUIUpdate;
		//ResultsController.onResultsFinished += onResultsFinished;
		//ResultsController.onResultsStart += onResultsStart;
		TutorialGrid.onTutorialUpdate += onTutorialUpdate;
	}
	
	void OnDisable() 
	{ 
		PlayerController.onUIUpdate -= onUIUpdate; 
		//ResultsController.onResultsFinished -= onResultsFinished;
		//ResultsController.onResultsStart -= onResultsStart;
		TutorialGrid.onTutorialUpdate -= onTutorialUpdate;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{	
		if( clicked == gameObject )
		{			
			if( onPause != null )
				onPause();
		}
		
		/*if( state == PlayerController._GameState.Reload )
		{	
			iTween.MoveTo( PARENT, iTween.Hash(
				"y", TWEEN_POS,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutBack
				)
			);
		}
		if( state == PlayerController._GameState.Active || state == PlayerController._GameState.IsReloading )
		{
			iTween.MoveTo( PARENT, iTween.Hash(
				"y", RESET_POS,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInBack
				)
			);
		}*/
	}
	
	void onTutorialUpdate( int curPos )
	{
		if( curPos == 38 )
		{
			iTween.MoveTo( PARENT, iTween.Hash(
				"y", TWEEN_POS,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutBack
				)
			);
		}
	}
	
	void onResultsStart()
	{
		iTween.MoveTo( PARENT, iTween.Hash(
			"y", RESET_POS,
			"islocal", true,
			"time", 4f,
			"easetype", iTween.EaseType.easeInBack
			)
		);
	}
	
	void onResultsFinished()
	{
		iTween.MoveTo( PARENT, iTween.Hash(
			"y", TWEEN_POS,
			"islocal", true,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeOutBack
			)
		);
		
		UnityEngine.Debug.Log("MK - onPause ResultsFinished");
	}
	
	void onHitByRayCast( GameObject clicked )
	{
		if( clicked != null )
		{
			if( clicked == gameObject )
			{
				Debug.Log ("here");
				if( onPause != null )
					onPause();
			}
		}
	}
}
