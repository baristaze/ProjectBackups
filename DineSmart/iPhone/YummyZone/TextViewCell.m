#import "TextViewCell.h"

#define kInsertValue	8.0

@implementation TextViewCell

@synthesize textView;

- (id)initWithReuseIdentifier:(NSString *)identifier
{
	self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:identifier];
	if (self)
	{
		// turn off selection use
		self.selectionStyle = UITableViewCellSelectionStyleNone;
	}
	return self;
}

- (void)setTextView:(UITextView *)inView
{
	textView = [inView retain];
	[self.contentView addSubview:inView];
	[self layoutSubviews];
}

- (void)layoutSubviews
{
	[super layoutSubviews];
	
	CGRect contentRect = [self.contentView bounds];
	
	// inset the text view within the cell
	if (contentRect.size.width > (kInsertValue*2))	// but not if the width is too small
	{
		self.textView.frame  = CGRectMake(contentRect.origin.x + kInsertValue,
                                          contentRect.origin.y + kInsertValue,
                                          contentRect.size.width - (kInsertValue*2),
                                          contentRect.size.height - (kInsertValue*2));
	}
}

- (void)dealloc
{
    [textView release];
    [super dealloc];
}

@end
