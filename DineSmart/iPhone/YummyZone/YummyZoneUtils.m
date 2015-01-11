//
//  YummyZoneUtils.m
//  YummyZone
//
//  Created by Baris Taze on 2/23/12.
//  Copyright 2012 __MyCompanyName__. All rights reserved.
//
//  See: http://sebastiancelis.com/2009/12/21/adding-background-image-uinavigationbar/
//

#import "YummyZoneUtils.h"
#import "YummyZoneAppDelegate.h"

@implementation YummyZoneUtils

+ (void)changeBkgImgOfNavBar:(UINavigationController *)navController imageIndex:(NSInteger)imageIndex
{
    UINavigationBar *navBar = [navController navigationBar];
    
    NSArray* _bkgImages = [NSArray arrayWithObjects:
                            @"navBarBkg.png",
                            @"dashNavbarBkg.png",
                            nil];
    
    // If condition needs to be re-opened when compiling against iOS-5
    if ([navBar respondsToSelector:@selector(setBackgroundImage:forBarMetrics:)])
    {
        [navBar setBackgroundImage:[UIImage imageNamed:[_bkgImages objectAtIndex:imageIndex]] 
                     forBarMetrics:UIBarMetricsDefault];
    }
    else
    {
	    // IOS-4 
        UIImageView *imageView = (UIImageView *)[navBar viewWithTag:kSCNavBarImageTag];
        if(imageView != nil)
        {
            [imageView removeFromSuperview];
            imageView = nil;
        }
        
        imageView = [[UIImageView alloc] initWithImage:
                    [UIImage imageNamed:[_bkgImages objectAtIndex:imageIndex]]];
		imageView.contentMode = UIViewContentModeLeft;
        [imageView setTag:kSCNavBarImageTag];
        [navBar insertSubview:imageView atIndex:0];
        [imageView release];
    }
}

+ (void)loadBackButton:(UINavigationItem *)navigationItem
{
	UIImage* backButtonImage = [UIImage imageNamed:@"backBtn.png"];
	UIImage* backButtonHighlightImage = [UIImage imageNamed:@"backBtn_press.png"];
	
	// Create a custom button
	UIButton* backButton = [UIButton buttonWithType:UIButtonTypeCustom];
	
	// Make the button as high as the passed in image
	backButton.frame = CGRectMake(0, 0, backButtonImage.size.width, backButtonImage.size.height);
	
	// Set the stretchable images as the background for the button
	[backButton setBackgroundImage:backButtonImage forState:UIControlStateNormal];
	[backButton setBackgroundImage:backButtonHighlightImage forState:UIControlStateHighlighted];
	[backButton setBackgroundImage:backButtonHighlightImage forState:UIControlStateSelected];
	
	// Add an action for going back
	[backButton addTarget:self action:@selector(goBack:) forControlEvents:UIControlEventTouchUpInside];
	
	// add it to the navigation control
	navigationItem.leftBarButtonItem = [[[UIBarButtonItem alloc] initWithCustomView:backButton] autorelease];
}

+ (UIButton*)createNavBarButtonWithText:(NSString*)buttonText 
                                  width:(CGFloat)buttonWidth
                                 target:(id)buttonTarget 
                                 action:(SEL)buttonAction
{
    UIImage* actBtnImg = [UIImage imageNamed:@"navBarActBtn.png"];
    UIImage* actBtnImgSel = [UIImage imageNamed:@"navBarActBtnSel.png"];
    
    UIImage *stretchActButton = [actBtnImg stretchableImageWithLeftCapWidth:5 topCapHeight:0];
    UIImage *stretchActSelButton = [actBtnImgSel stretchableImageWithLeftCapWidth:5 topCapHeight:0];
    
    UIButton* button = [UIButton buttonWithType:UIButtonTypeCustom];
    button.frame = CGRectMake(0.0, 0.0, buttonWidth, actBtnImg.size.height);
    button.titleLabel.font = [UIFont boldSystemFontOfSize:[UIFont smallSystemFontSize]];
    button.titleLabel.textColor = [UIColor whiteColor];
    button.titleLabel.shadowOffset = CGSizeMake(0,-1);
    button.titleLabel.shadowColor = [UIColor darkGrayColor];
    
    [button setTitle:buttonText forState:UIControlStateNormal];
    [button setBackgroundImage:stretchActButton forState:UIControlStateNormal];
    [button setBackgroundImage:stretchActSelButton forState:UIControlStateHighlighted];
    [button setBackgroundImage:stretchActSelButton forState:UIControlStateSelected];
    button.adjustsImageWhenHighlighted = NO;
    
    [button addTarget:buttonTarget action:buttonAction forControlEvents:UIControlEventTouchUpInside];
    
    return button;
}


+ (UIButton*) createActionButtonWithText:(NSString*)buttonText
                                   width:(CGFloat)buttonWidth 
                                    left:(CGFloat)buttonLeft
                                     top:(CGFloat)buttonTop
                                  target:(id)buttonTarget 
                                  action:(SEL)buttonAction
{
    UIImage* actBtnImg = [UIImage imageNamed:@"actionBtnStretch.png"];
    UIImage* actBtnImgSel = [UIImage imageNamed:@"actionBtnStretchSelected.png"];
    
    UIImage *stretchActButton = [actBtnImg stretchableImageWithLeftCapWidth:14 topCapHeight:0];
    UIImage *stretchActSelButton = [actBtnImgSel stretchableImageWithLeftCapWidth:14 topCapHeight:0];
    
    UIButton *button = [UIButton buttonWithType:UIButtonTypeCustom];
    
    [button setBackgroundImage:stretchActButton forState:UIControlStateNormal];	
    [button setBackgroundImage:stretchActSelButton forState:UIControlStateHighlighted];
    
    [button setTitle:buttonText forState:UIControlStateNormal];
    button.frame = CGRectMake(buttonLeft, buttonTop, buttonWidth, actBtnImg.size.height);
    [button addTarget:buttonTarget action:buttonAction forControlEvents:UIControlEventTouchUpInside];
    
    return button;

}

+ (void)goBack:(id)sender
{	
	UINavigationController * navCtrl = [(YummyZoneAppDelegate*)[[UIApplication sharedApplication] delegate] navigationController];
	[navCtrl popViewControllerAnimated:YES];
}

@end
