//
//  FavoritesViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageCache.h"

@interface HistoryDetailsViewController : UIViewController <UITableViewDelegate, UITableViewDataSource, ImageCacheDelegate> 
{
	UITableView	*_tableView;	
    
	NSArray *_menuItems;
    
	NSString *_restaurantId;
    
    ImageCache * _thumbnailCache;
}

- (id)initWithMenuItems:(NSArray*)menuItems;

@end
