//
//  TitleAndYesNoCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/28/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol SwitchValueChangedDelegate

- (void)switchValueChanged:(BOOL)newValue key:(NSObject*)key;

@end

@interface TitleAndYesNoCell : UITableViewCell
{
    id <SwitchValueChangedDelegate> delegate;
    
    UILabel *_titleLabel;
    UIColor *_titleTextColor;
    CGFloat _titleWidth;
    
    UISwitch *_switchControl;
    
    NSObject *_key;
}

@property (nonatomic, assign) id <SwitchValueChangedDelegate> delegate;

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)text on:(BOOL)on key:(NSObject*)key;

- (void)setTitleWidth:(CGFloat)newValue;
- (void)setTitleFontSize:(CGFloat)newValue;
- (void)setTitleTextColor:(UIColor*)newColor;
- (void)setTitleTextAlignment:(UITextAlignment)newAlignment;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end
