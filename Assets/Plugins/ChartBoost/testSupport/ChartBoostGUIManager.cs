using UnityEngine;
using System.Collections.Generic;


public class ChartBoostGUIManager : MonoBehaviour
{
#if UNITY_IPHONE
	void OnGUI()
	{
		float yPos = 5.0f;
		float xPos = 5.0f;
		float width = ( Screen.width >= 960 || Screen.height >= 960 ) ? 320 : 160;
		float height = ( Screen.width >= 960 || Screen.height >= 960 ) ? 80 : 40;
		float heightPlus = height + 10.0f;
	
	
		if( GUI.Button( new Rect( xPos, yPos, width, height ), "Init" ) )
		{
			ChartBoostBinding.init( "YOUR_APP_ID", "YOUR_APP_SIGNATURE" );
		}
	
	
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Cache Interstitial" ) )
		{
			ChartBoostBinding.cacheInterstitial( "default" );
		}
	
	
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Is Interstitial Cached?" ) )
		{
			Debug.Log( "is cached: " + ChartBoostBinding.hasCachedInterstitial( "default" ) );
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Show Interstitial" ) )
		{
			ChartBoostBinding.showInterstitial( "default" );
		}
	
	
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Cache More Apps" ) )
		{
			ChartBoostBinding.cacheMoreApps();
		}
	
	
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Show More Apps" ) )
		{
			ChartBoostBinding.showMoreApps();
		}
		
		
		xPos = Screen.width - width - 5.0f;
		yPos = 5.0f;
		
		if( GUI.Button( new Rect( xPos, yPos, width, height ), "Track Event" ) )
		{
			ChartBoostBinding.trackEvent( "some_event" );
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Track Event with Metadata" ) )
		{
			var dict = new Dictionary<string,string>();
			dict.Add( "key", "theValue" );
			ChartBoostBinding.trackEventWithMetadata( "some_event_with_data", dict );
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Track Event with Value" ) )
		{
			ChartBoostBinding.trackEventWithValue( "event_with_value", 123 );
		}
		
		
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Track Event with Value and Metadata" ) )
		{
			var dict = new Dictionary<string,string>();
			dict.Add( "key", "theValue" );
			ChartBoostBinding.trackEventWithValueAndMetadata( "event_with_value_and_data", 9809823, dict );
		}
		
	}
#endif
}
