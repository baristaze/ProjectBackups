//
//  FeedbackViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreLocation/CoreLocation.h>
#import "ImageCache.h"
#import "RatingChangedDelegate.h"
#import "RestaurantMenuViewController.h"
#import "TextInputController.h"
#import "SingleSelectKeyedListViewController.h"
#import "TitleAndYesNoCell.h"
#import "FeedbackFooterView.h"
#import "DictionaryDownloader.h"
#import "TitleAndSegmentedControl.h"
#import "MBProgressHUD.h"

@interface FeedbackViewController : UIViewController <CLLocationManagerDelegate,
    UITableViewDelegate, UITableViewDataSource, ImageCacheDelegate, RatingChangedDelegate,
    RestaurantMenuViewControllerDelegate, DictionaryDownloaderDelegate, SwitchValueChangedDelegate,
    FeedbackFooterViewDelegate, SegmentValueChangedDelegate, MBProgressHUDDelegate> 
{
	UITableView	*_tableView;	
    
    BOOL _populatingItems;
	BOOL _lastPopulationFailed;
    NSString *_lastPopulationError;

	NSString *_restaurantId;
	NSString *_restaurantName;
    NSMutableDictionary *_stateStore;
    
    NSMutableArray *_customSections;
    NSMutableArray *_selectedPlates;
    
    ImageCache * _thumbnailCache;
    DictionaryDownloader *_downloader;

	TextInputController *_textInputController;
	SingleSelectKeyedListViewController *_singleSelectListInputController;
    
    NSError *_lastLocationError;
    CLLocationManager *_locationManager;
    CLLocation *_lastUsedLocation;
    
    MBProgressHUD* _progressBar;
}

- (id)initWithRestaurantId:(NSString*)restaurantId restaurantName:(NSString*)restaurantName stateStore:(NSMutableDictionary*)stateStore;

@end
