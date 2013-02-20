using UnityEngine;
using System.Collections;

public class EnemyPivot : MonoBehaviour {
	
	private bool tweenFinished = false;			// Used by iTween to wait for RotateTo finish
	
	void OnEnable()
	{
		//PlayerController.onCalibratingStop += onCalibratingStop;
	}
	
	void OnDisable()
	{
		//PlayerController.onCalibratingStop -= onCalibratingStop;;
	}
	
	// Wait for calibration finish event to fire before playing
	private void onCalibratingStop( bool isCalibrating )
	{
		if( isCalibrating )		StopAllCoroutines();
		else  					StartCoroutine( enemyAI() );
	}
	
	// Random AI movement to continue through until game end
	private IEnumerator enemyAI()
	{
		Vector3 curPos;
		Vector3 randDeviation;
		Vector3 endPos;
		float randWait;
		float speed = 10f;
		
		while(true)
		{
			tweenFinished = false;
			
			curPos = transform.eulerAngles;
			
			if( curPos.y > 60 && curPos.y < 90)
				randDeviation = new Vector3( 0, Random.Range( -10f, -30f ), 0 );
			else if( curPos.y < -60 && curPos.y > -90 )
				randDeviation = new Vector3( 0, Random.Range( 10f, 30f ) , 0 );
			else
				randDeviation = new Vector3( 0, Random.Range( -30f, 30f ), 0 );
			
			endPos = curPos + randDeviation;
			randWait = Random.Range( 0f, 3f );
				
			iTween.RotateTo( gameObject, iTween.Hash
				( 
					"rotation", endPos, 
					"speed", speed,
					"oncompletetarget", gameObject,
					"oncomplete", "changeTweenState",
					"easetype", iTween.EaseType.linear 
				) );
			
			while( !tweenFinished ) { yield return null; }
									
			yield return new WaitForSeconds( randWait );
		}
	}
	
	// Called to signify end of a movement cycle by enemyAI()
	private void changeTweenState()
	{
		tweenFinished = true;
	}
}
