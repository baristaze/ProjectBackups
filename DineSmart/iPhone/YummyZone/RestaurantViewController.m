//
//  StartupViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "RestaurantViewController.h"
#import "RestaurantMenuViewController.h"
#import "FeedbackViewController.h"
#import "MessageListViewController.h"
#import "YummyZoneUtils.h"

@interface RestaurantViewController(private)
- (void)menuClicked;
- (void)couponsClicked;
- (void)feedbackClicked;
@end


@implementation RestaurantViewController


- (id)initWithRestaurantId:(NSString*)restaurandId 
            restaurantName:(NSString*)restaurantName 
         restaurantAddress:(NSString*)restaurantAddress
{
    if ((self = [super init]))
	{
		_restaurantId = [[NSString alloc] initWithString:restaurandId];
		_restaurantName = [[NSString alloc] initWithString:restaurantName];
        _restaurantAddress = [[NSString alloc] initWithString:restaurantAddress];
		
		self.title = _restaurantName;
		_lastSelectedButtonIndex = -1;
        
        _stateStore = [[NSMutableDictionary alloc] init];
    }
	
    return self;
}


- (void)dealloc 
{
    [_stateStore release];
	[_restaurantName release];
    [_restaurantAddress release];
	[_restaurantId release];
	[_fileList release];
    [_pressedFileList release];
    [super dealloc];
}

- (void)loadView 
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	CGRect screenRect = [[UIScreen mainScreen] applicationFrame];
	
	UIView *contentView = [[UIView alloc] initWithFrame:screenRect];
	contentView.autoresizesSubviews = YES;
	contentView.autoresizingMask = (UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight);

	contentView.backgroundColor = [UIColor viewFlipsideBackgroundColor];
	self.view = contentView;
	[contentView release];
    
    // background image
	NSString *backgroundPath = [[NSBundle mainBundle] pathForResource:@"dashPageBkg-568h@2x" ofType:@"png"];
	UIImage *backgroundImage = [UIImage imageWithContentsOfFile:backgroundPath];
	UIColor *backgroundColor = [[UIColor alloc] initWithPatternImage:backgroundImage];
	self.view.backgroundColor = backgroundColor; 
	[backgroundColor release];
	
    // tint image
    UIImage* tintImage = [UIImage imageNamed:@"restDashTint.png"];
    UIImageView* tintImgView = [[[UIImageView alloc] initWithImage:tintImage] autorelease];
    CGFloat tintTop = 40.0;
    CGFloat tintLeft = (self.view.bounds.size.width - tintImage.size.width) / 2.0;
	tintImgView.frame = CGRectMake(tintLeft, tintTop, tintImage.size.width, tintImage.size.height);
    [self.view addSubview:tintImgView];
    
    // location icon: rain drop
    UIImage* locImage = [UIImage imageNamed:@"rainDrop.png"];
    UIImageView* locImgView = [[[UIImageView alloc] initWithImage:locImage] autorelease];
    CGFloat imgTop = 65.0;
    CGFloat imgLeft = (self.view.bounds.size.width - locImage.size.width) / 2.0;
	locImgView.frame = CGRectMake(imgLeft, imgTop, locImage.size.width, locImage.size.height);
    [self.view addSubview:locImgView];
	
    // restaurant info
    UILabel* _restNameLabel = [[UILabel alloc] initWithFrame:CGRectZero];
    _restNameLabel.font = [UIFont systemFontOfSize:14];
    _restNameLabel.textAlignment = UITextAlignmentCenter;
    _restNameLabel.backgroundColor = [UIColor clearColor];
    _restNameLabel.textColor = [[UIColor alloc] initWithRed:0.4980 green:0.8118 blue:0.9059 alpha:1.0];
    _restNameLabel.text = _restaurantAddress;
    
    CGFloat labelTop = imgTop + locImage.size.height + 15.0;
    CGFloat labelWidth = 248.0;// self.view.bounds.size.width;
    CGFloat labelHeight = 15.0;
    CGFloat labelLeft = 36.0; // 0.0
    _restNameLabel.frame = CGRectMake(labelLeft,labelTop,labelWidth,labelHeight);
    [self.view addSubview:_restNameLabel];
    
	//
	// Icon list
	//
    
	_fileList = [[NSArray arrayWithObjects:
				  @"menu.png", 
				  @"feedback.png",
				  @"couponsDash.png", 
				  nil] retain];
    
    _pressedFileList = [[NSArray arrayWithObjects:
				  @"menu_press.png", 
				  @"feedback_press.png",
				  @"couponsDash_press.png", 
				  nil] retain];
	
    CGFloat buttonWidth = 229.0;
	CGFloat buttonHeight = 43.0;
	CGFloat topOffset = 188.0;
    CGFloat verticalMargin = 19.0;
	CGFloat leftOffset = (self.view.bounds.size.width - buttonWidth) / 2.0;
	
	NSInteger currentIndex = 0;
    for (NSString *filename in _fileList)
	{
        UIImage* pressedImage = [UIImage imageNamed:[_pressedFileList objectAtIndex:currentIndex]];
        UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
        button.tag = currentIndex;
        button.frame = CGRectMake(leftOffset, topOffset, buttonWidth, buttonHeight);
        [button setImage:[UIImage imageNamed:filename] forState:UIControlStateNormal];
        [button setImage:pressedImage forState:UIControlStateSelected];
        [button setImage:pressedImage forState:UIControlStateHighlighted];
        [button addTarget:self action:@selector(buttonListAction:) forControlEvents:UIControlEventTouchUpInside];
        [self.view addSubview:button];
    
        topOffset += buttonHeight + verticalMargin;
        currentIndex++;
    }
    
	[pool release];
}


- (void)viewDidLoad 
{
    [super viewDidLoad];
	[YummyZoneUtils loadBackButton:[self navigationItem]];
}

- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
    [YummyZoneUtils changeBkgImgOfNavBar:self.navigationController imageIndex:1];
    [super viewWillAppear:animated];
}


- (void)didReceiveMemoryWarning 
{
    [super didReceiveMemoryWarning];
}


- (void)cancelAction:(id)sender
{
	[self.navigationController popToRootViewControllerAnimated:YES];
}


//
// Sound Effects
//

- (void)buttonListAction:(id)sender
{
	UIButton *button = sender;
	if (button.tag >= 0 && button.tag < [_fileList count])
	{
		_lastSelectedButtonIndex = button.tag;
		
		switch (_lastSelectedButtonIndex) 
		{
			case 0:
				[self menuClicked];
				break;
			case 1:
				[self feedbackClicked];
				break;
			case 2:
				[self couponsClicked];
				break;
			default:
				break;
		}
	}
}

- (void)menuClicked
{
	//self.navigationItem.title = @"Back";
	RestaurantMenuViewController *viewController = [[RestaurantMenuViewController alloc] 
        initWithRestaurantId:_restaurantId restaurantName:_restaurantName isModalPlateSelectorDialog:NO];
	[[self navigationController] pushViewController:viewController animated:YES];
	[viewController release];
}

- (void)couponsClicked
{
	//self.navigationItem.title = @"Back";
	MessageListViewController *viewController = [[MessageListViewController alloc] initWithType:YES restaurantId:_restaurantId];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)feedbackClicked
{
	//self.navigationItem.title = @"Back";
	FeedbackViewController *viewController = [[FeedbackViewController alloc] 
                                               initWithRestaurantId:_restaurantId restaurantName:_restaurantName stateStore:_stateStore];
	[[self navigationController] pushViewController:viewController animated:YES];
	[viewController release];
}



@end
