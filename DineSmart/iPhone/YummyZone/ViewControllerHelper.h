//
//  ViewControllerHelper.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/14/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface ViewControllerHelper : NSObject
{
}

+ (UITableViewCell*)getSpinnerCell:(UITableView*)tableView title:(NSString*)title;
+ (UITableViewCell*)getTextAndDescriptionCell:(UITableView*)tableView title:(NSString*)title description:(NSString*)description;

@end
