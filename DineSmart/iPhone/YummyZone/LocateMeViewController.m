//
//  LocateMeViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "LocateMeViewController.h"
#import "RestaurantInfoCell.h"
#import "KeyConstants.h"
#import "RestaurantViewController.h"
#import "YummyZoneUrls.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneUtils.h"

@interface LocateMeViewController(private)

- (void)startUpdatingRestaurantList;

@end


@implementation LocateMeViewController

- (id)init
{
	if ((self = [super init]))
	{
        _downloader = nil;
		_restaurants = [[NSMutableArray alloc] init];
        
        _lastUsedLocation = nil;
		_lastWebServiceError = nil;
        _lastLocationError = nil;

	    _gettingInitialCoordinates = YES;
        
        _locationManager = [[CLLocationManager alloc] init];
        _locationManager.delegate = self;
        _locationManager.distanceFilter = 5.0;
        _locationManager.desiredAccuracy = kCLLocationAccuracyNearestTenMeters;
    }
	return self;
}


- (void)dealloc
{
    _tableView.delegate = nil;
    _tableView.dataSource = nil;
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_lastWebServiceError release];
    [_lastLocationError release];
    [_lastUsedLocation release];
    [_locationManager release];
	[_restaurants release];
	[_tableView release];
    [super dealloc];
}


- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
	[_tableView reloadData];
    [_locationManager startUpdatingLocation];
	[super viewWillAppear:animated];
}


- (void)viewWillDisappear:(BOOL)animated
{
    [_locationManager stopUpdatingLocation];
}


- (void)viewDidLoad 
{
	self.title = @"Locate Me";
	[YummyZoneUtils loadBackButton:[self navigationItem]];
	[super viewDidLoad];
}


- (void)locationManager:(CLLocationManager *)manager 
    didUpdateToLocation:(CLLocation *)newLocation 
           fromLocation:(CLLocation *)oldLocation
{
    NSLog(@"locationManager:didUpdateToLocation: newLocation: %@", newLocation);
    
    BOOL updateRestaurantList = NO;
    
    _gettingInitialCoordinates = NO;
    
    [_lastLocationError release];
    _lastLocationError = nil;
    
    if (_lastUsedLocation == nil)
    {
        NSLog(@"locationManager:didUpdateToLocation: Last used location is nil. Will update the list");
        updateRestaurantList = YES;
    }
    else
    {
        CLLocationDistance distance = [_lastUsedLocation distanceFromLocation:_lastUsedLocation];
        // If the distance changed more than 50 meters, reload the data
        if (distance > 50.0)
        {
            NSLog(@"locationManager:didUpdateToLocation: Location changed for %f meters. Will update the list", distance);
            updateRestaurantList = YES;
        }
        else
        {
            NSLog(@"locationManager:didUpdateToLocation: Location changed for %f meters. Will NOT update the list", distance);
        }
    }
    
    if (updateRestaurantList)
    {
        [_lastUsedLocation release];
        _lastUsedLocation = [newLocation copy];
        
        [self startUpdatingRestaurantList];
    }
}


- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    NSLog(@"locationManager:didFailWithError: error: %@", error);

    NSInteger errorCode = [error code];
    switch (errorCode)
    {
        case kCLErrorLocationUnknown:
            // Ignore this one. Location manager will keep trying
            break;
        default:
            _gettingInitialCoordinates = NO;
            [_lastLocationError release];
            _lastLocationError = [error copy];
            [_tableView reloadData];
            break;
    }
}


- (void)startUpdatingRestaurantList
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_lastWebServiceError release];
    _lastWebServiceError = nil;
	
    // Update the display only if there are no restaurants in the list
    // Otherwise, keep showing old the list and we will update once we 
    // have the new data.
    if ([_restaurants count] == 0)
    {
        [_tableView reloadData];
    }
	
    _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForNearbyRestaurants:_lastUsedLocation] delegate:self key:[NSNull null] httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
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
    
    [_lastWebServiceError release];
    _lastWebServiceError = nil;
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
	[_restaurants removeAllObjects];
    NSMutableArray *restaurantsFound = [dict objectForKey:kKeyRestaurantList];
    
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    if (restaurantsFound != nil)
    {
        [_restaurants addObjectsFromArray:restaurantsFound];
    }
    
    [_tableView reloadData];
}


