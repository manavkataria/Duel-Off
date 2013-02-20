//
//  P31WebController.h
//  EtceteraTest
//
//  Created by Mike on 10/2/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface P31WebController : UIViewController <UIWebViewDelegate, UIActionSheetDelegate>
{
	UIWebView *_webView;
	NSString *_url;
	BOOL _showToolbar;
	UIBarButtonItem *backButton;
	UIBarButtonItem *forwardButton;
}
@property (nonatomic, retain) UIWebView *webView;
@property (nonatomic, retain) NSString *url;
@property (nonatomic, retain) UIBarButtonItem *backButton;
@property (nonatomic, retain) UIBarButtonItem *forwardButton;


- (id)initWithUrl:(NSString*)url showControls:(BOOL)showControls;

@end
