using UnityEngine;
using System.Collections;

public class HealhPackSprite : MonoBehaviour {

	UISprite thisSprite;
	
	private Vector3 CACHED_SCALE;
	
	void Awake() 
	{ 
		thisSprite = GetComponent<UISprite>(); 
		CACHED_SCALE = transform.localScale;
	}
	
	void OnEnable() 
	{ 
		PlayerController.onUIUpdate += onUIUpdate; 
		PlayerSavedPrefs.onHealthPackUse += onHealthPackUse; 
	}
	
	void OnDisable() 
	{ 
		PlayerController.onUIUpdate -= onUIUpdate; 
		PlayerSavedPrefs.onHealthPackUse -= onHealthPackUse;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		if( clicked == gameObject && state != PlayerController._GameState.None )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "Health Pack Use" );
			
			switch( name )
			{
			case "HPackTiny":
				DBAccess.instance.userPrefs.useHealthPack(0);
				tweenScale();
				break;
			case "HPackSmall":
				DBAccess.instance.userPrefs.useHealthPack(1);
				tweenScale();
				break;
			case "HPackMedium":
				DBAccess.instance.userPrefs.useHealthPack(2);
				tweenScale();
				break;
			case "HPackLarge":
				DBAccess.instance.userPrefs.useHealthPack(3);
				tweenScale();
				break;
			case "HPackHuge":
				DBAccess.instance.userPrefs.useHealthPack(4);
				tweenScale();
				break;
			default:
				break;
			}
		}
	}
	
	void onHealthPackUse()
	{
		switch( name )
		{
		case "HPackTiny":
			if( DBAccess.instance.userPrefs.hPacks[0].quantity > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1f;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "HPackSmall":
			if( DBAccess.instance.userPrefs.hPacks[1].quantity > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1f;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "HPackMedium":
			if( DBAccess.instance.userPrefs.hPacks[2].quantity > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1f;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "HPackLarge":
			if( DBAccess.instance.userPrefs.hPacks[3].quantity > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1f;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "HPackHuge":
			if( DBAccess.instance.userPrefs.hPacks[4].quantity > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1f;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		default:
			break;
		}
	}
	
	void tweenScale()
	{
		iTween.ScaleTo( gameObject, iTween.Hash(
			"scale", CACHED_SCALE * 1.3f,
			"islocal", true,
			"time", 0.15f,
			"easetype", iTween.EaseType.easeOutQuad
			)
		);
		iTween.ScaleTo( gameObject, iTween.Hash(
			"scale", CACHED_SCALE,
			"islocal", true,
			"time", 0.15f,
			"delay", 0.15f,
			"easetype", iTween.EaseType.easeInQuad
			)
		);
	}
}
