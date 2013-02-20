using UnityEngine;
using System.Collections;

public class AmmoCountTween : MonoBehaviour {

	private UILabel thisLabel;
	
	private Vector3 SCALE_TO = new Vector3( 120, 120, 1 );
	private Vector3 CACHED_SCALE;
	
	private Color32 ORIGINAL_COLOR = new Color32( 242, 143, 16, 255 );
	private Color32 PANIC_COLOR = new Color32( 255, 13, 13, 255 );
	
	private bool isPanic = false;
	
	void Awake() 
	{ 
		thisLabel = GetComponent<UILabel>();
		CACHED_SCALE = transform.localScale;
	}
	
	void OnEnable() 
	{ 
		PlayerController.onUIUpdate += onUIUpdate; 
		Tutorial.onTutorialUpdate += onTutorialUpdate;
		Gun.onReloadFinish += onTutorialUpdate;
	}
	
	void OnDisable() 
	{
		PlayerController.onUIUpdate -= onUIUpdate; 
		Tutorial.onTutorialUpdate -= onTutorialUpdate;
		Gun.onReloadFinish -= onTutorialUpdate;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState gameState, GameObject clicked, EnemyStats stats ) 
	{ 
		thisLabel.text = gun.capacity.ToString(); 
		thisLabel.color = ORIGINAL_COLOR;
		
		if( gun.capacity > 0 )
		{
			isPanic = false;
			
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", SCALE_TO,
				"islocal", true,
				"time", 0.05f,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", CACHED_SCALE,
				"islocal", true,
				"time", 0.05f,
				"delay", 0.05f,
				"easetype", iTween.EaseType.easeInQuad
				)
			);
		}
		else
		{
			thisLabel.color = PANIC_COLOR;
			
			if( !isPanic )
			{
				iTween.Stop(gameObject);
				transform.localScale = CACHED_SCALE;
				
				isPanic = true;
			}
			
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", SCALE_TO,
				"islocal", true,
				"time", 0.1f,
				"looptype", iTween.LoopType.pingPong,
				"easetype", iTween.EaseType.easeInQuad
				)
			);
		}
	}
	
	void onTutorialUpdate( GameObject clicked, Gun gun )
	{
		thisLabel.text = gun.capacity.ToString(); 
		thisLabel.color = ORIGINAL_COLOR;
		
		if( gun.capacity > 0 )
		{
			isPanic = false;
			
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", SCALE_TO,
				"islocal", true,
				"time", 0.05f,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", CACHED_SCALE,
				"islocal", true,
				"time", 0.05f,
				"delay", 0.05f,
				"easetype", iTween.EaseType.easeInQuad
				)
			);
		}
		else
		{
			thisLabel.color = PANIC_COLOR;
			
			if( !isPanic )
			{
				iTween.Stop(gameObject);
				transform.localScale = CACHED_SCALE;
				
				isPanic = true;
			}
			
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", SCALE_TO,
				"islocal", true,
				"time", 0.1f,
				"looptype", iTween.LoopType.pingPong,
				"easetype", iTween.EaseType.easeInQuad
				)
			);
		}
	}	
}
