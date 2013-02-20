using UnityEngine;
using System;
using System.Collections;

public class BuyButtonController : MonoBehaviour {
	
	public UISprite itemInQuestion;
	public UILabel itemDescription;
	
	public delegate void OnPurchasedHandler(SuppliesUIObject bought);
	public static event OnPurchasedHandler onPurchased;
	
	private SuppliesUIObject s;
	private Purchasable p;
	
	Vector3 RESET_POS;
	
	void OnEnable()
	{
		StoreController.onPurchasing += onPurchasing;
		RESET_POS = transform.localPosition;
	}
	
	void OnDisable()
	{
		StoreController.onPurchasing -= onPurchasing;
	}
	
	void onPurchasing( SuppliesUIObject item, GameObject tag )
	{
		if( tag.tag == "BuyButton" )
		{
			Type t = item.GetType();
			
			AmmoUIObject a;
			HealthUIObject h;
			GunUIObject g;
			
			if( tag.name == "Button Background" )
			{
				tweenIn();

				if( t == typeof(AmmoUIObject) )
				{
					a = (AmmoUIObject)item;
					itemInQuestion.spriteName = a.ammo.spriteName;
					itemDescription.text = a.ammo.ammoName;
					s = a;
					p = a.ammo;
				}
				if( t == typeof(GunUIObject) )
				{
					g= (GunUIObject)item;
					itemInQuestion.spriteName = g.gunObj.model + "_Icon";
					itemDescription.text = g.gunObj.model;
					s = g;
					p = g.gunObj;
				}
				if( t == typeof(HealthUIObject) )
				{
					h = (HealthUIObject)item;
					itemInQuestion.spriteName = "HealthIcon";
					itemDescription.text = h.hPack.model;
					s = h;
					p = h.hPack;
				}
			}
			if( tag.name == "Label OK" )
			{
				if( p.GetType() == typeof(Ammo) )
				{
					Ammo a2 = (Ammo)p;
					
					if( DBAccess.instance.userPrefs.Gold >= a2.price )
					{
						DBAccess.instance.userPrefs.findAmmoAndSetAsPurchased(a2);
						DBAccess.instance.userPrefs.Gold -= a2.price;
					}
				}
				if( p.GetType() == typeof(Gun) )
				{
					Gun g2 = (Gun)p;
					
					if( DBAccess.instance.userPrefs.Gold >= g2.price )
					{
						DBAccess.instance.userPrefs.findGunAndSetAsPurchased((Gun)p);
						DBAccess.instance.userPrefs.Gold -= g2.price;
					}
				}
				if( p.GetType() == typeof(HealthPack) )
				{
					HealthPack h2 = (HealthPack)p;
					
					if( DBAccess.instance.userPrefs.Gold >= h2.price )
					{
						DBAccess.instance.userPrefs.findHealthPackAndSetAsPurchased((HealthPack)p);
						DBAccess.instance.userPrefs.Gold -= h2.price;
					}
				}
				
				if( onPurchased != null )
				{
					Debug.Log("onPurchased");
					onPurchased(s);
				}
				
				tweenOut();
			}
			if( tag.name == "Label Back" )
			{
				tweenOut();
			}
		}
	}
	
	void tweenIn()
	{
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", 0,
			"islocal", true,
			"time", 0.4f,
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
	}
	
	void tweenOut()
	{
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", -1.5f,
			"islocal", true,
			"time", 0.4f,
			"oncompletetarget", gameObject,
			"oncomplete", "resetPos",
			"easetype", iTween.EaseType.easeOutExpo
			)
		);
	}
	
	void resetPos()
	{
		transform.localPosition = RESET_POS;
	}
}
