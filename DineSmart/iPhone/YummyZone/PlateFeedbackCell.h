//
//  PlateFeedbackCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DLStarRatingControl.h"
#import "RatingChangedDelegate.h"


@interface PlateFeedbackCell : UITableViewCell <DLStarRatingDelegate>
{
    id <RatingChangedDelegate> delegate;

    UILabel *_title;
    DLStarRatingControl * _ratingControl;
    UIImageView *_itemImage;

    NSObject *_key;
}

@property (nonatomic, assign) id <RatingChangedDelegate> delegate;

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setTitle:(NSString*)text rating:(NSUInteger)rating key:(NSObject*)key;

- (void)setItemImage:(UIImage *)image;
- (void)displayPlaceholderImage;
- (void)displayBusyImage;

@end
