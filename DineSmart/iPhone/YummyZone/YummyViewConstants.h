//
//  YummyViewConstants.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/28/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface YummyViewConstants : NSObject
{
	UIFont *textAndPlaceholderCellFont;
    UIFont *titleAndTextCellTitleFont;
    UIFont *titleAndTextCellTextFont;
    UIFont *titleAndStarCellFont;
	UIFont *titleAndYesNoCellFont;
    
    UIImage *multiEditUnSelectedCheckImage;
    UIImage *multiEditSelectedCheckImage;
    
    UIColor *maroonColor;
}

+ (YummyViewConstants*)singleton;

- (UIFont*)getTextAndPlaceholderCellFont;
- (UIFont*)getTitleAndTextCellTitleFont;
- (UIFont*)getTitleAndTextCellTextFont;
- (UIFont*)getTitleAndStarCellFont;
- (UIFont*)getTitleAndYesNoCellFont;

- (UIImage*)getMultiEditUnSelectedCheckImage;
- (UIImage*)getMultiEditSelectedCheckImage;

- (UIColor*)getMaroonColor;

@end
