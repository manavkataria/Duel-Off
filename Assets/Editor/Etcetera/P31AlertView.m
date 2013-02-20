//
//  P31AlertView.m
//  P31AlertView
//
//  Created by Mike on 9/12/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "P31AlertView.h"
#import "P31RadialBackgroundView.h"
#import <QuartzCore/QuartzCore.h>
#import "EtceteraManager.h"



// Constants for laying out the buttons and background
const static CGFloat kButtonHorizontalSpacing = 10.0;
const static CGFloat kButtonSpacing = 4.0;
const static CGFloat kButtonHeight = 40.0;

const static int kTitleLabelTag = 2;
const static int kMessageLabelTag = 3;
const static int kCancelButtonTag = 4;
const static int kOKButtonTag = 5;


CGAffineTransform TTRotateTransformForOrientation(UIInterfaceOrientation orientation)
{
	if( orientation == UIInterfaceOrientationLandscapeLeft )
		return CGAffineTransformMakeRotation( M_PI * 1.5 );
	else if( orientation == UIInterfaceOrientationLandscapeRight )
		return CGAffineTransformMakeRotation( M_PI/2 );
	else if( orientation == UIInterfaceOrientationPortraitUpsideDown )
		return CGAffineTransformMakeRotation( -M_PI );
	else
		return CGAffineTransformIdentity;
}


CGRect TTScreenBounds()
{
	CGRect bounds = [UIScreen mainScreen].bounds;
	if( UIInterfaceOrientationIsLandscape( [UIApplication sharedApplication].statusBarOrientation ) )
	{
		CGFloat width = bounds.size.width;
		bounds.size.width = bounds.size.height;
		bounds.size.height = width;
	}
	return bounds;
}


@implementation P31AlertView

@synthesize delegate = _delegate, cancelButtonTitle = _cancelButtonTitle, okButtonTitle = _okButtonTitle,
			textFields = _textFields, backgroundView = _backgroundView, title = _title, message = _message;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (Class)layerClass
{
	return [CAGradientLayer class];
}


- (id)initWithTitle:(NSString*)title message:(NSString*)message delegate:(id/*<P31AlertViewDelegate>*/)delegate cancelButtonTitle:(NSString*)cancelButtonTitle okButtonTitle:(NSString*)okButtonTitle
{
    if( ( self = [super initWithFrame:CGRectMake( (320 - 280) / 2, 300, 280, 100 )] ) )
	{
		self.autoresizingMask = UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleBottomMargin;
		// Setup our layer
		self.layer.cornerRadius = 10.0f;
		self.layer.borderColor = [UIColor colorWithRed:(219.0f/255.0f) green:(221.0f/255.0f) blue:(228.0f/255.0f) alpha:1.0].CGColor;
		self.layer.borderWidth = 2.0;
		
		UIColor *color1 = [UIColor colorWithRed:(11.0f/255.0f) green:(28.0f/255.0f) blue:(67.0f/255.0f) alpha:0.7];
		UIColor *color2 = [UIColor colorWithRed:39.0f/255.0f green:54.0f/255.0f blue:94.0f/255.0f alpha:0.7];
		[self setBackgroundGradientStop1:color1 stop2:color2];
		
		self.delegate = delegate;
		
		// Deal with the rest of the parameters
		self.title = title;
		self.message = message;
		self.cancelButtonTitle = cancelButtonTitle;
		self.okButtonTitle = okButtonTitle;
	}
	return self;
}


