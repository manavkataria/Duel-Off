using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ChartBoostEventListener : MonoBehaviour
{
#if UNITY_IPHONE
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		ChartBoostManager.didFailToLoadInterstitialEvent += didFailToLoadInterstitialEvent;
		ChartBoostManager.didDismissInterstitialEvent += didDismissInterstitialEvent;
		ChartBoostManager.didCloseInterstitialEvent += didCloseInterstitialEvent;
		ChartBoostManager.didClickInterstitialEvent += didClickInterstitialEvent;
		ChartBoostManager.didCacheInterstitialEvent += didCacheInterstitialEvent;
		ChartBoostManager.didFailToLoadMoreAppsEvent += didFailToLoadMoreAppsEvent;
		ChartBoostManager.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
		ChartBoostManager.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
	}


	void OnDisable()
	{
		// Remove all event handlers
		ChartBoostManager.didFailToLoadInterstitialEvent -= didFailToLoadInterstitialEvent;
		ChartBoostManager.didDismissInterstitialEvent -= didDismissInterstitialEvent;
		ChartBoostManager.didCloseInterstitialEvent -= didCloseInterstitialEvent;
		ChartBoostManager.didClickInterstitialEvent -= didClickInterstitialEvent;
		ChartBoostManager.didCacheInterstitialEvent -= didCacheInterstitialEvent;
		ChartBoostManager.didFailToLoadMoreAppsEvent -= didFailToLoadMoreAppsEvent;
		ChartBoostManager.didFinishMoreAppsEvent -= didFinishMoreAppsEvent;
		ChartBoostManager.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
	}



	void didFailToLoadInterstitialEvent( string location )
	{
		Debug.Log( "didFailToLoadInterstitialEvent: " + location );
	}


	void didDismissInterstitialEvent( string location )
	{
		Debug.Log( "didDismissInterstitialEvent: " + location );
	}
	
	
	void didCloseInterstitialEvent( string location )
	{
		Debug.Log( "didCloseInterstitialEvent: " + location );
	}
	
	
	void didClickInterstitialEvent( string location )
	{
		Debug.Log( "didClickInterstitialEvent: " + location );
	}
	
	
	void didCacheInterstitialEvent( string location )
	{
		Debug.Log( "didCacheInterstitialEvent: " + location );
	}

	
	void didFailToLoadMoreAppsEvent()
	{
		Debug.Log( "didFailToLoadMoreAppsEvent" );
	}


	void didFinishMoreAppsEvent( string param )
	{
		Debug.Log( "didFinishMoreAppsEvent: " + param );
	}
	
	
	void didCacheMoreAppsEvent()
	{
		Debug.Log( "didCacheMoreAppsEvent" );
	}

#endif
}


