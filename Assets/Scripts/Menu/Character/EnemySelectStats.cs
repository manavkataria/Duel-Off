using UnityEngine;
using System.Collections;

public class EnemySelectStats : MonoBehaviour {

	public EnemyStats stats;
	
	public UILabel[] labels;
	
	void OnEnable()
	{
		PlayCrate.onCharacterSelectStart += onCharacterSelectStart;
		MainMenuController.onCharacterArrow += onCharacterArrow;
		OptionsOmniCrate.onOptionsBack += onOptionsBack;
	}
	
	void OnDisable()
	{
		PlayCrate.onCharacterSelectStart -= onCharacterSelectStart;
		MainMenuController.onCharacterArrow -= onCharacterArrow;
		OptionsOmniCrate.onOptionsBack -= onOptionsBack;
	}
	
	void onCharacterSelectStart()
	{
		transform.parent.transform.localEulerAngles = new Vector3( 0, 0, 90 );
		
		iTween.RotateTo( transform.parent.gameObject, iTween.Hash(
			"z", 0,
			"islocal", true,
			"time", 0.4f,
			"delay", 2.4f,
			"easetype", iTween.EaseType.easeOutSine
			)
		);
	}
	
	void onCharacterArrow( bool isRight)
	{
		float angle = (isRight) ? 90f : -90f;
		
		iTween.RotateTo( transform.parent.gameObject, iTween.Hash(
			"z", angle,
			"islocal", true,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeInOutSine
			)
		);
		
		iTween.RotateTo( transform.parent.gameObject, iTween.Hash(
			"z", 0,
			"time", 0.6f,
			"islocal", true,
			"delay", 0.8f,
			"easetype", iTween.EaseType.easeInOutSine
			)
		);
	}
	
	void onOptionsBack()
	{
		iTween.RotateTo( transform.parent.gameObject, iTween.Hash(
			"z", -90f,
			"islocal", true,
			"time", 1f,
			"easetype", iTween.EaseType.easeInOutSine,
			"oncompletetarget", gameObject,
			"oncomplete", "revertState"
			)
		);
	}
	
	void revertState() { transform.parent.transform.localEulerAngles = Vector3.zero; }
}
