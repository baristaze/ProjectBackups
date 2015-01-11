//
//  MessageViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/29/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <QuartzCore/QuartzCore.h>
#import "MessageListViewController.h"
#import "YummyZoneUrls.h"
#import "KeyConstants.h"
#import "MessageSummaryCell.h"
#import "ViewControllerHelper.h"
#import "MessageDetailsViewController.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneSession.h"
#import "YummyZoneUtils.h"

// pull-down start
#define REFRESH_HEADER_HEIGHT 52.0f
// pull-down end

@interface MessageListViewController(private)
- (void)startPopulatingItems:(BOOL)appendToExistingMessages;
- (void)setEditMode:(BOOL)mode;
@end

@implementation MessageListViewController

// pull-down start
@synthesize textPull, textRelease, textLoading, refreshHeaderView, refreshLabel, refreshArrow, refreshSpinner;
// pull-down end

- (id)initWithType:(BOOL)isCouponList restaurantId:(NSString*)restaurantId
{
    self = [super init];
	if (self)
	{
        _isCouponList = isCouponList;
        _restaurantId = [restaurantId copy];
        _messages = [[NSMutableArray alloc] init];
        _hasMoreMessages = NO;
        _hintForNextPage = nil;
        _appendMessages = NO;
        _downloader = nil;
        
        _selectedMessages = [[NSMutableArray alloc] init];

		_populatingItems = FALSE;
		_lastPopulationFailed = FALSE;
        _lastPopulationError = nil;
		
		// pull-down start
		[self setupStrings];
		// pull-down end
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
    
    [_lastPopulationError release];
    [_restaurantId release];
    [_hintForNextPage release];
    [_selectedImage release];
    [_unselectedImage release];
    [_messages release];
    [_selectedMessages release];
	[_tableView release];
	
	// pull-down start
	[refreshHeaderView release];
    [refreshLabel release];
    [refreshArrow release];
    [refreshSpinner release];
    [textPull release];
    [textRelease release];
    [textLoading release];
	// pull-down end
	
    [super dealloc];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
}

- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
    [_tableView reloadData];
    [super viewWillAppear:animated];
}

- (void)cancelAction:(id)sender
{
	[self.navigationController popViewControllerAnimated:YES];
}

- (void)viewDidLoad 
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

    if (_isCouponList)
    {
        self.title = @"Coupons";
    }
    else
    {
        self.title = @"Inbox";
    }
    
    
    UIButton* editButton = [YummyZoneUtils createNavBarButtonWithText:@"Edit" 
                                                                width:50.0
                                                               target:self 
                                                               action:@selector(toggleEditAction:)];
    
    UIButton* doneButton = [YummyZoneUtils createNavBarButtonWithText:@"Done" 
                                                                 width:50.0
                                                                target:self 
                                                                action:@selector(toggleEditAction:)];
    
    _editBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:editButton];  
    _doneBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:doneButton];
   
	//self.navigationItem.rightBarButtonItem = _editBarButtonItem;	
    //[self.navigationItem.rightBarButtonItem setEnabled:FALSE];
    [self startPopulatingItems:NO];
    
	[YummyZoneUtils loadBackButton:[self navigationItem]];
	
    [pool release];
	[super viewDidLoad];
	
	// pull-down start
	[self addPullToRefreshHeader];
	// pull-down end
}

- (void) toggleEditAction:(id)sender
{
	refreshLabel.hidden = !self.editing;
	refreshArrow.hidden = !self.editing;
	[self setEditMode:(!self.editing)];
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
	_tableView.rowHeight = 92;
    _tableView.backgroundColor = [UIColor clearColor];

	self.view = _tableView;
}

