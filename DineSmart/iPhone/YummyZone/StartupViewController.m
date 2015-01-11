//
//  StartupViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "StartupViewController.h"
#import "LocateMeViewController.h"
#import "MessageListViewController.h"
#import "FavoritesViewController.h"
#import "HistoryViewController.h"
#import "YummyZoneSession.h"
#import "ProfileViewController.h"
#import "YummyZoneUtils.h"

@interface StartupViewController(private)
- (void)locateMeClicked;
- (void)messagesClicked;
- (void)settingsClicked;
- (void)favoritesClicked;
- (void)historyClicked;
- (void)couponsClicked;
- (void)loginUser;
@end


@implementation StartupViewController


- (id)init
{
    if ((self = [super init]))
	{
		self.title = @"Dine Smart 365";
		_lastSelectedButtonIndex = -1;		
    }
	
    return self;
}


- (void)dealloc 
{
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
    NSString *backgroundPath = [[NSBundle mainBundle] pathForResource:@"pageBkg-568h@2x" ofType:@"png"];
    UIImage *backgroundImage = [UIImage imageWithContentsOfFile:backgroundPath];
	UIColor *backgroundColor = [[UIColor alloc] initWithPatternImage:backgroundImage];
	self.view.backgroundColor = backgroundColor; 
	[backgroundColor release];
    
	//
	// Icon list
	//
	_fileList = [[NSArray arrayWithObjects:
				  @"locateMe.png", 
				  @"favorites.png",
				  @"messages.png", 
				  @"coupons.png", 
				  @"history.png", 
				  @"profile.png",
				  nil] retain];
	
	_pressedFileList = [[NSArray arrayWithObjects:
						 @"locateMe_press.png", 
						 @"favorites_press.png",
						 @"messages_press.png", 
						 @"coupons_press.png", 
						 @"history_press.png", 
						 @"profile_press.png",
						 nil] retain];
		
	CGFloat buttonWidth = 127.0;
	CGFloat buttonHeight = 111.0;
	CGFloat topOffset = (self.view.bounds.size.height - self.view.bounds.origin.y - (3*buttonHeight)) / 3.0;
	CGFloat leftOffset = (self.view.bounds.size.width - (2.0 * buttonWidth)) / 2.0;
	
	NSInteger currentIndex = 0;
	for (NSString *filename in _fileList)
	{
		CGFloat offsetX = leftOffset;
		if(currentIndex % 2 == 1)
		{
			offsetX += buttonWidth;
		}
		
		UIImage* pressedImage = [UIImage imageNamed:[_pressedFileList objectAtIndex:currentIndex]];
		
		UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
		button.tag = currentIndex;
		button.frame = CGRectMake(offsetX, topOffset, buttonWidth, buttonHeight);
		[button setImage:[UIImage imageNamed:filename] forState:UIControlStateNormal];
		[button setImage:pressedImage forState:UIControlStateSelected];
		[button setImage:pressedImage forState:UIControlStateHighlighted];
		[button addTarget:self action:@selector(buttonListAction:) forControlEvents:UIControlEventTouchUpInside];
		[self.view addSubview:button];
		
		++currentIndex;
		if (currentIndex % 2 == 0)
		{
			topOffset += buttonHeight;
		}
	}
    
	[pool release];
}


- (void)viewDidLoad 
{
	[super viewDidLoad];
    [self loginUser];
}

- (void)loginUser
{
    NSString *error = [[YummyZoneSession singleton] obtainAuthorizationCookie];
    if (error != nil)
    {
		UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:error
													   delegate:self cancelButtonTitle:@"Retry" otherButtonTitles:nil];
        [alert show];
        [alert release];
    }
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    [self loginUser];
}

- (void)viewWillAppear:(BOOL)animated
{
    [YummyZoneUtils changeBkgImgOfNavBar:self.navigationController imageIndex:0];
	self.navigationController.toolbarHidden = YES;
    [super viewWillAppear:animated];
}


- (void)didReceiveMemoryWarning 
{
    [super didReceiveMemoryWarning];
}


- (void)buttonListAction:(id)sender
{
	UIButton *button = sender;
	if (button.tag >= 0 && button.tag < [_fileList count])
	{
		_lastSelectedButtonIndex = button.tag;
		
		switch (_lastSelectedButtonIndex) 
		{
			case 0:
				[self locateMeClicked];
				break;
			case 1:
				[self favoritesClicked];
				break;
			case 2:
				[self messagesClicked];
				break;
			case 3:
				[self couponsClicked];
				break;
			case 4:
				[self historyClicked];
				break;
			case 5:
				[self settingsClicked];
				break;
			default:
				break;
		}
	}
}


- (void)locateMeClicked
{
	LocateMeViewController *viewController = [[LocateMeViewController alloc] init];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)messagesClicked
{
	MessageListViewController *viewController = [[MessageListViewController alloc] initWithType:NO restaurantId:nil];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)favoritesClicked
{
	FavoritesViewController *viewController = [[FavoritesViewController alloc] initWithRestaurantId:nil];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)couponsClicked
{
	MessageListViewController *viewController = [[MessageListViewController alloc] initWithType:YES restaurantId:nil];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)historyClicked
{
	HistoryViewController *viewController = [[HistoryViewController alloc] initWithRestaurantId:nil];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}


- (void)settingsClicked
{
	ProfileViewController *viewController = [[ProfileViewController alloc] init];
    [[self navigationController] pushViewController:viewController animated:YES];
    [viewController release];
}

@end
