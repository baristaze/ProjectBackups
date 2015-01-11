//
//  YummyZoneUrls.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "YummyZoneUrls.h"


@implementation YummyZoneUrls

+ (NSURL*)urlForNearbyRestaurants:(CLLocation*)location
{
    // BUGBUG: TEST ONLY
    // return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Venues?lat=47.686494448883195&long=-122.1293368935585"];
    
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Venues?lat=%f&long=%f", location.coordinate.latitude, location.coordinate.longitude];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlForRestaurantMenu:(NSString*)restaurantId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Menu?venue=%@", restaurantId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlForRequestedFeedback:(NSString*)restaurantId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Survey?venue=%@", restaurantId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlForInbox:(NSString*)hintForNextPage restaurantId:(NSString*)restaurantId
{
    NSString *urlString;
    if (hintForNextPage != nil && [hintForNextPage length] > 0)
    {
        urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Messages?pageHint=%@", hintForNextPage];
    }
    else
    {
        urlString = @"http://isvc.dinesmart365.com/plist/Messages?pageHint=0";
    }
    
    if (restaurantId != nil)
    {
        return [NSURL URLWithString:[NSString stringWithFormat:@"%@&venue=%@", urlString, restaurantId]];
    }
    else
    {
        return [NSURL URLWithString:urlString];
    }
}

+ (NSURL*)urlForCoupons:(NSString*)hintForNextPage restaurantId:(NSString*)restaurantId
{
    NSString *urlString;
    if (hintForNextPage != nil && [hintForNextPage length] > 0)
    {
         
        urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Coupons?pageHint=%@", hintForNextPage];
    }
    else
    {
        urlString = @"http://isvc.dinesmart365.com/plist/Coupons?pageHint=0";
    }

    if (restaurantId != nil)
    {
        return [NSURL URLWithString:[NSString stringWithFormat:@"%@&venue=%@", urlString, restaurantId]];
    }
    else
    {
        return [NSURL URLWithString:urlString];
    }
}

+ (NSURL*)urlForFavorites:(NSString*)restaurantId
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Favorites"];
}

+ (NSURL*)urlForHistory:(NSString *)restaurantId
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/History"];
}

+ (NSURL*)urlToDeleteMessage:(NSString*)messageId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/DeleteMessage?msg=%@", messageId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlToDeleteCoupon:(NSString*)couponId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/DeleteCoupon?coupon=%@", couponId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlToMarkMessageRead:(NSString*)messageId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/MarkMessageAsRead?msg=%@", messageId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlToNotify:(NSString*)deviceToken
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Notify?device=%@", deviceToken];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlToMarkCouponRead:(NSString*)couponId
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/MarkCouponAsRead?coupon=%@", couponId];
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlToRedeemCoupon:(NSString*)couponId location:(CLLocation*)location
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/RedeemCoupon?coupon=%@&lat=%f&long=%f", couponId, location.coordinate.latitude, location.coordinate.longitude];
    
    // BUGBUG TEST ONLY
    /*
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/RedeemCoupon?coupon=%@&lat=47.686494448883195&long=-122.1293368935585", couponId];
    */
    
    return [NSURL URLWithString:urlString];
}

+ (NSURL*)urlForSendFeedback
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Feedback"];
}

+ (NSURL*)urlForSignUp1
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Signup"];
}

+ (NSURL*)urlForSignIn1
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Signin"];
}

+ (NSURL*)urlForSignUp2
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Signup2"];
}

+ (NSURL*)urlForSignIn2
{
    return [NSURL URLWithString:@"http://isvc.dinesmart365.com/plist/Signin2"];
}

+ (NSURL*)urlForSettings:(NSString*)name value:(NSString*)value
{
    NSString *urlString = [NSString stringWithFormat:@"http://isvc.dinesmart365.com/plist/Settings?name=%@&value=%@", name, value];
    
    return [NSURL URLWithString:urlString];
}

@end
