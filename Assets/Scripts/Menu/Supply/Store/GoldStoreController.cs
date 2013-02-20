using UnityEngine;
using System.Collections;

public class GoldStoreController : MonoBehaviour {
	
	Vector3 RESET_POS;
	
	void OnEnable()
	{
		StoreController.onPurchasing += onPurchasing;
		
#if UNITY_IPHONE
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
#endif		

		RESET_POS = transform.localPosition;
	}

	void OnDisable()
	{
		StoreController.onPurchasing -= onPurchasing;
		
#if UNITY_IPHONE
		StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
		StoreKitManager.purchaseFailedEvent -= purchaseFailedEvent;
		StoreKitManager.purchaseCancelledEvent -= purchaseCancelledEvent;
#endif
	}
	
	void onPurchasing( SuppliesUIObject item, GameObject tag )
	{
		if( tag.tag == "GoldButton" )
		{
			Debug.Log(tag.name);
			
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				ChartBoostBinding.trackEvent( "Gold Buy Attempt" );
			
			if( tag.name == "Gold" )
				tweenIn();
			else if( tag.name == "GoldButtonBG" )
				tweenOut();
			else if( tag.name == "Coins_L1" )
			{
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				tweenOut();
#elif UNITY_IPHONE
				EtceteraBinding.showBezelActivityViewWithLabel("Connecting to iTunes Store...");
				StoreKitBinding.purchaseProduct( MainMenuController.GOLD_200, 1 );
#endif
			}
			else if( tag.name == "Coins_L2" )
			{
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				tweenOut();
#elif UNITY_IPHONE
				EtceteraBinding.showBezelActivityViewWithLabel("Connecting to iTunes Store...");
				StoreKitBinding.purchaseProduct( MainMenuController.GOLD_500, 1 );
#endif
			}
			else if( tag.name == "Coins_L3" )
			{
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				tweenOut();
#elif UNITY_IPHONE
				EtceteraBinding.showBezelActivityViewWithLabel("Connecting to iTunes Store...");
				StoreKitBinding.purchaseProduct( MainMenuController.GOLD_4000, 1 );
#endif
			}
		}
	}
	
	void tweenIn()
	{
		iTween.MoveTo( gameObject, iTween.Hash(
			"x", 0f,
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
#if UNITY_IPHONE	
	void purchaseSuccessfulEvent( StoreKitTransaction transaction )
	{
		switch( transaction.productIdentifier )
		{
		case MainMenuController.GOLD_200:
			DBAccess.instance.userPrefs.Gold += 200;
			DBAccess.instance.userPrefs.CommitToDB(true);
			break;
		case MainMenuController.GOLD_500:
			DBAccess.instance.userPrefs.Gold += 500;
			DBAccess.instance.userPrefs.CommitToDB(true);
			break;
		case MainMenuController.GOLD_4000:
			DBAccess.instance.userPrefs.Gold += 4000;
			DBAccess.instance.userPrefs.CommitToDB(true);
			break;
		default:
			break;
		}
		
		EtceteraBinding.hideActivityView();
		tweenOut();
	}
	
	void purchaseFailedEvent( string error )
	{
		EtceteraBinding.hideActivityView();
		EtceteraBinding.showAlertWithTitleMessageAndButtons( "Oops...", error, new string[] { "OK" } );
	}
	
	void purchaseCancelledEvent( string error )
	{
		EtceteraBinding.hideActivityView();
		EtceteraBinding.showAlertWithTitleMessageAndButtons( "Oops...", error, new string[] { "OK" } );
	}
#endif
}
