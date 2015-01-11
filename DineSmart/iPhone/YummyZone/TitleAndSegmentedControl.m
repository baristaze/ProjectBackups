//
//  TitleAndSegmentedControl.m
//  YummyZone
//
//  Created by Baris Taze on 3/4/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//

#import "TitleAndSegmentedControl.h"
#import "YummyViewConstants.h"

@implementation TitleAndSegmentedControl

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
        _titleLabel.font = [[YummyViewConstants singleton] getTitleAndYesNoCellFont];
        _titleLabel.textAlignment = UITextAlignmentLeft;
		_titleLabel.lineBreakMode = UILineBreakModeWordWrap;
        _titleLabel.numberOfLines = 0;  // Multiline
        _titleLabel.backgroundColor = [UIColor clearColor];
		_titleLabel.textColor = _titleTextColor;
        
		NSArray* items = [NSArray arrayWithObjects:@"Yes",@"No",nil];
		
		// Creates an 
        _segmentedControl = [[UISegmentedControl alloc] initWithItems:items];
        [_segmentedControl addTarget:self action:@selector(segmentValueChanged:) forControlEvents:UIControlEventValueChanged];
		_segmentedControl.segmentedControlStyle = UISegmentedControlStylePlain;	

        // no need this in plain mode... only bar-style might need this
        //_segmentedControl.tintColor = [UIColor grayColor];
                         
		[self.contentView addSubview:_titleLabel];
		[self.contentView addSubview:_segmentedControl];
		 
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
    [_segmentedControl release];
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
#define SEGMENT_HEIGHT              40.0

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
    /*
     _segmentedControl.frame = CGRectMake(LEFT_OFFSET + _titleWidth + TITLE_DETAIL_SEPARATION,
     VERTICAL_OFFSET + 5, // BUGBUG: HACK 
     self.contentView.bounds.size.width - _titleWidth - TITLE_DETAIL_SEPARATION - LEFT_OFFSET - RIGHT_OFFSET,
     self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET)-5);*/
          
    _segmentedControl.frame = CGRectMake(LEFT_OFFSET + _titleWidth + TITLE_DETAIL_SEPARATION,
                                         (self.contentView.bounds.size.height - SEGMENT_HEIGHT) / 2.0, 
                                         self.contentView.bounds.size.width - _titleWidth - TITLE_DETAIL_SEPARATION - LEFT_OFFSET - RIGHT_OFFSET,
                                         SEGMENT_HEIGHT);
    
}		 

- (void)setTitle:(NSString*)text value:(int)value key:(NSObject*)key
{
    _titleLabel.text = text;
    
    if(value == 0) // no
    {
        _segmentedControl.selectedSegmentIndex = 1; // second button
    }
    else if(value == 1)
    {
        _segmentedControl.selectedSegmentIndex = 0; // first button
    }
    
    [key retain];
    [_key release];
    _key = key;
}

- (void)segmentValueChanged:(id)sender
{
    int value = -1;
    
    if(_segmentedControl.selectedSegmentIndex == 0) // first button, yes
    {
        value = 1;
    }
    else if(_segmentedControl.selectedSegmentIndex == 1) // second button, no
    {
        value = 0;
    }
    
    [delegate segmentValueChanged:value key:_key];
}

@end
		 
