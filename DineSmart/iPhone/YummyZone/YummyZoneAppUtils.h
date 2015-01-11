//
//  YummyZoneAppUtils.h
//  YummyZone
//
//  Created by Baris Taze on 2/23/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//
//  See: http://sebastiancelis.com/2009/12/21/adding-background-image-uinavigationbar/
//

#import <Foundation/Foundation.h>


@interface YummyZoneAppUtils : NSObject {

}

+ (void)swizzleSelector:(SEL)orig ofClass:(Class)c withSelector:(SEL)new;

@end
