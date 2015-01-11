//
//  MessageDetailsViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/14/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "MessageDetailsViewController.h"
#import "DictionaryDownloader.h"
#import "YummyZoneUrls.h"
#import "YummyZoneSession.h"
#import "YummyZoneUtils.h"
#import "YummyZoneHelper.h"
#import "DictionaryHelper.h"
#import "NSDictionary+Helpers.h"
#import "YummyZoneUtils.h"

@interface MessageDetailsViewController(private)

- (void)redeemProgressive;
- (void)hideProgressBar;
- (void)showSuccessAndHideProgressBar;

@end

@implementation MessageDetailsViewController
@synthesize redeemButton = _redeemButton;
@synthesize fromLabel;
@synthesize subjectLabel;
@synthesize dateLabel;
@synthesize webView;

#define BUTTON_WIDTH		300
#define BUTTON_X			10

- (id)initWithFrom:(NSString*)from subject:(NSString*)subject date:(NSString*)date message:(NSString*)message messageId:(NSString*)messageId  delegate:(id<MessageDetailsViewControllerDelegate>)delegate isCoupon:(BOOL)isCoupon
{
    self = [super initWithNibName:@"MessageDetailsViewController" bundle:nil];
    if (self) 
    {  
        _from = [from copy];
        _subject = [subject copy];
        _date = [date copy];
        _message = [message copy];
        _messageId = [messageId copy];
        _delegate = delegate;
        _isCoupon = isCoupon;
        
        _lastUsedLocation = nil;
        _lastLocationError = nil;
        
        if (_isCoupon)
        {
            _locationManager = [[CLLocationManager alloc] init];
            _locationManager.delegate = self;
            _locationManager.distanceFilter = 5.0;
            _locationManager.desiredAccuracy = kCLLocationAccuracyNearestTenMeters;
        }
    }
    return self;
}


- (void)dealloc 
{
    [_from release];
    [_subject release];
    [_date release];
    [_message release];
    [_messageId release];
    [_lastLocationError release];
    [_lastUsedLocation release];
    [_locationManager release];

    [fromLabel release];
    [subjectLabel release];
    [dateLabel release];
    [webView release];
    
    if(_isCoupon)
    {
        [_redeemButton release];
    }
    
    [super dealloc];
}


- (void)viewDidUnload
{
    [self setFromLabel:nil];
    [self setSubjectLabel:nil];
    [self setDateLabel:nil];
    [self setWebView:nil];
    
    if(_isCoupon)
    {
        [self setRedeemButton:nil];
    }
    
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}


- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

- (void) loadView
{
    [super loadView];
    
    BOOL isIPhone5 = FALSE;
    CGFloat extraHeight = 0.0;
    CGFloat screenHeight = [[UIScreen mainScreen] bounds].size.height;
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) {
        if (screenHeight > 480.0f) {
            isIPhone5 = TRUE;
            extraHeight = 44.0;
        } 
    }
    
    // a dirty code goes here
    if(isIPhone5)
    {
        if(_isCoupon)
        {
            CGFloat buttonHeight = 53.0;
            CGFloat height = self.view.bounds.size.height - webView.frame.origin.y - buttonHeight + extraHeight - 16.0;
            [webView setFrame:CGRectMake(webView.frame.origin.x,
                                         webView.frame.origin.y,
                                         webView.frame.size.width,
                                         height)];
            
            CGFloat top = self.view.bounds.size.height - buttonHeight + extraHeight - 8.0;
            _redeemButton = [[YummyZoneUtils createActionButtonWithText:@"Redeem Coupon"
                                                                width:BUTTON_WIDTH 
                                                                 left:BUTTON_X 
                                                                  top:top 
                                                               target:self 
                                                               action:@selector(onRedeem:)] retain];
            [self.view addSubview:_redeemButton];
        }
        else 
        {        
            // enlarge the web view
            CGFloat height = self.view.bounds.size.height - webView.frame.origin.y + extraHeight - 8.0;
            [webView setFrame:CGRectMake(webView.frame.origin.x,
                                         webView.frame.origin.y,
                                         webView.frame.size.width,
                                         height)];
        }
    }
    else
    {
        if(_isCoupon)
        {
            CGFloat top = self.webView.frame.origin.y + self.webView.frame.size.height + 6.0;
            
            _redeemButton = [[YummyZoneUtils createActionButtonWithText:@"Redeem Coupon"
                                                                  width:BUTTON_WIDTH
                                                                   left:BUTTON_X
                                                                    top:top
                                                                 target:self
                                                                 action:@selector(onRedeem:)] retain];
            
            [self.view addSubview:_redeemButton];
        }
        else
        {
            // enlarge the web view
            CGFloat height = self.view.bounds.size.height - webView.frame.origin.y;
            [webView setFrame:CGRectMake(webView.frame.origin.x,
                                         webView.frame.origin.y,
                                         webView.frame.size.width,
                                         height)];
        }
    }
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    if(_isCoupon)
    {
        self.title = @"Coupon";
    }
    else 
    {
        self.title = @"Message";
    }
    
    [fromLabel setText:_from];
    [subjectLabel setText:_subject];
    [dateLabel setText:_date];
    [webView loadHTMLString:_message baseURL:nil];
    
    [YummyZoneUtils loadBackButton:[self navigationItem]];
    
    if (_isCoupon)
    {
        // Disable the redeem button until the location is populated
        self.redeemButton.enabled = NO;
        self.redeemButton.hidden = NO;
    }
}


