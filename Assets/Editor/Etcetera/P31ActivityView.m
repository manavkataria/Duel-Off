#import "P31ActivityView.h"
#import <QuartzCore/QuartzCore.h>
#import "EtceteraManager.h"


CGAffineTransform TRotateTransformForOrientation(UIInterfaceOrientation orientation)
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


@interface P31ActivityView(Private)
- (void)orientationDidChangeNotification:(NSNotification*)note;
@end


@implementation P31ActivityView

@synthesize originalView = _originalView, labelWidth = _labelWidth, showNetworkActivityIndicator = _showNetworkActivityIndicator;

static P31ActivityView *activityView = nil;

/*
 Returns the currently displayed activity view, or nil if there isn't one.
*/
+ (id)currentActivityView;
{
    return activityView;
}


/*
 Creates and adds an activity view centered within the specified view.  Returns the activity view, already added as a subview of the specified view.
*/
+ (id)newActivityView
{
    return [self newActivityViewWithLabel:nil width:0];
}


/*
 Creates and adds an activity view centered within the specified view, using the specified label.  Returns the activity view, already added as a subview of the specified view.
*/
+ (id)newActivityViewWithLabel:(NSString*)labelText;
{
    return [self newActivityViewWithLabel:labelText width:0];
}


/*
 Creates and adds an activity view centered within the specified view, using the specified label and a fixed label width.  The fixed width is useful if you want to change the label text while the view is visible.  Returns the activity view, already added as a subview of the specified view.
*/
+ (id)newActivityViewWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth;
{
    // Not autoreleased, as it is basically a singleton:
    P31ActivityView *activityView = [[self alloc] initWithLabel:labelText width:labelWidth];
	
	[activityView setupActivityIndicatorAndAnimateShow];
	
	return activityView;
}


/*
 Designated initializer.  Configures the activity view using the specified label text and width, and adds as a subview of the specified view.
*/
- (id)initWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth;
{
	if( !( self = [super initWithFrame:CGRectZero] ) )
		return nil;
	
    // Immediately remove any existing activity view:
    if( activityView )
        [[self class] removeView];
    
    // Remember the new view (it is already retained):
    activityView = self;
	
    // Allow subclasses to change the view to which to add the activity view (e.g. to cover the keyboard):
	self.originalView = [EtceteraManager unityViewController].view;
	
    // Configure this view (the background) and the label text (the label is automatically created):
    [self setupBackground];
    self.labelWidth = labelWidth;
    self.activityLabel.text = labelText;
	
	[self orientationDidChangeNotification:nil];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(orientationDidChangeNotification:) name:UIApplicationDidChangeStatusBarFrameNotification object:nil];
    
	return self;
}


