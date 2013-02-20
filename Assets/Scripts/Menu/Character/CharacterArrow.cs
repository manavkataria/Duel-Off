using UnityEngine;
using System.Collections;

public class CharacterArrow : MonoBehaviour {
	
	void hitByRayCast(GameObject clicked)
	{
		float xAmount = ( clicked.name == "Arrow Right" ) ? 20f : -20f;
		
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", gameObject.transform.localPosition.x + xAmount,
			"islocal", true,
			"time", 0.2f,
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", gameObject.transform.localPosition.x,
			"islocal", true,
			"time", 0.2f,
			"delay", 0.2f,
			"easetype", iTween.EaseType.easeInExpo
			)
		);
	}
}
