//
//  DictionaryDownloader.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "DictionaryDownloader.h"
#import "NSDictionary+Helpers.h"
#import "KeyConstants.h"

@implementation DictionaryDownloader

- (id)initWithUrl:(NSURL*)url delegate:(id <DictionaryDownloaderDelegate>)delegate key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders
{
    self = [super initWithUrl:url key:key httpHeaders:httpHeaders];
    if (self)
    {
        _delegate = delegate;
        _data = nil;
        _error = nil;
    }
    return self;
}

- (void)dealloc
{
    _delegate = nil;
    [_data release];
    _data = nil;
	[_error release];
    _error = nil;
    [super dealloc];
}

- (void)setDelegate:(id <DictionaryDownloaderDelegate>)delegate
{
    _delegate = delegate;
}

- (void)processFinishedData:(NSData*)finishedData key:(NSObject*)key
{
	NSLog(@"DictionaryDownloader:processFinishedData: url: %@", _url);

    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    _data = [[NSDictionary dictionaryWithContentsOfData:finishedData] retain];
    
    // BUGBUG: Implement a common error reporting mechanism. If the dictionary contains
    // a single element and it is called "WebRequestFailure", then return an error
    // and use the contents of it as the error message.
    
    if (_data == nil)
    {
        [self processFailure:@"Failed to convert the data to NSDictionary." key:key];
    }
    else
    {
        if (_delegate != nil)
        {
            [_delegate dictionaryDidLoad:_data key:key];
        }
    }
    
    [pool release];
}

- (void)processFailure:(NSString*)error key:(NSObject*)key
{
	NSLog(@"DictionaryDownloader:processFailure: url: %@, error: %@", _url, error);

    _error = [error copy];
    if (_delegate != nil)
    {
        [_delegate dictionaryFailedToLoad:_error key:key];
    }
}

@end