- (void)setEditMode:(BOOL)mode
{
    self.editing = mode;
    if (self.editing)
    {
        self.navigationItem.rightBarButtonItem = _doneBarButtonItem;
    }
    else
    {
        if([_messages count] == 0)
        {
            self.navigationItem.rightBarButtonItem = NULL;
        }
        else 
        {
            self.navigationItem.rightBarButtonItem = _editBarButtonItem;
        }
    }
    
    [_tableView setEditing:self.editing animated:YES];
    [_tableView reloadData];
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (self.editing)
    {
        return [_messages count];
    }
    else
    {
        if (_populatingItems || _hasMoreMessages || _lastPopulationFailed)
        {
            return [_messages count] + 1;
        }
        else
        {
            if ([_messages count] == 0)
            {
                return 1;
            }
            else
            {
                return [_messages count];
            }
        }
    }
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (indexPath.row < [_messages count])
    {
        static NSString *kMessageSummaryCell = @"kMessageSummaryCell";
        MessageSummaryCell *cell = (MessageSummaryCell*)[tableView dequeueReusableCellWithIdentifier:kMessageSummaryCell];
        if (cell == nil)
        {
            cell = [[[MessageSummaryCell alloc] initWithReuseIdentifier:kMessageSummaryCell] autorelease];
            cell.selectionStyle = UITableViewCellSelectionStyleBlue;
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
        }
        
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        
        NSDictionary *itemData = [_messages objectAtIndex:indexPath.row];
        [cell setSubject:[itemData objectForKey:kKeySubject] body:[itemData objectForKey:kKeyBody] from:[itemData objectForKey:kKeySender] date:[itemData objectForKey:kKeySentTimeUTC]];
        
        [pool release];
        return cell;
    }
    else
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
        else if (_hasMoreMessages)
        {
            static NSString *kLoadMoreCell = @"kLoadMoreCell";
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kLoadMoreCell];
            if (cell == nil)
            {
                cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:kLoadMoreCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.textLabel.textColor = [UIColor blueColor];
            }
            
            cell.textLabel.text = @"Load more messages";
            return cell;
        }
        else
        {
            if (_isCouponList)
            {
                return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"No coupons." description:@"Pull down to refresh..."];
            }
            else
            {
                return [ViewControllerHelper getTextAndDescriptionCell:tableView title:@"No messages." description:@"Pull down to refresh..."];
            }
        }
    }
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
	
    if (indexPath.row < [_messages count])
    {
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        
        NSDictionary *itemData = [_messages objectAtIndex:indexPath.row];
        NSDateFormatter *dateFormatter = [[[NSDateFormatter alloc] init] autorelease];
		[dateFormatter setDateFormat:@"EEEE, MMMM d yyyy HH:mm"];
        
        MessageDetailsViewController *viewController = [[[MessageDetailsViewController alloc] 
                                                         initWithFrom:[itemData objectForKey:kKeySender] 
                                                         subject:[itemData objectForKey:kKeySubject] 
                                                         date:[dateFormatter stringFromDate:[itemData objectForKey:kKeySentTimeUTC]] 
                                                         message:[itemData objectForKey:kKeyBody] 
                                                         messageId:[itemData objectForKey:kKeyId] 
                                                         delegate:self 
                                                         isCoupon:_isCouponList] autorelease];
        
        [[self navigationController] pushViewController:viewController animated:YES];
        
        if (_isCouponList)
        {
            [[YummyZoneSession singleton] queueAsyncRequest:[YummyZoneUrls urlToMarkCouponRead:[itemData objectForKey:kKeyId]] useDefaultHeaders:YES];
        }
        else
        {
            [[YummyZoneSession singleton] queueAsyncRequest:[YummyZoneUrls urlToMarkMessageRead:[itemData objectForKey:kKeyId]] useDefaultHeaders:YES];
        }

        [pool release];
    }
	else
	{
        if (_hasMoreMessages)
        {
            [self startPopulatingItems:YES];
        }
        else if (_lastPopulationFailed)
        {
            [self startPopulatingItems:NO];
        }
	}
}

- (UITableViewCellEditingStyle)tableView:(UITableView *)tableView editingStyleForRowAtIndexPath:(NSIndexPath *)indexPath 
{
    if (indexPath.row < [_messages count])
    {
        return UITableViewCellEditingStyleDelete;
    }
    else
    {
        return UITableViewCellEditingStyleNone;
    }
}


- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath 
{
    if (editingStyle == UITableViewCellEditingStyleDelete) 
	{
        if ([_messages count] > indexPath.row)
        {
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            NSDictionary *itemData = [_messages objectAtIndex:indexPath.row];
            NSString *itemid = [itemData objectForKey:kKeyId];
            NSURL *url = nil;
            if (_isCouponList)
            {
                url = [YummyZoneUrls urlToDeleteCoupon:itemid];
            }
            else
            {
                url = [YummyZoneUrls urlToDeleteMessage:itemid];
            }
            [[YummyZoneSession singleton] queueAsyncRequest:url useDefaultHeaders:YES];

            [_messages removeObjectAtIndex:indexPath.row];
            [_tableView reloadData];
            
            if([_messages count] == 0)
            {
                if(self.editing)
                {
                    // hide done button
                    [self setEditMode:NO];
                }
                else 
                {
                    // hide edit button
                    self.navigationItem.rightBarButtonItem = NULL;
                }
            }
            
            [pool release];
        }
    }
}

