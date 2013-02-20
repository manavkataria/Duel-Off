//
//  ChartBoostManager.h
//  CB
//
//  Created by Mike DeSaro on 12/20/11.
//


#import <Foundation/Foundation.h>
#import "ChartBoost.h"
#import "AppController.h"



@interface ChartBoostManager : NSObject <ChartboostDelegate>


+ (ChartBoostManager*)sharedManager;


- (void)startChartBoostWithAppId:(NSString*)appId appSignature:(NSString*)appSignature;

- (void)cacheInterstitial:(NSString*)location;

- (void)showInterstitial:(NSString*)location;

- (void)cacheMoreApps;

- (void)showMoreApps;

@end



@interface AppController(ChartBoostBugFix)

- (UIWindow*)window;

@end