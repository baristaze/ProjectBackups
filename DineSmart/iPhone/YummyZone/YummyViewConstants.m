#import "YummyViewConstants.h"

@implementation YummyViewConstants

static YummyViewConstants *_yummyViewConstantsSingleton;

// Initialize the singleton instance if needed and return
+ (YummyViewConstants*)singleton
{
	// @synchronized(self)
	{
		if (!_yummyViewConstantsSingleton)
			_yummyViewConstantsSingleton = [[YummyViewConstants alloc] init];
		
		return _yummyViewConstantsSingleton;
	}
}


+ (id)alloc
{
	//	@synchronized(self)
	{
		NSAssert(_yummyViewConstantsSingleton == nil, @"Attempted to allocate a second instance of a singleton.");
		_yummyViewConstantsSingleton = [super alloc];
		return _yummyViewConstantsSingleton;
	}
}


+ (id)copy
{
	//	@synchronized(self)
	{
		NSAssert(_yummyViewConstantsSingleton == nil, @"Attempted to copy the singleton.");
		return _yummyViewConstantsSingleton;
	}
}


+ (void)initialize
{
    static BOOL initialized = NO;
    if (!initialized)
	{
        initialized = YES;
    }
}


- (id)init
{
    self = [super init];
	if (self)
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
		textAndPlaceholderCellFont = [[UIFont systemFontOfSize:16] retain];
		titleAndTextCellTitleFont = [[UIFont boldSystemFontOfSize:14] retain];
		titleAndTextCellTextFont = [[UIFont boldSystemFontOfSize:14] retain];
        titleAndStarCellFont = [[UIFont systemFontOfSize:14] retain];
		titleAndYesNoCellFont = [[UIFont systemFontOfSize:14] retain];
        /*
		textAndPlaceholderCellFont = [[UIFont systemFontOfSize:14] retain];
		titleAndTextCellTitleFont = [[UIFont systemFontOfSize:14] retain];
		titleAndTextCellTextFont = [[UIFont systemFontOfSize:14] retain];
        titleAndStarCellFont = [[UIFont systemFontOfSize:14] retain];
		titleAndYesNoCellFont = [[UIFont systemFontOfSize:14] retain];*/
		
        // BUGBUG: PUT THE IMAGES HERE
        multiEditUnSelectedCheckImage = [[UIImage imageNamed:@"UnSelectedCell.png"] retain];
        multiEditSelectedCheckImage = [[UIImage imageNamed:@"SelectedCell.png"] retain];
        
        maroonColor = [[UIColor colorWithRed:128.0/255.0 green:0.0 blue:0.0 alpha:1.0] retain];
        
		[pool release];
	}
	return self;
}


- (void)dealloc
{
    [multiEditUnSelectedCheckImage release];
    [multiEditSelectedCheckImage release];
    [textAndPlaceholderCellFont release];
	[titleAndTextCellTitleFont release];
	[titleAndTextCellTextFont release];
	[titleAndStarCellFont release];
	[titleAndYesNoCellFont release];
    [maroonColor release];
	[super dealloc];
}


- (UIFont*)getTextAndPlaceholderCellFont
{
	return textAndPlaceholderCellFont;
}

- (UIFont*)getTitleAndTextCellTitleFont
{
	return titleAndTextCellTitleFont;
}

- (UIFont*)getTitleAndTextCellTextFont
{
	return titleAndTextCellTextFont;
}

- (UIFont*)getTitleAndStarCellFont
{
	return titleAndStarCellFont;
}

- (UIFont*)getTitleAndYesNoCellFont
{
    return titleAndYesNoCellFont;    
}

- (UIImage*)getMultiEditUnSelectedCheckImage
{
    return multiEditUnSelectedCheckImage;
}

- (UIImage*)getMultiEditSelectedCheckImage
{
    return multiEditSelectedCheckImage;    
}

- (UIColor*)getMaroonColor
{
    return maroonColor;    
}

@end
