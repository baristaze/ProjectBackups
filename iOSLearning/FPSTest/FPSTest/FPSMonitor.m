//
//  FPSMonitor.m
//  FPSTest
//
//  Created by Baris Taze on 3/19/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>
#import "FPSMonitor.h"

@implementation FPSMonitor
{
    CADisplayLink* _displayLink;
    CFTimeInterval _displayLinkTickTimeLast;
    CFTimeInterval _lastNotificationTime;
    NSUInteger _measureCountSoFar;
    CFTimeInterval _measureSumSoFar;
    CFTimeInterval _maxMeasureSoFar;
}

- (void)dealloc {
    [self pause];
    [_displayLink removeFromRunLoop:[NSRunLoop currentRunLoop] forMode:NSRunLoopCommonModes];
}

- (instancetype)init
{
    if(self = [super init]){
    
        // init the time
        _displayLinkTickTimeLast = CACurrentMediaTime();
        _lastNotificationTime = _displayLinkTickTimeLast;
        
        // Track FPS using display link
        _displayLink = [CADisplayLink displayLinkWithTarget:self selector:@selector(displayLinkCallBack)];
        [_displayLink setPaused:YES];
        [_displayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSRunLoopCommonModes];
        
        // init other fields
        _measureCountSoFar = 0;
        _measureSumSoFar = 0.0f;
        _maxMeasureSoFar = 0.0f;
        
        // listen application for major events
        [self listenMainApp];
        
    }
    return self;
}

- (void) listenMainApp
{
    // listen application
    [[NSNotificationCenter defaultCenter] addObserver: self
                                             selector: @selector(applicationDidBecomeActiveNotification)
                                                 name: UIApplicationDidBecomeActiveNotification
                                               object: nil];
    
    [[NSNotificationCenter defaultCenter] addObserver: self
                                             selector: @selector(applicationWillResignActiveNotification)
                                                 name: UIApplicationWillResignActiveNotification
                                               object: nil];

}

- (void)applicationDidBecomeActiveNotification
{
    [self resume];
}

- (void)applicationWillResignActiveNotification
{
    [self pause];
}

- (void) start
{
    [self resume];
}

- (void)resume
{
    [_displayLink setPaused:NO];
}

- (void)pause
{
    [_displayLink setPaused:YES];
}

- (void)displayLinkCallBack
{
    // get the time difference
    CFTimeInterval currentDT = _displayLink.timestamp - _displayLinkTickTimeLast;
    
    // calculate FPS
    _measureCountSoFar++;
    _measureSumSoFar += (float)currentDT;
    _maxMeasureSoFar = MAX(_maxMeasureSoFar, currentDT);
    
    // keep track of last tick
    _displayLinkTickTimeLast = _displayLink.timestamp;
    
    // do we need to fire a notification?
    CFTimeInterval gap = _displayLink.timestamp - _lastNotificationTime;
    if(gap >= 0.5f){
        
        // calculate average FPS
        CFTimeInterval avgFPS = roundf(1.0f / (_measureSumSoFar / _measureCountSoFar));
        CFTimeInterval minFPS = roundf(1.0f / _maxMeasureSoFar);
    
        // reset cunters
        _measureCountSoFar = 0;
        _measureSumSoFar = 0.0f;
        _maxMeasureSoFar = 0.0f;
        
        // notify
        //NSLog(@"Avg FPS: %.f", avgFPS);
        NSLog(@"Min FPS: %.f", minFPS);
        
        // keep track of last
        _lastNotificationTime = _displayLink.timestamp;
    }
    
    /*
    if(_measureCountSoFar >= 100){
        // reset cunters
        _measureCountSoFar = 0;
        _measureSumSoFar = 0.0f;
        _maxMeasureSoFar = 0.0f;
    }
    */
}

@end

