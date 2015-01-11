//
//  FeedbackViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "FeedbackViewController.h"
#import "TitleAndStarCell.h"
#import "PlateFeedbackCell.h"
#import "KeyConstants.h"
#import "MenuItemViewController.h"
#import "ImageCache.h"
#import "YummyZoneAppDelegate.h"
#import "KeyConstants.h"
#import "RestaurantMenuViewController.h"
#import "YummyZoneUrls.h"
#import "TextAndPlaceholderCell.h"
#import "TextAnd2PlaceholderCell.h"
#import "CellHeightHelper.h"
#import "TitleAndYesNoCell.h"
#import "TitleAndSegmentedControl.h"
#import "ViewControllerHelper.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "DictionaryHelper.h"
#import "NSDictionary+Helpers.h"
#import "YummyZoneUtils.h"

typedef enum tagFeedbackType
{
	FeedbackType_Unknown = 0,
	FeedbackType_Star = 1,
	FeedbackType_YesNo = 2,
	FeedbackType_MultiChoice = 3,
	FeedbackType_Comment = 4,
	FeedbackType_End = 5
} FeedbackType;


@interface FeedbackViewController(private)

- (void)startPopulatingItems;
- (NSUInteger)getCurrentPlateRatingForItem:(NSString*)itemId;

- (void)createTextInputControllerIfNeeded;
- (void)createSingleSelectListInputControllerIfNeeded;
- (UITableViewCell *)cellAtIndexPath:(NSIndexPath *)indexPath tableView:(UITableView *)tableView;
- (NSString*)getMatchingChoice:(NSString*)choiceId from:(NSArray*)choices;
- (void)sendFeedbackProgressive;
- (void)hideProgressBar;
- (void)showSuccessAndHideProgressBar;
- (void)showErrorAndHideProgressBar:(NSString*)errorMessage;

@end

#define kKeyStateCustomSections    @"FeedbackViewControllerState_CustomSections"
#define kKeyStateSelectedPlates    @"FeedbackViewControllerState_SelectedPlates"

#define HUD_SLEEP_WORKING    600
#define HUD_SLEEP_SUCCESS    600
#define HUD_SLEEP_FAILURE   1500

@implementation FeedbackViewController

- (id)initWithRestaurantId:(NSString*)restaurantId restaurantName:(NSString*)restaurantName stateStore:(NSMutableDictionary*)stateStore
{
    self = [super init];
	if (self)
	{
		_restaurantId = [[NSString alloc] initWithString:restaurantId];
		_restaurantName = [[NSString alloc] initWithString:restaurantName];
				        
        _stateStore = [stateStore retain];
		
        _customSections = [[NSMutableArray alloc] init];
        _selectedPlates = [[NSMutableArray alloc] init];
        
        _thumbnailCache = [[(YummyZoneAppDelegate*)[[UIApplication sharedApplication] delegate] getThumbnailCache] retain];
        
		_textInputController = nil;
		_singleSelectListInputController = nil;

		_populatingItems = FALSE;
		_lastPopulationFailed = FALSE;
        _lastPopulationError = nil;
        _downloader = nil;
        
        _lastUsedLocation = nil;
        _lastLocationError = nil;
        
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
    // Make sure to remove self from active delegates
    [_thumbnailCache removeDelegate:self];
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_lastPopulationError release];
    [_stateStore release];
    [_textInputController release];
    [_singleSelectListInputController release];
    [_customSections release];
    [_selectedPlates release];
    [_thumbnailCache release];
	[_restaurantId release];
	[_restaurantName release];
	[_tableView release];
    
    [_lastLocationError release];
    [_lastUsedLocation release];
    [_locationManager release];
    
    [super dealloc];
}


- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
}

- (void)viewDidLoad 
{
    NSLog(@"FeedbackViewController::viewDidLoad");
    
	self.title = @"Feedback";
	[YummyZoneUtils loadBackButton:[self navigationItem]];
	[super viewDidLoad];
}

