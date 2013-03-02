#import "RevMobUnityiOSDelegate.h"
#import "RevMobAds.h"
#import "RevMobAdvertiser.h"
#import "RevMobAdLink.h"

void UnitySendMessage(const char *className, const char *methodName, const char *param);

#define TO_NSSTRING( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]
#define CREATE_REVMOB_DELEGATE(_AD_TYPE_) [[RevMobUnityiOSDelegate alloc] initWithAdType:_AD_TYPE_]

// Converts NSString to C style string by way of copy (Mono will free it)
#define STRCOPY( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

static NSDictionary* revmobDelegatesDict = nil;
static NSDictionary* revmobDelegates() {
    if (revmobDelegatesDict == nil) {
        revmobDelegatesDict = [[NSDictionary alloc] initWithObjectsAndKeys:
                              CREATE_REVMOB_DELEGATE(@"Fullscreen"), @"Fullscreen",
                              CREATE_REVMOB_DELEGATE(@"Banner"), @"Banner",
                              CREATE_REVMOB_DELEGATE(@"Popup"), @"Popup",
                              CREATE_REVMOB_DELEGATE(@"Link"), @"Link", nil];
    }
    return revmobDelegatesDict;
}

void RevMobUnityiOSBinding_setGameObjectDelegateCallback(const char* gameObjectName) {
    for (RevMobUnityiOSDelegate* revmobDelegate in [revmobDelegates() allValues]) {
        NSLog(@"Set delegate callback to %@", TO_NSSTRING(gameObjectName));
        revmobDelegate.gameObjectName = TO_NSSTRING(gameObjectName);
    }
}

@interface RevMobAds()
+ (void) startSessionWithAppID:(NSString *)appID delegate:(NSObject<RevMobAdsDelegate> *)delegate
                   testingMode:(RevMobAdsTestingMode)testingMode
                       sdkName:(NSString *)sdkName
                    sdkVersion:(NSString *)sdkVersion;
+ (RevMobAds *)sharedObject;
@end

static RevMobAds *revmob = nil;

void RevMobUnityiOSBinding_startSession(const char* appId, const char* version) {
    [RevMobAds startSessionWithAppID:TO_NSSTRING(appId)
                            delegate:nil
                         testingMode:0
                             sdkName:@"unity-ios"
                          sdkVersion:TO_NSSTRING(version)];
    revmob = [RevMobAds session];
}

void RevMobUnityiOSBinding_setTestingMode(int testingMode) {
    [RevMobAds session].testingMode = testingMode;
}

void RevMobUnityiOSBinding_setTimeoutInSeconds(int timeout) {
    [RevMobAds session].connectionTimeout = timeout;
}

NSMutableArray* supportedInterfaceOrientations(int *orientations) {
    NSMutableArray *arrayOfOrientations = nil;
    if (orientations && orientations[0] >= 1 && orientations[0] <= 4) {
        arrayOfOrientations = [[NSMutableArray alloc] initWithCapacity:4];
        for (int i = 0; i < 4; i++) {
            [arrayOfOrientations addObject:[NSNumber numberWithInt:orientations[i]]];
            if (orientations[i] < 1 || orientations[i] > 4) {
                break;
            }
        }
    }
    return arrayOfOrientations;
}

#pragma mark Fullscreen

static RevMobFullscreen *fullscreen = nil;

void RevMobUnityiOSBinding_createFullscreen(const char* placementId) {
    if (fullscreen != nil) {
        [fullscreen release];
        fullscreen = nil;
    }
    if (placementId == NULL) {
        fullscreen = [[revmob fullscreen] retain];
    } else {
        NSLog(@"Using placement id: %@", TO_NSSTRING(placementId));
        fullscreen = [[revmob fullscreenWithPlacementId:TO_NSSTRING(placementId)] retain];
    }
    fullscreen.delegate = [revmobDelegates() valueForKey:@"Fullscreen"];
}

void RevMobUnityiOSBinding_loadFullscreen(const char* placementId) {
    RevMobUnityiOSBinding_createFullscreen(placementId);
    [fullscreen loadAd];
}

void RevMobUnityiOSBinding_showFullscreen(const char* placementId) {
    RevMobUnityiOSBinding_createFullscreen(placementId);
    [fullscreen showAd];
}

bool RevMobUnityiOSBinding_isLoadedFullscreen() {
    if (fullscreen == nil) return NO;
    bool isLoaded = [fullscreen isLoaded];
    return isLoaded;
}

void RevMobUnityiOSBinding_showLoadedFullscreen() {
    if (fullscreen != nil) [fullscreen showAd];
}

void RevMobUnityiOSBinding_releaseLoadedFullscreen() {
    NSLog(@"Releasing fullscreen.");
    [fullscreen release];
    fullscreen = nil;
}

void RevMobUnityiOSBinding_loadFullscreenWithSpecificOrientations(int *orientations) {
    RevMobUnityiOSBinding_loadFullscreen(NULL);
    fullscreen.supportedInterfaceOrientations = supportedInterfaceOrientations(orientations);
}

void RevMobUnityiOSBinding_showFullscreenWithSpecificOrientations(int *orientations) {
    RevMobUnityiOSBinding_loadFullscreenWithSpecificOrientations(orientations);
    [fullscreen showAd];
}

