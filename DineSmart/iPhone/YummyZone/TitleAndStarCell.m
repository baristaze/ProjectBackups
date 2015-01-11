//
//  GenericFeedbackCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "TitleAndStarCell.h"
#import "YummyViewConstants.h"


@implementation TitleAndStarCell

@synthesize delegate;


#define DEFAULT_TITLE_WIDTH		180.0

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    if (self) 
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
		_titleWidth = DEFAULT_TITLE_WIDTH;
		//_titleTextColor = [[UIColor colorWithRed:56.0/255.0 green:84.0/255.0 blue:135.0/255.0 alpha:1] retain];
        _titleTextColor = [[UIColor blackColor] retain];
        
        _titleLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        _titleLabel.font = [[YummyViewConstants singleton] getTitleAndStarCellFont];
        _titleLabel.textAlignment = UITextAlignmentLeft;
		_titleLabel.lineBreakMode = UILineBreakModeWordWrap;
        _titleLabel.numberOfLines = 0;  // Multiline
        _titleLabel.backgroundColor = [UIColor clearColor];
		_titleLabel.textColor = _titleTextColor;
        
        _ratingControl = [[DLStarRatingControl alloc] initWithFrame:CGRectZero];
        _ratingControl.delegate = self;

        [self.contentView addSubview:_titleLabel];
        [self.contentView addSubview:_ratingControl];

        _key = nil;

		[pool release];
    }
    return self;
}

- (void)dealloc 
{
	[_titleTextColor release];
    [_titleLabel release];
    [_key release];
    [_ratingControl release];
    [super dealloc];
}

- (void)setTitleWidth:(CGFloat)newValue
{
	_titleWidth = newValue;
	[self setNeedsLayout];
}

- (void)setTitleFontSize:(CGFloat)newValue
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	_titleLabel.font = [UIFont boldSystemFontOfSize:newValue];
	[pool release];
}

- (void)setTitleTextColor:(UIColor*)newColor
{
	[newColor retain];
	[_titleTextColor release];
	_titleTextColor = newColor;
	
	_titleLabel.textColor = _titleTextColor;
}

- (void)setTitleTextAlignment:(UITextAlignment)newAlignment
{
	_titleLabel.textAlignment = newAlignment;
}

#define LEFT_OFFSET					10.0
#define RIGHT_OFFSET				10.0
#define VERTICAL_OFFSET				5.0
#define TITLE_DETAIL_SEPARATION		10.0

+ (CGFloat)getTotalMissingTextWidth
{
	return DEFAULT_TITLE_WIDTH + TITLE_DETAIL_SEPARATION + LEFT_OFFSET + RIGHT_OFFSET;
}

+ (CGFloat)getTotalMissingTextHeight
{
	return (2 * VERTICAL_OFFSET);
}

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	_titleLabel.frame = CGRectMake(LEFT_OFFSET,
								  VERTICAL_OFFSET, 
								  _titleWidth, 
								  self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
    
	_ratingControl.frame = CGRectMake(LEFT_OFFSET + _titleWidth + TITLE_DETAIL_SEPARATION,
								   VERTICAL_OFFSET + 10, // BUGBUG: HACK 
								   self.contentView.bounds.size.width - _titleWidth - TITLE_DETAIL_SEPARATION - LEFT_OFFSET - RIGHT_OFFSET,
								   self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
}

- (void)setTitle:(NSString*)text rating:(NSUInteger)rating key:(NSObject*)key
{
    _titleLabel.text = text;
    _ratingControl.rating = rating;
    
    [key retain];
    [_key release];
    _key = key;
}

/*- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        _titleLabel.textColor = [UIColor whiteColor];
    }
	else
	{
        _titleLabel.textColor = _titleTextColor;
    }
	
	[pool release];
}*/

- (void)newRating:(int)rating
{
    [delegate ratingChanged:rating key:_key];
}

@end
