//
//  ProfileViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/16/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "ProfileViewController.h"
#import "YummyZoneSession.h"
#import "SigninViewController.h"
#import "YummyZoneUtils.h"
#import "YummyZoneUrls.h"
#import "KeyConstants.h"
#import <MessageUI/MFMailComposeViewController.h>
//#import "FBHelper.h"

@implementation ProfileViewController

- (id)init
{
    self = [super init];
    if (self) 
    {
	    // Custom initialization
    }
    return self;
}

- (void)dealloc 
{
	_tableView.delegate = nil;
    _tableView.dataSource = nil;
    [super dealloc];
}

- (void)didReceiveMemoryWarning
{
    // unregister from Facebook session state change notifications
    // [[NSNotificationCenter defaultCenter] removeObserver:self];
    
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
}

- (void)viewDidLoad
{
	self.title = @"Profile";
    [YummyZoneUtils loadBackButton:[self navigationItem]];	
	[super viewDidLoad];
    
    // listen Facebook session state changes
    // blow blocked was disabled even when we had facebook. probably it is not necessary. revisit this.
    /*
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(facebookSessionStateChanged:)
                                                 name:FBSessionStateChangedNotification
                                               object:nil];
    */
    
    // Check the session for a cached token to show the proper authenticated
    // UI. However, since this is not user intitiated, do not show the login UX.
    /*
    if([[YummyZoneSession singleton] getSettingsEnableFacebook])
    {
        [[FBHelper singleton] openFacebookSessionWithAllowLoginUI:NO delegate:self];
    }
    */
}

-(NSString*)getIdentityRowText
{
	if ([[YummyZoneSession singleton] isUsingTemporaryAccount])
    {
		return @"Sign In / Sign Up";
	}
	else
	{
		NSString *user = [NSString stringWithFormat:@"Email: %@", [[YummyZoneSession singleton] getUserName]];
		return user;
	}
}

- (void)viewDidUnload
{
    [super viewDidUnload];
}

- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
    [_tableView reloadData];
	[super viewWillAppear:animated];
    
    //[self refreshFacebookRow];
}

- (void)loadView
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	// create and configure the table view
	_tableView = [[UITableView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame] style:UITableViewStyleGrouped];
	_tableView.delegate = self;
	_tableView.dataSource = self;
	_tableView.autoresizesSubviews = YES;
	_tableView.separatorStyle = UITableViewCellSeparatorStyleSingleLine;
	_tableView.allowsSelectionDuringEditing = YES;
	_tableView.rowHeight = 90;
    _tableView.backgroundView = nil;
    
    UIView* backGView = [[UIView alloc] initWithFrame:CGRectMake(_tableView.bounds.origin.x, 
                                                                 _tableView.bounds.origin.y, 
                                                                 _tableView.bounds.size.width, 
                                                                 _tableView.bounds.size.height)];
    
    CGFloat grayRatio = 237.0/255.0;
    backGView.backgroundColor = [UIColor colorWithRed:grayRatio 
                                                green:grayRatio 
                                                 blue:grayRatio 
                                                alpha:1.0];
    
    _tableView.backgroundView = backGView;
    
	self.view = _tableView;
   
    [pool release];
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    // authentication
    // social feeds // disabled for now
    // miscellaneous
    return 2;
}

/*
- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section 
{
    if (section == 0)
    {
        return @"User";
    }
	else if (section == 1)
    {
        return @"Misc. Actions";
    }
	
	return @" ";
}	
*/

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
	if (section == 0)
    {
        return 1;
    }
    /*
    else if (section == 1)
    {
        return 2;
    }
    */
	else if (section == 1)
    {
        return 2;
    }
    else 
    {
        return 0;
    }
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{   
    if (indexPath.section == 0)
    {
        return 44;
    }
	
	return 44;
}	

