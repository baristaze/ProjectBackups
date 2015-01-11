//
//  FavoritesViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "FavoritesViewController.h"
#import "FavoriteMenuItemCell.h"
#import "FavoriteRestaurantCell.h"
#import "KeyConstants.h"
#import "MenuItemViewController.h"
#import "ImageCache.h"
#import "YummyZoneAppDelegate.h"
#import "YummyZoneUrls.h"
#import "ViewControllerHelper.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneUtils.h"

@interface FavoritesViewController(private)

- (BOOL)isDataEmpty;
- (void)startPopulatingItems;
- (void)reloadTable;

@end


@implementation FavoritesViewController

- (id)initWithRestaurantId:(NSString*)restaurantId
{
    self = [super init];
	if (self)
	{
        if (restaurantId != nil)
        {
            _restaurantId = [[NSString alloc] initWithString:restaurantId];
        }
        else
        {
            _restaurantId = nil;
        }
		
		_favoriteMenuItems = [[NSMutableArray alloc] init];
		_favoriteRestaurants = [[NSMutableArray alloc] init];
		
		_populatingItems = FALSE;
		_lastPopulationFailed = FALSE;
        _lastPopulationError = nil;
        _downloader = nil;
        
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
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_lastPopulationError release];
    [_thumbnailCache release];
	[_restaurantId release];
	[_favoriteMenuItems release];
	[_favoriteRestaurants release];
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
	self.title = @"Favorites";
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
    
    // Now that the controllers are created, we can start populating the items.
    [self startPopulatingItems];
}


- (BOOL)isDataEmpty
{
    if (_populatingItems || _lastPopulationFailed || ([_favoriteMenuItems count] == 0 && [_favoriteRestaurants count] == 0))
        return YES;
    else
        return NO;
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    if ([self isDataEmpty])
		return 1;
	else
        return 2;
}


- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section 
{
    if ([self isDataEmpty])
		return nil;
	else
	{
        if (section == 0)
            return @"Favorite Menu Items";
        else if (section == 1)
            return @"Favorite Restaurants";
        else
            return nil;
	}
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if ([self isDataEmpty])
		return 1;
	else
	{
        if (section == 0)
            return [_favoriteMenuItems count];
        else
            return [_favoriteRestaurants count];
	}
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	if (_populatingItems)
	{
        return [ViewControllerHelper getSpinnerCell:tableView title:@"Loading"];
	}
	else if (_lastPopulationFailed || ([_favoriteMenuItems count] == 0 && [_favoriteRestaurants count] == 0))
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
            return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"Favorites is empty." description:@"Click to refresh..."];
        }
	}
	else
	{
        if (indexPath.section == 0) // favorite menu items
        {
            static NSString *kFavoriteMenuItemCell = @"kFavoriteMenuItemCell";
            FavoriteMenuItemCell *cell = (FavoriteMenuItemCell*)[tableView dequeueReusableCellWithIdentifier:kFavoriteMenuItemCell];
            if (cell == nil)
            {
                cell = [[[FavoriteMenuItemCell alloc] initWithReuseIdentifier:kFavoriteMenuItemCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            NSDictionary *menuItem = [_favoriteMenuItems objectAtIndex:indexPath.row];
            [cell setTitle:[menuItem objectForKey:kKeyMenuItemName] 
                restaurant:[menuItem objectForKey:kKeyRestaurantName] 
                  myRating:[menuItem objectForKey:kKeyMenuItemAverageRate]];

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
        else // if (indexPath.section == 1) // favorite restaurants
        {
            static NSString *kKeyFavoriteRestaurantCell = @"kKeyFavoriteRestaurantCell";
            FavoriteRestaurantCell *cell = (FavoriteRestaurantCell*)[tableView dequeueReusableCellWithIdentifier:kKeyFavoriteRestaurantCell];
            if (cell == nil)
            {
                cell = [[[FavoriteRestaurantCell alloc] initWithReuseIdentifier:kKeyFavoriteRestaurantCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            NSDictionary *menuItem = [_favoriteRestaurants objectAtIndex:indexPath.row];
            [cell setName:[menuItem objectForKey:kKeyRestaurantName] 
                 myRating:[menuItem objectForKey:kKeyRestaurantAverageRate]];
            
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
}


- (void)imageDidLoad:(UIImage*)image key:(NSObject*)key
{
	if ([self isDataEmpty])
	{
		return;
	}
	
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        FavoriteMenuItemCell *cell = (FavoriteMenuItemCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell setItemImage:image];
    }
    else if (indexPath.section == 1)
    {
        FavoriteRestaurantCell *cell = (FavoriteRestaurantCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell setItemImage:image];
    }
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        FavoriteMenuItemCell *cell = (FavoriteMenuItemCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell displayPlaceholderImage];
    }
    else if (indexPath.section == 1)
    {
        FavoriteRestaurantCell *cell = (FavoriteRestaurantCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell displayPlaceholderImage];
    }
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
	
	if (_populatingItems)
	{
		return;
	}
	// If last population failed, selecting the cell will initate a retry
	else if (_lastPopulationFailed || ([_favoriteMenuItems count] == 0 && [_favoriteRestaurants count] == 0))
	{
		[self startPopulatingItems];
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
	
	[_favoriteMenuItems removeAllObjects];
	[_favoriteRestaurants removeAllObjects];
    [self reloadTable];
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }
    
    _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForFavorites:_restaurantId] delegate:self key:nil httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
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
    
    NSArray *menuItems = [dict objectForKey:kKeyFavoriteMenuItems];
    if (menuItems != nil)
    {
        [_favoriteMenuItems addObjectsFromArray:menuItems];
    }

    NSArray *restaurants = [dict objectForKey:kKeyFavoriteRestaurants];
    if (restaurants != nil)
    {
        [_favoriteRestaurants addObjectsFromArray:restaurants];
    }
    
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [self reloadTable];   
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
