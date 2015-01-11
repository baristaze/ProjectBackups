//
//  LocateMeViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "DictionaryDownloader.h"
#import <CoreLocation/CoreLocation.h>

@interface LocateMeViewController : UIViewController <UITableViewDelegate, UITableViewDataSource, UIActionSheetDelegate, DictionaryDownloaderDelegate, CLLocationManagerDelegate> 
{
	UITableView	*_tableView;	
	NSMutableArray *_restaurants;
    BOOL _gettingInitialCoordinates;

	NSString *_lastWebServiceError;
	NSError *_lastLocationError;

    DictionaryDownloader *_downloader;
    
    CLLocationManager *_locationManager;
    CLLocation *_lastUsedLocation;
}

@end
