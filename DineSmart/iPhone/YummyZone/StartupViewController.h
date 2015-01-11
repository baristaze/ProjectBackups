//
//  StartupViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/21/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface StartupViewController : UIViewController <UIActionSheetDelegate, UIAlertViewDelegate> {

	NSArray *_fileList;
	NSArray *_pressedFileList;
	NSInteger _lastSelectedButtonIndex;
}

@end