- (void)dealloc;
{
	[[NSNotificationCenter defaultCenter] removeObserver:self];
	
    [_activityLabel release];
    [_activityIndicator release];
    [_borderView release];
    [_originalView release];
    
    [super dealloc];
    
    activityView = nil;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSNotifications

- (void)orientationDidChangeNotification:(NSNotification*)note
{
	[self setNeedsLayout];
}


/*
 Create and add the activityIndicator view.  This is done outside of the constructor so subclasses can override the behaviour
 and provide alternate views such as the DSImageActivityView
 */
- (void)setupActivityIndicatorAndAnimateShow
{
	UIView *addToView = [self viewForView:_originalView];
	
    // Assemble the subviews (the border and indicator are automatically created):
	[addToView addSubview:self];
    [self addSubview:self.borderView];
    [self.borderView addSubview:self.activityIndicator];
    [self.borderView addSubview:self.activityLabel];
    
	// Animate the view in, if appropriate:
	[self animateShow];
}


/*
 Immediately removes and releases the view without any animation.
*/
+ (void)removeView;
{
    if (!activityView)
        return;
    
    if (activityView.showNetworkActivityIndicator)
        [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    [activityView removeFromSuperview];
	
    // Remove the global reference:
    [activityView release];
    activityView = nil;
}


/*
 Returns the view to which to add the activity view.  By default returns the same view.  Subclasses may override this to change the view.
*/
- (UIView*)viewForView:(UIView*)view;
{
    return view;
}


/*
 Returns the frame to use for the activity view.  Defaults to the superview's bounds.  Subclasses may override this to use something different, if desired.
*/
- (CGRect)enclosingFrame;
{
    return self.superview.bounds;
}


/*
 Configure the background of the activity view.
*/
- (void)setupBackground;
{
	self.opaque = NO;
    self.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
}


/*
 Returns the view to contain the activity indicator and label.  By default this view is transparent.
*/
- (UIView*)borderView;
{
    if( !_borderView )
    {
        _borderView = [[UIView alloc] initWithFrame:CGRectZero];
        
        _borderView.opaque = NO;
        _borderView.autoresizingMask = UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleBottomMargin;
    }
    
    return _borderView;
}


/* 
 Returns the activity indicator view, creating it if necessary.
*/
- (UIActivityIndicatorView*)activityIndicator;
{
    if (!_activityIndicator)
    {
        _activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
        [_activityIndicator startAnimating];
    }
    
    return _activityIndicator;
}


/*
 Returns the activity label, creating it if necessary.
*/
- (UILabel*)activityLabel;
{
    if (!_activityLabel)
    {
        _activityLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        
        _activityLabel.font = [UIFont systemFontOfSize:[UIFont systemFontSize]];
        _activityLabel.textAlignment = UITextAlignmentLeft;
        _activityLabel.textColor = [UIColor blackColor];
        _activityLabel.backgroundColor = [UIColor clearColor];
        _activityLabel.shadowColor = [UIColor whiteColor];
        _activityLabel.shadowOffset = CGSizeMake(0.0, 1.0);
    }
    
    return _activityLabel;
}


/* 
 Positions and sizes the various views that make up the activity view, including after rotation.
*/
- (void)layoutSubviews;
{
    self.frame = [self enclosingFrame];
    
    // If we're animating a transform, don't lay out now, as can't use the frame property when transforming:
    if (!CGAffineTransformIsIdentity(self.borderView.transform))
        return;
    
    CGSize textSize = CGSizeZero;
	BOOL labelHasText = ( self.activityLabel.text != nil );
	
	if( labelHasText )
		textSize = [self.activityLabel.text sizeWithFont:[UIFont systemFontOfSize:[UIFont systemFontSize]]];
    
    // Use the fixed width if one is specified only if we have text
    if( labelHasText && self.labelWidth > 10 )
        textSize.width = self.labelWidth;
    
    //self.activityLabel.frame = CGRectMake( self.activityLabel.frame.origin.x, self.activityLabel.frame.origin.y, textSize.width, textSize.height );
    
    // Calculate the size and position for the border view: with the indicator to the left of the label, and centered in the receiver:
	CGRect borderFrame = CGRectZero;
	
	// If we have text, fix up the width and height
	if( labelHasText )
	{
		borderFrame.size.width = self.activityIndicator.frame.size.width + textSize.width + 25.0;
		borderFrame.size.height = self.activityIndicator.frame.size.height + 10.0;
		borderFrame.origin.x = floor(0.5 * (self.frame.size.width - borderFrame.size.width));
		borderFrame.origin.y = floor(0.5 * (self.frame.size.height - borderFrame.size.height - 20.0));
	}
	else
	{
		borderFrame.size.width = self.activityIndicator.frame.size.width;
		borderFrame.size.height = self.activityIndicator.frame.size.height;
		borderFrame.origin.x = floor(0.5 * (self.frame.size.width - borderFrame.size.width));
		borderFrame.origin.y = floor(0.5 * (self.frame.size.height - borderFrame.size.height));
	}

    self.borderView.frame = borderFrame;
	
    // Calculate the position of the indicator: vertically centered and at the left of the border view:
    CGRect indicatorFrame = self.activityIndicator.frame;
	indicatorFrame.origin.x = ( labelHasText ) ? 10.0f : 0.0f;
	indicatorFrame.origin.y = 0.5 * (borderFrame.size.height - indicatorFrame.size.height);
    self.activityIndicator.frame = indicatorFrame;
    
    // Calculate the position of the label: vertically centered and at the right of the border view:
	if( labelHasText )
	{
		CGRect labelFrame = self.activityLabel.frame;
		labelFrame.origin.x = borderFrame.size.width - labelFrame.size.width - 10.0;
		labelFrame.origin.y = floor(0.5 * (borderFrame.size.height - labelFrame.size.height));
		self.activityLabel.frame = labelFrame;
	}
}


/*
 Animates the view into visibility.  Does nothing for the simple activity view.
*/
- (void)animateShow;
{
    // Does nothing by default
}


/*
 Animates the view out of visibiltiy.  Does nothng for the simple activity view.
*/
- (void)animateRemove;
{
    // Does nothing by default
}


/*
 Sets whether or not to show the network activity indicator in the status bar.  Set to YES if the activity is network-related.  This can be toggled on and off as desired while the activity view is visible (e.g. have it on while fetching data, then disable it while parsing it).  By default it is not shown.
*/
- (void)setShowNetworkActivityIndicator:(BOOL)showNetworkActivityIndicator;
{
    _showNetworkActivityIndicator = showNetworkActivityIndicator;
    
    [UIApplication sharedApplication].networkActivityIndicatorVisible = showNetworkActivityIndicator;
}

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------


@implementation P31WhiteActivityView

/*
 Returns the activity indicator view, creating it if necessary.  This subclass uses a white activity indicator instead of gray.
*/

- (UIActivityIndicatorView*)activityIndicator;
{
    if (!_activityIndicator)
    {
        _activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhite];
        [_activityIndicator startAnimating];
    }
    
    return _activityIndicator;
}

/*
 Returns the activity label, creating it if necessary.  This subclass uses white text instead of black.
*/

- (UILabel*)activityLabel;
{
    if (!_activityLabel)
    {
        _activityLabel = [super activityLabel];
        
        _activityLabel.textColor = [UIColor whiteColor];
        _activityLabel.shadowColor = [UIColor blackColor];
    }
    
    return _activityLabel;
}

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------

static UIColor *__bezelColor;
static UIColor *__actvityLabelFontColor;


@implementation P31BezelActivityView


/*
 Sets the bezelColor used for all DSBezelActivityViews.  Defaults to black
 */
+ (void)setBezelColor:(UIColor*)newBezelColor
{
	// Release the old __bezelColor is we have one
	if( __bezelColor )
		[__bezelColor release];
	
	__bezelColor = [newBezelColor copy];
}


/*
 Sets theactvityLabels font color used for all DSBezelActivityViews.  Defaults to white
 */
+ (void)setActvityLabelFontColor:(UIColor*)newBezelColor
{
	// Release the old __bezelColor is we have one
	if( __actvityLabelFontColor )
		[__actvityLabelFontColor release];
	
	__actvityLabelFontColor = [newBezelColor copy];
}


/*
 Returns the view to which to add the activity view.  For the bezel style, if there is a keyboard displayed, the view is changed to the keyboard's superview.
*/
- (UIView*)viewForView:(UIView*)view;
{
    return view;
}


/*
 Returns the frame to use for the activity view.  For the bezel style, if there is a keyboard displayed, the frame is changed to cover the keyboard too.
*/
- (CGRect)enclosingFrame;
{
    CGRect frame = [super enclosingFrame];
    
    if (self.superview != self.originalView)
        frame = [self.originalView convertRect:self.originalView.bounds toView:self.superview];
    
    return frame;
}


/*
 Configure the background of the activity view.
*/
- (void)setupBackground;
{
    [super setupBackground];
    
	self.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.35];
}


