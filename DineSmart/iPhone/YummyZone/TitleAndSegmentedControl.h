//
//  TitleAndSegmentedControl.h
//  YummyZone
//
//  Created by Baris Taze on 3/4/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol SegmentValueChangedDelegate

- (void)segmentValueChanged:(int)newValue key:(NSObject*)key;

@end

@interface TitleAndSegmentedControl : UITableViewCell
{
    id <SegmentValueChangedDelegate> delegate;
    
    UILabel *_titleLabel;
    UIColor *_titleTextColor;
    CGFloat _titleWidth;
    
    UISegmentedControl *_segmentedControl;
    
    NSObject *_key;
}

@property (nonatomic, assign) id <SegmentValueChangedDelegate> delegate;

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)text value:(int)value key:(NSObject*)key;

- (void)setTitleWidth:(CGFloat)newValue;
- (void)setTitleFontSize:(CGFloat)newValue;
- (void)setTitleTextColor:(UIColor*)newColor;
- (void)setTitleTextAlignment:(UITextAlignment)newAlignment;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end
