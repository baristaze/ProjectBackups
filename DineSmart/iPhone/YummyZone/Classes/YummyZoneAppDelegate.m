//
//  YummyZoneAppDelegate.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright __MyCompanyName__ 2011. All rights reserved.
//

#import "YummyZoneAppDelegate.h"
#import "YummyZoneUtils.h"
#import "StartupViewController.h"
#import "SigninViewController.h"
#import "ImageCache.h"
#import "DictionaryCache.h"
#import "YummyZoneSession.h"
#import "YummyZoneUrls.h"
//#import "FBHelper.h"

#define THUMBNAIL_WIDTH					64.0
#define THUMBNAIL_HEIGHT				64.0

@implementation YummyZoneAppDelegate

@synthesize window;
@synthesize navigationController;

- (id)init
{
    self = [super init];
	if (self)
	{
        _thumbnailCache = [[ImageCache alloc] initWithImageWidth:THUMBNAIL_WIDTH imageHeight:THUMBNAIL_HEIGHT shrinkOnly:NO];
	}
	return self;
}


- (void)dealloc 
{
    [_thumbnailCache release];
	[navigationController release];
	[window release];
    
	[super dealloc];
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions 
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    
    // Initialize singletons here
    [DictionaryCache singleton];
    
	StartupViewController *startupView = [[StartupViewController alloc] init];
	navigationController = [[UINavigationController alloc] initWithRootViewController:startupView];
	[[navigationController navigationBar] setTintColor:[UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:0.7]];
    
    [YummyZoneUtils changeBkgImgOfNavBar:navigationController imageIndex:0];
	[startupView release];
    
	navigationController.toolbarHidden = YES;
	
	[window addSubview:[navigationController view]];
    [window makeKeyAndVisible];
    
    // ask for push notifications
    [[UIApplication sharedApplication] registerForRemoteNotificationTypes:
     (UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeSound | UIRemoteNotificationTypeAlert)];
    
    return YES;
}

- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken 
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    const unsigned *tokenBytes = [deviceToken bytes];
    NSString *hexToken = [NSString stringWithFormat:@"%08x%08x%08x%08x%08x%08x%08x%08x",
                          ntohl(tokenBytes[0]), ntohl(tokenBytes[1]), ntohl(tokenBytes[2]),
                          ntohl(tokenBytes[3]), ntohl(tokenBytes[4]), ntohl(tokenBytes[5]),
                          ntohl(tokenBytes[6]), ntohl(tokenBytes[7])];
    
    NSLog(@"My token is: %@", hexToken);
    
    [[YummyZoneSession singleton] queueAsyncRequest:[YummyZoneUrls urlToNotify:hexToken] useDefaultHeaders:YES];
    
    [pool release];
}

- (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error
{
    NSLog(@"Failed to get device token: %@", error);
}

+ (void) sendEmailTo:(NSString *)to withSubject:(NSString *)subject
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSString *mailString = [NSString stringWithFormat:@"mailto:?to=%@&subject=%@",
							[to stringByAddingPercentEscapesUsingEncoding:NSASCIIStringEncoding],
							[subject stringByAddingPercentEscapesUsingEncoding:NSASCIIStringEncoding]];
	
	[[UIApplication sharedApplication] openURL:[NSURL URLWithString:mailString]];
    
    [pool release];
}

- (void)applicationWillResignActive:(UIApplication *)application {
    /*
     Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
     Use this method to pause ongoing tasks, disable timers, and throttle down OpenGL ES frame rates. Games should use this method to pause the game.
     */
}


- (void)applicationDidEnterBackground:(UIApplication *)application {
    /*
     Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later. 
     If your application supports background execution, called instead of applicationWillTerminate: when the user quits.
     */
}


- (void)applicationWillEnterForeground:(UIApplication *)application {
    /*
     Called as part of  transition from the background to the inactive state: here you can undo many of the changes made on entering the background.
     */
}


- (void)applicationDidBecomeActive:(UIApplication *)application {
    /*
     Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
     */
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    
    // We need to properly handle activation of the application with regards to SSO
    // (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
    // [[FBHelper singleton] handleDidBecomeActive];
}


- (void)applicationWillTerminate:(UIApplication *)application {
    /*
     Called when the application is about to terminate.
     See also applicationDidEnterBackground:.
     */
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    
    // don't close... this part is not necessary
    // [[FBHelper singleton] closeSession];
}


- (void)applicationDidReceiveMemoryWarning:(UIApplication *)application {
    /*
     Free up as much memory as possible by purging cached data objects that can be recreated (or reloaded from disk) later.
     */
}

/*
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication
         annotation:(id)annotation
{
    // If we have a valid session at the time of openURL call, we handle
    // Facebook transitions by passing the url argument to handleOpenURL
    // attempt to extract a token from the url
    return [[FBHelper singleton] handleOpenUrl:url];
}
*/

- (ImageCache*)getThumbnailCache
{
    return _thumbnailCache;
}

/*- (void)processSettingsBundle
{
	static NSString *kUsername			= @"username";
	static NSString *kPassword			= @"password";
	static NSString *kCookie			= @"cookie";
	static NSString *kIsAutogenerated	= @"isAutogenerated";
    
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    NSDictionary *allSettings = [[NSUserDefaults standardUserDefaults] dictionaryRepresentation];
    if ([allSettings objectForKey:kIsAutogenerated] == nil)
    {
        NSString* username = [[(NSString*)CFUUIDCreateString(NULL, CFUUIDCreate(NULL)) autorelease] uppercaseString];
        NSString* password = [[(NSString*)CFUUIDCreateString(NULL, CFUUIDCreate(NULL)) autorelease] uppercaseString];
        
        // since no default values have been set (i.e. no preferences file created), create it here
        NSDictionary *appDefaults =  [NSDictionary dictionaryWithObjectsAndKeys:
                                      username, kUsername,
                                      password, kPassword,
                                      @"", kCookie,
                                      [NSNumber numberWithBool:YES], kIsAutogenerated,
                                      nil];
        
        [[NSUserDefaults standardUserDefaults] registerDefaults:appDefaults];
        [[NSUserDefaults standardUserDefaults] synchronize];
    }
	
	[pool release];
}
*/

@end

