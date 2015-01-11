#import <UIKit/UIKit.h>

@interface TextAndPlaceholderCell : UITableViewCell 
{
    UILabel *titleLabel;
	BOOL placeholderMode;
	
	UIColor *myTextColor;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTextContent:(NSString*)newText;
- (NSString*)getTextContent;
- (void)setTextContentAlignment:(UITextAlignment)alignment;
- (void)setTextContentColor:(UIColor*)color;
- (void)setTextContentFont:(UIFont*)font;

- (void)setPlaceholderMode:(BOOL)flag;
- (BOOL)getPlaceholderMode;

+ (CGFloat)getTotalMissingTextWidth;
+ (CGFloat)getTotalMissingTextHeight;

@end
