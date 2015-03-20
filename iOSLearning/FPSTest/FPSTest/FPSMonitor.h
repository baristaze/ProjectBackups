//
//  FPSMonitor.h
//  FPSTest
//
//  Created by Baris Taze on 3/19/15.
//  Copyright (c) 2015 Uber. All rights reserved.
//

#ifndef FPSTest_FPSMonitor_h
#define FPSTest_FPSMonitor_h

NSString* const FramePerSecondUpdatedNotification;

@interface FPSData : NSObject
{
}
@property(nonatomic, readonly) CGFloat minFPS;
@property(nonatomic, readonly) CGFloat avgFPS;

- (instancetype)initWithMin:(CGFloat)minFPS :(CGFloat)avgFPS;
@end


@interface FPSMonitor : NSObject
{
    
}

- (void) start;
- (void) pause;
- (void) resume;

@end

#endif
