//
//  FloatingViewController.m
//  FPSTest
//
//  Created by Baris Taze on 3/19/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#ifndef FPSTest_FloatingViewController_m
#define FPSTest_FloatingViewController_m

#import "FloatingView.h"
#import "FloatingViewController.h"

@implementation FloatingViewController

- (void)loadView
{
    self.view = [[FloatingView alloc] init];
}

- (id) init
{
    NSLog(@"FloatingViewController::init");
    if(self = [super init]){
    }
    return self;
}

- (void)viewDidLoad
{
    NSLog(@"FloatingViewController::viewDidLoad");
    [super viewDidLoad];
}

- (void)viewWillAppear:(BOOL)animated
{
    NSLog(@"FloatingViewController::viewWillAppear");
    [super viewWillAppear:animated];
}

- (void)didReceiveMemoryWarning
{
    NSLog(@"FloatingViewController::didReceiveMemoryWarning");
    [super didReceiveMemoryWarning];
}

- (void)viewWillDisappear:(BOOL)animated
{
    NSLog(@"FloatingViewController::viewWillDisappear");
}

+ (id)sharedInstance {
    static FloatingViewController *sharedViewController = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedViewController = [[self alloc] init];
    });
    return sharedViewController;
}

@end

#endif
