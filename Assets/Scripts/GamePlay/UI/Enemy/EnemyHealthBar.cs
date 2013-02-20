using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {
	
	UISprite thisSprite;
	
	private Vector3 scaleToScale;
	
	private Vector3 ORIGINAL_SCALE;
	private Vector3 RESET_SCALE;
	
	private Color FULL_HEALTH_COLOR;
	private Color FULL_DEAD_COLOR = new Color( 0.725f, 0f, 0f, 1f );
	private Color MIDDLE_COLOR = Color.yellow;
	
	public UISprite healthBarBG;
	
	void Awake()
	{
		thisSprite = GetComponent<UISprite>();
		
		ORIGINAL_SCALE = transform.localScale;
				
		FULL_HEALTH_COLOR = thisSprite.color;
	}
	
	void OnEnable()
	{
		EnemyBase.onEnemyStatsChange += onEnemyStatsChange; 
	}
	
	void OnDisable()
	{
		EnemyBase.onEnemyStatsChange -= onEnemyStatsChange; 
	}
	
	void onEnemyStatsChange( EnemyStats stats, EnemyBase._EnemyState state )
	{
		scaleToScale = transform.localScale;
		scaleToScale.x = ORIGINAL_SCALE.x * ( stats.health / 100f );
		scaleToScale.x = ( scaleToScale.x > 0 ) ? scaleToScale.x : 0f;
		
		Color tweenToColor;
		
		if( stats.health > 50 )
			tweenToColor = ( ( FULL_HEALTH_COLOR - MIDDLE_COLOR ) * ( ( stats.health -50 ) / 50f ) ) + MIDDLE_COLOR;
		else
			tweenToColor = ( ( MIDDLE_COLOR - FULL_DEAD_COLOR ) * ( stats.health / 50f ) ) + FULL_DEAD_COLOR;
		
		TweenColor.Begin( transform.gameObject, 0.15f, tweenToColor );
		
		iTween.Stop(gameObject);
		iTween.ScaleTo( gameObject, iTween.Hash(
			"scale", scaleToScale,
			"islocal", true,
			"time", 0.15f,
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
		
		if( stats.health == 0 && state == EnemyBase._EnemyState.Dead )
		{
			thisSprite.enabled = false;
			healthBarBG.enabled = false;
		}
	}
}
