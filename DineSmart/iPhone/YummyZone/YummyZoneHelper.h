//
//  YummyZoneHelper.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/15/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface YummyZoneHelper : NSObject
{
}

+ (bool)webRequestSucceeded:(NSDictionary*)dict;
+ (NSString*)getOperationErrorMessage:(NSDictionary*)dict;

@end
