//
//  MessageViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/29/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "DictionaryDownloader.h"
#import "MessageDetailsViewController.h"

@interface MessageListViewController : UIViewController <UITableViewDelegate, UITableViewDataSource, UIActionSheetDelegate, DictionaryDownloaderDelegate, MessageDetailsViewControllerDelegate> 
{
	UITableView	*_tableView;	
    
    NSString *_restaurantId;
    BOOL _isCouponList;
    
	BOOL _populatingItems;
    BOOL _appendMessages;
	BOOL _lastPopulationFailed;
    NSString *_lastPopulationError;

    UIImage *_selectedImage;
    UIImage *_unselectedImage;

    NSMutableArray *_messages;
    BOOL _hasMoreMessages;
    NSString *_hintForNextPage;
    NSMutableArray *_selectedMessages;
    
    DictionaryDownloader *_downloader;
	
	// pull-down refresh related things
	UIView *refreshHeaderView;
    UILabel *refreshLabel;
    UIImageView *refreshArrow;
    UIActivityIndicatorView *refreshSpinner;
    BOOL isDragging;
    BOOL isLoading;
    NSString *textPull;
    NSString *textRelease;
    NSString *textLoading;
	// end of pull-down refresh
    
    UIBarButtonItem* _editBarButtonItem;
    UIBarButtonItem* _doneBarButtonItem;
}

// pull-down refresh related things
@property (nonatomic, retain) UIView *refreshHeaderView;
@property (nonatomic, retain) UILabel *refreshLabel;
@property (nonatomic, retain) UIImageView *refreshArrow;
@property (nonatomic, retain) UIActivityIndicatorView *refreshSpinner;
@property (nonatomic, copy) NSString *textPull;
@property (nonatomic, copy) NSString *textRelease;
@property (nonatomic, copy) NSString *textLoading;

- (void)setupStrings;
- (void)addPullToRefreshHeader;
- (void)startLoading;
- (void)stopLoading;
- (void)refresh;
// end of pull-down refresh

- (id)initWithType:(BOOL)isCouponList restaurantId:(NSString*)restaurantId;
- (void)onMessageDeleted:(NSString*)messageId;

@end
