using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	UISprite thissprite;
	
	private Vector3 ORIGINAL_SCALE;
	private Vector3 RESET_SCALE;
	
	private Color FULL_HEALTH_COLOR;
	private Color MIDDLE_COLOR = Color.yellow;
	private Color FULL_DEAD_COLOR = new Color( 0.725f, 0f, 0f, 1f );
	
	private Vector3 scaleToScale;
	
	private PlayerController player;
	
	void Awake() 
	{ 
		thissprite = GetComponent<UISprite>(); 
		ORIGINAL_SCALE = transform.localScale;
				
		FULL_HEALTH_COLOR = thissprite.color;
		
		player = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	void OnEnable() 
	{ 
		PlayerController.onHitByEnemy += onHitByEnemy; 
		PlayerSavedPrefs.onHealthPackUse += onHealthPackUse; 
	}
	
	void OnDisable() 
	{ 
		PlayerController.onHitByEnemy -= onHitByEnemy; 
		PlayerSavedPrefs.onHealthPackUse -= onHealthPackUse; 
	}
	
	void onHitByEnemy( float health )
	{
		scaleToScale = transform.localScale;
		scaleToScale.x = ORIGINAL_SCALE.x * ( health / 100f );
		scaleToScale.x = ( scaleToScale.x > 0 ) ? scaleToScale.x : 0f;
		
		Color tweenToColor;
		
		if( health > 50 )
			tweenToColor = ( ( FULL_HEALTH_COLOR - MIDDLE_COLOR ) * ( ( health -50 ) / 50f ) ) + MIDDLE_COLOR;
		else
			tweenToColor = ( ( MIDDLE_COLOR - FULL_DEAD_COLOR ) * ( health / 50f ) ) + FULL_DEAD_COLOR;
		
		TweenColor.Begin( transform.gameObject, 0.15f, tweenToColor );
		
		iTween.Stop(gameObject);
		iTween.ScaleTo( gameObject, iTween.Hash(
			"scale", scaleToScale,
			"islocal", true,
			"time", 0.15f,
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
		
		if( health <= 50 )
		{
			float pitchPercent = ( (float)health / (float)50 );
			player.audioSources[9].pitch = 2 - pitchPercent;
			player.audioSources[10].volume -= ( 0.05f);
			player.audioSources[11].volume -= ( 0.05f);
			
			if( !player.audioSources[9].isPlaying )
				player.audioSources[9].Play();
			
		}
		else if( health > 50 )
		{
			player.audioSources[10].volume = 0.25f;
			player.audioSources[11].volume = 0.25f;
			player.audioSources[9].Stop();
		}
			
	}
	
	void onHealthPackUse()
	{
		onHitByEnemy( DBAccess.instance.userPrefs.health );
	}
}
