//
//  FBHelper.m
//  YummyZone
//
//  Created by Baris Taze on 10/9/12.
//
//

#import "FBHelper.h"
#import "KeyConstants.h"
#import "YummyZoneSession.h"
#import "YummyZoneUrls.h"
#import "DictionaryHelper.h"

@implementation FBHelper

// NSString *const FBSessionStateChangedNotification = @"com.DineSmart365.DineSmart:FBSessionStateChangedNotification";

static FBHelper *_fbHelper;

// Initialize the singleton instance if needed and return
+ (FBHelper*)singleton
{
	// @synchronized(self)
	{
		if (!_fbHelper)
        {
			_fbHelper = [[FBHelper alloc] init];
        }
		
		return _fbHelper;
	}
}

+ (id)alloc
{
	//	@synchronized(self)
	{
		NSAssert(_fbHelper == nil, @"Attempted to allocate a second instance of a FBHelper singleton.");
		_fbHelper = [super alloc];
		return _fbHelper;
	}
}

+ (id)copy
{
	//	@synchronized(self)
	{
		NSAssert(_fbHelper == nil, @"Attempted to copy the FBHelper singleton.");
		return _fbHelper;
	}
}

+ (void)initialize
{
    static BOOL initialized = NO;
    if (!initialized)
	{
        initialized = YES;
    }
}

- (id)init
{
    self = [super init];
	if (self)
	{
        _triggeredByUser = FALSE;
        _fbActionDelegate = nil;
    }
	return self;
}

- (void)dealloc
{
	[super dealloc];
}

- (void) handleDidBecomeActive
{
    [FBSession.activeSession handleDidBecomeActive];
}

- (void) closeSession
{
    [FBSession.activeSession close];
}

- (BOOL) handleOpenUrl:(NSURL*)url
{
    return [FBSession.activeSession handleOpenURL:url];
}

+ (void) logFBSessionState:(FBSessionState)state
{
    if(state == FBSessionStateCreated)
    {
        NSLog(@"FBSessionState: FBSessionStateCreated");
    }
    else if(state == FBSessionStateCreatedTokenLoaded)
    {
        NSLog(@"FBSessionState: FBSessionStateCreatedTokenLoaded");
    }
    else if(state == FBSessionStateCreatedOpening)
    {
        NSLog(@"FBSessionState: FBSessionStateCreatedOpening");
    }
    else if(state == FBSessionStateOpen)
    {
        NSLog(@"FBSessionState: FBSessionStateOpen");
    }
    else if(state == FBSessionStateOpenTokenExtended)
    {
        NSLog(@"FBSessionState: FBSessionStateOpenTokenExtended");
    }
    else if(state == FBSessionStateClosedLoginFailed)
    {
        NSLog(@"FBSessionState: FBSessionStateClosedLoginFailed");
    }
    else if(state == FBSessionStateClosed)
    {
        NSLog(@"FBSessionState: FBSessionStateClosed");
    }
    else
    {
        NSLog(@"FBSessionState: Unknown");
    }   
}

- (void) closeAndClearTokenInformation
{
    NSLog(@"Facebook session is being closed.");
    [FBSession.activeSession closeAndClearTokenInformation];
}

/*
 * Opens a Facebook session and optionally shows the login UX.
 */
- (BOOL)openFacebookSessionWithAllowLoginUI:(BOOL)allowLoginUI
                                  delegate:(id)actionDelegate
{
    _fbActionDelegate = nil;
    _fbActionDelegate = actionDelegate;
    _triggeredByUser = allowLoginUI;
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    /*
     NSArray *permissions = [[NSArray alloc] initWithObjects:
     @"email",
     @"user_hometown",
     @"user_birthday",
     nil];
    */
    
    // NSArray *permissions = nil;
    
    NSArray *permissions = [[NSArray alloc] initWithObjects:
                                    // @"publish_checkins",
                                    @"publish_stream",
                                    nil];
    
    NSLog(@"Calling FB:openActiveSession");
    
    // Returns true if the session was opened synchronously without presenting UI to the user
    BOOL var = [FBSession openActiveSessionWithPublishPermissions:permissions
                                                  defaultAudience:FBSessionDefaultAudienceFriends
                                                     allowLoginUI:allowLoginUI
                                                completionHandler:^(FBSession *session, FBSessionState state, NSError *error)
                          {
                              [self openSessionActionCompleted:session
                                                         state:state
                                                         error:error];
                          }];
    
    NSLog(@"Returned %@", var ? @"TRUE: from cache synchronously" : @"FALSE: asynchronously with User interaction");
    
    /*
    if(var) // return type is not dependable!!!!
    {
        // async method won't be called since we got the cookie synchronously from the cache.
        // Call follow-up method explicitly
        NSLog(@"Calling completion explicitly...");
        
        [self openSessionActionCompleted:FBSession.activeSession
                                   state:FBSessionStateOpen
                                   error:nil];
    }
    */
    
    if(_fbActionDelegate != nil)
    {
        [_fbActionDelegate refreshFBRelatedUI];
    }
    
    [pool release];
    return var;
}

