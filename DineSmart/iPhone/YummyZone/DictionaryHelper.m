//
//  DictionaryHelper.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/15/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "DictionaryHelper.h"

@implementation DictionaryHelper

+ (NSData*)getDictionaryContentsForHttpPost:(NSDictionary*)dict
{
    NSString *error;
    NSData *originalData = [NSPropertyListSerialization dataFromPropertyList:dict format:NSPropertyListXMLFormat_v1_0 errorDescription:&error];
    if(originalData)
    {
        NSString *originalString = [[NSString alloc] initWithData:originalData encoding:NSUTF8StringEncoding];
        if (originalString)
        {
            NSString *modifiedString = [originalString stringByReplacingOccurrencesOfString:@"<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" withString:@"" options:NSCaseInsensitiveSearch range:NSMakeRange(0, [originalString length])];
            if (modifiedString)
            {
                return [modifiedString dataUsingEncoding:NSUTF8StringEncoding];
            }
        }
    }

    return nil;
}

+ (NSString*)getDictionaryContentsAsText:(NSDictionary*)dict
{
    NSString *error;
    NSData *originalData = [NSPropertyListSerialization dataFromPropertyList:dict format:NSPropertyListXMLFormat_v1_0 errorDescription:&error];
    if(originalData)
    {
        NSString *originalString = [[NSString alloc] initWithData:originalData encoding:NSUTF8StringEncoding];
        if (originalString)
        {
            NSString *modifiedString = [originalString stringByReplacingOccurrencesOfString:@"<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" withString:@"" options:NSCaseInsensitiveSearch range:NSMakeRange(0, [originalString length])];
            if (modifiedString)
            {
                return modifiedString;
            }
        }
    }
	
    return nil;
}

@end
