#import <UIKit/UIKit.h>


@interface P31ActivityView : UIView
{
    UIView *_originalView;
    UIView *_borderView;
    UIActivityIndicatorView *_activityIndicator;
    UILabel *_activityLabel;
    NSUInteger _labelWidth;
    BOOL _showNetworkActivityIndicator;
}
// The view to contain the activity indicator and label.  The bezel style has a semi-transparent rounded rectangle, others are fully transparent:
@property (nonatomic, readonly) UIView *borderView;

// The activity indicator view; automatically created on first access:
@property (nonatomic, readonly) UIActivityIndicatorView *activityIndicator;

// The activity label; automatically created on first access:
@property (nonatomic, readonly) UILabel *activityLabel;

// A fixed width for the label text, or zero to automatically calculate the text size (normally set on creation of the view object):
@property (nonatomic) NSUInteger labelWidth;

// Whether to show the network activity indicator in the status bar.  Set to YES if the activity is network-related.  This can be toggled on and off as desired while the activity view is visible (e.g. have it on while fetching data, then disable it while parsing it).  By default it is not shown:
@property (nonatomic) BOOL showNetworkActivityIndicator;


//  Returns the currently displayed activity view, or nil if there isn't one:
+ (P31ActivityView*)currentActivityView;


// Creates and adds an activity view centered within the specified view, using the label "Loading...".  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityView;


// Creates and adds an activity view centered within the specified view, using the specified label.  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityViewWithLabel:(NSString*)labelText;


// Creates and adds an activity view centered within the specified view, using the specified label and a fixed label width.  The fixed width is useful if you want to change the label text while the view is visible.  Returns the activity view, already added as a subview of the specified view:
+ (id)newActivityViewWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth;


// Designated initializer.  Configures the activity view using the specified label text and width, and adds as a subview of the specified view:
- (id)initWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth;


- (void)setupActivityIndicatorAndAnimateShow;;


// Immediately removes and releases the view without any animation:
+ (void)removeView;

@end



// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------

// These methods are exposed for subclasses to override to customize the appearance and behavior; see the implementation for details:

@interface P31ActivityView ()

@property (nonatomic, retain) UIView *originalView;

- (UIView*)viewForView:(UIView*)view;
- (CGRect)enclosingFrame;
- (void)setupBackground;
- (void)animateShow;
- (void)animateRemove;

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------

@interface P31WhiteActivityView : P31ActivityView
{
}

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------

@interface P31BezelActivityView : P31ActivityView
{
}

// Sets the bezel color for all DSBezelActivityViews
+ (void)setBezelColor:(UIColor*)newBezelColor;


// Sets the font color for all DSBezelActivityView activityLabels
+ (void)setActvityLabelFontColor:(UIColor*)newBezelColor;


// Animates the view out from the superview and releases it, or simply removes and releases it immediately if not animating:
+ (void)removeViewAnimated:(BOOL)animated;

@end


// ----------------------------------------------------------------------------------------
#pragma mark -
// ----------------------------------------------------------------------------------------

@interface P31ImageActivityView : P31BezelActivityView
{
}

// Constructors
+ (id)newActivityViewWithImage:(UIImage*)image;

+ (id)newActivityViewWithLabel:(NSString*)labelText withImage:(UIImage*)image;

+ (id)newActivityViewWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth withImage:(UIImage*)image;

- (id)initWithLabel:(NSString*)labelText width:(NSUInteger)labelWidth withImage:(UIImage*)image;

@end


