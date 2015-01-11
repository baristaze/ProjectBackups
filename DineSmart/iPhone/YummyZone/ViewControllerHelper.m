//
//  ViewControllerHelper.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 1/14/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#import "ViewControllerHelper.h"

@implementation ViewControllerHelper

+ (UITableViewCell*)getSpinnerCell:(UITableView*)tableView title:(NSString*)title
{
    static NSString *kSpinnerCell = @"kSpinnerCell_ViewControllerHelper";
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kSpinnerCell];
    if (cell == nil)
    {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kSpinnerCell] autorelease];
        cell.accessoryType = UITableViewCellAccessoryNone;
    }
    
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    cell.textLabel.text = title;
    cell.textLabel.textAlignment = UITextAlignmentCenter;
    
    UIActivityIndicatorView *activityView = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    [activityView startAnimating];
    [cell setAccessoryView:activityView];
    [activityView release];
    
    [pool release];
    return cell;
}

+ (UITableViewCell*)getTextAndDescriptionCell:(UITableView*)tableView title:(NSString*)title description:(NSString*)description
{
    static NSString *kTextDescCell = @"kTextDescCell_ViewControllerHelper";
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kTextDescCell];
    if (cell == nil)
    {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:kTextDescCell] autorelease];
        cell.accessoryType = UITableViewCellAccessoryNone;
    }
    
    cell.textLabel.text = title;
    cell.detailTextLabel.text = description;
    cell.detailTextLabel.numberOfLines = 0;

    return cell;
}

@end