- (void)dealloc
{
	[_message release];
	[_title release];
	[_backgroundView release];
	[_textFields release];
	[_okButtonTitle release];
	[_cancelButtonTitle release];
	
	[super dealloc];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UIView

- (void)layoutSubviews
{
	// If we are landscape and we have 2 text fields, we are going to compress things a bit
	BOOL isLandscape = UIInterfaceOrientationIsLandscape( [UIApplication sharedApplication].statusBarOrientation );
	BOOL contractHeight = ( isLandscape && _textFields.count == 2 );
	CGFloat yPos = ( contractHeight ) ? 5.0 : 15.0;
	CGFloat labelPadding = ( contractHeight ) ? 5.0 : 10.0;
	
	// title
	UILabel *titleLabel = (UILabel*)[self viewWithTag:kTitleLabelTag];
	if( titleLabel )
	{
		titleLabel.frame = CGRectMake( 10.0, yPos, 260.0, _titleSize.height );
		yPos += _titleSize.height + labelPadding;
	}
	
	// message
	UILabel *messageLabel = (UILabel*)[self viewWithTag:kMessageLabelTag];
	if( messageLabel )
	{
		messageLabel.frame = CGRectMake( 10.0, yPos, 260.0, _messageSize.height );
		yPos += _messageSize.height + labelPadding;
	}
	
	// textFields
	if( !contractHeight )
		yPos += kButtonSpacing;
	
	for( UITextField *tf in _textFields )
	{
		tf.frame = CGRectMake( 10.0, yPos, self.frame.size.width - ( 2 * kButtonHorizontalSpacing ), 31.0 );		
		yPos += tf.frame.size.height + kButtonSpacing;
	}
	
	yPos += kButtonSpacing;
	
	// buttons
	UIButton *button = (UIButton*)[self viewWithTag:kCancelButtonTag];
	if( button )
	{
		CGRect oldFrame = button.frame;
		oldFrame.origin.y = yPos;
		button.frame = oldFrame;
	}
	
	button = (UIButton*)[self viewWithTag:kOKButtonTag];
	if( button )
	{
		CGRect oldFrame = button.frame;
		oldFrame.origin.y = yPos;
		button.frame = oldFrame;
	}
	
	// figure out the yPos of the alert view
	CGFloat startYPos = ( [UIApplication sharedApplication].statusBarHidden ) ? 0.0 : 20.0;
	UIInterfaceOrientation orient = [UIApplication sharedApplication].statusBarOrientation;
	if( !UIInterfaceOrientationIsLandscape( orient ) )
		startYPos = ( _textFields == nil ) ? ( 480.0 - _initialFrameHeight ) / 2 : ( 480.0 - _initialFrameHeight ) / 2 - 100.0;
	
	// iPad can have a lower frame height
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
		startYPos = ( _textFields == nil ) ? ( 768.0 - _initialFrameHeight ) / 2 : ( 768.0 - _initialFrameHeight ) / 2 - 100.0;

	if( contractHeight )
	{
		CGRect frame = self.frame;
		frame.origin.y = startYPos;
		frame.size.height -= 22.0;
		self.frame = frame;
	}
	else
	{
		CGRect frame = self.frame;
		frame.origin.y = startYPos;
		frame.size.height = _initialFrameHeight;
		self.frame = frame;
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)setupInitialFrame
{
	// Top and bottom padding
	CGFloat frameHeight = 35.0;
	
	// Add our UITextFields in if we have any
	if( _textFields != nil )
	{
		int totalTextFields = [_textFields count];
		frameHeight += ( totalTextFields * 31.0 ) + ( ( totalTextFields + 1 ) * kButtonSpacing );
	}
	
	// Add title height if we have one
	if( _title )
	{
		_titleSize = [_title sizeWithFont:[UIFont boldSystemFontOfSize:18] constrainedToSize:CGSizeMake( 270.0, 500.0 ) lineBreakMode:UILineBreakModeWordWrap];
		frameHeight += _titleSize.height;
	}
	
	// Add body height if we have one
	if( _message )
	{
		_messageSize = [_message sizeWithFont:[UIFont systemFontOfSize:14] constrainedToSize:CGSizeMake( 260.0, 500.0 ) lineBreakMode:UILineBreakModeWordWrap];
		frameHeight += _messageSize.height + kButtonSpacing;
	}
	
	// Buttons
	// Button height and spacing on top/bottom of button
	frameHeight += kButtonHeight + ( 2 * kButtonSpacing );
	
	// Default to 20.0 which is the height of the status bar
	CGFloat startYPos = ( [UIApplication sharedApplication].statusBarHidden ) ? 0.0 : 20.0;
	CGFloat startXPos = ( _backgroundView.bounds.size.width - 280.0 ) / 2.0;
	
	// Adjust our yPos only if we are not in landscape
	UIInterfaceOrientation orient = [UIApplication sharedApplication].statusBarOrientation;
	if( !UIInterfaceOrientationIsLandscape( orient ) )
		startYPos = ( _textFields == nil ) ? ( 480.0 - frameHeight ) / 2 : ( 480.0 - frameHeight ) / 2 - 100.0;
	
	// iPad can have a lower frame height
	if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
		startYPos = ( _textFields == nil ) ? ( 768.0 - frameHeight ) / 2 : ( 768.0 - frameHeight ) / 2 - 100.0;
	
	self.frame = CGRectMake( startXPos, startYPos, 280.0, frameHeight );
	
	// save the original frame height
	_initialFrameHeight = frameHeight;
}


- (void)addButtonWithTitle:(NSString*)title frame:(CGRect)frame isCancelButton:(BOOL)isCancelButton
{
	UIButton *button;
	
	if( isCancelButton )
		button = [UIButton buttonWithType:UIButtonTypeRoundedRect];
	else
		button = [UIButton buttonWithType:UIButtonTypeRoundedRect];
	
	[button setTitle:title forState:UIControlStateNormal];
	button.tag = ( isCancelButton ) ? kCancelButtonTag : kOKButtonTag;
	button.frame = frame;
	[button addTarget:self action:@selector(onTouchPopupButton:) forControlEvents:UIControlEventTouchUpInside];
	[self addSubview:button];
}


// The workhorse.  This function adds all our views.
- (void)addButtonsAndTextToAlertView
{
	// Add our title buttons
	CGFloat yPos = 15.0;
	
	if( _title )
	{
		UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake( 10.0, yPos, 260.0, _titleSize.height )];
		label.tag = kTitleLabelTag;
		label.lineBreakMode = UILineBreakModeWordWrap;
		label.textAlignment = UITextAlignmentCenter;
		label.numberOfLines = 10;
		label.shadowColor = [UIColor colorWithRed:15.0/255.0 green:15.0/255.0 blue:15.0/255.0 alpha:1.0];
		label.shadowOffset = CGSizeMake( 0, -1 );
		label.text = _title;
		label.font = [UIFont boldSystemFontOfSize:18];
		label.textColor = [UIColor whiteColor];
		label.backgroundColor = [UIColor clearColor];
		
		[self addSubview:label];
		[label release];
		
		yPos += _titleSize.height + 10.0;
	}
	
	// Message text
	if( _message )
	{		
		UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake( 10.0, yPos, 260.0, _messageSize.height )];
		label.tag = kMessageLabelTag;
		label.lineBreakMode = UILineBreakModeWordWrap;
		label.textAlignment = UITextAlignmentCenter;
		label.numberOfLines = 20;
		label.text = _message;
		label.font = [UIFont systemFontOfSize:14];
		label.textColor = [UIColor colorWithRed:119.0/255.0 green:140.0/255.0 blue:168.0/255.0 alpha:1.0];
		label.backgroundColor = [UIColor clearColor];
		
		[self addSubview:label];
		[label release];
		
		yPos += _messageSize.height + 10.0;
	}
	
	// UITextFields
	if( _textFields != nil )
	{
		yPos += kButtonSpacing;
		
		for( UITextField *tf in _textFields )
		{
			tf.frame = CGRectMake( 10.0, yPos, self.frame.size.width - ( 2 * kButtonHorizontalSpacing ), 31.0 );
			[self addSubview:tf];
			
			yPos += tf.frame.size.height + kButtonSpacing;
		}
		
		yPos += kButtonSpacing;
	}
	
	// Buttons
	CGFloat buttonWidth = self.frame.size.width - ( 2 * kButtonHorizontalSpacing );

	// Add our cancelButton half width
	CGRect frame = CGRectMake( kButtonHorizontalSpacing, yPos, ( buttonWidth / 2 ) - 2.0, kButtonHeight );
	[self addButtonWithTitle:_cancelButtonTitle frame:frame isCancelButton:YES];
	
	// Add our only other button
	frame.origin.x = kButtonHorizontalSpacing + frame.size.width + ( kButtonHorizontalSpacing / 2 );
	[self addButtonWithTitle:_okButtonTitle frame:frame isCancelButton:NO];
}


