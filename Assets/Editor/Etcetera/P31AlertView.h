//
//  P31AlertView.h
//  P31AlertView
//
//  Created by Mike on 9/12/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import <Foundation/Foundation.h>


@protocol P31AlertViewDelegate;
@class P31RadialBackgroundView;

@interface P31AlertView : UIView <UITextFieldDelegate>
{
@private
	id<P31AlertViewDelegate> _delegate;
	NSString *_cancelButtonTitle;
	NSString *_okButtonTitle;
	NSMutableArray *_textFields;
	
	P31RadialBackgroundView *_backgroundView;
	NSString *_title;
	NSString *_message;
	
	BOOL _keyboardIsShowing;
	CGSize _titleSize;
	CGSize _messageSize;
	CGFloat _initialFrameHeight;
}
@property (nonatomic, assign) id<P31AlertViewDelegate> delegate;
@property (nonatomic, copy) NSString *cancelButtonTitle;
@property (nonatomic, copy) NSString *okButtonTitle;
@property (nonatomic, retain) NSMutableArray *textFields;

@property (nonatomic, retain) P31RadialBackgroundView *backgroundView;
@property (nonatomic, copy) NSString *title;
@property (nonatomic, copy) NSString *message;



- (id)initWithTitle:(NSString*)title message:(NSString*)message delegate:(id /*<P31AlertViewDelegate>*/)delegate cancelButtonTitle:(NSString*)cancelButtonTitle okButtonTitle:(NSString*)okButtonTitle;

- (void)addTextFieldWithValue:(NSString*)value placeHolder:(NSString*)placeHolder;

- (void)setBorderColor:(UIColor*)color;

- (void)setBackgroundGradientStop1:(UIColor*)stop1 stop2:(UIColor*)stop2;

- (void)show;

- (UITextField*)textFieldAtIndex:(int)index;
- (NSString*)textForTextFieldAtIndex:(int)index;


@end



@protocol P31AlertViewDelegate <NSObject>
@optional

// Called when a button is clicked. The view will be automatically dismissed after this call returns
- (void)alertView:(P31AlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex withTitle:(NSString*)title;

@end