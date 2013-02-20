using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;



#if UNITY_IPHONE
public class ChartBoostBinding
{
	[DllImport("__Internal")]
	private static extern void _chartBoostInit( string appId, string appSignature );

	// Starts up ChartBoost and records an app install
	public static void init( string appId, string appSignature )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostInit( appId, appSignature );
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostCacheInterstitial( string location );

	// Caches an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void cacheInterstitial( string location )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostCacheInterstitial( location );
	}
	
	
	[DllImport("__Internal")]
	private static extern bool _chartBoostHasCachedInterstitial( string location );

	// Checks to see if an interstitial is cached
	public static bool hasCachedInterstitial( string location )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			return _chartBoostHasCachedInterstitial( location );
		
		return false;
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostShowInterstitial( string location );

	// Shows an interstitial. Location is optional. Pass in null if you do not want to specify the location.
	public static void showInterstitial( string location )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostShowInterstitial( location );
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostCacheMoreApps();

	// Caches the more apps screen
	public static void cacheMoreApps()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostCacheMoreApps();
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostShowMoreApps();

	// Shows the more apps screen
	public static void showMoreApps()
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostShowMoreApps();
	}
	
	
	[DllImport("__Internal")]
	private static extern void _chartBoostForceOrientation( string orient );

	// Forces the orientation of interstital ads. If your project is proper setup to autoroate animated native views will work as expected and you should not need to set this
	public static void forceOrientation( ScreenOrientation orient )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostForceOrientation( orient.ToString() );
	}
	
	
	#region event tracking
	
	[DllImport("__Internal")]
	private static extern void _chartBoostTrackEvent( string eventIdentifier );

	// Tracks an event
	public static void trackEvent( string eventIdentifier )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostTrackEvent( eventIdentifier );
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostTrackEventWithMetadata( string eventIdentifier, string metadata );

	// Tracks an event with additional metadata
	public static void trackEventWithMetadata( string eventIdentifier, Dictionary<string,string> metadata )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostTrackEventWithMetadata( eventIdentifier, metadata.toJson() );
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostTrackEventWithValue( string eventIdentifier, float value );

	// Tracks an event with a value
	public static void trackEventWithValue( string eventIdentifier, float value )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostTrackEventWithValue( eventIdentifier, value );
	}


	[DllImport("__Internal")]
	private static extern void _chartBoostTrackEventWithValueAndMetadata( string eventIdentifier, float value, string metadata );

	// Tracks an event with a value and additional metadata
	public static void trackEventWithValueAndMetadata( string eventIdentifier, float value, Dictionary<string,string> metadata )
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer )
			_chartBoostTrackEventWithValueAndMetadata( eventIdentifier, value, metadata.toJson() );
	}
	
	#endregion

}
#endif