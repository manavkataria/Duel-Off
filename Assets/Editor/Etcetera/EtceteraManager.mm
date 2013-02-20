//
//  EtceteraManager.m
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "EtceteraManager.h"
#import "P31WebController.h"
#include <sys/socket.h>
#include <sys/sysctl.h>
#include <net/if.h>
#include <net/if_dl.h>
#import <CommonCrypto/CommonDigest.h>


void UnityPause( bool pause );

void UnitySendMessage( const char * className, const char * methodName, const char * param );

UIViewController *UnityGetGLViewController();


UIColor * ColorFromHex( int hexcolor )
{
	int r = ( hexcolor >> 24 ) & 0xFF;
	int g = ( hexcolor >> 16 ) & 0xFF;
	int b = ( hexcolor >> 8 ) & 0xFF;
	int a = hexcolor & 0xFF;

	return [UIColor colorWithRed:(r/255.0) green:(g/255.0) blue:(b/255.0) alpha:(a/255.0)];
}


// UIAlertView tags
#define kStandardAlertTag		1111
#define kRTAAlertTag			4444
#define kRTAAlertTagNoOptions	7777

// RTA defaults keys
#define kRTADontAskAgain			@"RTADontAskAgain"
#define kRTALastReviewedVersion		@"RTALastReviewedVersion"
#define kRTANextTimeToAsk			@"RTANextTimeToAsk"
#define kRTATimesLaunchedSinceAsked	@"RTATimesLaunchedSinceAsked"



@implementation EtceteraManager

@synthesize urbanAirshipAppKey = _urbanAirshipAppKey, urbanAirshipAppSecret = _urbanAirshipAppSecret, scaledImageSize = _scaledImageSize,
			borderColor = _borderColor, gradientStopOne = _gradientStopOne, gradientStopTwo = _gradientStopTwo,
			keyboardView = _keyboardView, popoverRect, pickerAllowsEditing = _pickerAllowsEditing, popoverViewController = _popoverViewController, urbanAirshipAlias,
			inlineWebView;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Class Method

+ (NSString*)stringWithNewUUID
{
    // Create a new UUID
    CFUUIDRef uuidObj = CFUUIDCreate(nil);
    
    // Get the string representation of the UUID
    NSString *newUUID = (NSString*)CFUUIDCreateString( nil, uuidObj );
    CFRelease( uuidObj );
    return [newUUID autorelease];
}


+ (UIViewController*)unityViewController
{
	return UnityGetGLViewController();
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (EtceteraManager*)sharedManager
{
	static EtceteraManager *sharedManager = nil;
	
	if( !sharedManager )
		sharedManager = [[EtceteraManager alloc] init];
	
	return sharedManager;
}


- (id)init
{
	if( ( self = [super init] ) )
	{
		_pickerAllowsEditing = NO;
		_scaledImageSize = 1.0f;
		popoverRect = CGRectMake( 20, 15, 10, 0 );
	}
	return self;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (UIViewController*)getViewControllerForModalPresentation:(BOOL)destroyIfExists
{
	return UnityGetGLViewController();

	/* pre Unity 3.4 path
	if( destroyIfExists && _viewControllerWrapper )
	{
		[_viewControllerWrapper dismissModalViewControllerAnimated:NO];
		[_viewControllerWrapper.view removeFromSuperview];
		[_viewControllerWrapper release];
		_viewControllerWrapper = nil;
	}
	else if( !_viewControllerWrapper )
	{
		// Create a wrapper controller to house the picker
		_viewControllerWrapper = [[UIViewController alloc] initWithNibName:nil bundle:nil];
		
		// add the wrapper to the window
		[[UIApplication sharedApplication].keyWindow addSubview:_viewControllerWrapper.view];
	}
	
	// zero the frame so it is hidden
	_viewControllerWrapper.view.frame = CGRectZero;
	
	return _viewControllerWrapper;
	*/
}


- (void)showViewControllerModallyInWrapper:(UIViewController*)viewController
{
	// pause the game
	UnityPause( true );
	
	// cancel the previous delayed call to dismiss the view controller if it exists
	[NSObject cancelPreviousPerformRequestsWithTarget:self];

	UIViewController *vc = [self getViewControllerForModalPresentation:YES];
	
	// show the mail composer on iPad in a form sheet
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [viewController isKindOfClass:[MFMailComposeViewController class]] )
		viewController.modalPresentationStyle = UIModalPresentationFormSheet;
	
	// show the view controller
	[vc presentModalViewController:viewController animated:YES];
}


- (void)dismissWrappedController
{
	UnityPause( false );

	UIViewController *vc = [self getViewControllerForModalPresentation:NO];
	
	// No view controller? Get out of here.
	if( !vc )
		return;
	
	// dismiss the view controller
	[vc dismissModalViewControllerAnimated:YES];

	// remove the wrapper view controller
	[self performSelector:@selector(removeAndReleaseViewControllerWrapper) withObject:nil afterDelay:1.0];
	
	UnitySendMessage( "EtceteraManager", "dismissingViewController", "" );
}


- (void)removeAndReleaseViewControllerWrapper
{
	// iPad might have a popover
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && _popoverViewController )
	{
		[_popoverViewController dismissPopoverAnimated:YES];
		self.popoverViewController = nil;
	}
	else if( _viewControllerWrapper )
	{
		[_viewControllerWrapper.view removeFromSuperview];
		[_viewControllerWrapper release];
		_viewControllerWrapper = nil;
	}
}


