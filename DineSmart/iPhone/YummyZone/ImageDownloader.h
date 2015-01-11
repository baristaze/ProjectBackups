//
//  ImageDownloader.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "DataDownloader.h"

@protocol ImageDownloaderDelegate 

- (void)imageDidLoad:(UIImage*)image key:(NSObject*)key;
- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key;

@end

@interface ImageDownloader : DataDownloader 
{
	UIImage *_image;
    NSString * _error;
    id <ImageDownloaderDelegate> _delegate;
}

- (id)initWithUrl:(NSURL*)url delegate:(id <ImageDownloaderDelegate>)delegate key:(NSObject*)key;
- (void)setDelegate:(id <ImageDownloaderDelegate>)delegate;

- (BOOL)isImageReady;
- (UIImage*)getImage;

@end
