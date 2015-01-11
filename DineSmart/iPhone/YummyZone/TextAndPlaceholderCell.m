#import "TextAndPlaceholderCell.h"
#import "YummyViewConstants.h"

@implementation TextAndPlaceholderCell

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    if (self) 
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
        titleLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		titleLabel.font = [[YummyViewConstants singleton] getTextAndPlaceholderCellFont];
        titleLabel.textAlignment = UITextAlignmentLeft;
        titleLabel.backgroundColor = [UIColor clearColor];
        titleLabel.numberOfLines = 0;
		myTextColor = [[UIColor blackColor] retain];
		titleLabel.textColor = myTextColor;
        
        [self.contentView addSubview:titleLabel];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
	[myTextColor release];
    [titleLabel release];
    [super dealloc];
}

- (void)setPlaceholderMode:(BOOL)flag
{
	placeholderMode = flag;
	
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	if (placeholderMode) 
		titleLabel.textColor = [UIColor darkGrayColor];
	else 
		titleLabel.textColor = myTextColor;
	
	[pool release];
}

- (BOOL)getPlaceholderMode
{
    return placeholderMode;
}

#define VERTICAL_OFFSET		6.0
#define HORIZONTAL_OFFSET	15.0

+ (CGFloat)getTotalMissingTextWidth
{
	return 2 * HORIZONTAL_OFFSET;
}

+ (CGFloat)getTotalMissingTextHeight
{
	return 2 * VERTICAL_OFFSET;
}

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
	titleLabel.frame = CGRectMake(HORIZONTAL_OFFSET,
								  VERTICAL_OFFSET, 
								  self.contentView.bounds.size.width - (2 * HORIZONTAL_OFFSET), 
								  self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
}

- (void)setTextContent:(NSString*)newText
{
	titleLabel.text = newText;
}

- (NSString*)getTextContent
{
	return titleLabel.text;
}

- (void)setTextContentAlignment:(UITextAlignment)alignment;
{
	titleLabel.textAlignment = alignment;
}

- (void)setTextContentColor:(UIColor*)color
{
	[color retain];
	[myTextColor release];
	myTextColor = color;
	
    if ([self isSelected] == NO && placeholderMode == NO)
		titleLabel.textColor = myTextColor;
}

- (void)setTextContentFont:(UIFont*)font
{
	titleLabel.font = font;
}

- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated 
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        titleLabel.textColor = [UIColor whiteColor];
    }
	else
	{
		if (placeholderMode) 
			titleLabel.textColor = [UIColor darkGrayColor];
		else 
			titleLabel.textColor = myTextColor;
    }
	
	[pool release];
}

@end
