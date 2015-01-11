//
//  FavoriteRestaurantCell.h
//  YummyZone
//
//  Created by Baris Taze on 4/27/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface FavoriteRestaurantCell : UITableViewCell
{
    UILabel *_restaurantName;
    UILabel *_myRatingLabel;
    UILabel *_myRating;
    
    UIImageView *_itemImage;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setName:(NSString*)restaurantName myRating:(NSNumber*)myRating;
- (void)setItemImage:(UIImage *)image;
- (void)displayPlaceholderImage;
- (void)displayBusyImage;

@end