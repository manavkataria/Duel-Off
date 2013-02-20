using UnityEngine;
using System.Collections;

public class PhoneTiltAnim : MonoBehaviour {

	void OnEnable() { TutorialGrid.onTutorialUpdate += onTutorialUpdate; }
	void OnDisable() { TutorialGrid.onTutorialUpdate -= onTutorialUpdate; }
	
	void onTutorialUpdate( int curPos )
	{
		Vector3 rotation = ( curPos == 3) ? new Vector3( 0, 180, -30f ) : new Vector3( 0, 0, -30f );
		
		if( curPos == 3 || curPos == 5 )
		{
			iTween.RotateTo( gameObject, iTween.Hash(
				"name", "rotation",
				"rotation", rotation,
				"islocal", true,
				"time", 1f,
				"looptype", iTween.LoopType.loop,
				"easetype", iTween.EaseType.easeInOutExpo
				)
			);
		} else {
			iTween.StopByName( gameObject, "rotation" );
			transform.localEulerAngles = ( gameObject.name == "TiltPhoneRight" ) ? Vector3.zero : new Vector3( 0, 180, 0 );
		}
	}
}
