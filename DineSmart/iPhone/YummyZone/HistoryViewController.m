//
//  FavoritesViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "HistoryViewController.h"
#import "KeyConstants.h"
#import "MenuItemViewController.h"
#import "ImageCache.h"
#import "YummyZoneAppDelegate.h"
#import "YummyZoneUrls.h"
#import "RestaurantCell.h"
#import "ViewControllerHelper.h"
#import "HistoryDetailsViewController.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneUtils.h"

@interface HistoryViewController(private)

- (void)startPopulatingItems;
- (void)reloadTable;

@end


@implementation HistoryViewController

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
		
		_history = [[NSMutableArray alloc] init];
		
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
	[_history release];
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
	self.title = @"History";
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


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (_populatingItems || _lastPopulationFailed || [_history count] == 0)
		return 1;
	else
        return [_history count];
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	if (_populatingItems)
	{
        return [ViewControllerHelper getSpinnerCell:tableView title:@"Loading"];
	}
	else if (_lastPopulationFailed || [_history count] == 0)
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
            return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"History is empty." description:@"Click to refresh..."];
        }
	}
	else
	{
        static NSString *kKeyRestaurantCell = @"kKeyRestaurantCell";
        RestaurantCell *cell = (RestaurantCell*)[tableView dequeueReusableCellWithIdentifier:kKeyRestaurantCell];
        if (cell == nil)
        {
            cell = [[[RestaurantCell alloc] initWithReuseIdentifier:kKeyRestaurantCell] autorelease];
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        
        NSDictionary *historyItem = [_history objectAtIndex:indexPath.row];
        NSString *dateString = @"";
        NSString *timeString = @"";
        NSDate *date = [historyItem objectForKey:kKeyCheckinTimeUTC];
        if (date != nil)
        {
            NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
            [dateFormatter setDateFormat:@"yyyy-MM-dd"];
            dateString = [dateFormatter stringFromDate:date];
            [dateFormatter setDateFormat:@"HH:mm"];
            timeString = [dateFormatter stringFromDate:date];
            [dateFormatter release];
        }
        
        int itemCount = 0;
        NSArray *menuItems = [historyItem objectForKey:kKeyRatedItems];
        if (menuItems != nil)
        {
            itemCount = [menuItems count];
        }

        if (itemCount == 0)
        {
            [cell setTitle:[historyItem objectForKey:kKeyRestaurantName] description:[NSString stringWithFormat:@"Visited on %@ at %@\nNo items rated", dateString, timeString]];
        }
        else if (itemCount == 1)
        {
            [cell setTitle:[historyItem objectForKey:kKeyRestaurantName] description:[NSString stringWithFormat:@"Visited on %@ at %@\n1 item rated", dateString, timeString]];
        }
        else
        {
            [cell setTitle:[historyItem objectForKey:kKeyRestaurantName] description:[NSString stringWithFormat:@"Visited on %@ at %@\n%d items rated", dateString, timeString, itemCount]];
        }
        
        NSString* urlText = [historyItem objectForKey:kKeyImageUrl];
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
    if (_populatingItems || _lastPopulationFailed || [_history count] == 0)
	{
		return;
	}
	
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        RestaurantCell *cell = (RestaurantCell*)[_tableView cellForRowAtIndexPath:indexPath];
        [cell setItemImage:image];
    }
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    if (_populatingItems || _lastPopulationFailed || [_history count] == 0)
	{
		return;
	}
	
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        RestaurantCell *cell = (RestaurantCell*)[_tableView cellForRowAtIndexPath:indexPath];
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
	else if (_lastPopulationFailed || [_history count] == 0)
	{
		[self startPopulatingItems];
	}
    else
    {
        if ([_history count] > indexPath.row)
        {
            NSDictionary *historyItem = [_history objectAtIndex:indexPath.row];
            NSArray *menuItems = [historyItem objectForKey:kKeyRatedItems];
            if (menuItems != nil)
            {
                HistoryDetailsViewController *viewController = [[HistoryDetailsViewController alloc] initWithMenuItems:menuItems];
                [[self navigationController] pushViewController:viewController animated:YES];
                [viewController release];
            }
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
	
	[_history removeAllObjects];
    [self reloadTable];
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForHistory:_restaurantId] delegate:self key:nil httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
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
    
    NSArray *history = [dict objectForKey:kKeyCheckins];
    if (history != nil)
    {
        [_history addObjectsFromArray:history];
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
    NSLog(@"+failedToPopulateDictionary: %@", error);
    
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