- (void)dictionaryFailedToLoad:(NSString*)error key:(NSObject*)key
{
    NSLog(@"+dictionaryFailedToLoad: %@", error);
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    [_lastWebServiceError release];
    _lastWebServiceError = nil;
    if (error != nil)
    {
        _lastWebServiceError = [error copy];
    }
    else
    {
        _lastWebServiceError = [[NSString stringWithFormat:@"Request failed."] retain];
    }
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    // Do NOT clear the restaurant list. We will use the previous list.
    // However; clear the last coordinates so that we will try updating
    // this list once the coordinates change again.
    [_lastUsedLocation release];
    _lastUsedLocation = nil;
    
    // Clean this before we call reloadData.
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_tableView reloadData];
    
    [pool release];
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
	_tableView.rowHeight = 55;
	
	self.view = _tableView;
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
	return 1;
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if ([_restaurants count] > 0)
        return [_restaurants count];
    else
		return 1;
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if ([_restaurants count] > 0)
    {
		static NSString *kRestaurantInfoCell = @"kRestaurantInfoCell";
		RestaurantInfoCell *cell = (RestaurantInfoCell *)[tableView dequeueReusableCellWithIdentifier:kRestaurantInfoCell];
		if (cell == nil)
		{
			cell = [[[RestaurantInfoCell alloc] initWithReuseIdentifier:kRestaurantInfoCell] autorelease];
			cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
		}
		
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
		NSAssert(indexPath.row < [_restaurants count], @"indexPath is larger than restaurant count");
		
		NSDictionary *restaurantInfo = [_restaurants objectAtIndex:indexPath.row];
		
		[cell setTitleText:[restaurantInfo objectForKey:kKeyName]];
		[cell setDetailText:[restaurantInfo objectForKey:kKeyAddress]];
		[cell setDistanceText:[restaurantInfo objectForKey:kKeyDistance]];
		
		[pool release];
		return cell;
    }
    else
    {
        NSString *title = nil;
        NSString *details = nil;
        BOOL showActivityIndicatior = NO;
        
        static NSString *kTextDescCell = @"kTextDescCell";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kTextDescCell];
        if (cell == nil)
        {
            cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:kTextDescCell] autorelease];
            cell.accessoryType = UITableViewCellAccessoryNone;
        }
        
        NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];

        if (_downloader != nil)
        {
            title = @"Loading restaurants...";
            showActivityIndicatior = YES;
        }
        else if (_lastLocationError != nil)
        {
            if ([_lastLocationError code] == kCLErrorDenied)
            {
                title = @"Cannot get the location. Click to retry.";
                details = @"User denied access.";
            }
            else
            {
                title = @"Cannot get the location. Click to retry.";
                details = @"";
            }
        }
        else if (_gettingInitialCoordinates)
        {
            title = @"Getting your location...";
            showActivityIndicatior = YES;
        }
        else if (_lastWebServiceError)
        {
            title = @"Failed to load. Click to retry.";
            details = [NSString stringWithString:_lastWebServiceError];
        }
        else
        {
            title = @"No restaurants found. Click to reload.";
            details = @"";
        }
                 
        if (showActivityIndicatior)
        {
            UIActivityIndicatorView *activityView = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
            [activityView startAnimating];
            [cell setAccessoryView:activityView];
            [activityView release];
        }
        else
        {
            [cell setAccessoryView:nil];            
        }
        
        cell.textLabel.text = title;
        if (details != nil)
        {
            cell.detailTextLabel.text = details;
            cell.detailTextLabel.numberOfLines = 0;
        }
        
        [pool release];
        
        return cell;
    }
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
	
    if ([_restaurants count] > 0)
    {
		NSAssert(indexPath.row < [_restaurants count], @"indexPath is larger than restaurant count");
		
		NSDictionary *restaurantInfo = [_restaurants objectAtIndex:indexPath.row];
		
		RestaurantViewController *viewController = [[RestaurantViewController alloc] 
													initWithRestaurantId:[restaurantInfo objectForKey:kKeyId] 
													restaurantName:[restaurantInfo objectForKey:kKeyName]
                                                    restaurantAddress:[restaurantInfo objectForKey:kKeyAddress]];
        
		[[self navigationController] pushViewController:viewController animated:YES];
		[viewController release];
    }
    else
    {
        if ((_downloader != nil) || _gettingInitialCoordinates)
        {
            return;
        }
        else
        {
            if (_lastLocationError != nil)
            {
                [_locationManager stopUpdatingLocation];
                [_locationManager startUpdatingLocation];
            }
            else
            {
                [self startUpdatingRestaurantList];
            }
        }
    }
}

@end
