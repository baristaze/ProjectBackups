//
//  UINavigationBar+SCBackgroundImage.m
//  YummyZone
//
//  Created by Baris Taze on 2/23/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//
//  See: http://sebastiancelis.com/2009/12/21/adding-background-image-uinavigationbar/
//

#import "UINavigationBar+SCBackgroundImage.h"
#import "YummyZoneUtils.h"

@implementation UINavigationBar (SCBackgroundImage)

- (void)scInsertSubview:(UIView *)view atIndex:(NSInteger)index
{
    [self scInsertSubview:view atIndex:index];
	
    UIView *backgroundImageView = [self viewWithTag:kSCNavBarImageTag];
    if (backgroundImageView != nil)
    {
        [self scSendSubviewToBack:backgroundImageView];
    }
}

- (void)scSendSubviewToBack:(UIView *)view
{
    [self scSendSubviewToBack:view];
	
    UIView *backgroundImageView = [self viewWithTag:kSCNavBarImageTag];
    if (backgroundImageView != nil)
    {
        [self scSendSubviewToBack:backgroundImageView];
    }
}

@end
