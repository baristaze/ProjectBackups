//
//  FloatingView.h
//  FPSTest
//
//  Created by Baris Taze on 3/18/15.
//  Copyright (c) 2015 Uber. All rights reserved.
//

#ifndef FPSTest_FloatingView_h
#define FPSTest_FloatingView_h

#import <UIKit/UIKit.h>

@interface FloatingView : UIView
{
}

- (id)initWithSize:(CGFloat)size;
- (void)moveTo:(CGPoint)origin;

- (void)startMonitoring;
- (void)stopMonitoring;

@end



#endif