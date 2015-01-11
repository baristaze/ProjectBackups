//
//  MessageDetailsViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/14/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreLocation/CoreLocation.h>
#import "MBProgressHUD.h"

@protocol MessageDetailsViewControllerDelegate 

- (void)onMessageDeleted:(NSString*)messageId;

@end

@interface MessageDetailsViewController : UIViewController <CLLocationManagerDelegate, MBProgressHUDDelegate>
{
    NSString *_from;
    NSString *_subject;
    NSString *_date;
    NSString *_message;
    NSString *_messageId;
    BOOL _isCoupon;
    id<MessageDetailsViewControllerDelegate> _delegate;
    
	NSError *_lastLocationError;
    CLLocationManager *_locationManager;
    CLLocation *_lastUsedLocation;
    
    MBProgressHUD* _progressBar;
}

- (id)initWithFrom:(NSString*)from subject:(NSString*)subject date:(NSString*)date message:(NSString*)message messageId:(NSString*)messageId  delegate:(id<MessageDetailsViewControllerDelegate>)delegate isCoupon:(BOOL)isCoupon;

@property (retain, nonatomic) IBOutlet UIButton *redeemButton;
@property (retain, nonatomic) IBOutlet UILabel *fromLabel;
@property (retain, nonatomic) IBOutlet UILabel *subjectLabel;
@property (retain, nonatomic) IBOutlet UILabel *dateLabel;
@property (retain, nonatomic) IBOutlet UIWebView *webView;

- (IBAction)onRedeem:(id)sender;

@end
