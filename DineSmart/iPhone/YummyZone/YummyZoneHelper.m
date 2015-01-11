//
//  YummyZoneHelper.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/15/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "YummyZoneHelper.h"
#import "KeyConstants.h"

@implementation YummyZoneHelper

+ (bool)webRequestSucceeded:(NSDictionary*)dict
{
    NSDictionary *operationResult = [dict objectForKey:kKeyOperationResult];
    if (operationResult == nil)
    {
		NSNumber *errorCode = [dict objectForKey:kKeyErrorCode];
		if (errorCode == nil)
		{
			NSLog(@"webRequestSucceeded: Dictionary does not contain ErrorCode: %@", dict);
			return false;
		}
		
		BOOL succeeded = ([errorCode intValue] == 0);
		if (!succeeded)
		{
			NSLog(@"webRequestSucceeded: Error code (%d) is non-zero: %@", [errorCode intValue], dict);
		}
		
		return succeeded;
    }
	else
	{
		NSNumber *errorCode = [operationResult objectForKey:kKeyErrorCode];
		if (errorCode == nil)
		{
			NSLog(@"webRequestSucceeded: Dictionary does not contain ErrorCode: %@", dict);
			return false;
		}
		
		BOOL succeeded = ([errorCode intValue] == 0);
		if (!succeeded)
		{
			NSLog(@"webRequestSucceeded: Error code (%d) is non-zero: %@", [errorCode intValue], dict);
		}
		
		return succeeded;
	}    
}

+ (NSString*)getOperationErrorMessage:(NSDictionary*)dict
{
    NSDictionary *operationResult = [dict objectForKey:kKeyOperationResult];
    if (operationResult == nil)
    {
        NSString *errorMessage = [dict objectForKey:kKeyErrorMessage];
		if (errorMessage == nil)
		{
			return nil;
		}
		
		return [[errorMessage copy] autorelease];
    }
	else
	{
		NSString *errorMessage = [operationResult objectForKey:kKeyErrorMessage];
		if (errorMessage == nil)
		{
			return nil;
		}
		
		return [[errorMessage copy] autorelease];
	}
}

@end
