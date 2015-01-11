//
//  UIImage+Resize.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface UIImage (Resize)

+ (id)imageUsingImage:(UIImage*)image width:(CGFloat)width height:(CGFloat)height onlyShrink:(BOOL)onlyShrink;

@end
