//
//  NSDictionary+Helpers.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "NSDictionary+Helpers.h"


@implementation NSDictionary (Helpers)

+ (NSDictionary *)dictionaryWithContentsOfData:(NSData *)data
{
    NSString *errorString = nil;
    
	// uses toll-free bridging for data into CFDataRef and CFPropertyList into NSDictionary
	CFPropertyListRef plist =  CFPropertyListCreateFromXMLData(kCFAllocatorDefault, (CFDataRef)data,
															   kCFPropertyListImmutable,
															   (CFStringRef *)&errorString);
    
    if (errorString != nil)
    {
        NSLog(@"dictionaryWithContentsOfData failed: Error: %@", errorString);
        [errorString release];
        errorString = nil;
    }
    
    if (plist == nil)
    {
        return nil;
    }
    
	// we check if it is the correct type and only return it if it is
	if ([(id)plist isKindOfClass:[NSDictionary class]])
	{
		return [(NSDictionary *)plist autorelease];
	}
	else
	{
		// clean up ref
		CFRelease(plist);
		return nil;
	}
}

@end