- (void)viewWillAppear:(BOOL)animated
{
    NSLog(@"FeedbackViewController::viewWillAppear");
    
	self.navigationController.toolbarHidden = YES;
    [_tableView reloadData];
    [_locationManager startUpdatingLocation]; 
    
	[super viewWillAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
    NSLog(@"FeedbackViewController::viewWillDisappear");
    
    // Save the current state to the store if any provided
    if (_stateStore != nil && _lastPopulationFailed == NO)
    {
        NSMutableArray *customSectionsCopy = [[NSMutableArray alloc] initWithArray:_customSections copyItems:YES];
        [_stateStore setObject:customSectionsCopy forKey:kKeyStateCustomSections];
        [customSectionsCopy release];
        
        NSMutableArray *selectedPlatesCopy = [[NSMutableArray alloc] initWithArray:_selectedPlates copyItems:YES];
        [_stateStore setObject:selectedPlatesCopy forKey:kKeyStateSelectedPlates];
        [selectedPlatesCopy release];
    }
    
    [_locationManager stopUpdatingLocation];
}

- (void)locationManager:(CLLocationManager *)manager 
    didUpdateToLocation:(CLLocation *)newLocation 
           fromLocation:(CLLocation *)oldLocation
{
    NSLog(@"locationManager:didUpdateToLocation: newLocation: %@", newLocation);
    
    [_lastLocationError release];
    _lastLocationError = nil;
    
    [_lastUsedLocation release];
    _lastUsedLocation = [newLocation copy];
    
    if(_tableView != nil && _tableView.tableFooterView != nil)
    {
        NSLog(@"Enabling send feedback button");
        _tableView.tableFooterView.hidden = NO;
    }
    else 
    {
        NSLog(@"Omit enabling send feedback button since FORM is not ready");
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
            [_lastLocationError release];
            _lastLocationError = [error copy];
            break;
    }
}

- (void)loadView
{
    NSLog(@"FeedbackViewController::loadView");
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	// create and configure the table view
	_tableView = [[UITableView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame] style:UITableViewStylePlain];
	_tableView.delegate = self;
	_tableView.dataSource = self;
	_tableView.autoresizesSubviews = YES;
	_tableView.separatorStyle = UITableViewCellSeparatorStyleSingleLine;
	_tableView.allowsSelectionDuringEditing = YES;
	_tableView.rowHeight = 90;
    //_tableView.separatorColor = [UIColor lightGrayColor];
	
	self.view = _tableView;

    FeedbackFooterView *footerView = [[FeedbackFooterView alloc] initWithFrame:CGRectZero delegate:self];
	footerView.frame = CGRectMake(0.0, 0.0, self.view.bounds.size.width, [FeedbackFooterView getRequiredHeight]);
    footerView.hidden = YES;
	_tableView.tableFooterView = footerView;
    [footerView release];
    
    // Now that we created the controls, we can start populating the data
    if (_stateStore != nil)
    {
        if ([_stateStore objectForKey:kKeyStateCustomSections] != nil && 
            [_stateStore objectForKey:kKeyStateSelectedPlates] != nil)
        {
            NSLog(@"Loading Feedback View from cache...");
            
            [_customSections release];
            _customSections = [[NSMutableArray alloc] init];
            // Don't use _customSections = [[NSMutableArray alloc] initWithArray:[_stateStore objectForKey:kKeyStateCustomSections] copyItems:YES];
            // Reason is, MutableDictionaries inside the arrays are becoming immutable after the copy (no idea why).
            // Instead, iterate over the array and copy each item one by one using NSMutableDictionary's copy constructor.
            for (NSMutableDictionary *dict in [_stateStore objectForKey:kKeyStateCustomSections])
            {
                NSMutableDictionary *dictCopy = [[NSMutableDictionary alloc] initWithDictionary:dict copyItems:YES];
                [_customSections addObject:dictCopy];
                [dictCopy release];
            }
            
            [_selectedPlates release];
            _selectedPlates = [[NSMutableArray alloc] init];
            // Same as above: Don't use _selectedPlates = [[NSMutableArray alloc] initWithArray:[_stateStore objectForKey:kKeyStateSelectedPlates] copyItems:YES];
            for (NSMutableDictionary *dict in [_stateStore objectForKey:kKeyStateSelectedPlates])
            {
                NSMutableDictionary *dictCopy = [[NSMutableDictionary alloc] initWithDictionary:dict copyItems:YES];
                [_selectedPlates addObject:dictCopy];
                [dictCopy release];
            }
            
            if(_lastUsedLocation != nil)
            {
                NSLog(@"Enabling send feedback button");
                _tableView.tableFooterView.hidden = NO;
            }
            else 
            {
                NSLog(@"Omit enabling send feedback button since LocManager is not ready");
            }
        }
        else
        {
            [self startPopulatingItems];
        }
    }
    else
    {
        [self startPopulatingItems];
    }
    
    [pool release];
}


- (void)reloadTable
{
    // Remove ourselves from the notifications we subscribed for images.
    // We will resubscribe once we reload the menu
    [_thumbnailCache removeDelegate:self];
    [_tableView reloadData];
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return [_customSections count] + 1;
}

/*
- (UIView *) tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section 
{
    NSString* title = @" ";
    if (section == 0)
    {
        title = @"Plates";
    }
    else
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(section - 1)];
        FeedbackType sectionType = [[sectionData objectForKey:kKeyType] intValue];
        if (sectionType == FeedbackType_Comment)
        {
            title = @"Comments";
        }
    }
    
    UIView *headerView = [[[UIView alloc] initWithFrame:CGRectMake(0, 0, tableView.bounds.size.width, 30)] autorelease];
    [headerView setBackgroundColor:[UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:0.50]];
    
    UILabel *label = [[[UILabel alloc] initWithFrame:CGRectMake(10, 2, tableView.bounds.size.width - 10, 18)] autorelease];
    label.text = title;
    label.font = [UIFont boldSystemFontOfSize:15];
    label.textColor = [UIColor colorWithRed:1.0 green:1.0 blue:1.0 alpha:0.75];
    label.backgroundColor = [UIColor clearColor];
    [headerView addSubview:label];
    
    return headerView;
}
*/

- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section 
{
    if (section == 0)
    {
        return @"Plates";
    }
    else
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(section - 1)];
        FeedbackType sectionType = [[sectionData objectForKey:kKeyType] intValue];
        if (sectionType == FeedbackType_Comment)
        {
            return @"Comments";
        }
    }
    
    // Return a string with a "space" in it, so that the 
    // section separator is shown
    return @" ";
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (section == 0)
    {
        return [_selectedPlates count] + 1;
    }
    else
    {
        return [[[_customSections objectAtIndex:(section - 1)] objectForKey:kKeyItems] count];
    }
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    //return [CellHeightHelper calculateNotesCellHeightUsingText:[todoDictCopy objectForKey:kKeyNotes]];
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    int height = 54;
    
    if (indexPath.section == 0)
    {
        height = 64;
    }
    else
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSMutableDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];
        NSString *title = [itemData objectForKey:kKeyText];
        NSString *userResponse = [itemData objectForKey:kKeyUserResponse];
        
        FeedbackType sectionType = [[sectionData objectForKey:kKeyType] intValue];
        if (sectionType == FeedbackType_Comment)
        {
            if ([userResponse length] > 0)
            {
                height = [CellHeightHelper calculateNotesCellHeightUsingText:userResponse];
            }
            else
            {
                height = [CellHeightHelper calculateNotesCellHeightUsingText:title];
            }
        }
        else if (sectionType == FeedbackType_MultiChoice)
        {
            NSString *matchingChoiceText = [self getMatchingChoice:userResponse from:[itemData objectForKey:kKeyChoices]];
            if (matchingChoiceText != nil)
            {
                height = [CellHeightHelper calculateNotesCellHeightUsingText:[NSString stringWithFormat:@"%@\n%@", title, matchingChoiceText]];
                
                //height = [CellHeightHelper calculateNotesCellHeightUsingText:title] + 
                  //  [CellHeightHelper calculateNotesCellHeightUsingText:matchingChoiceText];
            }
            else
            {
                height = [CellHeightHelper calculateNotesCellHeightUsingText:title];
            }
        }
        else if (sectionType == FeedbackType_Star)
        {
            height = [CellHeightHelper calculateTitleAndStarCellHeightUsingText:title];
        }
        else if (sectionType == FeedbackType_YesNo)
        {
            height = [CellHeightHelper calculateTitleAndYesNoCellHeightUsingText:title];
        }

    }
    
    [pool release];
    return height;
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	if (_populatingItems)
	{
        return [ViewControllerHelper getSpinnerCell:tableView title:@"Loading"];
	}
	else if (_lastPopulationFailed)
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
        return [self cellAtIndexPath:indexPath tableView:tableView];
    }
}


