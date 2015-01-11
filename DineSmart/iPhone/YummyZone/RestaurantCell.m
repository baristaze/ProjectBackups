//
//  FavoriteRestaurantCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "RestaurantCell.h"
#import "KeyConstants.h"
#import "UIImage+Resize.h"


@implementation RestaurantCell

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
		_description.font = [UIFont systemFontOfSize:14];
        _description.textAlignment = UITextAlignmentLeft;
		_description.lineBreakMode = UILineBreakModeWordWrap;
        _description.backgroundColor = [UIColor clearColor];
		_description.textColor = [UIColor blackColor];
		_description.numberOfLines = 0; // Make this multi line
        
		_itemImage = [[UIImageView alloc] initWithFrame:CGRectZero];
		_itemImage.backgroundColor = [UIColor clearColor];
        
        [self.contentView addSubview:_title];
        [self.contentView addSubview:_description];
        [self.contentView addSubview:_itemImage];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
    [_title release];
    [_description release];
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
#define VERTICAL_SEPARATION			5.0

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
                                   contentHeight - TITLE_HEIGHT - TOP_OFFSET - BOTTOM_OFFSET - VERTICAL_SEPARATION );
    
	_itemImage.frame = CGRectMake(LEFT_OFFSET, (contentHeight - IMAGE_HEIGHT) / 2, IMAGE_WIDTH, IMAGE_HEIGHT);
}


- (void)setTitle:(NSString*)title description:(NSString*)description;
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
    }
	else
	{
		_title.textColor = [UIColor blueColor];
		_description.textColor = [UIColor blackColor];
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
    
	_itemImage.image = [UIImage imageNamed:@"restNoImage.png"];
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
    
    [pool release];
}

- (void)displayBusyImage
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	_itemImage.image = [UIImage imageNamed:@"restDownloading.png"];
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
    
    [pool release];
}

@end


