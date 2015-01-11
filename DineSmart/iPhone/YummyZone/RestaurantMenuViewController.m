//
//  RestaurantMenuViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "RestaurantMenuViewController.h"
#import "RestaurantMenuCell.h"
#import "KeyConstants.h"
#import "MenuItemViewController.h"
#import "ImageCache.h"
#import "YummyZoneAppDelegate.h"
#import "YummyZoneUrls.h"
#import "DictionaryCache.h"
#import "ViewControllerHelper.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneUtils.h"

@interface RestaurantMenuViewController(private)

- (void)startPopulatingItems;
- (NSDictionary*)findItemWithId:(NSString*)itemId;
- (void)reloadTable;

@end


@implementation RestaurantMenuViewController

@synthesize delegate;

- (id)initWithRestaurantId:(NSString*)restaurantId restaurantName:(NSString*)restaurantName isModalPlateSelectorDialog:(BOOL)isModalPlateSelectorDialog
{
    self = [super init];
	if (self)
	{
		_restaurantId = [[NSString alloc] initWithString:restaurantId];
		_restaurantName = [[NSString alloc] initWithString:restaurantName];
		
		_categorizedItems = [[NSMutableArray alloc] init];
		
		_populatingItems = FALSE;
		_lastPopulationFailed = FALSE;
        _lastPopulationError = nil;
        
        _thumbnailCache = [[(YummyZoneAppDelegate*)[[UIApplication sharedApplication] delegate] getThumbnailCache] retain];
        _downloader = nil;

        _isModalPlateSelectorDialog = isModalPlateSelectorDialog;
        
        _selectedItemKeys = [[NSMutableDictionary alloc] init];
	}
	return self;
}


- (void)dealloc
{
    _tableView.delegate = nil;
    _tableView.dataSource = nil;
    // Make sure to remove self from active delegates
    [_thumbnailCache removeDelegate:self];
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }
    
    [_lastPopulationError release];
    [_selectedItemKeys release];
    [_thumbnailCache release];
	[_restaurantId release];
	[_restaurantName release];
	[_categorizedItems release];
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

    if (_isModalPlateSelectorDialog)
    {
        UIButton* doneButton = [YummyZoneUtils createNavBarButtonWithText:@"Done" 
                                                                    width:50.0
                                                                   target:self 
                                                                   action:@selector(doneAction:)];
        
        UIButton* cancelButton = [YummyZoneUtils createNavBarButtonWithText:@"Cancel" 
                                                                     width:50.0
                                                                    target:self 
                                                                    action:@selector(cancelAction:)];
        
        
        UIBarButtonItem* doneItem = [[UIBarButtonItem alloc] initWithCustomView:doneButton];  
        UIBarButtonItem* cancelItem = [[UIBarButtonItem alloc] initWithCustomView:cancelButton];
        
        self.navigationItem.leftBarButtonItem = cancelItem;
        self.navigationItem.rightBarButtonItem = doneItem;
                
        [doneItem release];
        [cancelItem release];
    }

	[self reloadTable];
	[super viewWillAppear:animated];
}

- (void)cancelAction:(id)sender
{
	[self.navigationController popViewControllerAnimated:YES];
}

- (NSDictionary*)findItemWithId:(NSString*)itemId
{
    for (NSDictionary *category in _categorizedItems)
    {
        for (NSDictionary * item in [category objectForKey:kKeyMenuItems])
        {
            if ([itemId isEqualToString:[item objectForKey:kKeyId]])
            {
                return item;
            }
        }
    }
    
    return nil;
}

- (void)doneAction:(id)sender
{
    if (delegate != nil)
    {
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        
        NSMutableArray *selectedItems = [[[NSMutableArray alloc] init] autorelease];
        
        for (NSString *key in _selectedItemKeys)
        {
            NSDictionary *item = [self findItemWithId:key];
            if (item != nil)
            {
                NSMutableDictionary *itemCopy = [[NSMutableDictionary alloc] initWithDictionary:item copyItems:YES];
                [selectedItems addObject:itemCopy];
                [itemCopy release];
            }
            else
            {
                NSLog(@"ERROR: ERROR: ERROR:Cannot find item %@ in any of the lists.", key);
            }
        }
        
        [delegate menuItemsSelected:selectedItems];
        
        [pool release];
    }
	[self.navigationController popViewControllerAnimated:YES];
}


