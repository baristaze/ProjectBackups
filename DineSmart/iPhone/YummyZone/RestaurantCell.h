//
//  FavoriteRestaurantCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface RestaurantCell : UITableViewCell
{
    UILabel *_title;
    UILabel *_description;
    
    UIImageView *_itemImage;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)title description:(NSString*)description;
- (void)setItemImage:(UIImage *)image;
- (void)displayPlaceholderImage;
- (void)displayBusyImage;

@end
