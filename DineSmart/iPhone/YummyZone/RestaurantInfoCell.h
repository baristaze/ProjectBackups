//
//  RestaurantInfoCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface RestaurantInfoCell : UITableViewCell 
{
    UILabel *_titleLabel;
    UILabel *_detailLabel;
	UILabel *_distanceLabel;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitleText:(NSString*)newText;
- (NSString*)getTitleText;

- (void)setDetailText:(NSString*)newText;
- (NSString*)getDetailText;

- (void)setDistanceText:(NSString*)newText;
- (NSString*)getDistanceText;

@end
