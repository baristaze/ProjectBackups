//
//  DictionaryDownloader.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "DataDownloader.h"

@protocol DictionaryDownloaderDelegate

- (void)dictionaryDidLoad:(NSDictionary*)dict key:(NSObject*)key;
- (void)dictionaryFailedToLoad:(NSString*)error key:(NSObject*)key;

@end

@interface DictionaryDownloader : DataDownloader 
{
    NSDictionary * _data;
    NSString * _error;
    id <DictionaryDownloaderDelegate> _delegate;
}

- (id)initWithUrl:(NSURL*)url delegate:(id <DictionaryDownloaderDelegate>)delegate key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders;
- (void)setDelegate:(id <DictionaryDownloaderDelegate>)delegate;

@end
