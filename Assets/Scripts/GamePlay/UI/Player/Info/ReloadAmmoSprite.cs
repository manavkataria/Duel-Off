using UnityEngine;
using System.Collections;

public class ReloadAmmoSprite : MonoBehaviour {
	
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
		Tutorial.onTutorialUpdate += onTutorialUpdate;
		Gun.onReloadStart += onReloadStart;
		Gun.onGunEmptied += onGunEmptied;
	}
	
	void OnDisable() 
	{ 
		Tutorial.onTutorialUpdate -= onTutorialUpdate;
		PlayerController.onUIUpdate -= onUIUpdate; 
		Gun.onReloadStart -= onReloadStart;
		Gun.onGunEmptied -= onGunEmptied;
	}
	
	void onUIUpdate( Gun gun, PlayerController._GameState state, GameObject clicked, EnemyStats stats )
	{
		if( clicked == gameObject && !gun.isReloading && state != PlayerController._GameState.None )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "Secondary Ammo Reload Use" );
			
			switch( name )
			{
			case "Ammo_L0":
				gun.Reload(gun.compatibleAmmo[0]);
				tweenScale();
				break;
			case "Ammo_L1":
				if( gun.compatibleAmmo[1].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[1]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			case "Ammo_L2":
				if( gun.compatibleAmmo[2].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[2]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			case "Ammo_L3":
				if( gun.compatibleAmmo[3].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[3]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			default:
				switch( thisSprite.spriteName )
				{
				case "Ammo_L0":
					gun.Reload(gun.compatibleAmmo[0]);
					tweenScale();
					break;
					case "Ammo_L1":
					if( gun.compatibleAmmo[1].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[1]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				case "Ammo_L2":
					if( gun.compatibleAmmo[2].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[2]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				case "Ammo_L3":
					if( gun.compatibleAmmo[3].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[3]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				}
				break;
			}
		}
	}
	
	void onReloadStart( float reloadTime, Gun gun )
	{
		switch( name )
		{
		case "Ammo_L0":
			if( gun.loadedAmmo.spriteName == thisSprite.spriteName )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "Ammo_L1":
			if( gun.loadedAmmo.spriteName == thisSprite.spriteName )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "Ammo_L2":
			if( gun.loadedAmmo.spriteName == thisSprite.spriteName )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "Ammo_L3":
			if( gun.loadedAmmo.spriteName == thisSprite.spriteName )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
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
	
	void onGunEmptied(Gun gun)
	{
		switch(name)
		{
		case "Ammo_L0":
			thisSprite.collider.enabled = true;
			thisSprite.alpha = 1;
			break;			
		case "Ammo_L1":
			if( gun.compatibleAmmo[1].amountOwned > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "Ammo_L2":
			if( gun.compatibleAmmo[2].amountOwned > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		case "Ammo_L3":
			if( gun.compatibleAmmo[3].amountOwned > 0 )
			{
				thisSprite.collider.enabled = true;
				thisSprite.alpha = 1;
			} else {
				thisSprite.collider.enabled = false;
				thisSprite.alpha = 0.5f;
			}
			break;
		default:
			break;
			
		}
	}
	
	void onTutorialUpdate( GameObject clicked, Gun gun )
	{
		if( clicked == gameObject )
		{
			Debug.Log("tutorialreload");
			switch( name )
			{
			case "Ammo_L0":
				gun.Reload(gun.compatibleAmmo[0]);
				tweenScale();
				break;
			case "Ammo_L1":
				if( gun.compatibleAmmo[1].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[1]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			case "Ammo_L2":
				if( gun.compatibleAmmo[2].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[2]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			case "Ammo_L3":
				if( gun.compatibleAmmo[3].amountOwned > 0 )
					gun.Reload(gun.compatibleAmmo[3]);
				else
				{
					thisSprite.collider.enabled = false;
					thisSprite.alpha = 0.5f;
				}
				tweenScale();
				break;
			default:
				switch( thisSprite.spriteName )
				{
				case "Ammo_L0":
					gun.Reload(gun.compatibleAmmo[0]);
					tweenScale();
					break;
					case "Ammo_L1":
					if( gun.compatibleAmmo[1].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[1]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				case "Ammo_L2":
					if( gun.compatibleAmmo[2].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[2]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				case "Ammo_L3":
					if( gun.compatibleAmmo[3].amountOwned > 0 )
						gun.Reload(gun.compatibleAmmo[3]);
					else
					{
						thisSprite.collider.enabled = false;
						thisSprite.alpha = 0.5f;
					}
					tweenScale();
					break;
				}
				break;
			}
		}
	}
}
