//
//  ImageCacheDictData.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//
#import <UIKit/UIKit.h>
#import "ImageDownloader.h"
#import "ImageCache.h"

@interface ImageCacheData : NSObject 
{
    UIImage * _image;
    NSString * _error;
    NSMutableArray * _delegates;
    NSMutableArray * _keys;
    ImageDownloader * _downloader;
}

- (UIImage*)getImage;
- (void)setImage:(UIImage*)image;

- (NSString*)getError;
- (void)setError:(NSString*)error;

- (void)addDelegate:(id <ImageCacheDelegate>)delegate key:(NSObject*)key;
- (void)removeDelegateAndItsKey:(id <ImageCacheDelegate>)delegate;
- (void)clearDelegatesAndKeys;
- (void)notifyAllDelegates;

- (ImageDownloader*)getDownloader;
- (void)setDownloader:(ImageDownloader*)downloader;
- (void)clearDownloader;

@end
