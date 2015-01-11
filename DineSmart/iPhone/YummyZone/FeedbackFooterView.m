//
//  FeedbackFooterView.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/15/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "FeedbackFooterView.h"
#import "YummyZoneUtils.h"

@implementation FeedbackFooterView

#define BUTTON_WIDTH		300
#define BUTTON_X			10
#define BUTTON_Y			10

+ (CGFloat)getRequiredHeight
{
	//return 52;
	UIImage* actionButtonImage = [UIImage imageNamed:@"actionBtnStretch.png"];
	return actionButtonImage.size.height + 20;
}

- (id)initWithFrame:(CGRect)frame delegate:(id<FeedbackFooterViewDelegate>)delegate 
{
    self = [super initWithFrame:frame];
	if (self) 
	{
        _delegate = delegate;
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];		
        
        UIButton *fbkBtn = [YummyZoneUtils createActionButtonWithText:@"Send Feedback"
                                                                width:BUTTON_WIDTH 
                                                                 left:BUTTON_X 
                                                                  top:BUTTON_Y 
                                                               target:self 
                                                               action:@selector(sendFeedbackAction:)];
 		[self addSubview:fbkBtn];        
              
		[pool release];
	}
	return self;
}


- (void)dealloc 
{
	_delegate = nil;
    [super dealloc];
}

- (void)sendFeedbackAction:(id)sender
{
	if (_delegate != nil)
	{
        [_delegate onSendFeedback];
	}
}


@end