- (UITableViewCell *)cellAtIndexPath:(NSIndexPath *)indexPath tableView:(UITableView *)tableView
{
    static NSString *kTextDescCell = @"kTextDescCell";

    if (indexPath.section == 0) // selected plates
    {
        if (indexPath.row == [_selectedPlates count])
        {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kTextDescCell];
            if (cell == nil)
            {
                cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:kTextDescCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.selectionStyle = UITableViewCellSelectionStyleBlue;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            if ([_selectedPlates count] == 0)
            {
                cell.textLabel.text = @"Select plates to rate...";
                cell.textLabel.textColor = [UIColor orangeColor];
            }
            else
            {
                cell.textLabel.text = @"Change plate selection...";
                cell.textLabel.textColor = [UIColor blackColor];
            }
            
            cell.textLabel.textAlignment = UITextAlignmentCenter;
            //cell.textLabel.textColor = [UIColor blackColor];
            
            [pool release];
            
            return cell;
        }
        else
        {
            static NSString *kPlateFeedbackCell = @"kPlateFeedbackCell";
            PlateFeedbackCell *cell = (PlateFeedbackCell*)[tableView dequeueReusableCellWithIdentifier:kPlateFeedbackCell];
            if (cell == nil)
            {
                cell = [[[PlateFeedbackCell alloc] initWithReuseIdentifier:kPlateFeedbackCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.selectionStyle = UITableViewCellSelectionStyleBlue;
                cell.delegate = self;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            NSDictionary *data = [_selectedPlates objectAtIndex:indexPath.row];
            [cell setTitle:[data objectForKey:kKeyName] rating:[self getCurrentPlateRatingForItem:[data objectForKey:kKeyId]] key:indexPath];
            
            NSString* urlText = [data objectForKey:kKeyImageUrl];
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
    else // Other rows; i.e. NON-{selected plates} row
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];
        NSString *title = [itemData objectForKey:kKeyText];
        NSString *userResponse = [itemData objectForKey:kKeyUserResponse];
        NSNumber *rating = [itemData objectForKey:kKeyRating];
        
        FeedbackType sectionType = [[sectionData objectForKey:kKeyType] intValue];
        
        if (sectionType == FeedbackType_Star)
        {
            static NSString *kTitleAndStarCell = @"kTitleAndStarCell";
            TitleAndStarCell *cell = (TitleAndStarCell*)[tableView dequeueReusableCellWithIdentifier:kTitleAndStarCell];
            if (cell == nil)
            {
                cell = [[[TitleAndStarCell alloc] initWithReuseIdentifier:kTitleAndStarCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.selectionStyle = UITableViewCellSelectionStyleNone;
                cell.delegate = self;
            }
            
            if ([rating intValue] < 0 || [rating intValue] > 5)
            {
                [cell setTitle:title rating:0 key:indexPath];
            }
            else
            {
                [cell setTitle:title rating:[rating intValue] key:indexPath];
            }
            
            return cell;
        }
        else if (sectionType == FeedbackType_Comment)
        {
            static NSString *kTextAndPlaceholderCell = @"kTextAndPlaceholderCell";
            TextAndPlaceholderCell *cell = (TextAndPlaceholderCell*)[tableView dequeueReusableCellWithIdentifier:kTextAndPlaceholderCell];
            if (cell == nil)
            {
                cell = [[[TextAndPlaceholderCell alloc] initWithReuseIdentifier:kTextAndPlaceholderCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
                cell.selectionStyle = UITableViewCellSelectionStyleBlue;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
                                       
            if ([userResponse length] > 0)
            {
                [cell setTextContent:userResponse];
                [cell setPlaceholderMode:NO];
            }
            else
            {
                [cell setTextContent:title];
                [cell setPlaceholderMode:YES];
            }
            
            [pool release];
            
            return cell;
        }
        else if (sectionType == FeedbackType_YesNo)
        {
			static NSString *kTitleAndYesNoCell = @"kTitleAndYesNoCell";
            TitleAndSegmentedControl *cell = (TitleAndSegmentedControl*)[tableView dequeueReusableCellWithIdentifier:kTitleAndYesNoCell];
            if (cell == nil)
            {
                cell = [[[TitleAndSegmentedControl alloc] initWithReuseIdentifier:kTitleAndYesNoCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.selectionStyle = UITableViewCellSelectionStyleNone;
                cell.delegate = self;
            }
            
            int yesNoVal = -1;
			if ([userResponse caseInsensitiveCompare:@"0"] == NSOrderedSame)
			{
				yesNoVal = 0;
			}
			else if ([userResponse caseInsensitiveCompare:@"1"] == NSOrderedSame)
			{
				yesNoVal = 1;
			}
			
            [cell setTitle:title value:yesNoVal key:indexPath];
            
            return cell;
						
        }
        else if (sectionType == FeedbackType_MultiChoice)
        {
            static NSString *kTextAnd2PlaceholderCell = @"kTextAnd2PlaceholderCell";
            TextAnd2PlaceholderCell *cell = (TextAnd2PlaceholderCell*)[tableView dequeueReusableCellWithIdentifier:kTextAnd2PlaceholderCell];
            if (cell == nil)
            {
                cell = [[[TextAnd2PlaceholderCell alloc] initWithReuseIdentifier:kTextAnd2PlaceholderCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
                cell.selectionStyle = UITableViewCellSelectionStyleBlue;
            }
            
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            NSString *matchingChoiceText = [self getMatchingChoice:userResponse from:[itemData objectForKey:kKeyChoices]];
            if (matchingChoiceText != nil)
            {
                [cell setTextContent:title placeholderText:matchingChoiceText];
                [cell setPlaceholderMode:NO];
            }
            else
            {
                [cell setTextContent:title placeholderText:@""];
                [cell setPlaceholderMode:YES];
            }
            
            [pool release];
            
            return cell;
        }
    }
    
    return nil;
}


- (NSString*)getMatchingChoice:(NSString*)choiceId from:(NSArray*)choices
{
    for (NSDictionary *choice in choices)
    {
        if ([[choice objectForKey:kKeyId] caseInsensitiveCompare:choiceId] == NSOrderedSame)
        {
            return [choice objectForKey:kKeyText];
        }
    }
    return nil;
}

- (void)imageDidLoad:(UIImage*)image key:(NSObject *)key
{
	PlateFeedbackCell *cell = (PlateFeedbackCell*)[_tableView cellForRowAtIndexPath:(NSIndexPath*)key];
	[cell setItemImage:image];
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject *)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    PlateFeedbackCell *cell = (PlateFeedbackCell*)[_tableView cellForRowAtIndexPath:(NSIndexPath*)key];
	[cell displayPlaceholderImage];
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (_populatingItems || _lastPopulationFailed)
    {
        return;
    }

    if (indexPath.section == 0)
    {
        if (indexPath.row == [_selectedPlates count])
        {
            RestaurantMenuViewController *viewController = [[RestaurantMenuViewController alloc] 
                                                        initWithRestaurantId:_restaurantId restaurantName:_restaurantName isModalPlateSelectorDialog:YES];
            [viewController setSelectedItemKeys:_selectedPlates];
            viewController.delegate = self;
            [[self navigationController] pushViewController:viewController animated:YES];
            [viewController release];
        }
    }
    else
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSMutableDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];

        FeedbackType sectionType = [[sectionData objectForKey:kKeyType] intValue];
        if (sectionType == FeedbackType_Comment)
        {
            [self createTextInputControllerIfNeeded];
            [_textInputController setTitle:@"Comment"];
            [_textInputController setInputDialogResultDictionary:itemData inputKey:kKeyUserResponse];
            [[self navigationController] pushViewController:_textInputController animated:YES];
        }
        else if (sectionType == FeedbackType_MultiChoice)
        {
            [self createSingleSelectListInputControllerIfNeeded];

            [_singleSelectListInputController setInputArray:[itemData objectForKey:kKeyChoices] inputDisplayKey:kKeyText inputResultKey:kKeyId outputDict:itemData outputKey:kKeyUserResponse title:@"select an option"];
            [[self navigationController] pushViewController:_singleSelectListInputController animated:YES];		
        }
    }
}


- (void)inputDialogSaveAction:(id)sender
{
}


- (void)createTextInputControllerIfNeeded
{
	if (_textInputController == nil)
	{
		_textInputController = [[TextInputController alloc] init];
	}
}


- (void)createSingleSelectListInputControllerIfNeeded
{
	if (_singleSelectListInputController == nil)
    {
		_singleSelectListInputController = [[SingleSelectKeyedListViewController alloc] init];
    }
}


- (void)switchValueChanged:(BOOL)newValue key:(NSObject*)key
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section > 0)
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];
        if (newValue)
        {
            [itemData setValue:@"True" forKey:kKeyUserResponse];
        }
        else
        {
            [itemData setValue:@"False" forKey:kKeyUserResponse];
        }
    }
    
    [pool release];
}

- (void)segmentValueChanged:(int)newValue key:(NSObject*)key
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section > 0)
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];
		
        if (newValue == 0)
        {
            [itemData setValue:@"0" forKey:kKeyUserResponse];
        }
        else if (newValue == 1)
        {
            [itemData setValue:@"1" forKey:kKeyUserResponse];
        }
		else
        {
            [itemData setValue:@"-1" forKey:kKeyUserResponse];
        }
    }
    
    [pool release];
}


- (void)ratingChanged:(int)rating key:(NSObject*)key
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

    NSIndexPath *indexPath = (NSIndexPath*)key;
    if (indexPath.section == 0)
    {
        NSMutableDictionary *data = [_selectedPlates objectAtIndex:indexPath.row];
        [data setValue:[NSNumber numberWithInt:rating] forKey:kKeyRating];
    }
    else
    {
        NSDictionary *sectionData = [_customSections objectAtIndex:(indexPath.section - 1)];
        NSMutableDictionary *itemData = [[sectionData objectForKey:kKeyItems] objectAtIndex:indexPath.row];
        [itemData setValue:[NSNumber numberWithInt:rating] forKey:kKeyRating];
    }
    
    [pool release];
}


