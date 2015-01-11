#import "CellHeightHelper.h"
#import "YummyViewConstants.h"
#import "TitleAndStarCell.h"
#import "TextAndPlaceholderCell.h"
#import "TitleAndTextCell.h"
#import "TitleAndSegmentedControl.h"

@implementation CellHeightHelper

+(id)alloc
{
	return [super alloc];
}

#define GENERICCELL_WIDTH			300.0
#define GENERICCELL_MINHEIGHT		54.0

#define TITLESTARCELL_MAXHEIGHT		80.0

+ (CGFloat)calculateTitleAndStarCellHeightUsingText:(NSString*)text
{
	CGFloat availableWidth = GENERICCELL_WIDTH - [TitleAndStarCell getTotalMissingTextWidth];
	CGFloat verticalInsets = [TitleAndStarCell getTotalMissingTextHeight];
	
	CGSize size = [text sizeWithFont:[[YummyViewConstants singleton] getTitleAndStarCellFont] 
				   constrainedToSize:CGSizeMake(availableWidth, TITLESTARCELL_MAXHEIGHT) lineBreakMode:UILineBreakModeWordWrap];
	
	CGFloat requiredHeight = size.height + verticalInsets;
	
	if (requiredHeight < GENERICCELL_MINHEIGHT)
		return GENERICCELL_MINHEIGHT;
	else
		return requiredHeight;
}

+ (CGFloat)calculateTitleAndYesNoCellHeightUsingText:(NSString*)text
{
	CGFloat availableWidth = GENERICCELL_WIDTH - [TitleAndSegmentedControl getTotalMissingTextWidth];
	CGFloat verticalInsets = [TitleAndSegmentedControl getTotalMissingTextHeight];
	
	CGSize size = [text sizeWithFont:[[YummyViewConstants singleton] getTitleAndYesNoCellFont] 
				   constrainedToSize:CGSizeMake(availableWidth, TITLESTARCELL_MAXHEIGHT) lineBreakMode:UILineBreakModeWordWrap];
	
	CGFloat requiredHeight = size.height + verticalInsets;
	
	if (requiredHeight < GENERICCELL_MINHEIGHT)
		return GENERICCELL_MINHEIGHT;
	else
		return requiredHeight;
}


#define NOTESCELL_MAXHEIGHT			120.0

+ (CGFloat)calculateNotesCellHeightUsingText:(NSString*)text
{
	CGFloat availableWidth = GENERICCELL_WIDTH - [TextAndPlaceholderCell getTotalMissingTextWidth];
	CGFloat verticalInsets = [TextAndPlaceholderCell getTotalMissingTextHeight];
	
	CGSize size = [text sizeWithFont:[[YummyViewConstants singleton] getTextAndPlaceholderCellFont]
				   constrainedToSize:CGSizeMake(availableWidth, NOTESCELL_MAXHEIGHT) lineBreakMode:UILineBreakModeWordWrap];
	
	CGFloat requiredHeight = size.height + verticalInsets;
	
	if (requiredHeight < GENERICCELL_MINHEIGHT)
		return GENERICCELL_MINHEIGHT;
	else
		return requiredHeight;
}


#define TEXTONLYCELL_MAXHEIGHT		80.0

+ (CGFloat)calculateTextAndPlaceholderCellHeightUsingText:(NSString*)text
{
	CGFloat availableWidth = GENERICCELL_WIDTH - [TextAndPlaceholderCell getTotalMissingTextWidth];
	CGFloat verticalInsets = [TextAndPlaceholderCell getTotalMissingTextHeight];
	
	CGSize size = [text sizeWithFont:[[YummyViewConstants singleton] getTextAndPlaceholderCellFont]
				   constrainedToSize:CGSizeMake(availableWidth, TEXTONLYCELL_MAXHEIGHT) lineBreakMode:UILineBreakModeWordWrap];
	
	CGFloat requiredHeight = size.height + verticalInsets;
	
	if (requiredHeight < GENERICCELL_MINHEIGHT)
		return GENERICCELL_MINHEIGHT;
	else
		return requiredHeight;
}

#define TITLEANDTEXTCELL_MAXHEIGHT		80.0

+ (CGFloat)calculateTitleAndTextCellHeightUsingText:(NSString*)text
{
	CGFloat availableWidth = GENERICCELL_WIDTH - [TitleAndTextCell getTotalMissingTextWidth];
	CGFloat verticalInsets = [TitleAndTextCell getTotalMissingTextHeight];
	
	CGSize size = [text sizeWithFont:[[YummyViewConstants singleton] getTitleAndTextCellTextFont]
				   constrainedToSize:CGSizeMake(availableWidth, TITLEANDTEXTCELL_MAXHEIGHT) lineBreakMode:UILineBreakModeWordWrap];
	
	CGFloat requiredHeight = size.height + verticalInsets;
	
	if (requiredHeight < GENERICCELL_MINHEIGHT)
		return GENERICCELL_MINHEIGHT;
	else
		return requiredHeight;
}

@end
