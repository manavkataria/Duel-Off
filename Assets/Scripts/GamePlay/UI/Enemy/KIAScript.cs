using UnityEngine;
using System.Collections;

public class KIAScript : MonoBehaviour {
	
	UILabel thisLabel;
	
	private Vector3 ORIGINAL_SCALE;
	
	void Awake()
	{
		ORIGINAL_SCALE = transform.localScale;
		
		thisLabel = GetComponent<UILabel>();
		thisLabel.enabled = false;
	}

	void OnEnable() { EnemyBase.onEnemyStatsChange += onEnemyStatsChange; }
	void OnDisable() { EnemyBase.onEnemyStatsChange -= onEnemyStatsChange; }
	
	void onEnemyStatsChange( EnemyStats stats, EnemyBase._EnemyState state )
	{
		if( stats.health == 0 && state == EnemyBase._EnemyState.Dead )
		{
			thisLabel.enabled = true;
			
			iTween.ScaleTo( gameObject, iTween.Hash(
				"scale", ORIGINAL_SCALE * 1.3f,
				"islocal", true,
				"time", 0.2f,
				"looptype", iTween.LoopType.pingPong,
				"easetype", iTween.EaseType.easeOutQuad
				)
			);
		}
	}
}
