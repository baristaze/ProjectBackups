//
//  RestaurantMenuViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageCache.h"
#import "DictionaryDownloader.h"

@protocol RestaurantMenuViewControllerDelegate

@optional
- (void)menuItemsSelected:(NSArray*)selectedItems;

@end

@interface RestaurantMenuViewController : UIViewController <UITableViewDelegate, UITableViewDataSource, UIActionSheetDelegate, ImageCacheDelegate, DictionaryDownloaderDelegate> 
{
    id <RestaurantMenuViewControllerDelegate> delegate;

	UITableView	*_tableView;	

	NSMutableArray *_categorizedItems;

	BOOL _populatingItems;
	BOOL _lastPopulationFailed;
    NSString *_lastPopulationError;
	
	NSString *_restaurantId;
	NSString *_restaurantName;
    
    ImageCache * _thumbnailCache;
    DictionaryDownloader *_downloader;

    BOOL _isModalPlateSelectorDialog;
    
    NSMutableDictionary *_selectedItemKeys;
}

@property (nonatomic, assign) id <RestaurantMenuViewControllerDelegate> delegate;

- (id)initWithRestaurantId:(NSString*)restaurandId restaurantName:(NSString*)restaurantName isModalPlateSelectorDialog:(BOOL)isModalPlateSelectorDialog;

- (void)setSelectedItemKeys:(NSArray*)selectedItems;

@end
