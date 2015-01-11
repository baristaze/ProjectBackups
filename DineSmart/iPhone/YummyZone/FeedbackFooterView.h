//
//  FeedbackFooterView.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/15/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol FeedbackFooterViewDelegate 

- (void)onSendFeedback;

@end

@interface FeedbackFooterView : UIView
{
	id<FeedbackFooterViewDelegate>  _delegate;
}

+ (CGFloat)getRequiredHeight;

- (id)initWithFrame:(CGRect)frame delegate:(id<FeedbackFooterViewDelegate>)delegate;
- (void)sendFeedbackAction:(id)sender;

@end