/*
 Returns the view to contain the activity indicator and label.  The bezel style has a semi-transparent rounded rectangle.
*/
- (UIView*)borderView;
{
    if( !_borderView )
    {
        [super borderView];
        
		UIColor *color = ( __bezelColor ) ? __bezelColor : [UIColor blackColor];
        _borderView.backgroundColor = [color colorWithAlphaComponent:0.6];
        _borderView.layer.cornerRadius = 10.0;
    }
    
    return _borderView;
}


/*
 Returns the activity indicator view, creating it if necessary.
*/
- (UIActivityIndicatorView*)activityIndicator;
{
    if (!_activityIndicator)
    {
        _activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
        [_activityIndicator startAnimating];
    }
    
    return _activityIndicator;
}


/*
 Returns the activity label, creating it if necessary.
*/
- (UILabel*)activityLabel;
{
    if (!_activityLabel)
    {
        _activityLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        
        _activityLabel.font = [UIFont boldSystemFontOfSize:[UIFont systemFontSize]];
        _activityLabel.textAlignment = UITextAlignmentCenter;
		UIColor *color = ( __actvityLabelFontColor ) ? __actvityLabelFontColor : [UIColor whiteColor];
        _activityLabel.textColor = color;
        _activityLabel.backgroundColor = [UIColor clearColor];
    }
    
    return _activityLabel;
}


