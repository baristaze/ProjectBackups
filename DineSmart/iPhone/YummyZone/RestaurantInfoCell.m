//
//  RestaurantInfoCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "RestaurantInfoCell.h"


@implementation RestaurantInfoCell

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    
    if (self) 
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
        _titleLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        _titleLabel.font = [UIFont boldSystemFontOfSize:16];
        _titleLabel.textAlignment = UITextAlignmentLeft;
		_titleLabel.lineBreakMode = UILineBreakModeWordWrap;
        _titleLabel.backgroundColor = [UIColor clearColor];
		_titleLabel.textColor = [UIColor blueColor];
		
        _detailLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		_detailLabel.font = [UIFont systemFontOfSize:14];
        _detailLabel.textAlignment = UITextAlignmentLeft;
        _detailLabel.backgroundColor = [UIColor clearColor];
		_detailLabel.textColor = [UIColor blackColor];
        
        _distanceLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		_distanceLabel.font = [UIFont systemFontOfSize:14];
        _distanceLabel.textAlignment = UITextAlignmentRight;
		_distanceLabel.lineBreakMode = UILineBreakModeWordWrap;
        _distanceLabel.backgroundColor = [UIColor clearColor];
		_distanceLabel.textColor = [UIColor redColor];

        [self.contentView addSubview:_titleLabel];
        [self.contentView addSubview:_detailLabel];
        [self.contentView addSubview:_distanceLabel];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
    [_titleLabel release];
    [_detailLabel release];
	[_distanceLabel release];
    [super dealloc];
}

#define LEFT_OFFSET					15.0
#define RIGHT_OFFSET				15.0
#define VERTICAL_OFFSET				7.0
#define TITLE_DETAIL_SEPARATION		0.0
#define DISTANCE_WIDTH				60.0
#define TITLE_HEIGHT				18.0

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	_titleLabel.frame = CGRectMake(LEFT_OFFSET,
								  VERTICAL_OFFSET, 
								  self.contentView.bounds.size.width - DISTANCE_WIDTH - RIGHT_OFFSET - LEFT_OFFSET, 
								  TITLE_HEIGHT);
	
	_detailLabel.frame = CGRectMake(LEFT_OFFSET,
								   VERTICAL_OFFSET + TITLE_HEIGHT + TITLE_DETAIL_SEPARATION,
								   self.contentView.bounds.size.width - DISTANCE_WIDTH - RIGHT_OFFSET - LEFT_OFFSET, 
								   self.contentView.bounds.size.height - TITLE_HEIGHT - TITLE_DETAIL_SEPARATION - VERTICAL_OFFSET);

	_distanceLabel.frame = CGRectMake(self.contentView.bounds.size.width - DISTANCE_WIDTH - RIGHT_OFFSET, 
								  VERTICAL_OFFSET, 
								  DISTANCE_WIDTH, 
								  self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
}

- (void)setTitleText:(NSString*)newText
{
	_titleLabel.text = newText;
}

- (NSString*)getTitleText
{
	return _titleLabel.text;
}

- (void)setDetailText:(NSString*)newText
{
	_detailLabel.text = newText;
}

- (NSString*)getDetailText
{
	return _detailLabel.text;
}

- (void)setDistanceText:(NSString*)newText
{
	_distanceLabel.text = newText;
}

- (NSString*)getDistanceText
{
	return _distanceLabel.text;
}

- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        _titleLabel.textColor = [UIColor whiteColor];
        _detailLabel.textColor = [UIColor whiteColor];
        _distanceLabel.textColor = [UIColor whiteColor];
    }
	else
	{
		_titleLabel.textColor = [UIColor blueColor];
		_detailLabel.textColor = [UIColor blackColor];
		_distanceLabel.textColor = [UIColor redColor];
    }
	
	[pool release];
}

@end