-(NSString*)getRowTextAt:(int)row section:(int)section
{
	if(section == 0)
	{
		if(row == 0)
		{
			return [self getIdentityRowText];
		}
	}
    /*
    else if(section == 1)
	{
		if(row == 0)
		{
			return @"Facebook";
		}
        else if(row == 1)
		{
			return @"Post on Facebook Wall";
		}
	}
    */
    else if(section == 1)
	{
		if(row == 0)
		{
			return @"Report an issue";
		}
        else if(row == 1)
		{
			return @"Rate this app";
		}
	}
	
	return @" ";
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath 
{	
	static NSString *kProfileView = @"kProfileView";
	UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kProfileView];
	if (cell == nil) 
	{ 
		cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault 
									   reuseIdentifier:kProfileView] autorelease];
	}
	
	cell.textLabel.text = [self getRowTextAt:indexPath.row section:indexPath.section]; 
    cell.textLabel.textColor = [UIColor colorWithRed:50.0/255.0 green:100.0/255.0 blue:150.0/255.0 alpha:1.0];
    
    /*
    if(indexPath.section == 1 && indexPath.row == 0)
    {
        [self refreshFacebookRow];
    }
    */
    
	return cell;
}

- (CGFloat) tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section
{
	return 44.0;
}

- (UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section
{
    NSString* headerText = @" ";
    if (section == 0)
    {
        headerText = @"User";
    }
    /*
    else if (section == 1)
    {
        headerText = @"Go Social";
    }
    */
	else if (section == 1)
    {
        headerText = @"Misc. Actions";
    }
    
	// create the parent view that will hold header Label
	UIView* customView = [[UIView alloc] initWithFrame:CGRectMake(20.0, 5.0, 300.0, 44.0)];
	
	// create the button object
	UILabel * headerLabel = [[UILabel alloc] initWithFrame:CGRectZero];
	headerLabel.backgroundColor = [UIColor clearColor];
	headerLabel.opaque = NO;
	headerLabel.textColor = [UIColor blackColor];
	headerLabel.highlightedTextColor = [UIColor whiteColor];
	headerLabel.font = [UIFont systemFontOfSize:18];
	headerLabel.frame = CGRectMake(20.0, 5.0, 300.0, 44.0);
    
	// If you want to align the header text as centered
	// headerLabel.frame = CGRectMake(150.0, 0.0, 300.0, 44.0);
    
	headerLabel.text = headerText;
	[customView addSubview:headerLabel];
    	
	return customView;
}

/*
- (UITableViewCell *)cellAtIndexPath:(NSIndexPath *)indexPath tableView:(UITableView *)tableView
{
    static NSString *kProfileView = @"kProfileView";
	
    if (indexPath.section == 0)
    {
        if (indexPath.row == 0)
        {
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kProfileView];
            if (cell == nil)
            {
                cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault 
											   reuseIdentifier:kProfileView] autorelease];
				
				NSUInteger row = [indexPath row]; 
				cell.textLabel.text = [listData objectAtIndex:row];
			}
			
			return cell;
		}
	}
	
	return nil;
}
*/

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[_tableView deselectRowAtIndexPath:indexPath animated:YES];
	
	if (indexPath.section == 0 && indexPath.row == 0 && [[YummyZoneSession singleton] isUsingTemporaryAccount])
    {
        SigninViewController *viewController = [[SigninViewController alloc] init];
		[[self navigationController] pushViewController:viewController animated:YES];
		[viewController release];
	}
    /*
	else if (indexPath.section == 1 && indexPath.row == 0)
    {
        if([[YummyZoneSession singleton] getSettingsEnableFacebook])
        {
            // it is currently enabled. user clicked it to disable;
            [[YummyZoneSession singleton] queueAsyncRequest:
             [YummyZoneUrls urlForSettings:kKeyEnableFacebook value:@"0"] useDefaultHeaders:YES];
            
            [[YummyZoneSession singleton] setSettingsEnableFacebook:FALSE];
            
            // do some clean up, too. Firstly check if there is an open FB session before cleanup
            [[FBHelper singleton] closeAndClearTokenInformation];
            
            [self refreshFacebookRow];
        }
        else
        {
            [[FBHelper singleton] openFacebookSessionWithAllowLoginUI:YES delegate:self];
        }
    }
	else if (indexPath.section == 1 && indexPath.row == 1)
    {
        [[FBHelper singleton] postOnWall];
	}
    */
    else if (indexPath.section == 1 && indexPath.row == 0)
    {
        /*
         [YummyZoneAppDelegate sendEmailTo:@"info@dinesmart365.com" withSubject:@"Bug Report"];
         */
        
        if([MFMailComposeViewController canSendMail])
        {
            MFMailComposeViewController *mailCont = [[MFMailComposeViewController alloc] init];
            mailCont.mailComposeDelegate = self;
            
            [mailCont setSubject:@"Bug Report"];
            [mailCont setToRecipients:[NSArray arrayWithObject:@"info@dinesmart365.com"]];
            // [mailCont setMessageBody:@"Don't ever want to give you up" isHTML:NO];
            
            [self presentModalViewController:mailCont animated:YES];
            [mailCont release];
        }
	}
    else if (indexPath.section == 1 && indexPath.row == 1)
    {
        #if TARGET_IPHONE_SIMULATOR
            NSLog(@"Cannot rate the App on the iOS simulator.");
        #else
            NSString *reviewURL = @"itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=543828918";
            [[UIApplication sharedApplication] openURL:[NSURL URLWithString:reviewURL]];
        #endif
    }
}

