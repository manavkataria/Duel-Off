using UnityEngine;
using System.Collections;

public class GunTweenIn : MonoBehaviour {

	void OnEnable() { TutorialGrid.onTutorialUpdate += onTutorialUpdate; }
	void OnDisable() { TutorialGrid.onTutorialUpdate -= onTutorialUpdate; }
	
	void onTutorialUpdate( int curPos )
	{
		if( curPos == 6 )
		{
			Vector3 rotation = new Vector3( transform.localEulerAngles.x, 90, 0 );
			
			iTween.RotateTo( gameObject, iTween.Hash(
				"rotation", rotation,
				"islocal", true,
				"time", 0.5f,
				"easetype", iTween.EaseType.easeOutExpo
				)
			);
		}
	}
}