- (void) callDelegate:(BOOL)isSuccess
{
    if(_fbActionDelegate != nil)
    {
        [_fbActionDelegate onCompletedFBAction:_triggeredByUser result:isSuccess];
    }
}

- (void)openSessionActionCompleted:(FBSession *)session
                             state:(FBSessionState) state
                             error:(NSError *)error
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    NSLog(@"FB:openSessionActionCompleted");
    
    if((state == FBSessionStateOpen || state == FBSessionStateOpenTokenExtended) && !error)
    {
        // We have a valid session
        NSLog(@"Facebook session has found for the user. Checking extended permissions");
        
        // check permissions
        //[self checkExtendedPermission];
        [self callDelegate:TRUE];
    }
    else if(state == FBSessionStateClosed || state == FBSessionStateClosedLoginFailed)
    {
        NSLog(@"Facebook session closed or login failed.");        
        [self callDelegate:FALSE];
    }
    else
    {
        NSLog(@"interim FB Session State!!!");
        
        if(_fbActionDelegate != nil)
        {
            [_fbActionDelegate refreshFBRelatedUI];
        }
    }
    
    // [[NSNotificationCenter defaultCenter] postNotificationName:FBSessionStateChangedNotification object:session];
    
    [pool release];
}

- (void) checkExtendedPermission
{
    NSLog(@"Calling checkExtendedPermission");
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    // create the connection object
    FBRequestConnection *newConnection = [[FBRequestConnection alloc] init];
    
    // create a handler block to handle the results of the request for permissions
    FBRequestHandler handler = ^(FBRequestConnection *connection, id result, NSError *error)
    {
        [self checkExtendedPermissionCompleted:connection
                               result:result
                                error:error];
    };
    
    // create the request object, using the 'me/permissions' as the graph path
    // as an alternative the request* static methods of the FBRequest class could
    // be used to fetch common requests, such as /me and /me/friends
    FBRequest *request = [[FBRequest alloc] initWithSession:FBSession.activeSession graphPath:@"me/permissions"];
    
    // add the request to the connection object, if more than one request is added
    // the connection object will compose the requests as a batch request; whether or
    // not the request is a batch or a singleton, the handler behavior is the same,
    // allowing the application to be dynamic in regards to whether a single or multiple
    // requests are occuring
    [newConnection addRequest:request completionHandler:handler];
    
    // if there's an outstanding connection, just cancel
    [self.requestConnection cancel];
    
    // keep track of our connection, and start it
    self.requestConnection = newConnection;
    [newConnection start];
    
    [pool release];
}

// Report any results.  Invoked once for each request we make.
- (void)checkExtendedPermissionCompleted:(FBRequestConnection *)connection
                                   result:(id)result
                                    error:(NSError *)error
{
    // not the completion we were looking for...
    if (self.requestConnection && connection != self.requestConnection)
    {
        NSLog(@"Irrelevant completion (FB extended perm). wait more...");
        return;
    }
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    // clean this up, for posterity
    self.requestConnection = nil;
    
    if (error)
    {
        // error contains details about why the request failed
        NSLog(@"Reading current Facebook permission list failed. See below:");
        NSLog(@"%@", error.localizedDescription);
        
        [self callDelegate:FALSE];
    }
    else
    {
        // result is the json response from a successful request
        NSLog(@"Current Facebook permissions have been retrieved successfully");
        NSDictionary *dictionary = (NSDictionary *)result;
        
        //NSString* permissionsContent = [DictionaryHelper getDictionaryContentsAsText:dictionary];
        //NSLog(@"%@", permissionsContent);
        
        BOOL extensionNeeded = TRUE;
        NSArray *data = [dictionary objectForKey:@"data"];
        if(data != nil)
        {
            // we pull the permission name property out, if there is one
            for (NSDictionary *permGroup in data)
            {
                NSNumber* writePermission = (NSNumber *)[permGroup objectForKey:@"publish_stream"];
                if(writePermission != nil && [writePermission isEqualToNumber:[NSNumber numberWithDouble:1.0]])
                {
                    NSLog(@"Facebook write permission is already granted");
                    extensionNeeded = FALSE;
                    break;
                }
            }
        }
        
        // extend the permissions
        if(extensionNeeded)
        {
            @try
            {
                NSLog(@"Requesting extendend permission");
                [self requestExtendedFacebookPermissions];
            }
            @catch (NSException *exception)
            {
                NSLog(@"%@", exception);
                
                [self callDelegate:FALSE];
            }
        }
        else
        {
            [self callDelegate:TRUE];
        }
    }
    
    [pool release];
}

