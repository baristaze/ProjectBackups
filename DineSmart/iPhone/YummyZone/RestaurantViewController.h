//
//  RestaurantViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface RestaurantViewController : UIViewController <UIActionSheetDelegate> {
	
	NSArray *_fileList;
    NSArray *_pressedFileList;
	NSInteger _lastSelectedButtonIndex;
	
	NSString *_restaurantId;
	NSString *_restaurantName;
    NSString *_restaurantAddress;
    
    NSMutableDictionary *_stateStore;
}

- (id)initWithRestaurantId:(NSString*)restaurandId 
            restaurantName:(NSString*)restaurantName 
         restaurantAddress:(NSString*)restaurantAddress;

@end
