//
//  RestaurantMenuCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "RestaurantMenuCell.h"
#import "KeyConstants.h"
#import "UIImage+Resize.h"
#import "YummyViewConstants.h"


@implementation RestaurantMenuCell

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
		
        _description = [[UILabel alloc] initWithFrame:CGRectZero];
		_description.font = [UIFont systemFontOfSize:13];
        _description.textAlignment = UITextAlignmentLeft;
		_description.lineBreakMode = UILineBreakModeWordWrap;
        _description.backgroundColor = [UIColor clearColor];
		_description.textColor = [UIColor blackColor];
		_description.numberOfLines = 0; // Make this multi line
        
        _price = [[UILabel alloc] initWithFrame:CGRectZero];
		_price.font = [UIFont systemFontOfSize:14];
        _price.textAlignment = UITextAlignmentRight;
		_price.lineBreakMode = UILineBreakModeWordWrap;
        _price.backgroundColor = [UIColor clearColor];
		_price.textColor = [UIColor colorWithRed:80.0/255.0 green:139.0/255.0 blue:145.0/255.0 alpha:1.0];
       
        _footer1 = [[UILabel alloc] initWithFrame:CGRectZero];
		_footer1.font = [UIFont systemFontOfSize:13];
        _footer1.textAlignment = UITextAlignmentLeft;
		_footer1.lineBreakMode = UILineBreakModeWordWrap;
        _footer1.backgroundColor = [UIColor clearColor];
		_footer1.textColor = [UIColor grayColor];
        
        _footer2 = [[UILabel alloc] initWithFrame:CGRectZero];
		_footer2.font = [UIFont systemFontOfSize:13];
        _footer2.textAlignment = UITextAlignmentLeft;
		_footer2.lineBreakMode = UILineBreakModeWordWrap;
        _footer2.backgroundColor = [UIColor clearColor];
		_footer2.textColor = [UIColor orangeColor];
        
        _myRate = [[UILabel alloc] initWithFrame:CGRectZero];
		_myRate.font = [UIFont systemFontOfSize:13];
        _myRate.textAlignment = UITextAlignmentLeft;
		_myRate.lineBreakMode = UILineBreakModeWordWrap;
        _myRate.backgroundColor = [UIColor clearColor];
		_myRate.textColor = [UIColor redColor];

		_itemImage = [[UIImageView alloc] initWithFrame:CGRectZero];
		_itemImage.backgroundColor = [UIColor clearColor];

		//_footerImage = [[UIImageView alloc] initWithFrame:CGRectZero];
		//_footerImage.backgroundColor = [UIColor clearColor];

        [self.contentView addSubview:_title];
        [self.contentView addSubview:_description];
        [self.contentView addSubview:_price];
        [self.contentView addSubview:_footer1];
        [self.contentView addSubview:_footer2];
        [self.contentView addSubview:_myRate];
        [self.contentView addSubview:_itemImage];
        //[self.contentView addSubview:_footerImage];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
    [_title release];
    [_description release];
    [_price release];
    [_footer1 release];
    [_footer2 release];
    [_myRate release];
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

