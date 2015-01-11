//
//  FavoriteMenuItemCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "FavoriteMenuItemCell.h"
#import "KeyConstants.h"
#import "UIImage+Resize.h"

@implementation FavoriteMenuItemCell

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    if (self)
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
        _title = [[UILabel alloc] initWithFrame:CGRectZero];
        _title.font = [UIFont boldSystemFontOfSize:16];
        _title.textAlignment = UITextAlignmentLeft;
		_title.lineBreakMode = UILineBreakModeWordWrap;
        _title.backgroundColor = [UIColor clearColor];
		_title.textColor = [UIColor blueColor];
		
        _restaurant = [[UILabel alloc] initWithFrame:CGRectZero];
		_restaurant.font = [UIFont systemFontOfSize:14];
        _restaurant.textAlignment = UITextAlignmentLeft;
		_restaurant.lineBreakMode = UILineBreakModeWordWrap;
        _restaurant.backgroundColor = [UIColor clearColor];
		_restaurant.textColor = [UIColor blackColor];
        
        _myRating = [[UILabel alloc] initWithFrame:CGRectZero];
		_myRating.font = [UIFont systemFontOfSize:14];
        _myRating.textAlignment = UITextAlignmentLeft;
		_myRating.lineBreakMode = UILineBreakModeWordWrap;
        _myRating.backgroundColor = [UIColor clearColor];
		_myRating.textColor = [UIColor redColor];
        
        _myRatingLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		_myRatingLabel.font = [UIFont systemFontOfSize:14];
        _myRatingLabel.textAlignment = UITextAlignmentLeft;
		_myRatingLabel.lineBreakMode = UILineBreakModeWordWrap;
        _myRatingLabel.backgroundColor = [UIColor clearColor];
		_myRatingLabel.textColor = [UIColor grayColor];
        
		_itemImage = [[UIImageView alloc] initWithFrame:CGRectZero];
		_itemImage.backgroundColor = [UIColor clearColor];
        
        [self.contentView addSubview:_title];
        [self.contentView addSubview:_restaurant];
        [self.contentView addSubview:_myRating];
        [self.contentView addSubview:_myRatingLabel];
        [self.contentView addSubview:_itemImage];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
    [_title release];
    [_restaurant release];
    [_myRating release];
    [_myRatingLabel release];
    [_itemImage release];
    //[_footerImage release];
    [super dealloc];
}

#define TOP_OFFSET					7.0
#define BOTTOM_OFFSET				7.0
#define LEFT_OFFSET					3.0
#define RIGHT_OFFSET				5.0

#define IMAGE_WIDTH					64.0
#define IMAGE_HEIGHT				64.0

#define TITLE_HEIGHT				17.0
#define RESTAURANT_HEIGHT			15.0
#define RATING_HEIGHT				15.0

#define HORIZONTAL_SEPARATION		10.0
#define VERTICAL_SEPARATION			5.0

#define RATING_LABEL                70.0

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	CGFloat contentWidth = self.contentView.bounds.size.width;
	CGFloat contentHeight = self.contentView.bounds.size.height;
    
	_title.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION,
							  TOP_OFFSET,
							  contentWidth - IMAGE_WIDTH - LEFT_OFFSET - RIGHT_OFFSET - HORIZONTAL_SEPARATION,
							  TITLE_HEIGHT);
	
    _restaurant.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION,
                                   TOP_OFFSET + TITLE_HEIGHT + VERTICAL_SEPARATION,
                                   contentWidth - IMAGE_WIDTH - LEFT_OFFSET - RIGHT_OFFSET - HORIZONTAL_SEPARATION,
                                   RESTAURANT_HEIGHT);
            
	_myRatingLabel.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION,
                                 TOP_OFFSET + TITLE_HEIGHT + RESTAURANT_HEIGHT + 2 * VERTICAL_SEPARATION, 
                                 RATING_LABEL,
                                 RATING_HEIGHT);
    
    _myRating.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION + RATING_LABEL,
                                 TOP_OFFSET + TITLE_HEIGHT + RESTAURANT_HEIGHT + 2 * VERTICAL_SEPARATION, 
                                 contentWidth - IMAGE_WIDTH - LEFT_OFFSET - RIGHT_OFFSET - HORIZONTAL_SEPARATION - RATING_LABEL,
                                 RATING_HEIGHT);
	
	_itemImage.frame = CGRectMake(LEFT_OFFSET, (contentHeight - IMAGE_HEIGHT) / 2, IMAGE_WIDTH, IMAGE_HEIGHT);
}


- (void)setTitle:(NSString*)title restaurant:(NSString*)restaurant myRating:(NSNumber*)myRating;
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    if (title != nil)
    {
        _title.text = title;
    }
    else
    {
        _title.text = @"";
    }
	
    if (restaurant != nil)
    {
        _restaurant.text = restaurant;
    }
    else
    {
        _restaurant.text = @"";
    }
    
    if (myRating != nil)
    {
        if ([myRating floatValue] <= 5 && [myRating floatValue] >= 1)
        {
            _myRatingLabel.text = @"My rating: ";
            _myRating.text = [NSString stringWithFormat:@"%@", myRating];
        }
    }
    
	[pool release];
}


- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        _title.textColor = [UIColor whiteColor];
        _restaurant.textColor = [UIColor whiteColor];
        _myRating.textColor = [UIColor whiteColor];
        _myRatingLabel.textColor = [UIColor whiteColor];
    }
	else
	{
		_title.textColor = [UIColor blueColor];
		_restaurant.textColor = [UIColor blackColor];
		_myRating.textColor = [UIColor redColor];
        _myRatingLabel.textColor = [UIColor grayColor];
    }
	
	[pool release];
}


- (void)setItemImage:(UIImage *)image
{
	_itemImage.image = image;
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
}

- (void)displayPlaceholderImage
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	_itemImage.image = [UIImage imageNamed:@"plateNoPhoto.png"];
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
    
    [pool release];
}

- (void)displayBusyImage
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	_itemImage.image = [UIImage imageNamed:@"plateDownloading.png"];
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
    
    [pool release];
}

@end