/*
 Positions and sizes the various views that make up the activity view, including after rotation.
*/
- (void)layoutSubviews;
{
    // If we're animating a transform, don't lay out now, as can't use the frame property when transforming:
    if (!CGAffineTransformIsIdentity(self.borderView.transform))
        return;
    
	
	
    self.frame = [self enclosingFrame];
    
    CGSize textSize = [self.activityLabel.text sizeWithFont:[UIFont boldSystemFontOfSize:[UIFont systemFontSize]]];
    
    // Use the fixed width if one is specified:
    if (self.labelWidth > 10)
        textSize.width = self.labelWidth;
    
    // Require that the label be at least as wide as the indicator, since that width is used for the border view:
    if (textSize.width < self.activityIndicator.frame.size.width)
        textSize.width = self.activityIndicator.frame.size.width + 10.0;
    
    // If there's no label text, don't need to allow height for it:
    if (self.activityLabel.text.length == 0)
        textSize.height = 0.0;
    
    self.activityLabel.frame = CGRectMake(self.activityLabel.frame.origin.x, self.activityLabel.frame.origin.y, textSize.width, textSize.height);
    
    // Calculate the size and position for the border view: with the indicator vertically above the label, and centered in the receiver:
	CGRect borderFrame = CGRectZero;
    borderFrame.size.width = textSize.width + 30.0;
    borderFrame.size.height = self.activityIndicator.frame.size.height + textSize.height + 40.0;
    borderFrame.origin.x = floor(0.5 * (self.frame.size.width - borderFrame.size.width));
    borderFrame.origin.y = floor(0.5 * (self.frame.size.height - borderFrame.size.height));
    self.borderView.frame = borderFrame;
	
    // Calculate the position of the indicator: horizontally centered and near the top of the border view:
    CGRect indicatorFrame = self.activityIndicator.frame;
	indicatorFrame.origin.x = 0.5 * (borderFrame.size.width - indicatorFrame.size.width);
	indicatorFrame.origin.y = 20.0;
    self.activityIndicator.frame = indicatorFrame;
    
    // Calculate the position of the label: horizontally centered and near the bottom of the border view:
	CGRect labelFrame = self.activityLabel.frame;
    labelFrame.origin.x = floor(0.5 * (borderFrame.size.width - labelFrame.size.width));
	labelFrame.origin.y = borderFrame.size.height - labelFrame.size.height - 10.0;
    self.activityLabel.frame = labelFrame;
}


/*
 Animates the view into visibility.  For the bezel style, fades in the background and zooms the bezel down from a large size.
*/
- (void)animateShow;
{
    self.alpha = 0.0;
    self.borderView.transform = CGAffineTransformMakeScale(3.0, 3.0);
    
	[UIView beginAnimations:nil context:nil];
//	[UIView setAnimationDuration:5.0];            // Uncomment to see the animation in slow motion
	
    self.borderView.transform = CGAffineTransformIdentity;
    self.alpha = 1.0;
    
	[UIView commitAnimations];
}


/*
 Animates the view out, deferring the removal until the animation is complete.  For the bezel style, fades out the background and zooms the bezel down to half size.
*/
- (void)animateRemove;
{
    if (self.showNetworkActivityIndicator)
        [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    self.borderView.transform = CGAffineTransformIdentity;
    
	[UIView beginAnimations:nil context:nil];
//	[UIView setAnimationDuration:5.0];            // Uncomment to see the animation in slow motion
	[UIView setAnimationDelegate:self];
	[UIView setAnimationDidStopSelector:@selector(removeAnimationDidStop:finished:context:)];
	
    self.borderView.transform = CGAffineTransformMakeScale(0.5, 0.5);
    self.alpha = 0.0;
    
	[UIView commitAnimations];
}


- (void)removeAnimationDidStop:(NSString*)animationID finished:(NSNumber*)finished context:(void*)context;
{
    [[self class] removeView];
}


/*
 Animates the view out from the superview and releases it, or simply removes and releases it immediately if not animating.
*/
+ (void)removeViewAnimated:(BOOL)animated;
{
    if (!activityView)
        return;
    
    if (animated)
        [activityView animateRemove];
    else
        [[self class] removeView];
}

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------


@implementation P31ImageActivityView

// Creates and adds an activity view centered within the specified view, using the label "Loading...".  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityViewWithImage:(UIImage*)image
{
	return [self newActivityViewWithImage:image];
}


// Creates and adds an activity view centered within the specified view, using the specified label.  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityViewWithLabel:(NSString*)labelText withImage:(UIImage*)image
{
	return [self newActivityViewWithLabel:labelText width:0 withImage:image];
}


// Creates and adds an activity view centered within the specified view, using the specified label and a fixed label width.  The fixed width is useful if you want to change the label text while the view is visible.  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityViewWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth withImage:(UIImage*)image
{
	P31ImageActivityView *activityView = [[self alloc] initWithLabel:labelText width:labelWidth withImage:image];
	[activityView setupActivityIndicatorAndAnimateShow];
	
	return activityView;
}


// Designated initializer.  Configures the activity view using the specified label text and width, and adds as a subview of the specified view:
- (id)initWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth withImage:(UIImage*)image
{
	if( self = [super initWithLabel:labelText width:labelWidth] )
	{
		// Save our image in the activity indicator.  It will serve as the indicator
		_activityIndicator = (id)[[UIImageView alloc] initWithImage:image];
	}
	return self;
}


@end