#define HORIZONTAL_SEPARATION		10.0
#define VERTICAL_SEPARATION			0.0
#define PRICE_WIDTH					100.0
#define FOOTER_WIDTH				65.0
#define FOOTER_HEIGHT				20.0

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	CGFloat contentWidth = self.contentView.bounds.size.width;
	CGFloat contentHeight = self.contentView.bounds.size.height;
	
	_title.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION,
							  TOP_OFFSET,
							  contentWidth - IMAGE_WIDTH - LEFT_OFFSET - RIGHT_OFFSET - HORIZONTAL_SEPARATION,
							  TITLE_HEIGHT);
	
	_description.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION,
									TOP_OFFSET + TITLE_HEIGHT + VERTICAL_SEPARATION,
									contentWidth - IMAGE_WIDTH - LEFT_OFFSET - RIGHT_OFFSET - HORIZONTAL_SEPARATION,
									contentHeight - TOP_OFFSET - BOTTOM_OFFSET - TITLE_HEIGHT - FOOTER_HEIGHT - (2 * VERTICAL_SEPARATION));

	_price.frame = CGRectMake(contentWidth - PRICE_WIDTH - RIGHT_OFFSET, 
							  contentHeight - FOOTER_HEIGHT - BOTTOM_OFFSET, 
							  PRICE_WIDTH, 
							  FOOTER_HEIGHT);
	
	_footer1.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION, 
							  contentHeight - FOOTER_HEIGHT - BOTTOM_OFFSET, 
							  FOOTER_WIDTH, 
							  FOOTER_HEIGHT);
    
    _footer2.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION, 
                                contentHeight - FOOTER_HEIGHT - BOTTOM_OFFSET, 
                                FOOTER_WIDTH, 
                                FOOTER_HEIGHT);
    
    _myRate.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + HORIZONTAL_SEPARATION + FOOTER_WIDTH, 
                                contentHeight - FOOTER_HEIGHT - BOTTOM_OFFSET, 
                                FOOTER_WIDTH, 
                                FOOTER_HEIGHT);

	_itemImage.frame = CGRectMake(LEFT_OFFSET, (contentHeight - IMAGE_HEIGHT) / 2, IMAGE_WIDTH, IMAGE_HEIGHT);
	
//	_footerImage.frame = CGRectMake(LEFT_OFFSET,
//									contentHeight - FOOTER_HEIGHT - BOTTOM_OFFSET,
//									FOOTER_WIDTH,
//									FOOTER_HEIGHT);
}


- (void)setTitle:(NSString*)title description:(NSString*)description price:(NSNumber*)price myRating:(NSNumber*)myRating isTopPick:(NSNumber*)isTopPick
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
	
    if (description != nil)
    {
        _description.text = description;
    }
    else
    {
        _description.text = @"";
    }
    
    // price = [NSNumber numberWithDouble:19.50];
    if (price != nil)
    {
        if([price floatValue] > 0.0)
        {
            NSString *priceString = [[NSString alloc] initWithFormat:@"$%3.1f", [price floatValue]];
            _price.text = priceString;
            [priceString release];
        }
        else
        {
            _price.text = @"";
        }
    }
    else
    {
        _price.text = @"";
    }
	    
    _footer1.text = @"My rating: ";
    _footer2.text = @"Top pick";
    _myRate.text = @"";
    
    _footer1.hidden = YES;
    _footer2.hidden = YES;
    _myRate.hidden = YES;
    
    BOOL footerSet = NO;
    
    if (myRating != nil)
    {
        if ([myRating floatValue] <= 5 && [myRating floatValue] >= 1)
        {
            _myRate.text = [NSString stringWithFormat:@"%@", myRating];
            _footer1.hidden = NO;
            _myRate.hidden = NO;
            footerSet = YES;
        }
    }
    
    if (footerSet == NO && isTopPick != nil)
    {
        if ([isTopPick boolValue])
        {
            _footer2.hidden = NO;
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
        _description.textColor = [UIColor whiteColor];
        _price.textColor = [UIColor whiteColor];
        _footer1.textColor = [UIColor whiteColor];
        _footer2.textColor = [UIColor whiteColor];
        _myRate.textColor = [UIColor whiteColor];
    }
	else
	{
		_title.textColor = [UIColor blueColor];
		_description.textColor = [UIColor blackColor];
        _price.textColor = [UIColor colorWithRed:80.0/255.0 green:139.0/255.0 blue:145.0/255.0 alpha:1.0];
        _footer1.textColor = [UIColor grayColor];
        _footer2.textColor = [UIColor orangeColor];
        _myRate.textColor = [UIColor redColor];
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

- (void)setPriceHidden:(BOOL)hidden
{
    _price.hidden = hidden;
}

@end


