#import <UIKit/UIKit.h>


@interface TitleAndTextCell : UITableViewCell 
{
    UILabel *titleLabel;
    UILabel *detailLabel;
	
	UIColor *titleTextColor;
	
	CGFloat titleWidth;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitleText:(NSString*)newTitle;
- (NSString*)getTitleText;

- (void)setDetailText:(NSString*)newText;
- (NSString*)getDetailText;

- (void)setTitleWidth:(CGFloat)newValue;
- (void)setTitleFontSize:(CGFloat)newValue;
- (void)setTitleTextColor:(UIColor*)newColor;
- (void)setTitleTextAlignment:(UITextAlignment)newAlignment;

- (void)setDetailFontSize:(int)newValue;
- (void)setDetailTextAlignment:(UITextAlignment)newAlignment;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end