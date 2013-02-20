using UnityEngine;
using System.Collections;

public class HealthAnchor : MonoBehaviour {

	void OnEnable() { ResultsController.onResultsStart += onResultsStart; }
	void OnDisable() { ResultsController.onResultsStart -= onResultsStart; }
	
	void onResultsStart()
	{
		iTween.MoveTo( gameObject, iTween.Hash(
			"y", 300f,
			"islocal", true,
			"time", 4f,
			"easetype", iTween.EaseType.easeOutQuad
			)
		);
	}
}
