//
//  DictionaryCache.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "DictionaryCache.h"
#import "DictionaryCacheData.h"

@implementation DictionaryCache

static DictionaryCache *_dictionaryCacheSingleton;


// Initialize the singleton instance if needed and return
+ (DictionaryCache*)singleton
{
	// @synchronized(self)
	{
		if (!_dictionaryCacheSingleton)
			_dictionaryCacheSingleton = [[DictionaryCache alloc] init];
		
		return _dictionaryCacheSingleton;
	}
}

+ (id)alloc
{
	//	@synchronized(self)
	{
		NSAssert(_dictionaryCacheSingleton == nil, @"Attempted to allocate a second instance of a singleton.");
		_dictionaryCacheSingleton = [super alloc];
		return _dictionaryCacheSingleton;
	}
}

+ (id)copy
{
	//	@synchronized(self)
	{
		NSAssert(_dictionaryCacheSingleton == nil, @"Attempted to copy the singleton.");
		return _dictionaryCacheSingleton;
	}
}

+ (void)initialize
{
    static BOOL initialized = NO;
    if (!initialized)
	{
        initialized = YES;
    }
}

- (id)init
{
    self  = [super init];
	if (self)
	{
        _cachedDictionaries = [[NSMutableDictionary alloc] init];
	}
	return self;
}

- (void)dealloc
{
    [_cachedDictionaries release];
	[super dealloc];
}

- (void)loadDictionaryAtUrl:(NSURL*)url delegate:(id <DictionaryCacheDelegate>)delegate key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders
{
    BOOL startNewDownloader = NO;
    
    // Check cached items first
    DictionaryCacheData * data = [_cachedDictionaries objectForKey:url];
    if (data != nil)
    {
        NSDictionary *dict = [data getData];
        if (dict != nil)
        {
            NSLog(@"DictionaryCache:loadDictionaryAtUrl: Dictionary is already in the cache for URL %@", url);
            
            // Dictionary is already downloaded. Notify the delegate.
            [delegate dictionaryPopulated:dict key:key];
        }
        else
        {
            NSString *error = [data getError];
            if (error != nil)
            {
                // We failed to download this data previously. Retry downloading it.
                NSLog(@"DictionaryCache:loadDictionaryAtUrl: Dictionary is in cache but failed previously. Retrying URL %@", url);

                startNewDownloader = YES;
            }
            else
            {
                // Dictionary is currently being downloaded. Add this delegate/key to the list
                // of the delegates/keys to be notified once the download finishes.
                NSLog(@"DictionaryCache:loadDictionaryAtUrl: Dictionary is in the cache but still being downloaded for URL %@", url);
            }
            
            [data addDelegate:delegate key:key];
        }
    }
    else
    {
        NSLog(@"DictionaryCache:loadDictionaryAtUrl: Dictionary is NOT in the cache for URL %@", url);

        startNewDownloader = YES;
    }
    
    if (startNewDownloader)
    {
        [data clearDownloader];

        data = [[DictionaryCacheData alloc] init];
        [_cachedDictionaries setObject:data forKey:url];
        
        [data addDelegate:delegate key:key];
        
        DictionaryDownloader * downloader = [[DictionaryDownloader alloc] initWithUrl:url delegate:self key:url httpHeaders:httpHeaders];
        [data setDownloader:downloader];
        [downloader startDownload];
    }
}

- (BOOL)isUrlCached:(NSURL*)url
{
    DictionaryCacheData * data = [_cachedDictionaries objectForKey:url];
    if (data != nil)
    {
        NSDictionary *dict = [data getData];
        if (dict != nil)
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

- (NSDictionary*)getCachedDictionaryAtUrl:(NSURL*)url
{
    DictionaryCacheData * data = [_cachedDictionaries objectForKey:url];
    if (data != nil)
    {
        return [data getData];
    }
    return nil;
}

- (void)clearCache
{
    NSLog(@"DictionaryCache:clearCache");
    
    // terminate all pending downloads first
    [[_cachedDictionaries allValues] performSelector:@selector(clearDownloader)];
    
    [_cachedDictionaries removeAllObjects];
}

- (void)removeDelegate:(id <DictionaryCacheDelegate>)delegate
{
    for (DictionaryCacheData *data in [_cachedDictionaries allValues])
    {
        [data removeDelegateAndItsKey:delegate];
    }
}

- (void)dictionaryDidLoad:(NSDictionary*)dict key:(NSObject*)key
{
    NSLog(@"DictionaryCache:dictionaryDidLoad: URL %@", key);
    
    DictionaryCacheData * data = [_cachedDictionaries objectForKey:key];
    
    [data setData:dict];
    [data clearDownloader];
    [data notifyAllDelegates];
    [data clearDelegatesAndKeys];
}

- (void)dictionaryFailedToLoad:(NSString*)error key:(NSObject*)key
{
    NSLog(@"DictionaryCache:dictionaryFailedToLoad: Error:%@, URL %@", error, key);
    
    DictionaryCacheData * data = [_cachedDictionaries objectForKey:key];
    
    [data setData:nil];
    [data setError:error];
    [data clearDownloader];
    [data notifyAllDelegates];
    [data clearDelegatesAndKeys];
}

@end
