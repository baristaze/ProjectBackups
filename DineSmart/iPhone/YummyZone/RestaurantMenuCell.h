//
//  RestaurantMenuCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface RestaurantMenuCell : UITableViewCell
{
    UILabel *_title;
    UILabel *_description;
    UILabel *_price;
    UILabel *_footer1;
    UILabel *_footer2;
    UILabel *_myRate;

    UIImageView *_itemImage;
    //UIImageView *_footerImage;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)title description:(NSString*)description price:(NSNumber*)price myRating:(NSNumber*)myRating isTopPick:(NSNumber*)isTopPick;
- (void)setItemImage:(UIImage *)image;
- (void)displayPlaceholderImage;
- (void)displayBusyImage;
- (void)setPriceHidden:(BOOL)hidden;

@end
