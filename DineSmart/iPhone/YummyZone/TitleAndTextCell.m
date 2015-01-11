#import "TitleAndTextCell.h"
#import "YummyViewConstants.h"

@implementation TitleAndTextCell

#define DEFAULT_TITLE_WIDTH		180.0

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    if (self) 
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
		titleWidth = DEFAULT_TITLE_WIDTH;
		titleTextColor = [[UIColor colorWithRed:56.0/255.0 green:84.0/255.0 blue:135.0/255.0 alpha:1] retain];
        
        titleLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        titleLabel.font = [[YummyViewConstants singleton] getTitleAndTextCellTitleFont];
        titleLabel.textAlignment = UITextAlignmentRight;
        //titleLabel.textAlignment = UITextAlignmentLeft;
		titleLabel.lineBreakMode = UILineBreakModeWordWrap;
        titleLabel.backgroundColor = [UIColor clearColor];
		
        detailLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		detailLabel.font = [[YummyViewConstants singleton] getTitleAndTextCellTextFont];
        detailLabel.textAlignment = UITextAlignmentLeft;
        detailLabel.backgroundColor = [UIColor clearColor];
        detailLabel.numberOfLines = 0;
        
        [self.contentView addSubview:titleLabel];
        [self.contentView addSubview:detailLabel];
        
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
	[titleTextColor release];
    [titleLabel release];
    [detailLabel release];
    [super dealloc];
}

- (void)setTitleWidth:(CGFloat)newValue
{
	titleWidth = newValue;
	[self setNeedsLayout];
}

- (void)setTitleFontSize:(CGFloat)newValue
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	titleLabel.font = [UIFont boldSystemFontOfSize:newValue];
	[pool release];
}

- (void)setTitleTextColor:(UIColor*)newColor
{
	[newColor retain];
	[titleTextColor release];
	titleTextColor = newColor;
	
	titleLabel.textColor = titleTextColor;
}

- (void)setTitleTextAlignment:(UITextAlignment)newAlignment
{
	titleLabel.textAlignment = newAlignment;
}

- (void)setDetailFontSize:(int)newValue
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	detailLabel.font = [UIFont boldSystemFontOfSize:newValue];
	[pool release];
}

- (void)setDetailTextAlignment:(UITextAlignment)newAlignment
{
	detailLabel.textAlignment = newAlignment;
}

#define LEFT_OFFSET					15.0
#define RIGHT_OFFSET				15.0
#define VERTICAL_OFFSET				10.0
#define TITLE_DETAIL_SEPARATION		15.0

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
    
	titleLabel.frame = CGRectMake(LEFT_OFFSET,
								  VERTICAL_OFFSET, 
								  titleWidth, 
								  self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
    
	detailLabel.frame = CGRectMake(LEFT_OFFSET + titleWidth + TITLE_DETAIL_SEPARATION,
								   VERTICAL_OFFSET, 
								   self.contentView.bounds.size.width - titleWidth - TITLE_DETAIL_SEPARATION - LEFT_OFFSET - RIGHT_OFFSET,
								   self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
}

- (void)setTitleText:(NSString*)newTitle
{
	titleLabel.text = newTitle;
}

- (NSString*)getTitleText
{
	return titleLabel.text;
}

- (void)setDetailText:(NSString*)newText
{
	detailLabel.text = newText;
}

- (NSString*)getDetailText
{
	return detailLabel.text;
}

- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        titleLabel.textColor = [UIColor whiteColor];
        detailLabel.textColor = [UIColor whiteColor];
    }
	else
	{
        titleLabel.textColor = titleTextColor;
		detailLabel.textColor = [UIColor blackColor];
    }
	
	[pool release];
}

@end
