#import <UIKit/UIKit.h>

@interface SingleSelectKeyedListViewController : UIViewController <UITableViewDelegate, UITableViewDataSource>
{
	UITableView	*_tableView;
	
	NSArray *_inputArray;
    NSString *_inputDisplayKey;
    NSString *_inputResultKey;

    NSMutableDictionary *_outputDict;
    NSString *_outputKey;
    
	int _selectionIndex;
}

- (id)init;

- (void)setInputArray:(NSArray*)inputArray inputDisplayKey:(NSString*)inputDisplayKey 
       inputResultKey:(NSString*)inputResultKey outputDict:(NSMutableDictionary*)outputDict
            outputKey:(NSString*)outputKey title:(NSString*)title;

@end