- (void)popupAlertView
{
	CGAffineTransform afterAnimationTransform = CGAffineTransformIdentity;
	
	self.transform = CGAffineTransformMakeScale( 0.5, 0.5 );
	
	// Adjust our transforms if we have textFields
	if( _textFields != nil )
	{
		//self.transform = CGAffineTransformTranslate( self.transform, 0, -100 );
		//afterAnimationTransform = CGAffineTransformMakeTranslation( 0, -100 );
		[[_textFields objectAtIndex:0] becomeFirstResponder];
	}
	
	// Create a keyframe animation to follow a path back to the center
	CAKeyframeAnimation *bounceAnimation = [CAKeyframeAnimation animationWithKeyPath:@"transform.scale"];
	bounceAnimation.removedOnCompletion = NO;
	
	NSArray *values = [NSArray arrayWithObjects:[NSNumber numberWithFloat:0.5], [NSNumber numberWithFloat:1.2], [NSNumber numberWithFloat:0.8], [NSNumber numberWithFloat:1.0], nil];
	//NSArray *timings = [NSArray arrayWithObjects:[CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionDefault], [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseIn], [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseOut], [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseIn], nil];
	
	bounceAnimation.values = values;
	//bounceAnimation.timingFunctions = timings;
	bounceAnimation.duration = 0.4;
	
	[self.layer addAnimation:bounceAnimation forKey:@"transformScale"];
	
	self.transform = afterAnimationTransform;
}