- (void)extractUnityKeyboardView
{
	// kill the Unity keyboard view for now.  we will readd it later
	for( UIView *v in [UIApplication sharedApplication].keyWindow.subviews )
	{
		if( [v isMemberOfClass:[UIView class]] && v.hidden )
		{
			// retain the keyboard view
			self.keyboardView = v;
			[v removeFromSuperview];
		}
	}
}


- (void)injectUnityKeyboardView
{
	// early out if we dont have the keyboard view saved
	if( !_keyboardView )
		return;
	
	[[UIApplication sharedApplication].keyWindow addSubview:_keyboardView];
	self.keyboardView = nil;
}


- (NSString*)MD5HashString:(NSString*)input
{
    if( input == nil || input.length == 0 )
        return nil;
	
	NSData *data = [input dataUsingEncoding:NSUTF8StringEncoding];
    
	unsigned char result[CC_MD5_DIGEST_LENGTH];
	CC_MD5( data.bytes, data.length, result );
	
	return [NSString stringWithFormat:
			@"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
			result[0], result[1], result[2], result[3], result[4], result[5], result[6], result[7],
			result[8], result[9], result[10], result[11], result[12], result[13], result[14], result[15]
			];
}


- (NSString*)macaddress
{
    int mib[6];
    size_t len;
    char *buf;
    unsigned char *ptr;
    struct if_msghdr *ifm;
    struct sockaddr_dl *sdl;
    
    mib[0] = CTL_NET;
    mib[1] = AF_ROUTE;
    mib[2] = 0;
    mib[3] = AF_LINK;
    mib[4] = NET_RT_IFLIST;
    
    if( ( mib[5] = if_nametoindex( "en0" ) ) == 0 )
	{
        printf("Error: if_nametoindex error\n");
        return NULL;
    }
    
    if( sysctl( mib, 6, NULL, &len, NULL, 0 ) < 0 )
	{
        printf("Error: sysctl, take 1\n");
        return NULL;
    }
    
	buf = (char*)malloc( len );
    if( buf == NULL )
	{
        printf("Could not allocate memory. error!\n");
        return NULL;
    }
    
    if( sysctl( mib, 6, buf, &len, NULL, 0 ) < 0 )
	{
        printf("Error: sysctl, take 2");
        return NULL;
    }
    
    ifm = (struct if_msghdr*)buf;
    sdl = (struct sockaddr_dl*)( ifm + 1 );
    ptr = (unsigned char*)LLADDR( sdl );
    NSString *outstring = [NSString stringWithFormat:@"%02X:%02X:%02X:%02X:%02X:%02X", 
                           *ptr, *(ptr+1), *(ptr+2), *(ptr+3), *(ptr+4), *(ptr+5)];
    free( buf );
    
    return outstring;
}


