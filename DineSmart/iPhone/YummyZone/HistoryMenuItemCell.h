//
//  HistoryMenuItemCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface HistoryMenuItemCell : UITableViewCell
{
    UILabel *_title;
    UILabel *_myRating;
    UILabel *_myRatingLabel;
    UIImageView *_itemImage;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)title myRating:(NSNumber*)myRating;
- (void)setItemImage:(UIImage *)image;
- (void)displayPlaceholderImage;
- (void)displayBusyImage;

@end