- (void)hide
{
	[[NSNotificationCenter defaultCenter] removeObserver:self];
	
	// Hide the keyboard if it's up
	if( _keyboardIsShowing )
	{
		for( UITextField *tf in _textFields )
			[tf resignFirstResponder];
	}
	
	// Fade ourself out and on animationEnd remove ourself from the window
	[UIView beginAnimations:nil context:NULL];
	[UIView setAnimationDuration:0.4];
	[UIView setAnimationDelegate:self];
	[UIView setAnimationDidStopSelector:@selector(animationDidStop:finished:context:)];
	
	_backgroundView.alpha = 0.0;
	self.alpha = 0.0;
	
	[UIView commitAnimations];
}


// Called when the fade out animation stops.  Removes the alert window
- (void)animationDidStop:(NSString*)animationID finished:(NSNumber*)finished context:(void*)context
{
	[self removeFromSuperview];
	[_backgroundView removeFromSuperview];
}


- (void)onTouchPopupButton:(UIButton*)sender
{
	// Grab button info
	NSString *buttonTitle = [sender titleForState:UIControlStateNormal];
	
	uint buttonIndex;
	// First, check for the cancel button
	if( [buttonTitle isEqualToString:_cancelButtonTitle] )
		buttonIndex = 0;
	else
		buttonIndex = 1;
	
	// Let our delegate know about the touch and dismiss ourself
	// Delegate method
	if( [_delegate respondsToSelector:@selector(alertView:clickedButtonAtIndex:withTitle:)] )
		[_delegate alertView:self clickedButtonAtIndex:buttonIndex withTitle:buttonTitle];
	
	[self hide];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark UITextFieldDelegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
	int totalTextFields = [_textFields count];
	
	if( ( totalTextFields - 1 ) > textField.tag )
		[[_textFields objectAtIndex:textField.tag + 1] becomeFirstResponder];
	else
		[[_textFields objectAtIndex:0] becomeFirstResponder];
	
	return NO;
}