- (void)image:(UIImage*)image didFinishSavingWithError:(NSError*)error contextInfo:(void*)contextInfo
{
	NSLog( @"image:didFinishSavingWithError:contextInfo: completed" );
	
	if( error )
		NSLog( @"image:didFinishSavingWithError:contextInfo: %@", error );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

// UIAlertView
- (void)showAlertWithTitle:(NSString*)title message:(NSString*)message buttons:(NSArray*)buttons
{
	UnityPause( true );
	UIAlertView *alert = [[[UIAlertView alloc] init] autorelease];
	alert.delegate = self;
	alert.title = title;
	alert.message = message;
	
	for( NSString *b in buttons )
		[alert addButtonWithTitle:b];
	
	alert.tag = kStandardAlertTag;
	[alert show];
}


// P31AlertView
- (void)setBorderColor:(int)borderColor gradientStopOne:(int)gradientStopOne gradientStopTwo:(int)gradientStopTwo
{
	self.borderColor = ColorFromHex( borderColor );
	self.gradientStopOne = ColorFromHex( gradientStopOne );
	self.gradientStopTwo = ColorFromHex( gradientStopTwo );
}


- (void)setPromptColors:(P31AlertView*)alert
{
	if( _borderColor )
		[alert setBorderColor:_borderColor];
	
	if( _gradientStopOne && _gradientStopTwo )
		[alert setBackgroundGradientStop1:_gradientStopOne stop2:_gradientStopTwo];
}


- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder:(NSString*)placeHolder autocorrect:(BOOL)autocorrect
{
	UnityPause( true );
	P31AlertView *alert = [[P31AlertView alloc] initWithTitle:title
													  message:message
													 delegate:self
											cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
												okButtonTitle:NSLocalizedString( @"OK", nil )];
	
	// Add colors
	[self setPromptColors:alert];
	
	// Add a textField
	[alert addTextFieldWithValue:nil placeHolder:placeHolder];
	
	if( !autocorrect )
		[alert textFieldAtIndex:0].autocorrectionType = UITextAutocorrectionTypeNo;
	
	[alert show];
	[alert release];
}


- (void)showPromptWithTitle:(NSString*)title message:(NSString*)message placeHolder1:(NSString*)placeHolder1 placeHolder2:(NSString*)placeHolder2 autocorrect:(BOOL)autocorrect
{
	UnityPause( true );
	P31AlertView *alert = [[P31AlertView alloc] initWithTitle:title
													  message:message
													 delegate:self
											cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
												okButtonTitle:NSLocalizedString( @"OK", nil )];
	
	// Add colors
	[self setPromptColors:alert];
	
	// Add a textField
	[alert addTextFieldWithValue:nil placeHolder:placeHolder1];
	[alert addTextFieldWithValue:nil placeHolder:placeHolder2];
	
	if( !autocorrect )
	{
		[alert textFieldAtIndex:0].autocorrectionType = UITextAutocorrectionTypeNo;
		[alert textFieldAtIndex:1].autocorrectionType = UITextAutocorrectionTypeNo;
	}
	
	// If the second placeHolder has 'password' in it, make it a password field
	if( [placeHolder2 hasPrefix:@"password"] )
	{
		[alert textFieldAtIndex:1].secureTextEntry = YES;
	}
	
	[alert show];
	[alert release];
}


// P31WebController
- (void)showWebControllerWithUrl:(NSString*)url showingControls:(BOOL)showControls
{
	UnityPause( true );
	
	P31WebController *webCon = [[P31WebController alloc] initWithUrl:url showControls:showControls];
	UINavigationController *navCon = [[UINavigationController alloc] initWithRootViewController:webCon];
	[self showViewControllerModallyInWrapper:navCon];
	[navCon release];
	[webCon release];
}


// Mail and SMS
- (BOOL)isEmailAvailable
{
	return [MFMailComposeViewController canSendMail];
}


- (BOOL)isSMSAvailable
{
	Class composerClass = NSClassFromString( @"MFMessageComposeViewController" );
	
	if( !composerClass )
		return NO;
	
	return [composerClass canSendText];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML
{
	[self showMailComposerWithTo:toAddress
						 subject:subject
							body:body
						  isHTML:isHTML
					  attachment:nil
						mimeType:nil
						filename:nil];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML attachment:(NSData*)data mimeType:(NSString*)mimeType filename:(NSString*)filename
{
	// early out if email isnt setup
	if( ![self isEmailAvailable] )
		return;
	
	MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
	mailer.mailComposeDelegate = self;
	
	[mailer setSubject:subject];
	[mailer setMessageBody:body isHTML:isHTML];
	
	// Add the to address if we have one and it has an '@'
	if( toAddress && toAddress.length && [toAddress rangeOfString:@"@"].location != NSNotFound )
		[mailer setToRecipients:[NSArray arrayWithObject:toAddress]];
	
	// Add the attachment if we have one
	if( data && filename && mimeType )
		[mailer addAttachmentData:data mimeType:mimeType fileName:filename];
	
	[self showViewControllerModallyInWrapper:mailer];
}


- (void)showMailComposerWithTo:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML imageAttachment:(NSData*)imageData
{
	// early out if email isnt setup
	if( ![self isEmailAvailable] )
		return;
	
	MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
	mailer.mailComposeDelegate = self;
	
	[mailer setSubject:subject];
	[mailer setMessageBody:body isHTML:isHTML];
	
	// Add the to address if we have one and it has an '@'
	if( toAddress && toAddress.length && [toAddress rangeOfString:@"@"].location != NSNotFound )
		[mailer setToRecipients:[NSArray arrayWithObject:toAddress]];
	
	// Add the attachment if we have one
	if( imageData )
		[mailer addAttachmentData:imageData mimeType:@"image/png" fileName:@"image.png"];
	
	[self showViewControllerModallyInWrapper:mailer];
}


- (void)showSMSComposerWithBody:(NSString*)body
{
	[self showSMSComposerWithRecipients:nil body:body];
}


- (void)showSMSComposerWithRecipients:(NSArray*)recipients body:(NSString*)body
{
	if( ![self isSMSAvailable] )
		return;
	
	[UIApplication sharedApplication].statusBarHidden = NO;

	MFMessageComposeViewController *controller = [[MFMessageComposeViewController alloc] init];
	controller.body = body;
	controller.recipients = recipients;
	controller.messageComposeDelegate = self;
	
	// kill the keyboard view for this one
	//[self extractUnityKeyboardView];
	
	[self showViewControllerModallyInWrapper:controller];
}


// Rate This App

// checks for if the user asked not to be asked to review the app, makes sure it has been 2 days since last asking and checks to see if the app
// version was already reviewed
- (BOOL)isAppEligibleForReviewWithLaunchCount:(int)launchCount
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	
	// If the user doesnt want us to ever ask this question than dont ask
	if( [defaults boolForKey:kRTADontAskAgain] )
		return NO;
	
	// Grab the current version from the bundle and the last reviewed version
	NSString *currentVersion = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
	NSString *lastReviewedVersion = [defaults stringForKey:kRTALastReviewedVersion];
	
	// If this version has been reviewed, than get out of here
	if( [lastReviewedVersion isEqualToString:currentVersion] )
		return NO;
	
	// Take care of setting the launch count and keeping it up to date
	int count = [defaults integerForKey:kRTATimesLaunchedSinceAsked];
	[defaults setInteger:++count forKey:kRTATimesLaunchedSinceAsked];
	
	// see if we were launched enough times yet
	if( count < launchCount )
		return NO;
	
	// If we don't have a next time to ask yet, set it for 2 days from now.  We never prompt on the first run
	const double currentTime = CFAbsoluteTimeGetCurrent();
	if( [defaults objectForKey:kRTANextTimeToAsk] == nil )
	{
		const double nextTime = currentTime + _hoursBetweenPrompts * 60 * 60;
		[defaults setDouble:nextTime forKey:kRTANextTimeToAsk];
		return NO;
	}
	
	// Grab the next time we should ask and see if we are good to bug the user again
	const double nextTime = [defaults doubleForKey:kRTANextTimeToAsk];
	if( currentTime > nextTime )
		return YES;
	
	return NO;
}


// will ask for review if: the user didnt click 'dont ask again', it has been at least 2 days since last asking or since the first launch,
// the current version has not been reviewed and the app has been launched more than launchCount times since the last review
- (void)askForReviewWithLaunchCount:(int)launchCount hoursBetweenPrompts:(float)hoursBetweenPrompts title:(NSString*)title message:(NSString*)message iTunesUrl:(NSString*)iTunesUrl
{
	// store this globally for easy access
	_hoursBetweenPrompts = hoursBetweenPrompts;
	
	// early out if we don't pass the isEligible test
	if( ![self isAppEligibleForReviewWithLaunchCount:launchCount] )
		return;
	
	UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title
													message:message
												   delegate:self
										  cancelButtonTitle:NSLocalizedString( @"Remind me later", nil )
										  otherButtonTitles:NSLocalizedString( @"Yes, rate it!", nil ), NSLocalizedString( @"Don't ask again", nil ), nil];
	alert.tag = kRTAAlertTag;
	[alert show];
	[alert release];
	
	// Save the iTunesUrl for now
	_itunesUrl = [iTunesUrl retain];
}


// will ask for review no matter what
- (void)askForReviewWithTitle:(NSString*)title message:(NSString*)message iTunesUrl:(NSString*)iTunesUrl
{
	UIAlertView *alert = [[UIAlertView alloc] initWithTitle:title
													message:message
												   delegate:self
										  cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
										  otherButtonTitles:NSLocalizedString( @"OK!", nil ), nil];
	alert.tag = kRTAAlertTagNoOptions;
	[alert show];
	[alert release];
	
	// Save the iTunesUrl for now
	_itunesUrl = [iTunesUrl retain];
}


// Photo Library and Camera
- (void)showPicker:(UIImagePickerControllerSourceType)type
{
	UIImagePickerController *picker = [[[UIImagePickerController alloc] init] autorelease];
	picker.delegate = self;
	picker.sourceType = type;
	picker.allowsEditing = _pickerAllowsEditing;
	
	// We need to display this in a popover on iPad
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
	{
		Class popoverClass = NSClassFromString( @"UIPopoverController" );
		if( !popoverClass )
			return;

		_popoverViewController = [[popoverClass alloc] initWithContentViewController:picker];
		[_popoverViewController setDelegate:self];
		//picker.modalInPopover = YES;
		
		// Display the popover
		UIWindow *window = [UIApplication sharedApplication].keyWindow;
		[_popoverViewController presentPopoverFromRect:popoverRect
												inView:window
							  permittedArrowDirections:UIPopoverArrowDirectionAny
											  animated:YES];
	}
	else
	{
		// wrap and show the modal
		[self showViewControllerModallyInWrapper:picker];
	}
}


- (void)popoverControllerDidDismissPopover:(UIPopoverController*)popoverController
{
	self.popoverViewController = nil;
	UnityPause( false );
	
	UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
}


- (void)promptForPhotoWithType:(PhotoType)type
{
	UnityPause( true );
	
	// No need to give a choice for devices with no camera
	if( ![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera] )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
		return;
	}
	
	if( type == PhotoTypeAlbum )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
		return;
	}
	else if( type == PhotoTypeCamera )
	{
		[self showPicker:UIImagePickerControllerSourceTypeCamera];
		return;
	}
	
	UIActionSheet *sheet = [[UIActionSheet alloc] initWithTitle:nil
													   delegate:self
											  cancelButtonTitle:NSLocalizedString( @"Cancel", nil )
										 destructiveButtonTitle:nil
											  otherButtonTitles:NSLocalizedString( @"Take Photo", nil ), NSLocalizedString( @"Choose Existing Photo", nil ), nil];
	
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
		[sheet showFromRect:popoverRect inView:UnityGetGLViewController().view animated:YES];
	else
		[sheet showInView:UnityGetGLViewController().view];
	
	[sheet release];
}


