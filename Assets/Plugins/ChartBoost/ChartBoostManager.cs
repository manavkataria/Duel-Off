using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ChartBoostManager : MonoBehaviour
{
#if UNITY_IPHONE
	// Fired when an interstitial fails to load
	public static event Action<string> didFailToLoadInterstitialEvent;

	// Fired when an interstitial is finished
	public static event Action<string> didDismissInterstitialEvent;
	
	// Fired when an interstitial is closed
	public static event Action<string> didCloseInterstitialEvent;
	
	// Fired when an interstitial is clicked
	public static event Action<string> didClickInterstitialEvent;
	
	// Fired when an interstitial is cached
	public static event Action<string> didCacheInterstitialEvent;

	// Fired when the more apps screen fails to load
	public static event Action didFailToLoadMoreAppsEvent;

	// Fired when the more apps screen is finished. Possible reasons are 'dismiss', 'close' and 'click'
	public static event Action<string> didFinishMoreAppsEvent;
	
	// Fired when the more apps screen is cached
	public static event Action didCacheMoreAppsEvent;

	


	void Awake()
	{
		// Set the GameObject name to the class name for easy access from Obj-C
		gameObject.name = this.GetType().ToString();
		DontDestroyOnLoad( this );
	}


	public void didFailToLoadInterstitial( string location )
	{
		if( didFailToLoadInterstitialEvent != null )
			didFailToLoadInterstitialEvent( location );
	}


	public void didDismissInterstitial( string location )
	{
		if( didDismissInterstitialEvent != null )
			didDismissInterstitialEvent( location );
	}
	
	
	public void didClickInterstitial( string location )
	{
		if( didClickInterstitialEvent != null )
			didClickInterstitialEvent( location );
	}

	
	public void didCloseInterstitial( string location )
	{
		if( didCloseInterstitialEvent != null )
			didCloseInterstitialEvent( location );
	}


	public void didFailToLoadMoreApps( string empty )
	{
		if( didFailToLoadMoreAppsEvent != null )
			didFailToLoadMoreAppsEvent();
	}


	public void didFinishMoreApps( string param )
	{
		if( didFinishMoreAppsEvent != null )
			didFinishMoreAppsEvent( param );
	}
	
	
	public void didCacheInterstitial( string location )
	{
		if( didCacheInterstitialEvent != null )
			didCacheInterstitialEvent( location );
	}
	
	
	public void didCacheMoreApps( string empty )
	{
		if( didCacheMoreAppsEvent != null )
			didCacheMoreAppsEvent();
	}
	
#endif
}