- (void)startPopulatingItems:(BOOL)appendToExistingMessages
{
	if (_populatingItems)
	{
		if (isLoading) 
		{
			[self performSelector:@selector(stopLoading) withObject:nil afterDelay:0.0];
		}
		
		return;
	}
	
    _appendMessages = appendToExistingMessages;
    if (!appendToExistingMessages)
    {
        [_hintForNextPage release];
        _hintForNextPage = nil;
    }
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
	[UIApplication sharedApplication].networkActivityIndicatorVisible = YES;
	
	_populatingItems = TRUE;
	_lastPopulationFailed = FALSE;
	
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    NSLog(@"%@", _hintForNextPage);
    
    if (_isCouponList)
    {
        _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForCoupons:_hintForNextPage restaurantId:_restaurantId] delegate:self key:nil httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
    }
    else
    {
        _downloader = [[DictionaryDownloader alloc] initWithUrl:[YummyZoneUrls urlForInbox:_hintForNextPage restaurantId:_restaurantId] delegate:self key:nil httpHeaders:[[YummyZoneSession singleton] getHttpHeaders]];
    }
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
		
		if (isLoading) 
		{
			[self performSelector:@selector(stopLoading) withObject:nil afterDelay:0.0];
		}
		
        return;
    }
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];

	_populatingItems = FALSE;
	_lastPopulationFailed = FALSE;
	_hasMoreMessages = FALSE;
    [_hintForNextPage release];
    _hintForNextPage = nil;
    
	[UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
        
    NSString* keyForList = kKeyMessages;    
    if (_isCouponList)
    {
        keyForList = kKeyCoupons;
    }

    NSArray *tempMessages = [dict objectForKey:keyForList];
    if (tempMessages == nil)
    {
        tempMessages = [[[NSArray alloc] init] autorelease];
    }
    
    if (!_appendMessages)
    {
        [_messages removeAllObjects];
        [_tableView scrollToRowAtIndexPath:[NSIndexPath indexPathForRow:0 inSection:0] atScrollPosition:UITableViewScrollPositionTop animated:NO];
    }
    
    [_messages addObjectsFromArray:tempMessages];
    
    [_selectedMessages removeAllObjects];
    for (int i = 0; i < [_messages count]; i++)
    {
        [_selectedMessages addObject:[NSNumber numberWithBool:NO]];
    }
    
    NSString* keyForMore = kKeyHasMoreMessages;    
    if (_isCouponList)
    {
        keyForList = kKeyHasMoreCoupons;
    }
    
    if ([dict objectForKey:keyForMore] != nil)
    {
        _hasMoreMessages = [[dict objectForKey:keyForMore] boolValue];
        
        if (_hasMoreMessages)
        {
            _hintForNextPage = [[dict objectForKey:kKeyHintForNextPage] copy];
            if (_hintForNextPage == nil || [_hintForNextPage length] == 0)
            {
                _hasMoreMessages = NO;
            }
        }
    }
    
    [_tableView reloadData];
    
    if([_messages count] > 0)
    {
        //[self.navigationItem.rightBarButtonItem setEnabled:TRUE];
        self.navigationItem.rightBarButtonItem = _editBarButtonItem;
    }

    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

	if (isLoading) 
	{
		[self performSelector:@selector(stopLoading) withObject:nil afterDelay:0.0];
	}
	
    [pool release];
}


- (void)dictionaryFailedToLoad:(NSString*)error key:(NSObject*)key
{
    NSLog(@"+failedToPopulateDictionary: %@", error);
    
	_populatingItems = FALSE;
	_lastPopulationFailed = TRUE;
    _hasMoreMessages = FALSE;
    [_hintForNextPage release];
    _hintForNextPage = nil;
	
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
    
    if ([_messages count] == 0)
    {
        [_tableView reloadData];
    }
    
	if (isLoading) 
	{
		[self performSelector:@selector(stopLoading) withObject:nil afterDelay:0.0];
	}
	
    if([_messages count] > 0)
    {
        //[self.navigationItem.rightBarButtonItem setEnabled:TRUE];
        self.navigationItem.rightBarButtonItem = _editBarButtonItem;
    }
}


- (void)onMessageDeleted:(NSString*)messageId
{
    for (int i = 0; i < [_messages count]; i++)
    {
        NSDictionary *itemData = [_messages objectAtIndex:i];
        if ([messageId localizedCaseInsensitiveCompare:[itemData objectForKey:kKeyId]] == NSOrderedSame)
        {
            [_messages removeObjectAtIndex:i];
            break;
        }
    }
    
    if([_messages count] == 0)
    {
        if(self.editing)
        {
            // hide done button
            [self setEditMode:NO];
        }
        else 
        {
            // hide edit button
            self.navigationItem.rightBarButtonItem = NULL;
        }
    }
}

// pull-down start
- (void)setupStrings
{
	textPull = [[NSString alloc] initWithString:@"Pull down to refresh..."];
	textRelease = [[NSString alloc] initWithString:@"Release to refresh..."];
	textLoading = [[NSString alloc] initWithString:@"Loading..."];
}

