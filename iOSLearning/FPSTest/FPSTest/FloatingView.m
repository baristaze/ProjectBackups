//
//  FloatingView.m
//  FPSTest
//
//  Created by Baris Taze on 3/18/15.
//  Copyright (c) 2015 Uber. All rights reserved.
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
    BOOL isStarted;
    
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
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (id) initWithSize:(CGFloat)size
{
    NSLog(@"FloatingView::init");
    
    CGFloat heightF = 1.0f;
    //CGFloat heightF = 2.0f; // if cpuView is enabled
    self = [super initWithFrame:CGRectMake(INIT_ORIG_X, INIT_ORIG_Y, size, size * heightF)];
    if(self){
        
        isStarted = FALSE;
        
        // dim the background
        //[self setBackgroundColor:[UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:0.5]];
        
        // register for pan gesture
        [self registerForPanGesture];
        
        // register for tap gesture
        [self registerForTapAndHoldGesture];
        [self registerForDoubleTapGesture];
        
        // create FPS view and add it
        fpsView = [[QuickHealthViewItem alloc] initWithSize:size];
        [self addSubview:fpsView];
        
        // register for FPS update
        [self registerForFPSUpdates];
        
        // create FPS monitor
        fpsMonitor = [[FPSMonitor alloc] init];
        
        /*
        // create CPU view and add it
        cpuView = [[QuickHealthViewItem alloc] initWithSize:size];
        [cpuView setCenter:CGPointMake(size*0.5f, size * 1.5f)];
        [self addSubview:cpuView];
        */
        
        //[fpsView setTitleWithStatus:@"35" :Amber];
        //[cpuView setTitleWithStatus:@"48" :Red];
    }
    
    return self;
}

- (void)registerForDoubleTapGesture
{
    UITapGestureRecognizer *doubleTapGesture =
    [[UITapGestureRecognizer alloc] initWithTarget:self action:@selector(onDoubleTapped:)];
    
    [doubleTapGesture setNumberOfTapsRequired:2];
    [doubleTapGesture setNumberOfTouchesRequired:1];
    [self addGestureRecognizer:doubleTapGesture];
}

- (void)registerForTapAndHoldGesture
{
    UILongPressGestureRecognizer *tapAndHoldGesture =
        [[UILongPressGestureRecognizer alloc] initWithTarget:self action:@selector(onTapAndHold:)];
    
    //[tapAndHoldGesture setMinimumPressDuration:0.5f];
    //[tapAndHoldGesture setNumberOfTapsRequired:1];
    //[tapAndHoldGesture setNumberOfTouchesRequired:1];
    [self addGestureRecognizer:tapAndHoldGesture];
}

- (void) onDoubleTapped: (UITapGestureRecognizer*)recognizer
{
    if ([recognizer state] == UIGestureRecognizerStateEnded) {
        [self showInfo];
    }
}

- (void) onTapAndHold: (UILongPressGestureRecognizer *)recognizer
{
    if ([recognizer state] == UIGestureRecognizerStateBegan) {
        // [self showInfo];
        if(isStarted){
            [self stopMonitoring];
            [fpsView setTitleWithStatus:@"FPS" :Unknown];
        }
        else{
            [self startMonitoring];
        }
    }
}

- (void)showInfo
{
    UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Info"
                                                    message:@"Shows minimum FPS (Frame Per Second) for last 0.5 seconds. Swipe fast to get rid of it."
                                                   delegate:self
                                          cancelButtonTitle:@"OK"
                                          otherButtonTitles:nil];
    [alert show];

}

// Start observing the middleware for changes
- (void)registerForFPSUpdates {
    [[NSNotificationCenter defaultCenter]
     addObserver:self
     selector:@selector(fpsUpdated:)
     name:FramePerSecondUpdatedNotification
     object:nil];
}

- (void)fpsUpdated:(NSNotification *) notification
{
    FPSData *fpsData = (FPSData *)[notification object];
    CGFloat minFPS = fpsData.minFPS;
    NSString* minFpsText = [NSString stringWithFormat:@"%.f", minFPS];
    if(minFPS <= 20.0){
        [fpsView setTitleWithStatus:minFpsText :Red];
    }
    else if(minFPS <= 40.0){
        [fpsView setTitleWithStatus:minFpsText :Amber];
    }
    else {
        [fpsView setTitleWithStatus:minFpsText :Green];
    }
}

- (void) registerForPanGesture
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
    isStarted = TRUE;
}

- (void)stopMonitoring
{
    [fpsMonitor pause];
    isStarted = FALSE;
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