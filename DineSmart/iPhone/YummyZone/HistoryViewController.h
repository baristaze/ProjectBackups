//
//  HistoryViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageCache.h"
#import "DictionaryDownloader.h"

@interface HistoryViewController : UIViewController <UITableViewDelegate, UITableViewDataSource, ImageCacheDelegate, DictionaryDownloaderDelegate> 
{
	UITableView	*_tableView;	
    
	NSMutableArray *_history;
    
	BOOL _populatingItems;
	BOOL _lastPopulationFailed;
    NSString *_lastPopulationError;

	NSString *_restaurantId;
    
    ImageCache * _thumbnailCache;
    DictionaryDownloader *_downloader;
}

- (id)initWithRestaurantId:(NSString*)restaurandId;

@end
