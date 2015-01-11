//
//  DictionaryCache.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DictionaryDownloader.h"

@protocol DictionaryCacheDelegate

- (void)dictionaryPopulated:(NSDictionary*)dict key:(NSObject*)key;
- (void)failedToPopulateDictionary:(NSString*)error key:(NSObject*)key;

@end

@interface DictionaryCache : NSObject <DictionaryDownloaderDelegate>
{
    NSMutableDictionary * _cachedDictionaries;
}

+ (DictionaryCache*)singleton;

- (id)init;

- (void)loadDictionaryAtUrl:(NSURL*)url delegate:(id <DictionaryCacheDelegate>)delegate key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders;

- (BOOL)isUrlCached:(NSURL*)url;

- (NSDictionary*)getCachedDictionaryAtUrl:(NSURL*)url;

- (void)removeDelegate:(id <DictionaryCacheDelegate>)delegate;

- (void)clearCache;


@end
