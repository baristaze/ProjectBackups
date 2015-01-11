//
//  NSMutableArray+WeakReferences.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "NSMutableArray+WeakReferences.h"


@implementation NSMutableArray (WeakReferences)

+ (id)createMutableArrayWithWeakReferences 
{
    CFArrayCallBacks callbacks = {0, NULL, NULL, CFCopyDescription, CFEqual};
    // We create a weak reference array
    return (id)(CFArrayCreateMutable(0, 0, &callbacks));
}

@end
