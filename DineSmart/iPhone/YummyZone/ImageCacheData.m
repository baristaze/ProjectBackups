//
//  ImageCacheDictData.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ImageCacheData.h"
#import "NSMutableArray+WeakReferences.h"


@implementation ImageCacheData

- (id)init
{
    self  = [super init];
	if (self)
	{
        _image = nil;
        _error = nil;
        // We should not retain delegates - make sure that we create a special array
        // for them that doesn't do retain/release for the delegates.
        _delegates = [NSMutableArray createMutableArrayWithWeakReferences];
        _keys = [[NSMutableArray alloc] init];
        _downloader = nil;
	}
	return self;
}

- (void)dealloc
{
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }

    [_error release];
    [_image release];
    [_delegates release];
    [_keys release];
	[super dealloc];
}

- (UIImage*)getImage
{
    return _image;
}

- (void)setImage:(UIImage*)image
{
	[image retain];
	[_image release];
	_image = image;
}

- (NSString*)getError
{
    return _error;
}

- (void)setError:(NSString*)error
{
	[error retain];
	[_error release];
	_error = error;
}

- (void)addDelegate:(id <ImageCacheDelegate>)delegate key:(NSObject*)key
{
    [_delegates addObject:delegate];
    [_keys addObject:key];
}

- (void)removeDelegateAndItsKey:(id <ImageCacheDelegate>)delegate
{
    // The same object could appear multiple times in this array
    while (true)
    {
        NSUInteger index = [_delegates indexOfObject:delegate];
        if (index == NSNotFound)
        {
            break;
        }
        else
        {
            [_delegates removeObjectAtIndex:index];
            [_keys removeObjectAtIndex:index];
        }
    }
}

- (void)clearDelegatesAndKeys
{
    [_delegates removeAllObjects];
    [_keys removeAllObjects];
}

- (void)notifyAllDelegates
{
    for (int i = 0; i < [_delegates count]; i++)
    {
        id <ImageCacheDelegate> delegate = [_delegates objectAtIndex:i];
        NSObject * key = [_keys objectAtIndex:i];
        
        if (_image != nil)
        {
            [delegate imageDidLoad:_image key:key];
        }
        else
        {
            [delegate imageFailedToLoad:_error key:key];
        }
    }
}

- (ImageDownloader*)getDownloader
{
    return _downloader;
}

- (void)setDownloader:(ImageDownloader*)downloader
{
	[downloader retain];
    
    if (_downloader != nil && downloader != _downloader)
    {
        [_downloader setDelegate:nil];
    }
    
	[_downloader release];
	_downloader = downloader;
}

- (void)clearDownloader
{
    if (_downloader != nil)
    {
        [_downloader setDelegate:nil];
        [_downloader release];
        _downloader = nil;
    }
}
@end