- (NSUInteger)getCurrentPlateRatingForItem:(NSString*)itemId
{
    for (NSDictionary *plate in _selectedPlates)
    {
        NSString *plateId = [plate objectForKey:kKeyId];
        if ([plateId isEqualToString:itemId])
        {
            NSNumber *rating = [plate objectForKey:kKeyRating];
            return [rating intValue];
        }
    }
    return 0;
}


- (void)menuItemsSelected:(NSArray*)selectedItems
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSMutableArray *newSelection = [[NSMutableArray alloc] init];
    for (NSDictionary *item in selectedItems)
    {
        NSMutableDictionary *newItem = [[[NSMutableDictionary alloc] initWithDictionary:item] autorelease];
        [newItem setObject:[NSNumber numberWithInt:[self getCurrentPlateRatingForItem:[item objectForKey:kKeyId]]] forKey:kKeyRating];
        [newSelection addObject:newItem];
    }
    
    [_selectedPlates release];
    _selectedPlates = newSelection;
    
    [self reloadTable];

    [pool release];
}


- (void)startPopulatingItems
{
	if (_populatingItems)
    {
        return;
    }
	
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	_populatingItems = TRUE;
	_lastPopulationFailed = FALSE;
	
	[_customSections removeAllObjects];
    [self reloadTable];
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }
    
    _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForRequestedFeedback:_restaurantId] 
                                                   delegate:self 
                                                        key:nil 
                                                httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
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
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	_populatingItems = FALSE;
	_lastPopulationFailed = FALSE;
	
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    NSMutableArray *starItems = [[[NSMutableArray alloc] init] autorelease];
    NSMutableArray *yesNoItems = [[[NSMutableArray alloc] init] autorelease];
    NSMutableArray *multiChoiceItems = [[[NSMutableArray alloc] init] autorelease];
    NSMutableArray *commentItems = [[[NSMutableArray alloc] init] autorelease];
    
    NSArray *rateQuestions = [dict objectForKey:kKeyRateQuestions];
    if (rateQuestions != nil)
    {
        for (NSDictionary *item in rateQuestions)
        {
            NSString *itemId = [item objectForKey:kKeyId];
            NSString *itemText = [item objectForKey:kKeyText];

            if (itemId == nil || itemText == nil || [itemId length] == 0 || [itemText length] == 0)
            {
                NSLog(@"Invalid feedback item: %@", item);
                continue;
            }
            
            BOOL hasAnsweredInCurrentSession = false;
            if ([item objectForKey:kKeyHasAnsweredInCurrentSession] != nil)
            {
                hasAnsweredInCurrentSession = [[item objectForKey:kKeyHasAnsweredInCurrentSession] boolValue];
            }
            
            NSNumber *answerInCurrentSession = [item objectForKey:kKeyAnswerInCurrentSession];
            
            NSMutableDictionary *mutableItem = [[[NSMutableDictionary alloc] initWithDictionary:item] autorelease];
            if (hasAnsweredInCurrentSession && answerInCurrentSession > 0)
            {
                [mutableItem setObject:answerInCurrentSession forKey:kKeyRating];
            }
            else
            {
                [mutableItem setObject:[NSNumber numberWithInt:0] forKey:kKeyRating];
            }
            
            [starItems addObject:mutableItem];
        }
    }
    
    NSArray *yesNoQuestions = [dict objectForKey:kKeyYesNoQuestions];
    if (yesNoQuestions != nil)
    {
        for (NSDictionary *item in yesNoQuestions)
        {
            NSString *itemId = [item objectForKey:kKeyId];
            NSString *itemText = [item objectForKey:kKeyText];
            
            if (itemId == nil || itemText == nil || [itemId length] == 0 || [itemText length] == 0)
            {
                NSLog(@"Invalid feedback item: %@", item);
                continue;
            }
            
            BOOL hasAnsweredInCurrentSession = false;
            if ([item objectForKey:kKeyHasAnsweredInCurrentSession] != nil)
            {
                hasAnsweredInCurrentSession = [[item objectForKey:kKeyHasAnsweredInCurrentSession] boolValue];
            }
            
            BOOL answerInCurrentSession = false;
            if ([item objectForKey:kKeyAnswerInCurrentSession] != nil)
            {
                answerInCurrentSession = [[item objectForKey:kKeyAnswerInCurrentSession] boolValue];
            }
            
            NSMutableDictionary *mutableItem = [[[NSMutableDictionary alloc] initWithDictionary:item] autorelease];
			if(hasAnsweredInCurrentSession)
			{
				if (answerInCurrentSession)
				{
					[mutableItem setObject:@"1" forKey:kKeyUserResponse];
				}
				else
				{
					[mutableItem setObject:@"0" forKey:kKeyUserResponse];
				}
			}
			else
			{
				[mutableItem setObject:@"-1" forKey:kKeyUserResponse];
			}
            
            [yesNoItems addObject:mutableItem];
        }
    }
    
    NSArray *multipleChoiceQuestions = [dict objectForKey:kKeyMultipleChoiceQuestions];
    if (multipleChoiceQuestions != nil)
    {
        for (NSDictionary *item in multipleChoiceQuestions)
        {
            NSString *itemId = [item objectForKey:kKeyId];
            NSString *itemText = [item objectForKey:kKeyText];
            
            if (itemId == nil || itemText == nil || [itemId length] == 0 || [itemText length] == 0)
            {
                NSLog(@"Invalid feedback item: %@", item);
                continue;
            }
            
            NSArray *choices = [item objectForKey:kKeyChoices];
            if (choices == nil)
            {
                NSLog(@"Invalid feedback item: nil");
                continue;
            }
            else if ([choices count] < 2)
            {
                NSLog(@"Invalid feedback item: %@", item);
                continue;
            }

            BOOL hasAnsweredInCurrentSession = false;
            if ([item objectForKey:kKeyHasAnsweredInCurrentSession] != nil)
            {
                hasAnsweredInCurrentSession = [[item objectForKey:kKeyHasAnsweredInCurrentSession] boolValue];
            }
            
            NSString *answerInCurrentSession = [item objectForKey:kKeyAnswerInCurrentSession];
            
            NSMutableDictionary *mutableItem = [[[NSMutableDictionary alloc] initWithDictionary:item] autorelease];
            [mutableItem setObject:choices forKey:kKeyChoices];
            if (hasAnsweredInCurrentSession && answerInCurrentSession != nil)
            {
                [mutableItem setObject:answerInCurrentSession forKey:kKeyUserResponse];
            }
            else
            {
                [mutableItem setObject:@"" forKey:kKeyUserResponse];
            }
            
            [multiChoiceItems addObject:mutableItem];
        }
    }

    NSArray *openEndedQuestions = [dict objectForKey:kKeyOpenEndedQuestions];
    if (openEndedQuestions != nil)
    {
        for (NSDictionary *item in openEndedQuestions)
        {
            NSString *itemId = [item objectForKey:kKeyId];
            NSString *itemText = [item objectForKey:kKeyText];
            
            if (itemId == nil || itemText == nil || [itemId length] == 0 || [itemText length] == 0)
            {
                NSLog(@"Invalid feedback item: %@", item);
                continue;
            }
            
            BOOL hasAnsweredInCurrentSession = false;
            if ([item objectForKey:kKeyHasAnsweredInCurrentSession] != nil)
            {
                hasAnsweredInCurrentSession = [[item objectForKey:kKeyHasAnsweredInCurrentSession] boolValue];
            }
            
            NSString *answerInCurrentSession = [item objectForKey:kKeyAnswerInCurrentSession];
            
            NSMutableDictionary *mutableItem = [[[NSMutableDictionary alloc] initWithDictionary:item] autorelease];
            if (hasAnsweredInCurrentSession && answerInCurrentSession != nil)
            {
                [mutableItem setObject:answerInCurrentSession forKey:kKeyUserResponse];
            }
            else
            {
                [mutableItem setObject:@"" forKey:kKeyUserResponse];
            }
            
            [commentItems addObject:mutableItem];
        }
    }
    
    [_customSections removeAllObjects];

    if ([starItems count] > 0)
    {
        NSMutableDictionary *sectionData = [[[NSMutableDictionary alloc] init] autorelease];
        [sectionData setValue:[NSNumber numberWithInt:FeedbackType_Star] forKey:kKeyType];
        [sectionData setValue:starItems forKey:kKeyItems];
        [_customSections addObject:sectionData];
    }

    if ([yesNoItems count] > 0)
    {
        NSMutableDictionary *sectionData = [[[NSMutableDictionary alloc] init] autorelease];
        [sectionData setValue:[NSNumber numberWithInt:FeedbackType_YesNo] forKey:kKeyType];
        [sectionData setValue:yesNoItems forKey:kKeyItems];
        [_customSections addObject:sectionData];
    }
    
    if ([multiChoiceItems count] > 0)
    {
        NSMutableDictionary *sectionData = [[[NSMutableDictionary alloc] init] autorelease];
        [sectionData setValue:[NSNumber numberWithInt:FeedbackType_MultiChoice] forKey:kKeyType];
        [sectionData setValue:multiChoiceItems forKey:kKeyItems];
        [_customSections addObject:sectionData];
    }

    if ([commentItems count] > 0)
    {
        NSMutableDictionary *sectionData = [[[NSMutableDictionary alloc] init] autorelease];
        [sectionData setValue:[NSNumber numberWithInt:FeedbackType_Comment] forKey:kKeyType];
        [sectionData setValue:commentItems forKey:kKeyItems];
        [_customSections addObject:sectionData];
    }
    
    [pool release];
    
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_tableView reloadData];  
    
    if(_lastUsedLocation != nil)
    {
        NSLog(@"Enabling send feedback button");
        _tableView.tableFooterView.hidden = NO;
    }
    else 
    {
        NSLog(@"Omit enabling send feedback button since LocManager is not ready");
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
	[_tableView reloadData];
}


