//
//  QuickHealthViewItem.m
//  FPSTest
//
//  Created by Baris Taze on 3/19/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "QuickHealthViewItem.h"

@implementation QuickHealthViewItem
{
    UILabel* _titleLabel;
    UIView* _titleBackground;
}

- (instancetype)initWithSize:(CGFloat)size
{
    // createa a square frame
    if(self = [super initWithFrame:CGRectMake(0, 0, size, size)]){
        
        // make background transparent
        [self setBackgroundColor:[UIColor clearColor]];
        
        // draw a circle...
        // draw a rectangle first
        _titleBackground = [[UIView alloc] initWithFrame:CGRectMake(0, 0, size, size)];
        // round its corners to make a circle
        _titleBackground.layer.cornerRadius = size / 2.0f;
        // set its background to a neutral color: white
        _titleBackground.backgroundColor = [UIColor yellowColor];
        
        // create a label for the title
        _titleLabel = [[UILabel alloc]initWithFrame:CGRectMake(0, 0, size, size)];
        _titleLabel.text = @"?";
        _titleLabel.font = [UIFont fontWithName:@"ClanPro-Book" size:12.0f];
        _titleLabel.numberOfLines = 1;
        _titleLabel.baselineAdjustment = UIBaselineAdjustmentAlignCenters;
        _titleLabel.adjustsFontSizeToFitWidth = YES;
        _titleLabel.minimumScaleFactor = 10.0f/12.0f;
        _titleLabel.clipsToBounds = YES;
        _titleLabel.backgroundColor = [UIColor clearColor];
        _titleLabel.textColor = [UIColor blackColor];
        _titleLabel.textAlignment = NSTextAlignmentCenter;
        
        // add child views
        [self addSubview:_titleBackground];
        [self addSubview:_titleLabel];
    }
    
    return self;
}

- (void)setTitleWithStatus :(NSString *)title :(HealthStatus)status
{
    _title = title;
    _status = status;
    
    // set title text
    _titleLabel.text = title;
    
    // adjust color
    if(status == Green){
        _titleLabel.textColor = [UIColor whiteColor];
        _titleBackground.backgroundColor = [UIColor colorWithRed:0.0 green:0.5 blue:0.0 alpha:1.0];
    }
    else if(status == Amber){
        _titleLabel.textColor = [UIColor blackColor];
        _titleBackground.backgroundColor = [UIColor colorWithRed:1.0 green:0.6 blue:0.0 alpha:1.0];
    }
    else if(status == Red){
        _titleLabel.textColor = [UIColor whiteColor];
        _titleBackground.backgroundColor = [UIColor colorWithRed:1.0 green:0.0 blue:0.0 alpha:1.0];
    }
    else{
        _titleLabel.textColor = [UIColor blackColor];
        _titleBackground.backgroundColor = [UIColor colorWithRed:1.0 green:1.0 blue:0.0 alpha:1.0];
    }
}

@end
