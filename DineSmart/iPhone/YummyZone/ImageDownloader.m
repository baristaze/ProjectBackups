//
//  ImageDownloader.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ImageDownloader.h"

@implementation ImageDownloader

- (id)initWithUrl:(NSURL*)url delegate:(id <ImageDownloaderDelegate>)delegate key:(NSObject*)key
{
    self = [super initWithUrl:url key:key httpHeaders:nil];
    if (self)
    {
        _delegate = delegate;
        
        _image = nil;
        _error = nil;
    }
    return self;
}

- (void)dealloc
{
    _delegate = nil;
    [_image release];
    _image = nil;
	[_error release];
    _error = nil;
    [super dealloc];
}

- (void)setDelegate:(id <ImageDownloaderDelegate>)delegate
{
    _delegate = delegate;
}

- (void)processFinishedData:(NSData*)finishedData key:(NSObject*)key
{
	NSLog(@"ImageDownloader:processFinishedData: url: %@", _url);

    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    _image = [[UIImage alloc] initWithData:finishedData];
    
    if (_image == nil)
    {
        [self processFailure:@"Failed to convert the data to UIImage." key:key];
    }
    else
    {
        if (_delegate != nil)
        {
            [_delegate imageDidLoad:_image key:key];
        }
    }
    
    [pool release];
}

- (void)processFailure:(NSString*)error key:(NSObject*)key
{
	NSLog(@"ImageDownloader:processFailure: url: %@, error: %@", _url, error);
    
    if (_delegate != nil)
    {
        _error = [error copy];
        [_delegate imageFailedToLoad:_error key:key];
    }
}

- (BOOL)isImageReady
{
	return (_image != nil);
}

- (UIImage*)getImage
{
	return _image;
}

@end