- (void)onSendFeedback
{
    if (_lastUsedLocation == nil)
    {
        return;
    }

    _progressBar = [[MBProgressHUD alloc] initWithView:self.view];
    _progressBar.labelText = @"Sending feedback...";    
    [self.view addSubview:_progressBar];    
    [_progressBar showWhileExecuting:@selector(sendFeedbackProgressive) onTarget:self withObject:nil animated:YES];
}

- (void)hideProgressBar
{
    if(_progressBar != nil)
    {
        [_progressBar removeFromSuperview];
        [_progressBar release];
        _progressBar = nil;
    }    
}

- (void)showSuccessAndHideProgressBar
{
    if(_progressBar != nil)
    {
        _progressBar.customView = [[[UIImageView alloc] initWithImage:
                                    [UIImage imageNamed:@"Checkmark.png"]] autorelease];
        _progressBar.mode = MBProgressHUDModeCustomView;
        _progressBar.labelText = @"Sent successfully!";
        // sleep awhile msec to let user see this message
        usleep(HUD_SLEEP_SUCCESS * 1000);
        
        [self hideProgressBar];
    }    
}

- (void)showErrorAndHideProgressBar:(NSString*)errorMessage
{
    if(_progressBar != nil)
    {
        _progressBar.customView = [[[UIImageView alloc] initWithImage:
                                    [UIImage imageNamed:@"xcross.png"]] autorelease];
        _progressBar.mode = MBProgressHUDModeCustomView;
        _progressBar.labelText = errorMessage;
        // sleep awhile msec to let user see this message
        usleep(HUD_SLEEP_FAILURE * 1000);
        
        [self hideProgressBar];
    }    
}

