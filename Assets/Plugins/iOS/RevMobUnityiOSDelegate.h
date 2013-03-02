#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "RevMobAdsDelegate.h"


@interface RevMobUnityiOSDelegate : NSObject <RevMobAdsDelegate> {
    NSString *adType;
}

@property (retain) NSString* gameObjectName;

- (RevMobUnityiOSDelegate *)initWithAdType:(NSString *)adType;
@end