// UUID
- (NSString*)uniqueDeviceIdentifier
{
    NSString *stringToHash = [NSString stringWithFormat:@"%@%@", [self macaddress], [[NSBundle mainBundle] bundleIdentifier]];
	return [self MD5HashString:stringToHash];
}


- (NSString*)uniqueGlobalDeviceIdentifier
{
    return [self MD5HashString:[self macaddress]];
}


// Inline web view
- (void)inlineWebViewShowWithFrame:(CGRect)frame
{
	if( inlineWebView )
		[self inlineWebViewClose];
	
	inlineWebView = [[UIWebView alloc] initWithFrame:frame];
	inlineWebView.scalesPageToFit = YES;
	inlineWebView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
	[UnityGetGLViewController().view addSubview:inlineWebView];
}


- (void)inlineWebViewClose
{
	[inlineWebView removeFromSuperview];
	self.inlineWebView = nil;
}


- (void)inlineWebViewSetUrl:(NSString*)urlString
{
	NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:urlString]];
	[inlineWebView loadRequest:request];
}


- (void)inlineWebViewSetFrame:(CGRect)frame
{
	[UIView beginAnimations:nil context:NULL];
	inlineWebView.frame = frame;
	[UIView commitAnimations];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIActionSheetDelegate

- (void)actionSheet:(UIActionSheet*)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex
{
	if( buttonIndex == 0 )
	{
		[self showPicker:UIImagePickerControllerSourceTypeCamera];
	}
	else if( buttonIndex == 1 )
	{
		[self showPicker:UIImagePickerControllerSourceTypePhotoLibrary];
	}
	else // Cancelled
	{
		UnityPause( false );
		UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIImagePickerControllerDelegate

- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
	// Grab the image and write it to disk
	UIImage *image;
	
	if( _pickerAllowsEditing )
		image = [info objectForKey:UIImagePickerControllerEditedImage];
	else
		image = [info objectForKey:UIImagePickerControllerOriginalImage];

	// Do the save and resize on a background thread if we are on iOS 4 > (UIKit is threadsafe there)
	if( NULL != UIGraphicsBeginImageContextWithOptions )
		[self performSelectorInBackground:@selector(processImageFromImagePicker:) withObject:image];
	else
		[self performSelector:@selector(processImageFromImagePicker:) withObject:image];

	// Dimiss the pickerController
	[self dismissWrappedController];
}


- (void)processImageFromImagePicker:(UIImage*)image
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	// Get a filepath pointing to the docs directory
	NSArray *dirs = NSSearchPathForDirectoriesInDomains( NSDocumentDirectory, NSUserDomainMask, YES );
	NSString *filename = [NSString stringWithFormat:@"%@.png", [EtceteraManager stringWithNewUUID]];
	NSString *filePath = [[dirs objectAtIndex:0] stringByAppendingPathComponent:filename];
	
	// Shrink the monster image down
	if( _scaledImageSize != 1.0f )
	{
		float width = image.size.width * _scaledImageSize;
		float height = image.size.height * _scaledImageSize;
		CGSize targetSize = CGSizeMake( width, height );
		UIGraphicsBeginImageContext( targetSize );
		[image drawInRect:CGRectMake( 0, 0, targetSize.width, targetSize.height )];
		UIImage *targetImage = UIGraphicsGetImageFromCurrentImageContext();
		UIGraphicsEndImageContext();
		
		image = targetImage;
	}

	[UIImagePNGRepresentation( image ) writeToFile:filePath atomically:NO];
	
	[self performSelectorOnMainThread:@selector(notifyUnityOfSavedImageAtPath:) withObject:filePath waitUntilDone:NO];
	
	[pool release];
}


- (void)notifyUnityOfSavedImageAtPath:(NSString*)filePath
{
	// Message back to Unity
	UnitySendMessage( "EtceteraManager", "imageSavedToDocuments", [filePath UTF8String] );	
}


- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
	// dismiss the wrapper, unpause and notifiy Unity what happened
	[self dismissWrappedController];
	UnityPause( false );
	UnitySendMessage( "EtceteraManager", "imagePickerDidCancel", "" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIAlertViewDelegate

- (void)alertView:(UIAlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
	UnityPause( false );

	// always dump the button clicked
	NSString *title = [alertView buttonTitleAtIndex:buttonIndex];
	UnitySendMessage( "EtceteraManager", "alertViewClickedButton", [title UTF8String] );

	if( alertView.tag == kRTAAlertTag || alertView.tag == kRTAAlertTagNoOptions ) // Rate this app. We can share with the no options version
	{
		switch( buttonIndex )
		{
			case 0: // remind me later
			{
				const double nextTime = CFAbsoluteTimeGetCurrent() + _hoursBetweenPrompts * 60 * 60;
				[[NSUserDefaults standardUserDefaults] setDouble:nextTime forKey:kRTANextTimeToAsk];
				break;
			}
			case 1: // rate it now
			{
				// grab the current version and save it in the defaults
				NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
				[[NSUserDefaults standardUserDefaults] setValue:version forKey:kRTALastReviewedVersion];
				
				// http://creativealgorithms.com/blog/content/review-app-links-sorted-out
				// http://bjango.com/articles/ituneslinks/
				
				[[UIApplication sharedApplication] openURL:[NSURL URLWithString:_itunesUrl]];
				break;
			}
			case 2: // don't ask again
			{
				[[NSUserDefaults standardUserDefaults] setBool:YES forKey:kRTADontAskAgain];
				break;
			}
		}
		
		// release the url and reset the launch count
		[_itunesUrl release];
		_itunesUrl = nil;
		[[NSUserDefaults standardUserDefaults] setInteger:0 forKey:kRTATimesLaunchedSinceAsked];
	}
}
						   

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark P31AlertViewDelegate

- (void)alertView:(P31AlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex withTitle:(NSString*)title
{
	UnityPause( false );
	
	// We either return the text entered or a cancelled message
	if( [title isEqualToString:alertView.cancelButtonTitle] )
	{
		UnitySendMessage( "EtceteraManager", "alertPromptCancelled", "" );
	}
	else
	{
		// we either return one or two pieces of text
		NSString *returnText;
		if( alertView.textFields.count == 1 )
			returnText = [alertView textForTextFieldAtIndex:0];
		else
			returnText = [NSString stringWithFormat:@"%@|||%@", [alertView textForTextFieldAtIndex:0], [alertView textForTextFieldAtIndex:1]];
		
		UnitySendMessage( "EtceteraManager", "alertPromptEnteredText", [returnText UTF8String] );
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark MFMailComposerDelegate

- (void)mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError*)error
{
	[self dismissWrappedController];
	
	NSString *resultString = nil;
	
	switch( result )
	{
		case MFMailComposeResultCancelled:
			resultString = @"Cancelled";
			break;
		case MFMailComposeResultSaved:
			resultString = @"Saved";
			break;
		case MFMailComposeResultSent:
			resultString = @"Sent";
			break;
		case MFMailComposeResultFailed:
			resultString = @"Failed";
			break;
		default:
			resultString = @"";
	}
	
	UnitySendMessage( "EtceteraManager", "mailComposerFinishedWithResult", [resultString UTF8String] );
	
	// autorelease this after 2 seconds to avoid an odd crash when another mail composer is presented
	[controller performSelector:@selector(autorelease) withObject:nil afterDelay:2.0];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark MFMessageComposeViewControllerDelegate

- (void)messageComposeViewController:(MFMessageComposeViewController*)controller didFinishWithResult:(MessageComposeResult)result
{
	[self dismissWrappedController];
	
	NSString *resultString = nil;
	
	switch( result )
	{
		case MessageComposeResultCancelled:
			resultString = @"Cancelled";
			break;
		case MessageComposeResultSent:
			resultString = @"Sent";
			break;
		case MessageComposeResultFailed:
			resultString = @"Failed";
			break;
		default:
			resultString = @"";
	}
	
	UnitySendMessage( "EtceteraManager", "smsComposerFinishedWithResult", [resultString UTF8String] );
	
	// autorelease this after 2 seconds to avoid an odd crash when another SMS composer is presented
	[controller performSelector:@selector(autorelease) withObject:nil afterDelay:3.0];
	
	[UIApplication sharedApplication].statusBarHidden = YES;
}



@end
