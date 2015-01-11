//
//  HistoryDetailsViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "HistoryDetailsViewController.h"
#import "HistoryMenuItemCell.h"
#import "KeyConstants.h"
#import "MenuItemViewController.h"
#import "ImageCache.h"
#import "YummyZoneAppDelegate.h"
#import "YummyZoneUrls.h"
#import "RestaurantCell.h"
#import "ViewControllerHelper.h"
#import "YummyZoneUtils.h"

@implementation HistoryDetailsViewController

- (id)initWithMenuItems:(NSArray*)menuItems
{
    self = [super init];
	if (self)
	{
        _menuItems = [[NSArray alloc] initWithArray:menuItems];
        _thumbnailCache = [[(YummyZoneAppDelegate*)[[UIApplication sharedApplication] delegate] getThumbnailCache] retain];
	}
	return self;
}


- (void)dealloc
{
    _tableView.delegate = nil;
    _tableView.dataSource = nil;
    // Make sure to remove self from active delegates
    [_thumbnailCache removeDelegate:self];
    
    [_thumbnailCache release];
	[_menuItems release];
	[_tableView release];
    [super dealloc];
}


- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
}


- (void)reloadTable
{
    // Remove ourselves from the notifications we subscribed for images.
    // We will resubscribe once we reload the menu
    [_thumbnailCache removeDelegate:self];
    [_tableView reloadData];
}

- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
	[self reloadTable];
	[super viewWillAppear:animated];
}


- (void)viewDidLoad 
{
	self.title = @"Plates that I tried";
    [YummyZoneUtils loadBackButton:[self navigationItem]];
	[super viewDidLoad];
}


- (void)loadView
{
	// create and configure the table view
	_tableView = [[UITableView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame] style:UITableViewStylePlain];
	_tableView.delegate = self;
	_tableView.dataSource = self;
	_tableView.autoresizesSubviews = YES;
	_tableView.separatorStyle = UITableViewCellSeparatorStyleSingleLine;
	_tableView.allowsSelectionDuringEditing = YES;
	_tableView.rowHeight = 74;
	
	self.view = _tableView;
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if ([_menuItems count] == 0)
		return 1;
	else
        return [_menuItems count];
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	if ([_menuItems count] == 0)
	{
        return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"No rated items." description:@""];
	}
	else
	{
        static NSString *kHistoryMenuItemCell = @"kHistoryMenuItemCell";
        HistoryMenuItemCell *cell = (HistoryMenuItemCell*)[tableView dequeueReusableCellWithIdentifier:kHistoryMenuItemCell];
        if (cell == nil)
        {
            cell = [[[HistoryMenuItemCell alloc] initWithReuseIdentifier:kHistoryMenuItemCell] autorelease];
            cell.accessoryType = UITableViewCellAccessoryNone;
        }
        
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        
        NSDictionary *menuItem = [_menuItems objectAtIndex:indexPath.row];
        [cell setTitle:[menuItem objectForKey:kKeyMenuItemName] myRating:[menuItem objectForKey:kKeyMenuItemRate]];
        
        NSString* urlText = [menuItem objectForKey:kKeyImageUrl];
        if(urlText == nil || urlText.length == 0)
        {
            [cell displayPlaceholderImage];
        }
        else 
        {   
            NSURL *url = [NSURL URLWithString:urlText];
            if (url != nil)
            {
                if (![_thumbnailCache isImageCachedForUrl:url])
                {
                    // If the image is not cached yet, put a placeholder
                    [cell displayBusyImage];
                    [_thumbnailCache loadImageAtUrl:url delegate:self key:indexPath];
                }
                else
                {
                    [cell setItemImage:[_thumbnailCache getImageAtUrl:url]];
                }
            }
            else
            {
                [cell displayPlaceholderImage];
            }
        }
        
        [pool release];
        return cell;    
    }
}


- (void)imageDidLoad:(UIImage*)image key:(NSObject*)key
{
	if ([_menuItems count] == 0)
	{
		return;
	}
	
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        HistoryMenuItemCell *cell = (HistoryMenuItemCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell setItemImage:image];
    }
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    if ([_menuItems count] == 0)
	{
		return;
	}
    
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        HistoryMenuItemCell *cell = (HistoryMenuItemCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell displayPlaceholderImage];
    }
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
