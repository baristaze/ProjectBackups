//
//  FloatingView.m
//  FPSTest
//
//  Created by Baris Taze on 3/18/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#import <UIKit/UIPanGestureRecognizer.h>

#import "FloatingView.h"
#import "FPSMonitor.h"
#import "QuickHealthViewItem.h"

#define INIT_ORIG_X 50.0f
#define INIT_ORIG_Y 50.0f

#define INIT_SIZE 40.0f

@implementation FloatingView
{
    double initialDragLocationX;
    double initialDragLocationY;
    
    FPSMonitor* fpsMonitor;
    QuickHealthViewItem* fpsView;
    
    //QuickHealthViewItem* cpuView;
}

// method implementations

- (void) dealloc
{
    [self stopMonitoring];
}

- (id) initWithSize:(CGFloat)size
{
    NSLog(@"FloatingView::init");
    
    CGFloat heightF = 1.0f;
    //CGFloat heightF = 2.0f; // if cpuView is enabled
    self = [super initWithFrame:CGRectMake(INIT_ORIG_X, INIT_ORIG_Y, size, size * heightF)];
    if(self){
        
        // dim the background
        //[self setBackgroundColor:[UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:0.5]];
        
        // register pan gesture
        [self registerPanGesture];
        
        // create FPS view and add it
        fpsView = [[QuickHealthViewItem alloc] initWithSize:size];
        [self addSubview:fpsView];
        
        // create FPS monitor
        fpsMonitor = [[FPSMonitor alloc] init];
        
        /*
        // create CPU view and add it
        cpuView = [[QuickHealthViewItem alloc] initWithSize:size];
        [cpuView setCenter:CGPointMake(size*0.5f, size * 1.5f)];
        [self addSubview:cpuView];
        */
        
        [fpsView setTitleWithStatus:@"35" :Amber];
        //[cpuView setTitleWithStatus:@"48" :Red];
    }
    
    return self;
}

- (void) registerPanGesture
{
    UIPanGestureRecognizer* panGesture =
    [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(onPan:)];
    [panGesture setMinimumNumberOfTouches:1];
    [panGesture setMaximumNumberOfTouches:1];
    [self addGestureRecognizer:panGesture];
}

- (void) onPan:(id)sender
{
    
    [self bringSubviewToFront:[(UIPanGestureRecognizer*)sender view]];
    CGPoint translatedPoint = [(UIPanGestureRecognizer*)sender translationInView:self];
    
    if ([(UIPanGestureRecognizer*)sender state] == UIGestureRecognizerStateBegan) {
        initialDragLocationX = [[sender view] center].x;
        initialDragLocationY = [[sender view] center].y;
    }
    
    translatedPoint = CGPointMake(initialDragLocationX + translatedPoint.x, initialDragLocationY + translatedPoint.y);
    CGPoint normalizedPoint = [self normalizePointForCenter:translatedPoint];
    [[sender view] setCenter:normalizedPoint];
    
    if ([(UIPanGestureRecognizer*)sender state] == UIGestureRecognizerStateEnded) {
        
        CGFloat velocityX = (0.2*[(UIPanGestureRecognizer*)sender velocityInView:self].x);
        CGFloat velocityY = (0.2*[(UIPanGestureRecognizer*)sender velocityInView:self].y);
        
        CGFloat maxAbsVelocity = MAX(ABS(velocityX), ABS(velocityY));
        if(maxAbsVelocity > 30.0){
            
            CGFloat finalX = translatedPoint.x + velocityX;
            CGFloat finalY = translatedPoint.y + velocityY;
            
            CGFloat animationDuration = (maxAbsVelocity * 0.0002) + 0.2;
            
            [UIView beginAnimations:nil context:NULL];
            [UIView setAnimationDuration:animationDuration];
            [UIView setAnimationCurve:UIViewAnimationCurveEaseOut];
            [UIView setAnimationDelegate:self];
            [UIView setAnimationDidStopSelector:@selector(animationDidFinish)];
            [[sender view] setCenter:CGPointMake(finalX, finalY)];
            [UIView commitAnimations];
        }
    }
}

- (void)startMonitoring
{
    [fpsMonitor resume];
}

- (void)stopMonitoring
{
    [fpsMonitor pause];
}

- (void)animationDidFinish
{
    if(![self isInScreenBoundary:(self.center)]){
        NSLog(@"Floating view is being removed");
        [self stopMonitoring];
        [self removeFromSuperview];
    }
}

- (CGPoint)normalizePointForCenter:(CGPoint)point
{
    CGFloat screenWidth = [[UIScreen mainScreen] bounds].size.width;
    CGFloat screenHeight = [[UIScreen mainScreen] bounds].size.height;
    
    if(point.x < self.frame.size.width/2){
        point.x = self.frame.size.width/2;
    }
    if(point.x > screenWidth - self.frame.size.width/2){
        point.x = screenWidth - self.frame.size.width/2;
    }
    
    if(point.y < self.frame.size.height/2){
        point.y = self.frame.size.height/2;
    }
    if(point.y > screenHeight - self.frame.size.height/2){
        point.y = screenHeight - self.frame.size.height/2;
    }
    
    return point;
}

- (BOOL) isInScreenBoundary:(CGPoint)center
{
    CGFloat screenWidth = [[UIScreen mainScreen] bounds].size.width;
    CGFloat screenHeight = [[UIScreen mainScreen] bounds].size.height;
    
    if(center.x < self.frame.size.width/2){
        return FALSE;
    }
    if(center.x > screenWidth - self.frame.size.width/2){
        return FALSE;
    }
    
    if(center.y < self.frame.size.height/2){
        return FALSE;
    }
    if(center.y > screenHeight - self.frame.size.height/2){
        return FALSE;
    }
    
    return TRUE;
}

- (void)moveTo:(CGPoint)origin
{
    [self setFrame:CGRectMake(origin.x, origin.y, self.frame.size.width, self.frame.size.height)];
}

@end