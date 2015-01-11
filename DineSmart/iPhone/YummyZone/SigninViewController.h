//
//  SignupViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 12/30/11.
//  Copyright (c) 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface SigninViewController : UIViewController <UIActionSheetDelegate, UITableViewDelegate, UITableViewDataSource> 
{
	UITableView	*_tableView;
    
    bool _isSignUpDialog;
}

- (id)init;

@end