- (void)addPullToRefreshHeader 
{
    refreshHeaderView = [[UIView alloc] initWithFrame:CGRectMake(0, 0 - REFRESH_HEADER_HEIGHT, 320, REFRESH_HEADER_HEIGHT)];
    refreshHeaderView.backgroundColor = [UIColor clearColor];
	
    refreshLabel = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, 320, REFRESH_HEADER_HEIGHT)];
    refreshLabel.backgroundColor = [UIColor clearColor];
    refreshLabel.font = [UIFont boldSystemFontOfSize:12.0];
    refreshLabel.textAlignment = UITextAlignmentCenter;
	
    refreshArrow = [[UIImageView alloc] initWithImage:[UIImage imageNamed:@"arrow.png"]];
    refreshArrow.frame = CGRectMake(floorf((REFRESH_HEADER_HEIGHT - 27) / 2),
                                    (floorf(REFRESH_HEADER_HEIGHT - 44) / 2),
                                    27, 44);
	
    refreshSpinner = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    refreshSpinner.frame = CGRectMake(floorf(floorf(REFRESH_HEADER_HEIGHT - 20) / 2), floorf((REFRESH_HEADER_HEIGHT - 20) / 2), 20, 20);
    refreshSpinner.hidesWhenStopped = YES;
	
    [refreshHeaderView addSubview:refreshLabel];
    [refreshHeaderView addSubview:refreshArrow];
    [refreshHeaderView addSubview:refreshSpinner];
    [_tableView addSubview:refreshHeaderView];
}

- (void)scrollViewWillBeginDragging:(UIScrollView *)scrollView 
{
    if (isLoading || self.editing) 
	{
		return;
	}
	
    isDragging = YES;
}

- (void)scrollViewDidScroll:(UIScrollView *)scrollView 
{
	if(self.editing)
	{
		return;
	}
	
    if (isLoading) 
	{
        // Update the content inset, good for section headers
        if (scrollView.contentOffset.y > 0)
            _tableView.contentInset = UIEdgeInsetsZero;
        else if (scrollView.contentOffset.y >= -REFRESH_HEADER_HEIGHT)
            _tableView.contentInset = UIEdgeInsetsMake(-scrollView.contentOffset.y, 0, 0, 0);
    } 
	else if (isDragging && scrollView.contentOffset.y < 0) 
	{
        // Update the arrow direction and label
        [UIView beginAnimations:nil context:NULL];
        if (scrollView.contentOffset.y < -REFRESH_HEADER_HEIGHT) 
		{
            // User is scrolling above the header
            refreshLabel.text = self.textRelease;
            [refreshArrow layer].transform = CATransform3DMakeRotation(M_PI, 0, 0, 1);
        } 
		else 
		{ 
			// User is scrolling somewhere within the header
            refreshLabel.text = self.textPull;
            [refreshArrow layer].transform = CATransform3DMakeRotation(M_PI * 2, 0, 0, 1);
        }
        [UIView commitAnimations];
    }
}

- (void)scrollViewDidEndDragging:(UIScrollView *)scrollView willDecelerate:(BOOL)decelerate 
{
    if (isLoading || self.editing) 
	{
		return;
	}
	
    isDragging = NO;
    if (scrollView.contentOffset.y <= -REFRESH_HEADER_HEIGHT) 
	{
        // Released above the header
        [self startLoading];
    }
}

- (void)startLoading 
{
    isLoading = YES;
	
    // Show the header
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDuration:0.3];
    _tableView.contentInset = UIEdgeInsetsMake(REFRESH_HEADER_HEIGHT, 0, 0, 0);
    refreshLabel.text = self.textLoading;
    refreshArrow.hidden = YES;
    [refreshSpinner startAnimating];
    [UIView commitAnimations];
	
    // Refresh action!
    [self refresh];
}

- (void)stopLoading 
{
    isLoading = NO;
	
    // Hide the header
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDelegate:self];
    [UIView setAnimationDuration:0.3];
    [UIView setAnimationDidStopSelector:@selector(stopLoadingComplete:finished:context:)];
    _tableView.contentInset = UIEdgeInsetsZero;
    UIEdgeInsets tableContentInset = _tableView.contentInset;
    tableContentInset.top = 0.0;
    _tableView.contentInset = tableContentInset;
    [refreshArrow layer].transform = CATransform3DMakeRotation(M_PI * 2, 0, 0, 1);
    [UIView commitAnimations];
}

- (void)stopLoadingComplete:(NSString *)animationID finished:(NSNumber *)finished context:(void *)context 
{
    // Reset the header
    refreshLabel.text = self.textPull;
    refreshArrow.hidden = NO;
    [refreshSpinner stopAnimating];
}

- (void)refresh 
{
	// disable edit mode
	// [self.navigationItem.rightBarButtonItem setEnabled:FALSE];
    self.navigationItem.rightBarButtonItem = NULL;
	
	// reload data
	[self startPopulatingItems:NO];
}

// pull-down end

@end