- (void)viewDidLoad 
{
	self.title = @"Menu";
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
	_tableView.rowHeight = 90;
	
	self.view = _tableView;
    
    // Now that the controllers are created, we can start populating the items.
    [self startPopulatingItems];
}


- (void)setSelectedItemKeys:(NSArray*)selectedItems
{
    [_selectedItemKeys removeAllObjects];
    
    for (NSDictionary * item in selectedItems)
    {
        [_selectedItemKeys setObject:[NSNull null] forKey:[item objectForKey:kKeyId]];
    }
    
    if (!_populatingItems && !_lastPopulationFailed)
    {
        [self reloadTable];
    }
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
	if (_populatingItems || _lastPopulationFailed || [_categorizedItems count] == 0)
		return 1;
	else
		return [_categorizedItems count];
}


- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section 
{
	if (_populatingItems || _lastPopulationFailed || [_categorizedItems count] == 0)
	{
		return nil;
	}
	else
	{
		NSDictionary *category = [_categorizedItems objectAtIndex:section];
		return [category objectForKey:kKeyName];
	}
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
	if (_populatingItems || _lastPopulationFailed || [_categorizedItems count] == 0)
	{
		return 1;
	}
	else
	{
		NSDictionary *category = [_categorizedItems objectAtIndex:section];
		NSArray *categoryItems = [category objectForKey:kKeyMenuItems];
		return [categoryItems count];
	}
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	if (_populatingItems)
	{
        return [ViewControllerHelper getSpinnerCell:tableView title:@"Loading"];
	}
	else if (_lastPopulationFailed || [_categorizedItems count] == 0)
	{
		if (_lastPopulationFailed)
        {
            if (_lastPopulationError != nil)
            {
                return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"Failed to load. Click to retry." description:_lastPopulationError];
            }
            else
            {
                return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"Failed to load. Click to retry." description:@""];
            }
        }
		else
        {
            return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"Menu is not available." description:@"Click to refresh..."];
        }
	}
	else
	{
		static NSString *kRestaurantMenuCell = @"kRestaurantMenuCell";
		RestaurantMenuCell *cell = (RestaurantMenuCell *)[tableView dequeueReusableCellWithIdentifier:kRestaurantMenuCell];
		if (cell == nil)
		{
			cell = [[[RestaurantMenuCell alloc] initWithReuseIdentifier:kRestaurantMenuCell] autorelease];
            if (_isModalPlateSelectorDialog)
            {
                cell.accessoryType = UITableViewCellAccessoryNone;
                [cell setPriceHidden:YES];
            }
            else
            {
                cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
            }
		}
		
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
		NSDictionary *category = [_categorizedItems objectAtIndex:indexPath.section];
		NSArray *categoryItems = [category objectForKey:kKeyMenuItems];
		NSDictionary *menuItem = [categoryItems objectAtIndex:indexPath.row];
		[cell setTitle:[menuItem objectForKey:kKeyName] description:[menuItem objectForKey:kKeyShortDesc] price:[menuItem objectForKey:kKeyPrice] myRating:[menuItem objectForKey:kKeyMyOwnRating] isTopPick:[menuItem objectForKey:kKeyIsTopPick]];
		
        if (_isModalPlateSelectorDialog)
        {
            if ([_selectedItemKeys objectForKey:[menuItem objectForKey:kKeyId]] != nil)
            {
                cell.accessoryType = UITableViewCellAccessoryCheckmark;
            }
            else
            {
                cell.accessoryType = UITableViewCellAccessoryNone;
            }
        }
        
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
	if (_populatingItems || _lastPopulationFailed || [_categorizedItems count] == 0)
	{
		return;
	}
	
	RestaurantMenuCell *cell = (RestaurantMenuCell*)[_tableView cellForRowAtIndexPath:(NSIndexPath*)key];
	[cell setItemImage:image];
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    if (_populatingItems || _lastPopulationFailed || [_categorizedItems count] == 0)
	{
		return;
	}
	
	RestaurantMenuCell *cell = (RestaurantMenuCell*)[_tableView cellForRowAtIndexPath:(NSIndexPath*)key];
    [cell displayPlaceholderImage];
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
	
	if (_populatingItems)
	{
		return;
	}
	// If last population failed, selecting the cell will initate a retry
	else if (_lastPopulationFailed || [_categorizedItems count] == 0)
	{
		[self startPopulatingItems];
	}
	else
	{
        if ([_categorizedItems count] > indexPath.section)
        {
            NSDictionary *category = [_categorizedItems objectAtIndex:indexPath.section];
            NSArray *categoryItems = [category objectForKey:kKeyMenuItems];
            
            if ([categoryItems count] > indexPath.row)
            {
                NSDictionary *menuItem = [categoryItems objectAtIndex:indexPath.row];
                if (_isModalPlateSelectorDialog)
                {
                    NSString *key = [menuItem objectForKey:kKeyId];
                    if ([_selectedItemKeys objectForKey:key] != nil)
                    {
                        [_tableView cellForRowAtIndexPath:indexPath].accessoryType = UITableViewCellAccessoryNone;
                        [_selectedItemKeys removeObjectForKey:key];
                    }
                    else
                    {
                        [_tableView cellForRowAtIndexPath:indexPath].accessoryType = UITableViewCellAccessoryCheckmark;
                        [_selectedItemKeys setObject:[NSNull null] forKey:key];
                    }
                }
                else
                {
                    MenuItemViewController *viewController = [[MenuItemViewController alloc] initWithItemDetails:menuItem];
                    [[self navigationController] pushViewController:viewController animated:YES];
                    [viewController release];
                }
            }
            else
            {
                NSLog(@"Error: indexPath.row = %d, object count = %d", indexPath.row, [categoryItems count]);
            }
        }
        else
        {
            NSLog(@"Error: indexPath.section = %d, object count = %d", indexPath.section, [_categorizedItems count]);
        }
	}
}

