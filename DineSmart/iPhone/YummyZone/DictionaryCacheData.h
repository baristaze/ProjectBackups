//
//  UrlCacheData.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DictionaryCache.h"
#import "DictionaryDownloader.h"

@interface DictionaryCacheData : NSObject 
{
    NSDictionary * _data;
    NSString * _error;
    NSMutableArray * _delegates;
    NSMutableArray * _keys;
    DictionaryDownloader * _downloader;
}

- (NSDictionary*)getData;
- (void)setData:(NSDictionary*)dict;

- (NSString*)getError;
- (void)setError:(NSString*)error;

- (void)addDelegate:(id <DictionaryCacheDelegate>)delegate key:(NSObject*)key;
- (void)removeDelegateAndItsKey:(id <DictionaryCacheDelegate>)delegate;
- (void)clearDelegatesAndKeys;
- (void)notifyAllDelegates;

- (DictionaryDownloader*)getDownloader;
- (void)setDownloader:(DictionaryDownloader*)downloader;
- (void)clearDownloader;

@end
