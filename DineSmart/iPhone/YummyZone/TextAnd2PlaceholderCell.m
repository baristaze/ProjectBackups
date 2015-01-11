#import "TextAnd2PlaceholderCell.h"
#import "YummyViewConstants.h"
#import "CellHeightHelper.h"

@implementation TextAnd2PlaceholderCell

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
        titleLabel.text = @"";
        
        placeholderLabel = [[UILabel alloc] initWithFrame:CGRectZero];
		placeholderLabel.font = [[YummyViewConstants singleton] getTextAndPlaceholderCellFont];
        placeholderLabel.textAlignment = UITextAlignmentLeft;
        placeholderLabel.backgroundColor = [UIColor clearColor];
        placeholderLabel.numberOfLines = 0;
		placeholderLabel.textColor = [UIColor orangeColor];
        placeholderLabel.text = @"";
        
        [self.contentView addSubview:titleLabel];
        [self.contentView addSubview:placeholderLabel];
		
		[pool release];
    }
    return self;
}

- (void)dealloc 
{
	[myTextColor release];
    [titleLabel release];
    [placeholderLabel release];
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
#define SINGLELINE_HEIGHT   18.0

+ (CGFloat)getTotalMissingTextWidth
{
	return 2 * HORIZONTAL_OFFSET;
}

+ (CGFloat)getTotalMissingTextHeight
{
	return 3 * VERTICAL_OFFSET;
}

- (void)layoutSubviews 
{
    [super layoutSubviews];
    
    if(placeholderMode)
    {
        titleLabel.frame = CGRectMake(HORIZONTAL_OFFSET,
                                      VERTICAL_OFFSET, 
                                      self.contentView.bounds.size.width - (2 * HORIZONTAL_OFFSET), 
                                      self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET));
        
        placeholderLabel.frame = CGRectMake(0, 0, 0, 0);  
    }
    else
    {
        titleLabel.frame = CGRectMake(HORIZONTAL_OFFSET,
                                      VERTICAL_OFFSET, 
                                      self.contentView.bounds.size.width - (2 * HORIZONTAL_OFFSET), 
                                      self.contentView.bounds.size.height - (2 * VERTICAL_OFFSET) - SINGLELINE_HEIGHT);
        
        placeholderLabel.frame = CGRectMake(HORIZONTAL_OFFSET,
                                            self.contentView.bounds.size.height - SINGLELINE_HEIGHT - VERTICAL_OFFSET, 
                                            self.contentView.bounds.size.width - (2 * HORIZONTAL_OFFSET), 
                                            SINGLELINE_HEIGHT);        
        
    }
}

- (void)setTextContent:(NSString*)titleText placeholderText:(NSString*)placeholderText;
{
	titleLabel.text = titleText;
    placeholderLabel.text = placeholderText;
}

- (NSString*)getTextContent
{
	return placeholderLabel.text;
}

- (void)setTextContentAlignment:(UITextAlignment)alignment;
{
	titleLabel.textAlignment = alignment;
    placeholderLabel.textAlignment = alignment;
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
    placeholderLabel.font = font;
}

- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated 
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    [super setHighlighted:highlighted animated:animated];
    if (highlighted)
	{
        titleLabel.textColor = [UIColor whiteColor];
        placeholderLabel.textColor = [UIColor whiteColor];
    }
	else
	{
        placeholderLabel.textColor = [UIColor orangeColor];
        
		if (placeholderMode) 
			titleLabel.textColor = [UIColor darkGrayColor];
		else 
			titleLabel.textColor = myTextColor;
    }
	
	[pool release];
}

@end

