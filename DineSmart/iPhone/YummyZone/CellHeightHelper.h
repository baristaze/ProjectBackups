#import <Foundation/Foundation.h>


@interface CellHeightHelper : NSObject 
{
}

+ (CGFloat)calculateTitleAndStarCellHeightUsingText:(NSString*)text;
+ (CGFloat)calculateTitleAndYesNoCellHeightUsingText:(NSString*)text;
+ (CGFloat)calculateNotesCellHeightUsingText:(NSString*)text;
+ (CGFloat)calculateTextAndPlaceholderCellHeightUsingText:(NSString*)text;
+ (CGFloat)calculateTitleAndTextCellHeightUsingText:(NSString*)text;

@end

