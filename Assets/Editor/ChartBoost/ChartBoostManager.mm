//
//  ChartBoostManager.m
//  CB
//
//  Created by Mike DeSaro on 12/20/11.
//

#import "ChartBoostManager.h"


void UnitySendMessage( const char * className, const char * methodName, const char * param );

void UnityPause( bool pause );


@implementation ChartBoostManager

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (ChartBoostManager*)sharedManager
{
	static ChartBoostManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[ChartBoostManager alloc] init];
	
	return sharedSingleton;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)startChartBoostWithAppId:(NSString*)appId appSignature:(NSString*)appSignature
{
    Chartboost *cb = [Chartboost sharedChartboost];
    cb.appId = appId;
    cb.appSignature = appSignature;
    cb.delegate = self;
    
    [cb startSession];
}


- (void)cacheInterstitial:(NSString*)location
{
    if( location )
        [[Chartboost sharedChartboost] cacheInterstitial:location];
    else
        [[Chartboost sharedChartboost] cacheInterstitial];
}


- (void)showInterstitial:(NSString*)location
{
    if( location )
        [[Chartboost sharedChartboost] showInterstitial:location];
    else
        [[Chartboost sharedChartboost] showInterstitial];
}


- (void)cacheMoreApps
{
    [[Chartboost sharedChartboost] cacheMoreApps];
}


- (void)showMoreApps
{
    [[Chartboost sharedChartboost] showMoreApps];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark ChartboostDelegate

- (BOOL)shouldDisplayInterstitial:(NSString*)location
{
    UnityPause( true );
    return YES;
}


// Called when an interstitial has failed to come back from the server
- (void)didFailToLoadInterstitial:(NSString*)location
{
    UnitySendMessage( "ChartBoostManager", "didFailToLoadInterstitial", location.UTF8String );
}


- (void)didCacheInterstitial:(NSString*)location
{
	UnitySendMessage( "ChartBoostManager", "didCacheInterstitial", location.UTF8String );
}


// Called when the user dismisses the interstitial
- (void)didDismissInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didDismissInterstitial", location.UTF8String );
}


// Same as above, but only called when dismissed for a close
- (void)didCloseInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didCloseInterstitial", location.UTF8String );
}


// Same as above, but only called when dismissed for a click
- (void)didClickInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didClickInterstitial", location.UTF8String );
}


// Called when a more apps page has failed to come back from the server
- (void)didFailToLoadMoreApps
{
    UnitySendMessage( "ChartBoostManager", "didFailToLoadMoreApps", "" );
}


- (void)didCacheMoreApps
{
	UnitySendMessage( "ChartBoostManager", "didCacheMoreApps", "" );
}


- (BOOL)shouldDisplayMoreApps:(UIView*)moreAppsView
{
    UnityPause( true );
    return YES;
}


- (void)didDismissMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didFinishMoreApps", "dismiss" );
}


// Same as above, but only called when dismissed for a close
- (void)didCloseMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didFinishMoreApps", "close" );
}


// Same as above, but only called when dismissed for a click
- (void)didClickMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartBoostManager", "didFinishMoreApps", "click" );
}

@end



@implementation AppController(ChartBoostBugFix)

- (UIWindow*)window
{
	return [UIApplication sharedApplication].keyWindow;
}

@end
