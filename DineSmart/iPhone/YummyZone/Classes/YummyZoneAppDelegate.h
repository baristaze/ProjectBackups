//
//  YummyZoneAppDelegate.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright __MyCompanyName__ 2011. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageCache.h"

@interface YummyZoneAppDelegate : NSObject <UIApplicationDelegate> {
    
	IBOutlet UIWindow *window;
    IBOutlet UINavigationController *navigationController;
    ImageCache * _thumbnailCache;
}

@property (nonatomic, retain) UIWindow *window;
@property (nonatomic, retain) UINavigationController *navigationController;

+ (void) sendEmailTo:(NSString*)to withSubject:(NSString *)subject;
- (ImageCache*)getThumbnailCache;

@end

