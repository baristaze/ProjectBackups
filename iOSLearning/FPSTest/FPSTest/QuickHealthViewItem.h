//
//  QuickHealthViewItem.h
//  FPSTest
//
//  Created by Baris Taze on 3/19/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#ifndef FPSTest_QuickHealthViewItem_h
#define FPSTest_QuickHealthViewItem_h

#import <Foundation/NSString.h>
#import <UIKit/UIKit.h>

typedef enum HealthStatusEnum { Unknown, Green, Amber, Red } HealthStatus;

@interface QuickHealthViewItem : UIView
{
}

@property (nonatomic, readonly) NSString* title;
@property (nonatomic, readonly) HealthStatus status;

- (instancetype)initWithSize :(CGFloat)size;
- (void)setTitleWithStatus :(NSString *)title :(HealthStatus)status;

@end

#endif