- (BOOL)textFieldShouldBeginEditing:(UITextField *)textField
{
	if( !_keyboardIsShowing )
	{
		_keyboardIsShowing = YES;
		
		[UIView beginAnimations:nil context:NULL];
		// Adjust the transform the fit the keyboard if necessary
		self.transform = CGAffineTransformTranslate( self.transform, 0.0, -100 );
		[UIView commitAnimations];
	}
	
	return YES;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSNotifications

- (void)orientationDidChangeNotification:(NSNotification*)note
{
	// Take care of rotating the backgroundView
	_backgroundView.transform = TTRotateTransformForOrientation( [UIApplication sharedApplication].statusBarOrientation );
	_backgroundView.frame = [UIScreen mainScreen].bounds;

	[self setNeedsLayout];
	
	// Adjust our yPos
	//BOOL isLandscape = UIInterfaceOrientationIsLandscape( [UIApplication sharedApplication].statusBarOrientation );
	//CGFloat startXPos = ( TTScreenBounds().size.width - 280.0 ) / 2.0;
	
	//self.frame = CGRectMake( startXPos, self.frame.origin.y, 280.0, self.frame.size.height );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (UITextField*)textFieldAtIndex:(int)index
{
	if( [_textFields count] > index )
		return [_textFields objectAtIndex:index];
	
	return nil;
}


- (NSString*)textForTextFieldAtIndex:(int)index
{
	return [self textFieldAtIndex:index].text;
}


- (void)addTextFieldWithValue:(NSString*)value placeHolder:(NSString*)placeHolder
{
	// Create our array if we haven't already
	if( _textFields == nil )
		self.textFields = [NSMutableArray array];
	
	// Create out textField without a frame for now
	UITextField *tf = [[UITextField alloc] init];
	tf.delegate = self;
	tf.font = [UIFont systemFontOfSize:20.0];
	tf.keyboardAppearance = UIKeyboardAppearanceAlert;
	tf.borderStyle = UITextBorderStyleRoundedRect;
	tf.tag = [_textFields count];
	tf.placeholder = placeHolder;
	tf.text = value;
	
	[_textFields addObject:tf];
	[tf release];
	
	// If this is a 2nd text field change the buttons to be next buttons
	if( _textFields.count == 2 )
	{
		for( UITextField *textField in _textFields )
			textField.returnKeyType = UIReturnKeyNext;
	}
}


- (void)setBorderColor:(UIColor*)color
{
	self.layer.borderColor = color.CGColor;
}


- (void)setBackgroundGradientStop1:(UIColor*)stop1 stop2:(UIColor*)stop2
{
	((CAGradientLayer*)self.layer).colors = [NSArray arrayWithObjects:(id)stop1.CGColor, (id)stop2.CGColor, nil];
}


- (void)show
{
	// Listen for rotation events
	//[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(orientationDidChangeNotification:) name:UIApplicationDidChangeStatusBarFrameNotification object:nil];
	
	// Create the radial gradient background
	_backgroundView = [[P31RadialBackgroundView alloc] initWithFrame:CGRectZero];
	//_backgroundView.transform = TTRotateTransformForOrientation( [UIApplication sharedApplication].statusBarOrientation );
	_backgroundView.frame = TTScreenBounds();
	_backgroundView.autoresizingMask = UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth;
	_backgroundView.alpha = 0.3;
	
	// Add the background to the window
	UIViewController *unityViewController = [EtceteraManager unityViewController];
	[unityViewController.view addSubview:_backgroundView];
	//[[UIApplication sharedApplication].keyWindow addSubview:_backgroundView];
	
	// Setup our frame and add our buttons
	[self setupInitialFrame];
	[self addButtonsAndTextToAlertView];
	
	// Add ourself to the window
	[_backgroundView addSubview:self];
	
	// Fade in the background view
	[UIView beginAnimations:nil context:NULL];
	[UIView setAnimationDuration:0.3];
	
	_backgroundView.alpha = 1.0;
	
	[UIView commitAnimations];
	
	
	// Animate our entrance
	[self popupAlertView];
}


@end