- (void)startPopulatingItems
{
	if (_populatingItems)
		return;
	
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	_populatingItems = TRUE;
	_lastPopulationFailed = FALSE;
	
	[_categorizedItems removeAllObjects];
    [self reloadTable];
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForRestaurantMenu:_restaurantId] delegate:self key:nil httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
    [_downloader startDownload];

    [pool release];
}


- (void)dictionaryDidLoad:(NSDictionary*)dict key:(NSObject*)key
{
    NSLog(@"+dictionaryDidLoad");
	
    if (![YummyZoneHelper webRequestSucceeded:dict])
    {
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        NSString *error = [YummyZoneHelper getOperationErrorMessage:dict];
        if (error != nil)
        {
            [self dictionaryFailedToLoad:error key:key];
        }
        else
        {
            [self dictionaryFailedToLoad:@"Request failed." key:key];
        }
        [pool release];
        return;
    }
    
	_populatingItems = FALSE;
	_lastPopulationFailed = FALSE;
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    NSArray *items = [dict objectForKey:kKeyMenuCategories];
    if (items != nil)
    {
        [_categorizedItems addObjectsFromArray:items];
        [self reloadTable];
    }
    
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }
}


- (void)dictionaryFailedToLoad:(NSString*)error key:(NSObject*)key
{
    NSLog(@"+dictionaryFailedToLoad: %@", error);
    
	_populatingItems = FALSE;
	_lastPopulationFailed = TRUE;
	
    [_lastPopulationError release];
    _lastPopulationError = nil;
    if (error != nil)
    {
        _lastPopulationError = [error copy];
    }
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
	[self reloadTable];
}

@end