- (void)viewWillAppear:(BOOL)animated
{
    if (_isCoupon)
    {
        [_locationManager startUpdatingLocation];
    }
    
	[super viewWillAppear:animated];
}


- (void)viewWillDisappear:(BOOL)animated
{
    if (_isCoupon)
    {
        [_locationManager stopUpdatingLocation];
    }
}


- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
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
    
    if(_isCoupon)
    {
        self.redeemButton.enabled = YES;
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

#define HUD_SLEEP_WORKING    600
#define HUD_SLEEP_SUCCESS    600
#define HUD_SLEEP_FAILURE   1500

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
        _progressBar.labelText = @"Redeemed successfully!";
        // sleep awhile msec to let user see this message
        usleep(HUD_SLEEP_SUCCESS * 1000);
        
        [self hideProgressBar];
    }    
}

- (IBAction)onRedeem:(id)sender 
{
    if (!_isCoupon)
    {
        return;
    }
    
    if (_lastUsedLocation == nil)
    {
        return;
    }
    
    _progressBar = [[MBProgressHUD alloc] initWithView:self.view];
    _progressBar.labelText = @"Redeeming...";    
    [self.view addSubview:_progressBar];    
    [_progressBar showWhileExecuting:@selector(redeemProgressive) onTarget:self withObject:nil animated:YES];
}

- (void)redeemProgressive
{
    // send the redeem request synchronously
    // create pool to manage memory
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    // Get a current time to calculate elapsed time
    NSDate* startTime = [NSDate date];
    
    // prepare request
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:
                                    [YummyZoneUrls urlToRedeemCoupon:_messageId 
                                                            location:_lastUsedLocation]];
    
    // add http headers
    NSDictionary *httpHeaders = [[YummyZoneSession singleton] getHttpHeaders];
    for (NSString *key in httpHeaders)
    {
        [request setValue:[httpHeaders objectForKey:key] forHTTPHeaderField:key];
    }
    
    // send request
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
    
    // process result
    if (error) 
    {
        // request failed
        NSLog(@"%@", error);
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                        message:[error description]
                                                       delegate:self 
                                              cancelButtonTitle:@"Close" 
                                              otherButtonTitles:nil];
        [self hideProgressBar];
        [alert show];
        [alert release];
    }
    else
    {
        NSDictionary *dict = [NSDictionary dictionaryWithContentsOfData:data];
        if ([YummyZoneHelper webRequestSucceeded:dict])
        {
            // request succeeded and server returned good result
            // delete the coupon from the view
            if (_delegate != nil)
            {
                [_delegate onMessageDeleted:_messageId];
            }
            
            // go back to restaurant dashboard
            [self showSuccessAndHideProgressBar];
            [self.navigationController popViewControllerAnimated:YES];
        }
        else
        {
            NSString *serverMessage = [YummyZoneHelper getOperationErrorMessage:dict];
            if (serverMessage != nil)
            {
                // request succeeded but server returned a logical error
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                                message:[NSString stringWithFormat:@"Redeem Failed: %@", serverMessage]
                                                               delegate:self 
                                                      cancelButtonTitle:@"Close" 
                                                      otherButtonTitles:nil];
                [self hideProgressBar];
                [alert show];
                [alert release];
            }
            else
            {
                // request succeeded but server returned an unknown error
                UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" 
                                                                message:@"Redeem Failed. Server returned an unknown error."
                                                               delegate:self 
                                                      cancelButtonTitle:@"Close" 
                                                      otherButtonTitles:nil];
                [self hideProgressBar];
                [alert show];
                [alert release];
            }
        }
    }
    
    // release memory
    [pool release];
}

@end
