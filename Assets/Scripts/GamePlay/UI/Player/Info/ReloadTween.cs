using UnityEngine;
using System.Collections;

public class ReloadTween : MonoBehaviour {
	
	private float firstPos = 0f;
	private float secondPos;
	
	private string axis = "y";
	
	void Awake() 
	{ 
		secondPos = ( gameObject.name == "Ammo Buttons" ) ? -300f : 300f; 
		if( gameObject.name == "Ammo Count" )
		{
			axis = "x";
			firstPos = -300f;
			secondPos = 0f;
		}
	}
		
	void OnEnable() 
	{
		PlayerController.onUIUpdate += onUIUpdate; 
		ResultsController.onResultsStart += onResultsStart;
	}
	
	void OnDisable() 
	{ 
		PlayerController.onUIUpdate -= onUIUpdate;
		ResultsController.onResultsStart -= onResultsStart;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState gameState, GameObject clicked, EnemyStats stats )
	{
		if( gameState == PlayerController._GameState.Reload )
		{	
			iTween.MoveTo( gameObject, iTween.Hash(
				axis, firstPos,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeOutBack
				)
			);
			
			if( name == "Health Meter" )
			{
				iTween.MoveTo( gameObject, iTween.Hash(
					"y", 300f,
					"islocal", true,
					"time", 0.4f,
					"easetype", iTween.EaseType.easeOutBack
					)
				);
			}
		}
		if( gameState == PlayerController._GameState.Active || gameState == PlayerController._GameState.IsReloading )
		{
			iTween.MoveTo( gameObject, iTween.Hash(
				"y", secondPos,
				"islocal", true,
				"time", 0.4f,
				"easetype", iTween.EaseType.easeInBack
				)
			);
			
			if( gameState == PlayerController._GameState.Active )
			{
				if( name == "Health Meter" )
				{
					iTween.MoveTo( gameObject, iTween.Hash(
						"y", 0,
						"islocal", true,
						"time", 0.4f,
						"easetype", iTween.EaseType.easeOutBack
						)
					);
				} else {
					iTween.MoveTo( gameObject, iTween.Hash(
						axis, secondPos,
						"islocal", true,
						"time", 0.4f,
						"easetype", iTween.EaseType.easeOutBack
						)
					);
				}
			}
		}
	}
	
	void onResultsStart()
	{
		if( name.Contains("Button") )
		{
			iTween.MoveTo( gameObject, iTween.Hash(
				axis, secondPos,
				"islocal", true,
				"time", 4f,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
		} else {
			iTween.MoveTo( gameObject, iTween.Hash(
				axis, firstPos,
				"islocal", true,
				"time", 4f,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
		}
	}
}