- (void)sendFeedbackProgressive
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    // Get a current time to calculate elapsed time
    NSDate* startTime = [NSDate date];
   
    NSMutableDictionary *rootDict = [[[NSMutableDictionary alloc] init] autorelease];
    [rootDict setObject:_restaurantId forKey:kKeyRestaurantId];
    
    // set location
    NSMutableDictionary *currentLocation = [[[NSMutableDictionary alloc] init] autorelease];
    [currentLocation setObject:[NSNumber numberWithDouble:_lastUsedLocation.coordinate.latitude] forKey:kKeyLatitude];
    [currentLocation setObject:[NSNumber numberWithDouble:_lastUsedLocation.coordinate.longitude] forKey:kKeyLongitude];
    [rootDict setObject:currentLocation forKey:kKeyCurrentLocation];
    
    NSMutableArray *menuItemRatings = [[[NSMutableArray alloc] init] autorelease];
    
    // First, put the plate ratings
    for (NSDictionary *plate in _selectedPlates)
    {
        NSMutableDictionary *plateDict = [[[NSMutableDictionary alloc] init] autorelease];
        [plateDict setObject:[plate objectForKey:kKeyId] forKey:kKeyMenuItemId];
        [plateDict setObject:[plate objectForKey:kKeyRating] forKey:kKeyRate];
        [menuItemRatings addObject:plateDict];
    }

    [rootDict setObject:menuItemRatings forKey:kKeyMenuItemRates];
    
    for (NSDictionary *section in _customSections)
    {
        NSMutableArray *sectionFeedback = [[[NSMutableArray alloc] init] autorelease];
        NSNumber *sectionType = [section objectForKey:kKeyType];

        for (NSDictionary *itemData in [section objectForKey:kKeyItems])
        {
            NSString *itemId = [itemData objectForKey:kKeyId];
            NSString *userResponse = [itemData objectForKey:kKeyUserResponse];
            
			if((userResponse != nil && [userResponse length] > 0) || 
			   ([sectionType intValue] == FeedbackType_Star))
			{
				NSMutableDictionary *itemFeedback = [[[NSMutableDictionary alloc] init] autorelease];
				[itemFeedback setObject:itemId forKey:kKeyQuestionId];
				BOOL insertItem = TRUE;
				
				if([sectionType intValue] == FeedbackType_Star)
				{
					NSNumber *rating = [itemData objectForKey:kKeyRating];
					if([rating intValue] > 0)
					{							
						[itemFeedback setObject:rating forKey:kKeyRate];
					}
					else
					{
						insertItem = FALSE;
					}
				}
				else if([sectionType intValue] == FeedbackType_YesNo)
				{
					if ([userResponse caseInsensitiveCompare:@"1"] == NSOrderedSame)
					{
						[itemFeedback setObject:[NSNumber numberWithInt:1] forKey:kKeyAnswer];
					}
					else if ([userResponse caseInsensitiveCompare:@"0"] == NSOrderedSame)
					{
						[itemFeedback setObject:[NSNumber numberWithInt:0] forKey:kKeyAnswer];
					}
					else
					{
						insertItem = FALSE;
					}	
				}
				else if([sectionType intValue] == FeedbackType_MultiChoice)
				{
					[itemFeedback setObject:userResponse forKey:kKeyAnswerId];
				}
				else if([sectionType intValue] == FeedbackType_Comment)
				{
					[itemFeedback setObject:userResponse forKey:kKeyAnswer];
				}
				else
				{
					NSLog(@"Invalid section type: %@", sectionType);
				}
				
				if(insertItem)
				{
					[sectionFeedback addObject:itemFeedback];
				}
			
			} // if 
        } // for
        
        switch ([sectionType intValue])
        {
            case FeedbackType_Star:
                [rootDict setObject:sectionFeedback forKey:kKeyRateQuestionAnswers];
                break;
            case FeedbackType_YesNo:
                [rootDict setObject:sectionFeedback forKey:kKeyYesNoQuestionAnswers];
                break;
            case FeedbackType_MultiChoice:
                [rootDict setObject:sectionFeedback forKey:kKeyMultiChoiceQuestionAnswers];
                break;
            case FeedbackType_Comment:
                [rootDict setObject:sectionFeedback forKey:kKeyOpenEndedQuestionAnswers];
                break;
            default:
                NSLog(@"Invalid section type: %@", sectionType);
                break;
        }
    }
    
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:[YummyZoneUrls urlForSendFeedback]];
    [request setHTTPMethod:@"POST"];
    NSDictionary *httpHeaders = [[YummyZoneSession singleton] getHttpHeaders];
    for (NSString *key in httpHeaders)
    {
        [request setValue:[httpHeaders objectForKey:key] forHTTPHeaderField:key];
    }
    [request setHTTPBody:[DictionaryHelper getDictionaryContentsForHttpPost:rootDict]];
	//NSLog(@"%@", request);
	
	//NSString* postContent = [DictionaryHelper getDictionaryContentsAsText:rootDict];
	//NSLog(@"%@", postContent);
    
    NSURLResponse *response = nil;
    NSError *error = nil;
    NSData *data = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
    
    // Find elapsed time and convert to milliseconds
    // Use (-) modifier to conversion since receiver is earlier than now
    double elapsedTimeInMillisecs = [startTime timeIntervalSinceNow] * -1000.0;
    NSLog(@"Elapsed Time: %f", elapsedTimeInMillisecs);
    
    if(elapsedTimeInMillisecs < HUD_SLEEP_WORKING)
    {
        // sleep more to let the user see 'progress bar'
        usleep((HUD_SLEEP_WORKING-elapsedTimeInMillisecs) * 1000.0);
    }
    
    if (error) 
    {
        NSLog(@"%@", error);
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                        message:[error description]
                                                       delegate:self 
                                              cancelButtonTitle:@"Close" 
                                              otherButtonTitles:nil];
        [self hideProgressBar];
        [alert show];
        [alert release];    }
    else
    {
        //NSLog(@"%@", [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] autorelease]);

        NSDictionary *dict = [NSDictionary dictionaryWithContentsOfData:data];
        if ([YummyZoneHelper webRequestSucceeded:dict])
        {
            // Clear the state store since we are now done with the feedback
            if (_stateStore != nil)
            {
                [_stateStore removeAllObjects];
            }
            
            [self showSuccessAndHideProgressBar];
            [self.navigationController popViewControllerAnimated:YES];
        }
        else
        {
            NSString *serverMessage = [YummyZoneHelper getOperationErrorMessage:dict];
            if (serverMessage != nil)
            {
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                                message:[NSString stringWithFormat:@"Failed to send feedback. Server returned failure: %@.", serverMessage]
                                                               delegate:self 
                                                      cancelButtonTitle:@"Close" 
                                                      otherButtonTitles:nil];
                [self hideProgressBar];
                [alert show];
                [alert release];
            }
            else
            {
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                                message:@"Failed to send feedback. Server returned failure."
                                                               delegate:self 
                                                      cancelButtonTitle:@"Close" 
                                                      otherButtonTitles:nil];
                [self hideProgressBar];
                [alert show];
                [alert release];            
            }
        }
    }
    
    [pool release];
}

@end

