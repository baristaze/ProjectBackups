//
//  FBHelper.h
//  YummyZone
//
//  Created by Baris Taze on 10/9/12.
//
//

#import <UIKit/UIKit.h>
#import <FacebookSDK/FacebookSDK.h>

@protocol FBActionDelegate

- (void)refreshFBRelatedUI;
- (void)onCompletedFBAction:(BOOL)isByUser result:(BOOL)isSuccess;

@end

@interface FBHelper : NSObject
{
    BOOL _triggeredByUser;
    id<FBActionDelegate> _fbActionDelegate;
}

// extern NSString *const FBSessionStateChangedNotification;

@property (strong, nonatomic) FBRequestConnection *requestConnection;

+ (FBHelper*)singleton;
+ (void) logFBSessionState:(FBSessionState)state;

- (BOOL)openFacebookSessionWithAllowLoginUI:(BOOL)allowLoginUI delegate:(id)actionDelegate;

- (void) closeSession;
- (void) closeAndClearTokenInformation;
- (void) handleDidBecomeActive;
- (BOOL) handleOpenUrl:(NSURL*)url;
- (void) postOnWall;

@end

