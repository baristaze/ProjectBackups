//
//  PlateFeedbackCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "PlateFeedbackCell.h"
#import "UIImage+Resize.h"


@implementation PlateFeedbackCell

@synthesize delegate;

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    if (self)
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
        _title = [[UILabel alloc] initWithFrame:CGRectZero];
        _title.font = [UIFont boldSystemFontOfSize:14];
        _title.textAlignment = UITextAlignmentLeft;
		_title.lineBreakMode = UILineBreakModeWordWrap;
        _title.backgroundColor = [UIColor clearColor];
		_title.textColor = [UIColor blackColor];

        _itemImage = [[UIImageView alloc] initWithFrame:CGRectZero];
		_itemImage.backgroundColor = [UIColor clearColor];

        _ratingControl = [[DLStarRatingControl alloc] initWithFrame:CGRectZero];
        _ratingControl.delegate = self;
        
        [self.contentView addSubview:_title];
        [self.contentView addSubview:_itemImage];
        [self.contentView addSubview:_ratingControl];

        _key = nil;
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
    _ratingControl.delegate = nil;
    [_ratingControl release];
    [_itemImage release];
    [_key release];
    [_title release];
    [super dealloc];
}

#define IMAGE_WIDTH                 48
#define IMAGE_HEIGHT                48
#define IMAGE_TEXT_SEPARATION       15.0
#define TITLE_HEIGHT                15.0
#define TITLE_RATING_SEPARATION     5.0
#define TOP_OFFSET                  5.0
#define BOTTOM_OFFSET               5.0
#define LEFT_OFFSET                 10.0
#define RIGHT_OFFSET                10.0

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	CGFloat contentWidth = self.contentView.bounds.size.width;
	CGFloat contentHeight = self.contentView.bounds.size.height;
	
    _itemImage.frame = CGRectMake(LEFT_OFFSET, TOP_OFFSET, IMAGE_WIDTH, IMAGE_HEIGHT);

	_title.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + IMAGE_TEXT_SEPARATION, 
                              TOP_OFFSET, 
                              contentWidth - LEFT_OFFSET - RIGHT_OFFSET - IMAGE_WIDTH - IMAGE_TEXT_SEPARATION, 
                              TITLE_HEIGHT);
    
    _ratingControl.frame = CGRectMake(LEFT_OFFSET + IMAGE_WIDTH + IMAGE_TEXT_SEPARATION, 
                                      TOP_OFFSET + TITLE_HEIGHT + TITLE_RATING_SEPARATION + 5, // BUGBUG: HACK
                                      contentWidth - LEFT_OFFSET - RIGHT_OFFSET - IMAGE_WIDTH - IMAGE_TEXT_SEPARATION + 10, 
                                      contentHeight - TITLE_HEIGHT - TITLE_RATING_SEPARATION - BOTTOM_OFFSET + 3);
}


- (void)setItemImage:(UIImage *)image
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    _itemImage.image = [UIImage imageUsingImage:image width:IMAGE_WIDTH height:IMAGE_HEIGHT onlyShrink:TRUE];
	//_itemImage.image = image;
	[_itemImage sizeToFit];
	[_itemImage setNeedsDisplay];
    
    [pool release];
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


- (void)setTitle:(NSString*)text rating:(NSUInteger)rating key:(NSObject*)key
{
    _title.text = text;
    _ratingControl.rating = rating;
    
    [key retain];
    [_key release];
    _key = key;
}


- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        _title.textColor = [UIColor whiteColor];
    }
	else
	{
		_title.textColor = [UIColor blackColor];
    }
	
	[pool release];
}


- (void)newRating:(int)rating
{
    [delegate ratingChanged:rating key:_key];
}

@end
