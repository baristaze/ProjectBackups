#import <UIKit/UIKit.h>

@interface TextViewCell : UITableViewCell
{
    UITextView *textView;
}

- (id)initWithReuseIdentifier:(NSString *)identifier;

@property (nonatomic, retain) UITextView *textView;

@end
