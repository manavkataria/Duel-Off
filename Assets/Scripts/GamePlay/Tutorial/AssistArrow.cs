using UnityEngine;
using System.Collections;

public class AssistArrow : MonoBehaviour {
	
	UISprite thisSprite;
	
	void Awake() 
	{ 
		thisSprite = GetComponent<UISprite>(); 
		thisSprite.enabled = false;
	}
	
	void OnEnable() { TutorialGrid.onTutorialUpdate += onTutorialUpdate; }
	void OnDisable() { TutorialGrid.onTutorialUpdate -= onTutorialUpdate; }
	
	void onTutorialUpdate(int curPos)
	{
		if( curPos == 12 )
		{
			thisSprite.enabled = true;
			
			iTween.MoveTo( gameObject, iTween.Hash(
				"y", 135,
				"islocal", true,
				"time", 0.2f,
				"looptype", iTween.LoopType.pingPong,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
		}
		else
			thisSprite.enabled = false;
	}
}
