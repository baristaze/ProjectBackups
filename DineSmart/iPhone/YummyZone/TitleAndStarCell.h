//
//  GenericFeedbackCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DLStarRatingControl.h"
#import "RatingChangedDelegate.h"

@interface TitleAndStarCell : UITableViewCell <DLStarRatingDelegate>
{
    id <RatingChangedDelegate> delegate;

    UILabel *_titleLabel;
    DLStarRatingControl * _ratingControl;
    
    UIColor *_titleTextColor;
    
    CGFloat _titleWidth;
    
    NSObject *_key;
}

@property (nonatomic, assign) id <RatingChangedDelegate> delegate;

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)text rating:(NSUInteger)rating key:(NSObject*)key;

- (void)setTitleWidth:(CGFloat)newValue;
- (void)setTitleFontSize:(CGFloat)newValue;
- (void)setTitleTextColor:(UIColor*)newColor;
- (void)setTitleTextAlignment:(UITextAlignment)newAlignment;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end
