//
//  DataDownloader.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "DataDownloader.h"


@implementation DataDownloader

- (id)initWithUrl:(NSURL*)url key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders
{
	NSLog(@"New image downloader");
	
    self = [super init];
    if (self)
	{
		_url = [url retain];
        _key = [key retain];
        if (httpHeaders != nil)
        {
            _httpHeaders = [httpHeaders copy];
        }
        else
        {
            _httpHeaders = nil;
        }
	}
	return self;
}

- (void)dealloc
{
    [self cancelDownload];
    
    [_httpHeaders release];
    [_key release];
	[_url release];
    [_connection release];
    [_activeData release];
    [super dealloc];
}

- (void)startDownload
{
	NSLog(@"DataDownloader:startDownload: url = %@", _url);
	
	if (_activeData != nil)
	{
		[self cancelDownload];
	}
	
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    _activeData = [[NSMutableData data] retain];
	
	NSMutableURLRequest *urlRequest = [NSMutableURLRequest requestWithURL:_url];
    if (_httpHeaders != nil)
    {
        for (NSString *key in _httpHeaders)
        {
            [urlRequest setValue:[_httpHeaders objectForKey:key] forHTTPHeaderField:key];
        }
    }
    
    _connection = [[NSURLConnection connectionWithRequest:urlRequest delegate:self] retain];
	
	[pool release];
}

- (void)cancelDownload
{
    [_connection cancel];
    [_connection release];
	_connection = nil;
    
    [_activeData release];
	_activeData = nil;
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
	if (_activeData == nil)
	{
		NSLog(@"Active download is nil but data is still incoming!");
	}
	else
	{
		[_activeData appendData:data];
	}
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
	NSLog(@"DataDownloader:connection:didFailWithError: url: %@, error: %@", _url, error);
    
    [self processFailure:[NSString stringWithFormat:@"Failure reason: %@. Description: %@.", [error localizedFailureReason], [error localizedDescription]] key:_key];

    [_activeData release];
	_activeData = nil;
    
    [_connection release];
	_connection = nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
	NSLog(@"DataDownloader:connectionDidFinishLoading: url: %@", _url);

    [self processFinishedData:_activeData key:_key];
    
    [_activeData release];
	_activeData = nil;
	
    [_connection release];
	_connection = nil;
}

- (void)processFinishedData:(NSData*)finishedData key:(NSObject*)key
{
    [NSException raise:NSInternalInconsistencyException 
                format:@"You must override %@ in a subclass", NSStringFromSelector(_cmd)];
}

- (void)processFailure:(NSString*)error key:(NSObject*)key
{
    [NSException raise:NSInternalInconsistencyException 
                format:@"You must override %@ in a subclass", NSStringFromSelector(_cmd)];
}

@end
