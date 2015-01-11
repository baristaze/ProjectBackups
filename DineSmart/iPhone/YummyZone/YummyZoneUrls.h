//
//  YummyZoneUrls.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>


@interface YummyZoneUrls : NSObject 
{
}

+ (NSURL*)urlForNearbyRestaurants:(CLLocation*)location;
+ (NSURL*)urlForRestaurantMenu:(NSString*)restaurantId;
+ (NSURL*)urlForRequestedFeedback:(NSString*)restaurantId;
+ (NSURL*)urlForInbox:(NSString*)hintForNextPage restaurantId:(NSString*)restaurantId;
+ (NSURL*)urlForCoupons:(NSString*)hintForNextPage restaurantId:(NSString*)restaurantId;
+ (NSURL*)urlForFavorites:(NSString*)restaurantId;
+ (NSURL*)urlForHistory:(NSString*)restaurantId;
+ (NSURL*)urlToDeleteMessage:(NSString*)messageId;
+ (NSURL*)urlToDeleteCoupon:(NSString*)couponId;
+ (NSURL*)urlToMarkMessageRead:(NSString*)messageId;
+ (NSURL*)urlToMarkCouponRead:(NSString*)couponId;
+ (NSURL*)urlToRedeemCoupon:(NSString*)couponId location:(CLLocation*)location;
+ (NSURL*)urlToNotify:(NSString*)deviceToken;
+ (NSURL*)urlForSendFeedback;
+ (NSURL*)urlForSignUp1;
+ (NSURL*)urlForSignIn1;
+ (NSURL*)urlForSignUp2;
+ (NSURL*)urlForSignIn2;
+ (NSURL*)urlForSettings:(NSString*)name value:(NSString*)value;

@end