#pragma mark Banner

static RevMobBanner *revmobBanner = nil;

void RevMobUnityiOSBinding_deactivateBannerAd() {
    if (revmobBanner != nil) {
        [revmobBanner release];
        revmobBanner = nil;
    }
}

void RevMobUnityiOSBinding_loadBanner(const char *placementId, float x, float y, float width, float height) {
    RevMobUnityiOSBinding_deactivateBannerAd();
    if (placementId == NULL) {
        revmobBanner = [[revmob banner] retain];
    } else {
        NSLog(@"Using placement id: %@", TO_NSSTRING(placementId));
        revmobBanner = [[revmob bannerWithPlacementId:TO_NSSTRING(placementId)] retain];
    }
    revmobBanner.delegate = [revmobDelegates() valueForKey:@"Banner"];
    if (x != 0 && y != 0 && width != 0 && height != 0) {
        [revmobBanner setFrame:CGRectMake(x, y, width, height)];
    }
}

void RevMobUnityiOSBinding_showBanner(const char *placementId, int *orientations, float x, float y, float width, float height) {
    RevMobUnityiOSBinding_loadBanner(placementId, x, y, width, height);
    revmobBanner.supportedInterfaceOrientations = supportedInterfaceOrientations(orientations);
    [revmobBanner showAd];
}

void RevMobUnityiOSBinding_hideBanner() {
    if (revmobBanner != nil) [revmobBanner hideAd];
}

#pragma mark Ad Link

static RevMobAdLink *revmobAdLink = nil;

void RevMobUnityiOSBinding_loadAdLink(const char *placementId) {
    if ( revmobAdLink != nil) {
        [revmobAdLink release];
        revmobAdLink = nil;
    }
    revmobAdLink = (placementId == NULL) ? [revmob adLink] : [revmob adLinkWithPlacementId:TO_NSSTRING(placementId)];
    [revmobAdLink retain];
    revmobAdLink.delegate = [revmobDelegates() valueForKey:@"Link"];
    [revmobAdLink loadAd];
}

void RevMobUnityiOSBinding_openAdLink(const char *placementId) {
    RevMobUnityiOSBinding_loadAdLink(placementId);
    [revmobAdLink openLink];
}

bool RevMobUnityiOSBinding_isLoadedAdLink() {
    if (revmobAdLink == nil) return NO;
    return [revmobAdLink isLoaded];
}

void RevMobUnityiOSBinding_openLoadedAdLink() {
    if ( RevMobUnityiOSBinding_isLoadedAdLink() ) {
        [revmobAdLink openLink];
    }
}

#pragma mark Popup

void RevMobUnityiOSBinding_showPopup(const char *placementId) {
    if (placementId == NULL) {
        RevMobPopup *popup = [revmob popup];
        popup.delegate = [revmobDelegates() valueForKey:@"Popup"];
        [popup showAd];
    } else {
        if (revmob != nil) {
            NSLog(@"Using placement id: %@", TO_NSSTRING(placementId));
            RevMobPopup *popup = [revmob popupWithPlacementId: TO_NSSTRING(placementId)];
            popup.delegate = [revmobDelegates() valueForKey:@"Popup"];
            [popup showAd];
        }
    }
}


void RevMobUnityiOSBinding_printEnvironmentInformation() {
    if (revmob) {
        [revmob printEnvironmentInformation];
    }
}


@implementation RevMobUnityiOSDelegate

- (RevMobUnityiOSDelegate *)initWithAdType:(NSString *)theAdType {
    self = [super init];
    self.gameObjectName = @"RevMobEventListener";
    adType = theAdType;
    return self;
}

- (void)revmobAdDidReceive {
	UnitySendMessage(STRCOPY(self.gameObjectName), "AdDidReceive", STRCOPY(adType));
}

- (void)revmobAdDidFailWithError:(NSError *)error {
	UnitySendMessage(STRCOPY(self.gameObjectName), "AdDidFail", STRCOPY(adType));
    if ([adType isEqualToString:@"Fullscreen"]) {
        RevMobUnityiOSBinding_releaseLoadedFullscreen();
    }
    NSLog(@"%@", error);
}

- (void)revmobAdDisplayed {
	UnitySendMessage(STRCOPY(self.gameObjectName), "AdDisplayed", STRCOPY(adType));
}

- (void)revmobUserClickedInTheAd {
	UnitySendMessage(STRCOPY(self.gameObjectName), "UserClickedInTheAd", STRCOPY(adType));
    if ([adType isEqualToString:@"Fullscreen"]) {
        RevMobUnityiOSBinding_releaseLoadedFullscreen();
    }

}

- (void)revmobUserClosedTheAd {
	UnitySendMessage(STRCOPY(self.gameObjectName), "UserClosedTheAd", STRCOPY(adType));
    if ([adType isEqualToString:@"Fullscreen"]) {
        RevMobUnityiOSBinding_releaseLoadedFullscreen();
    }
}

# pragma mark Advertiser Callbacks

- (void)installDidReceive {
	UnitySendMessage(STRCOPY(self.gameObjectName), "InstallDidReceive", "InstallDidReceive");
}

- (void)installDidFail {
	UnitySendMessage(STRCOPY(self.gameObjectName), "InstallDidFail", "InstallDidFail");
}
@end