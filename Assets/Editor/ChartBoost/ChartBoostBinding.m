//
//  ChartBoostBinding.m
//  CB
//
//  Created by Mike DeSaro on 12/20/11.
//

#import "ChartBoostManager.h"
#import "JSONKit.h"
#import "CBAnalytics.h"


// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil



void _chartBoostInit( const char * appId, const char * appSignature )
{
    [[ChartBoostManager sharedManager] startChartBoostWithAppId:GetStringParam( appId ) appSignature:GetStringParam( appSignature )];
}


void _chartBoostCacheInterstitial( const char * location )
{
    [[ChartBoostManager sharedManager] cacheInterstitial:GetStringParamOrNil( location )];
}


BOOL _chartBoostHasCachedInterstitial( const char * location )
{
	return [[Chartboost sharedChartboost] hasCachedInterstitial:GetStringParamOrNil( location )];
}


void _chartBoostShowInterstitial( const char * location )
{
    [[ChartBoostManager sharedManager] showInterstitial:GetStringParamOrNil( location )];
}


void _chartBoostCacheMoreApps()
{
    [[ChartBoostManager sharedManager] cacheMoreApps];
}


void _chartBoostShowMoreApps()
{
    [[ChartBoostManager sharedManager] showMoreApps];
}


void _chartBoostForceOrientation( const char * orient )
{
	NSString *orientation = GetStringParam( orient );
	
	if( [orientation isEqualToString:@"LandscapeLeft"] )
		[Chartboost sharedChartboost].orientation = UIInterfaceOrientationLandscapeLeft;
	else if( [orientation isEqualToString:@"LandscapeRight"] )
		[Chartboost sharedChartboost].orientation = UIInterfaceOrientationLandscapeRight;
	else if( [orientation isEqualToString:@"Portrait"] )
		[Chartboost sharedChartboost].orientation = UIInterfaceOrientationPortrait;
	else if( [orientation isEqualToString:@"PortraitUpsideDown"] )
		[Chartboost sharedChartboost].orientation = UIInterfaceOrientationPortraitUpsideDown;
}


void _chartBoostTrackEvent( const char * eventIdentifier )
{
	[[CBAnalytics sharedAnalytics] trackEvent:GetStringParam( eventIdentifier )];
}


void _chartBoostTrackEventWithMetadata( const char * eventIdentifier, const char * metadata )
{
	NSDictionary *dict = [GetStringParam( metadata ) objectFromJSONString];
	[[CBAnalytics sharedAnalytics] trackEvent:GetStringParam( eventIdentifier ) withMetadata:dict];
}


void _chartBoostTrackEventWithValue( const char * eventIdentifier, float value )
{
	[[CBAnalytics sharedAnalytics] trackEvent:GetStringParam( eventIdentifier ) withValue:[NSNumber numberWithFloat:value]];
}


void _chartBoostTrackEventWithValueAndMetadata( const char * eventIdentifier, float value, const char * metadata )
{
	NSDictionary *dict = [GetStringParam( metadata ) objectFromJSONString];
	[[CBAnalytics sharedAnalytics] trackEvent:GetStringParam( eventIdentifier ) withValue:[NSNumber numberWithFloat:value] andMetadata:dict];
}

