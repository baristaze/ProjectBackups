//
//  DataDownloader.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface DataDownloader : NSObject 
{
    NSMutableData *_activeData;
    NSURLConnection *_connection;
	NSURL *_url;
	NSObject *_key;
    NSDictionary *_httpHeaders;
}

- (id)initWithUrl:(NSURL*)url key:(NSObject*)key httpHeaders:(NSDictionary*)httpHeaders;

- (void)startDownload;
- (void)cancelDownload;

- (void)processFinishedData:(NSData*)finishedData key:(NSObject*)key;
- (void)processFailure:(NSString*)error key:(NSObject*)key;

@end
