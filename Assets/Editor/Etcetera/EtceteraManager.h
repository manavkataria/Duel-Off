//
//  EtceteraManager.h
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMailComposeViewController.h>
#import "P31AlertView.h"


typedef enum
{
	PhotoTypeCamera,
	PhotoTypeAlbum,
	PhotoTypeBoth
} PhotoType;


@interface EtceteraManager : NSObject <UIAlertViewDelegate, P31AlertViewDelegate, MFMailComposeViewControllerDelegate, MFMessageComposeViewControllerDelegate, UINavigationControllerDelegate, UIImagePickerControllerDelegate, UIActionSheetDelegate>
{
@private
	UIViewController *_viewControllerWrapper;
	id _popoverViewController;
	UIView *_keyboardView;
	
	NSString *_itunesUrl;
	float _hoursBetweenPrompts;
}
@property (nonatomic, retain) NSString *urbanAirshipAppKey;
@property (nonatomic, retain) NSString *urbanAirshipAppSecret;
@property (nonatomic, retain) NSString *urbanAirshipAlias;
@property (nonatomic, assign) float scaledImageSize;
@property (nonatomic, assign) CGRect popoverRect;
@property (nonatomic, assign) BOOL pickerAllowsEditing;
@property (nonatomic, retain) id popoverViewController;

@property (nonatomic, retain) UIView *keyboardView;
@property (nonatomic, retain) UIColor *borderColor;
@property (nonatomic, retain) UIColor *gradientStopOne;
@property (nonatomic, retain) UIColor *gradientStopTwo;
@property (nonatomic, retain) UIWebView *inlineWebView;


+ (EtceteraManager*)sharedManager;

+ (NSString*)stringWithNewUUID;

+ (UIViewController*)unityViewController;

- (void)dismissWrappedController;


// UIAlertView
- (void)showAlertWithTitle:(NSString*)title message:(NSString*)message buttons:(NSArray*)buttons;


// P31AlertView
- (void)setBorderColor:(int)borderColor gradientStopOne:(int)gradientStopOne gradientStopTwo:(int)gradientStopTwo;

- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder:(NSString*)placeHolder autocorrect:(BOOL)autocorrect;

- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder1:(NSString*)placeHolder1 placeHolder2:(NSString*)placeHolder2 autocorrect:(BOOL)autocorrect;


// P31WebController
- (void)showWebControllerWithUrl:(NSString*)url showingControls:(BOOL)showControls;


// Mail
- (BOOL)isEmailAvailable;

- (BOOL)isSMSAvailable;

- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML;

- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML attachment:(NSData*)data mimeType:(NSString*)mimeType filename:(NSString*)filename;

- (void)showSMSComposerWithBody:(NSString*)body;

- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body;

- (void)showSMSComposerWithBody:(NSString*)body;

- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body;


// Rate this app
- (void)askForReviewWithLaunchCount:(int)launchCount hoursBetweenPrompts:(float)hoursBetweenPrompts title:(NSString*)title message:(NSString*)message iTunesUrl:(NSString*)iTunesUrl;

- (void)askForReviewWithTitle:(NSString*)title message:(NSString*)message iTunesUrl:(NSString*)iTunesUrl;


// Photo and Photo Library
- (void)promptForPhotoWithType:(PhotoType)type;


// UDID
- (NSString*)uniqueDeviceIdentifier;

- (NSString*)uniqueGlobalDeviceIdentifier;


// Inline web view
- (void)inlineWebViewShowWithFrame:(CGRect)frame;

- (void)inlineWebViewClose;

- (void)inlineWebViewSetUrl:(NSString*)urlString;

- (void)inlineWebViewSetFrame:(CGRect)frame;

@end