- (void)mailComposeController:(MFMailComposeViewController*)controller 
          didFinishWithResult:(MFMailComposeResult)result 
                        error:(NSError*)error
{
    // close email dialog
    [self dismissModalViewControllerAnimated:YES];
}

/*
- (void) refreshFacebookRow
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    UITableViewCell *cell = [_tableView cellForRowAtIndexPath:[NSIndexPath indexPathForRow:0 inSection:1]];
    
    if([[YummyZoneSession singleton] getSettingsEnableFacebook])
    {
        if (FBSession.activeSession != nil)
        {
            [FBHelper logFBSessionState:FBSession.activeSession.state];
            
            //if(FBSession.activeSession.state != FBSessionStateClosedLoginFailed &&
            //    FBSession.activeSession.state != FBSessionStateClosed)
            //{
            //    cell.textLabel.textColor = [UIColor colorWithRed:50.0/255.0 green:100.0/255.0 blue:150.0/255.0 alpha:1.0];
            //}
            
            if(FBSession.activeSession.isOpen || FBSession.activeSession.state == FBSessionStateCreatedTokenLoaded)
            {
                cell.textLabel.textColor = [UIColor colorWithRed:50.0/255.0 green:100.0/255.0 blue:150.0/255.0 alpha:1.0];
            }
            else
            {
                cell.textLabel.textColor = [UIColor blackColor];
            }
        }
        else
        {
            cell.textLabel.textColor = [UIColor blackColor];
        }
    }
    else
    {
        cell.textLabel.textColor = [UIColor lightGrayColor];
    }
    
    [pool release];
}

- (void)refreshFBRelatedUI
{
    [self refreshFacebookRow];
}

- (void)onCompletedFBAction:(BOOL)isByUser result:(BOOL)isSuccess
{
    NSLog(@"onCompletedFBAction. User Trigger = %@ Success = %@", isByUser ? @"Yes" : @"No",  isSuccess ? @"Yes" : @"No");
    
    if(isSuccess)
    {
        if(isByUser)
        {
            [[YummyZoneSession singleton] queueAsyncRequest:
             [YummyZoneUrls urlForSettings:kKeyEnableFacebook value:@"1"] useDefaultHeaders:YES];
            
            [[YummyZoneSession singleton] setSettingsEnableFacebook:TRUE];
        }
    }
    else
    {
        if (isByUser)
        {
            
            //NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            //
            //UIAlertView *alertView = [[UIAlertView alloc]
            //                          initWithTitle:@"Error"
            //                          message:@"Couldn't be logged in to Facebook"
            //                          delegate:nil
            //                          cancelButtonTitle:@"OK"
            //                          otherButtonTitles:nil];
            //[alertView show];
            //
            //[pool release];
            
        }
        
        [[FBHelper singleton] closeAndClearTokenInformation];
    }
    
    [self refreshFacebookRow];
}

- (void)facebookSessionStateChanged:(NSNotification*)notification
{
    NSLog(@"facebookSessionStateChanged");
    [self refreshFacebookRow];
}
*/
@end
