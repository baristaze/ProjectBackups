//
//  TextAnd2PlaceholderCell.h
//  YummyZone
//
//  Created by Baris Taze on 4/28/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TextAnd2PlaceholderCell : UITableViewCell 
{
    UILabel *titleLabel;
    UILabel *placeholderLabel;
	BOOL placeholderMode;
	
	UIColor *myTextColor;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTextContent:(NSString*)titleText placeholderText:(NSString*)placeholderText;
- (NSString*)getTextContent;
- (void)setTextContentAlignment:(UITextAlignment)alignment;
- (void)setTextContentColor:(UIColor*)color;
- (void)setTextContentFont:(UIFont*)font;

- (void)setPlaceholderMode:(BOOL)flag;
- (BOOL)getPlaceholderMode;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end

