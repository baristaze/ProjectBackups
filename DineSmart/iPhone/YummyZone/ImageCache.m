//
//  ImageCache.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ImageCache.h"
#import "ImageCacheData.h"
#import "UIImage+Resize.h"


@implementation ImageCache

- (id)init
{
    self  = [super init];
	if (self)
	{
        _cachedImages = [[NSMutableDictionary alloc] init];
        _resizeImages = NO;
	}
	return self;
}

- (id)initWithImageWidth:(CGFloat)width imageHeight:(CGFloat)height shrinkOnly:(BOOL)shrinkOnly
{
    self  = [super init];
	if (self)
	{
        _cachedImages = [[NSMutableDictionary alloc] init];
        _resizeImages = YES;
        _shrinkOnly = shrinkOnly;
        _imageWidth = width;
        _imageHeight = height;
	}
	return self;
}

- (void)dealloc
{
    for (ImageCacheData *data in _cachedImages)
    {
        [data setImage:nil];
        [data clearDownloader];
        [data clearDelegatesAndKeys];
    }
    
    [_cachedImages release];
	[super dealloc];
}

- (void)loadImageAtUrl:(NSURL*)url delegate:(id <ImageCacheDelegate>)delegate key:(NSObject*)key
{
    BOOL startNewDownloader = NO;
    
    // Check cached items first
    ImageCacheData * data = [_cachedImages objectForKey:url];
    if (data != nil)
    {
        // We either downloaded this image already or it is currently being downloaded.
        UIImage * image = [data getImage];
        if (image != nil)
        {
            NSLog(@"ImageCache:loadImageAtUrl: Image is already in the cache for URL %@", url);

            // Image is already downloaded. Notify the delegate.
            [delegate imageDidLoad:image key:key];
        }
        else
        {
            NSString *error = [data getError];
            if (error != nil)
            {
                // We failed to download this data previously. Retry downloading it.
                NSLog(@"ImageCache:loadImageAtUrl: Image is in cache but failed previously. Retrying URL %@", url);
                
                startNewDownloader = YES;
            }
            else
            {
                // Dictionary is currently being downloaded. Add this delegate/key to the list
                // of the delegates/keys to be notified once the download finishes.
                NSLog(@"ImageCache:loadImageAtUrl: Image is in the cache but still being downloaded for URL %@", url);
            }
            
            [data addDelegate:delegate key:key];
        }
    }
    else
    {
        NSLog(@"ImageCache:loadImageAtUrl: Image is NOT in the cache for URL %@", url);
        
        startNewDownloader = YES;
    }

    if (startNewDownloader)
    {
        [data clearDownloader];

        data = [[ImageCacheData alloc] init];
        [_cachedImages setObject:data forKey:url];
        
        [data addDelegate:delegate key:key];
        
        ImageDownloader * downloader = [[ImageDownloader alloc] initWithUrl:url delegate:self key:url];
        [data setDownloader:downloader];
        [downloader startDownload];
    }
}

- (BOOL)isImageCachedForUrl:(NSURL*)url
{
    ImageCacheData * data = [_cachedImages objectForKey:url];
    if (data != nil)
    {
        // We either downloaded this image already or it is currently being downloaded.
        UIImage * image = [data getImage];
        if (image != nil)
        {
            return YES;
        }
        else
        {
            return NO;
        }
    }
    return NO;
}

- (UIImage*)getImageAtUrl:(NSURL*)url
{
    ImageCacheData * data = [_cachedImages objectForKey:url];
    if (data != nil)
    {
        return [data getImage];
    }
    return nil;
}

- (void)clearCache
{
    NSLog(@"ImageCache:clearCache");

    // terminate all pending downloads first
    [[_cachedImages allValues] performSelector:@selector(clearDownloader)];
    
    [_cachedImages removeAllObjects];
}

- (void)imageDidLoad:(UIImage*)image key:(NSObject*)key
{
    NSLog(@"ImageCache:imageDidLoad: URL %@", key);

    ImageCacheData * data = [_cachedImages objectForKey:key];
    
    if (_resizeImages)
    {
        NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
        [data setImage:[UIImage imageUsingImage:image width:_imageWidth height:_imageHeight onlyShrink:_shrinkOnly]];
        [pool release];
    }
    else
    {
        [data setImage:image];
    }
    
    [data clearDownloader];
    [data notifyAllDelegates];
    [data clearDelegatesAndKeys];
}

- (void)imageFailedToLoad:(NSString*)error key:(NSObject*)key
{
    NSLog(@"ImageCache:imageFailedToLoad: Error: %@, Key: %@", error, key);

    ImageCacheData * data = [_cachedImages objectForKey:key];
    
    [data setImage:nil];
    [data setError:error];
    [data clearDownloader];
    [data notifyAllDelegates];
    [data clearDelegatesAndKeys];
}

- (void)removeDelegate:(id <ImageCacheDelegate>)delegate
{
    for (ImageCacheData *data in [_cachedImages allValues])
    {
        [data removeDelegateAndItsKey:delegate];
    }
}

@end

