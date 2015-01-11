//
//  YummyZoneUtils.h
//  YummyZone
//
//  Created by Baris Taze on 2/23/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//
//  See: http://sebastiancelis.com/2009/12/21/adding-background-image-uinavigationbar/
//

#import <UIKit/UIKit.h>

#define kSCNavBarImageTag 6183746
#define kSCNavBarColor [UIColor colorWithRed:0.055 green:0.063 blue:0.184 alpha:0.9]
@interface YummyZoneUtils : NSObject {

}

+ (void)changeBkgImgOfNavBar:(UINavigationController *)navController imageIndex:(NSInteger)imageIndex;
+ (void)loadBackButton:(UINavigationItem *)navigationItem;
+ (void)goBack:(id)sender;

+ (UIButton*)createNavBarButtonWithText:(NSString*)buttonText 
                                  width:(CGFloat)buttonWidth 
                                 target:(id)buttonTarget 
                                 action:(SEL)buttonAction;

+ (UIButton*) createActionButtonWithText:(NSString*)buttonText 
                                   width:(CGFloat)buttonWidth 
                                    left:(CGFloat)buttonLeft
                                     top:(CGFloat)buttonTop
                                  target:(id)buttonTarget 
                                  action:(SEL)buttonAction;

@end
