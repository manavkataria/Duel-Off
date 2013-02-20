using UnityEngine;
using System.Collections;

public class AssistHealth : MonoBehaviour {
	
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
		if( curPos == 30 )
			StartCoroutine(tweenUp());
	}
	
	IEnumerator tweenUp()
	{
		yield return new WaitForSeconds(1);
		thisSprite.enabled = true;
			
		iTween.MoveTo( gameObject, iTween.Hash(
			"y", 115,
			"islocal", true,
			"time", 4,
			"easetype", iTween.EaseType.linear
			)
		);
		
		TweenAlpha.Begin( thisSprite.gameObject, 4f, 0 );
		
		yield return new WaitForSeconds(4);
		thisSprite.enabled = false;
	}
}