- (void)requestExtendedFacebookPermissions
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSArray *extendedPermissions = [[NSArray alloc] initWithObjects:
                                    // @"publish_checkins",
                                    @"publish_stream",
                                    nil];
    
    
    NSLog(@"Calling reauthorizeWithPublishPermissions of Facebook SDK.");
    [FBSession.activeSession reauthorizeWithPublishPermissions:extendedPermissions
                                               defaultAudience:FBSessionDefaultAudienceFriends
                                             completionHandler:^(FBSession *session, NSError *error)
     {
         if (!error)
         {
             // success
             NSLog(@"Extended permissions on Facebook is granted");
             
             [self callDelegate:TRUE];
         }
         else
         {
             NSLog(@"Extended Facebook permissions couldn't be granted. See below:");
             NSLog(@"%@", [[error userInfo] objectForKey:@"com.facebook.sdk:ErrorLoginFailedReason"]);
             
             [self callDelegate:FALSE];
         }
     }];
    
    [pool release];
}

- (void) postOnWall
{
    if(FBSession.activeSession.isOpen)
    {
        [self postOnWall2];
    }
    else
    {
        [FBHelper logFBSessionState:FBSession.activeSession.state];
        
        [FBSession.activeSession openWithCompletionHandler:^(FBSession *session, FBSessionState status, NSError *error)
        {
            [FBHelper logFBSessionState:FBSession.activeSession.state];
            
            if(!error && FBSession.activeSession.isOpen)
            {
                [self postOnWall2];
            }
            else
            {
                [[[UIAlertView alloc] initWithTitle:@"Result"
                                            message:@"Facebook session couldn't be opened!"
                                           delegate:self
                                  cancelButtonTitle:@"OK!"
                                  otherButtonTitles:nil]
                 show];
            }
        }];
    }
}

- (void) postOnWall2
{
    NSString* msg = @"I've tried The Walrus and the Carpenter today and tasted their Grilled Chicken Caesar Salad.";
    msg = [msg stringByAppendingString:@" Here is how my experience was: http://dinesmart365.com/feed/abcdefgh"];
    
    NSString* desc = @"";
    desc = [desc stringByAppendingString:@"Leave feedback, win coupons!"];
    desc = [desc stringByAppendingString:@" View the dishes people liked the most!"];
    desc = [desc stringByAppendingString:@" See what you've ordered in a restaurant before!"];
    desc = [desc stringByAppendingString:@" Share with friends via email or Facebook!"];
    desc = [desc stringByAppendingString:@" Dowload our free IPhone app now!"];
    
    // post on facebook wall
    NSMutableDictionary *postParams =
    [[NSMutableDictionary alloc] initWithObjectsAndKeys:
     @"http://dinesmart365.com/app.html?id=random", @"link",
     @"http://img.dinesmart365.com/logo/1F18AFD7-EA24-4ED5-B0D0-B9A4BE3F691C", @"picture",
     @"The Walrus and the Carpenter", @"name",
     @"We listen to you via Dine Smart 365!", @"caption",
     desc, @"description",
     msg, @"message",
     nil];
   
    [FBRequestConnection
     startWithGraphPath:@"me/feed"
     parameters:postParams
     HTTPMethod:@"POST"
     completionHandler:^(FBRequestConnection *connection, id result, NSError *error)
     {
         if (error)
         {
             NSString *alertText = [NSString stringWithFormat:@"error: domain = %@, code = %d", error.domain, error.code];
             
             // Show the result in an alert
             [[[UIAlertView alloc] initWithTitle:@"Result"
                                         message:alertText
                                        delegate:self
                               cancelButtonTitle:@"OK!"
                               otherButtonTitles:nil]
              show];
         }
         else
         {
             NSString *alertText = [NSString stringWithFormat:@"Posted action, id: %@", [result objectForKey:@"id"]];
             
             // Show the result in an alert
             [[[UIAlertView alloc] initWithTitle:@"Result"
                                         message:alertText
                                        delegate:self
                               cancelButtonTitle:@"OK!"
                               otherButtonTitles:nil]
              show];
         }
     }];
}

@end