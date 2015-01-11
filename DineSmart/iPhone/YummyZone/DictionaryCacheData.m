//
//  UrlCacheData.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "DictionaryCacheData.h"
#import "NSMutableArray+WeakReferences.h"


@implementation DictionaryCacheData

- (id)init
{
    self  = [super init];
	if (self)
	{
        _data = nil;
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
    [_data release];
    [_delegates release];
    [_keys release];
	[super dealloc];
}

- (NSDictionary*)getData
{
    return _data;
}

- (void)setData:(NSDictionary*)data
{
	[data retain];
	[_data release];
	_data = data;
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

- (void)addDelegate:(id <DictionaryCacheDelegate>)delegate key:(NSObject*)key
{
    [_delegates addObject:delegate];
    [_keys addObject:key];
}

- (void)removeDelegateAndItsKey:(id <DictionaryCacheDelegate>)delegate
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
        id <DictionaryCacheDelegate> delegate = [_delegates objectAtIndex:i];
        NSObject * key = [_keys objectAtIndex:i];
        
        if (_data != nil)
        {
            [delegate dictionaryPopulated:_data key:key];
        }
        else
        {
            [delegate failedToPopulateDictionary:_error key:key];
        }
    }
}

- (DictionaryDownloader*)getDownloader
{
    return _downloader;
}

- (void)setDownloader:(DictionaryDownloader*)downloader
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
