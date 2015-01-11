#import <UIKit/UIKit.h>

@interface TextInputController : UIViewController <UIScrollViewDelegate, UITextViewDelegate,
UITableViewDelegate, UITableViewDataSource>
{
	UITableView	*myTableView;
	
	NSMutableDictionary *inputDialogResultDict;
	NSString *inputDialogResultKey;
}

@property (nonatomic, retain) UITableView *myTableView;

- (id)init;
- (void)setInputDialogResultDictionary:(NSMutableDictionary*)dict inputKey:(NSString*)key; 

@end

