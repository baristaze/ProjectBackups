//
//  ImageCache.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageDownloader.h"

@protocol ImageCacheDelegate 

- (void)imageDidLoad:(UIImage*)image key:(NSObject*)key;
- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key;

@end

@interface ImageCache : NSObject <ImageDownloaderDelegate>
{
    NSMutableDictionary * _cachedImages;
    
    BOOL _resizeImages;
    BOOL _shrinkOnly;
    CGFloat _imageWidth;
    CGFloat _imageHeight;
}

- (id)init;

- (id)initWithImageWidth:(CGFloat)width imageHeight:(CGFloat)height shrinkOnly:(BOOL)shrinkOnly;

- (void)loadImageAtUrl:(NSURL*)url delegate:(id <ImageCacheDelegate>)delegate key:(NSObject*)key;

- (BOOL)isImageCachedForUrl:(NSURL*)url;

- (UIImage*)getImageAtUrl:(NSURL*)url;

- (void)removeDelegate:(id <ImageCacheDelegate>)delegate;

- (void)clearCache;

@end
