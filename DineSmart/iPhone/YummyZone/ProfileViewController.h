//
//  ProfileViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 2/16/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <MessageUI/MFMailComposeViewController.h>
//#import "FBHelper.h"

@interface ProfileViewController : UIViewController <UITableViewDelegate, UITableViewDataSource,MFMailComposeViewControllerDelegate
    /*, FBActionDelegate*/>
{
	UITableView	*_tableView;
}

-(NSString*)getIdentityRowText;
-(NSString*)getRowTextAt:(int)row section:(int)section;

- (void)mailComposeController:(MFMailComposeViewController*)controller 
          didFinishWithResult:(MFMailComposeResult)result 
                        error:(NSError*)error;

//- (void)refreshFBRelatedUI;
//- (void)onCompletedFBAction:(BOOL)isByUser result:(BOOL)isSuccess;

@end
