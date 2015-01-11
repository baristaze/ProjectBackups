//
//  SignupViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 12/30/11.
//  Copyright (c) 2011 __MyCompanyName__. All rights reserved.
//

#import "SigninViewController.h"
#import <QuartzCore/QuartzCore.h>
#import "YummyZoneSession.h"
#import "YummyZoneUtils.h"

@interface SigninViewController(private)

- (UITextField *)textFieldInRow:(NSUInteger)row section:(NSUInteger)section;
- (void)doSignUpOrSignIn;
-(BOOL)isValidEmail:(NSString *)checkString;

@end

@implementation SigninViewController

- (id)init
{
    self = [super init];
	if (self)
	{
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardWillShow:) name:UIKeyboardWillShowNotification object:nil];
        
        _isSignUpDialog = NO;
	}
    
	return self;
}


- (void)dealloc
{
    _tableView.delegate = nil;
    _tableView.dataSource = nil;
    [[NSNotificationCenter defaultCenter] removeObserver:self name:UIKeyboardWillShowNotification object:nil];

    [_tableView release];
    [super dealloc];
}


- (void)keyboardWillShow:(NSNotification *)notification
{
		NSValue *keyboardBoundsValue = [[notification userInfo] objectForKey:UIKeyboardFrameEndUserInfoKey];
		CGRect keyboardBounds;
		[keyboardBoundsValue getValue:&keyboardBounds];
		UIEdgeInsets e = UIEdgeInsetsMake(0, 0, keyboardBounds.size.height, 0);
		[_tableView setScrollIndicatorInsets:e];
		[_tableView setContentInset:e];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
}


- (void)reloadTable
{
    [_tableView reloadData];
}


- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
	[self reloadTable];
	[super viewWillAppear:animated];
}


- (void)viewDidLoad 
{
	self.title = @"Sign In";
    [YummyZoneUtils loadBackButton:[self navigationItem]];
	[super viewDidLoad];
}


- (void)loadView
{
	// create and configure the table view
	_tableView = [[UITableView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame] style:UITableViewStyleGrouped];
	_tableView.delegate = self;
	_tableView.dataSource = self;
	_tableView.autoresizesSubviews = YES;
	_tableView.separatorStyle = UITableViewCellSeparatorStyleSingleLine;
	_tableView.allowsSelectionDuringEditing = YES;
	
    _tableView.backgroundView = nil;
    
    UIView* backGView = [[UIView alloc] initWithFrame:CGRectMake(_tableView.bounds.origin.x, 
                                                                 _tableView.bounds.origin.y, 
                                                                 _tableView.bounds.size.width, 
                                                                 _tableView.bounds.size.height)];
    
    CGFloat grayRatio = 237.0/255.0;
    backGView.backgroundColor = [UIColor colorWithRed:grayRatio 
                                                green:grayRatio 
                                                 blue:grayRatio 
                                                alpha:1.0];
    
    _tableView.backgroundView = backGView;
	
    self.view = _tableView;
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 3;
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if (section == 0)
    {
        if (_isSignUpDialog)
        {
            return 4;
        }
        else
        {
            return 3;
        }
    }
    else if (section == 1)
    {
        return 1;
    }
    
    return 0;
}


- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (indexPath.section == 0)
    {
        return 44;
    }
    else if (indexPath.section == 1)
    {
        return 44;
    }
    
    return 0;
}


- (UITextField *)textFieldInRow:(NSUInteger)row section:(NSUInteger)section
{
	return (UITextField *)[[[[_tableView cellForRowAtIndexPath:[NSIndexPath indexPathForRow:row inSection:section]] contentView] subviews] objectAtIndex:0];
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (indexPath.section == 0)
    {
        if ((_isSignUpDialog && indexPath.row == 3) || 
            (!_isSignUpDialog && indexPath.row == 2))
        {
            static NSString *kLoginCell = @"kLoginCell";
            UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kLoginCell];
            if (cell == nil)
            {
                cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kLoginCell] autorelease];
                cell.accessoryType = UITableViewCellAccessoryNone;
                cell.textLabel.textAlignment = UITextAlignmentCenter;
            }
            
            NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];
            
            if (_isSignUpDialog)
            {
                cell.textLabel.text = @"Create Account";
            }
            else
            {
                cell.textLabel.text = @"Login";
            }
            
            cell.textLabel.textColor = [UIColor colorWithRed:50.0/255.0 green:100.0/255.0 blue:150.0/255.0 alpha:1.0];
            
            [pool release];
            return cell;
        }
        else
        {
            UITableViewCell *cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:nil] autorelease];
            cell.accessoryType = UITableViewCellAccessoryNone;
            [cell setSelectionStyle:UITableViewCellSelectionStyleNone];
                
            NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
            
            CGRect f = CGRectInset([cell bounds], 10, 10);
            UITextField *textField = [[[UITextField alloc] initWithFrame:f] autorelease];
            [textField setAutoresizingMask:UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight];
            [textField setAutocapitalizationType:UITextAutocapitalizationTypeNone];
            [textField setAutocorrectionType:UITextAutocorrectionTypeNo];
            [cell.contentView addSubview:textField];
            
            if (indexPath.row == 0)
            {
                [textField setPlaceholder:@"Email"];
                [textField setSecureTextEntry:NO];
            }
            else if (indexPath.row == 1)
            {
                [textField setPlaceholder:@"Password"];
                [textField setSecureTextEntry:YES];
            }
            else if (indexPath.row == 2)
            {
                [textField setPlaceholder:@"Confirm Password"];
                [textField setSecureTextEntry:YES];
            }
            
            [pool release];
            return cell;
        }
    }
    else if (indexPath.section == 1)
    {
        static NSString *kDetailsText = @"kDetailsText";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kDetailsText];
        if (cell == nil)
        {
            cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleSubtitle reuseIdentifier:kDetailsText] autorelease];
            cell.accessoryType = UITableViewCellAccessoryNone;
            cell.detailTextLabel.numberOfLines = 2;
            cell.detailTextLabel.lineBreakMode = UILineBreakModeWordWrap;
        }
        
        NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];

        if (_isSignUpDialog)
        {
            cell.detailTextLabel.text = @"Select to login to an existing account.";
        }
        else
        {
            cell.detailTextLabel.text = @"Select to create a new account.";
        }
        [pool release];
        return cell;
    }

    return nil;
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (indexPath.section == 0)
    {
        if ((_isSignUpDialog && indexPath.row == 3) || 
            (!_isSignUpDialog && indexPath.row == 2))
        {
            [self doSignUpOrSignIn];
        }
    }
    else if (indexPath.section == 1)
    {
        _isSignUpDialog = !_isSignUpDialog;
        [_tableView reloadData];
    }
}


- (void)doSignUpOrSignIn
{
    NSString *email = [[self textFieldInRow:0 section:0] text];
    NSString *password = [[self textFieldInRow:1 section:0] text];

    if (![self isValidEmail:email])
    {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please enter a valid email address."
                                                       delegate:self cancelButtonTitle:@"Close" otherButtonTitles:nil];
        [alert show];
        [alert release];
        return;
    }

    if (_isSignUpDialog)
    {
        NSString *passwordConfirm = [[self textFieldInRow:2 section:0] text];

        if (password == nil || [password length] < 6)
        {
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Password must be at least 6 characters."
                                                           delegate:self cancelButtonTitle:@"Close" otherButtonTitles:nil];
            [alert show];
            [alert release];
            return;
        }
        
        if (passwordConfirm == nil || [passwordConfirm compare:password] != NSOrderedSame)
        {
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Passwords don't match."
                                                           delegate:self cancelButtonTitle:@"Close" otherButtonTitles:nil];
            [alert show];
            [alert release];
            return;
        }
    }
    else
    {
        if (password == nil || [password length] <= 0)
        {
            UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Please enter a password."
                                                           delegate:self cancelButtonTitle:@"Close" otherButtonTitles:nil];
            [alert show];
            [alert release];
            return;
        }
    }

    NSString *error = nil;
    if (_isSignUpDialog)
    {
        error = [[YummyZoneSession singleton] signupAndMergeTemporaryAccount:email password:password];
    }
    else
    {
        error = [[YummyZoneSession singleton] singinAndMergeTemporaryAccount:email password:password];
    }
    
    if (error == nil)
    {
        [self.navigationController popViewControllerAnimated:YES];
    }
    else
    {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Error" message:error
                                                       delegate:self cancelButtonTitle:@"Close" otherButtonTitles:nil];
        [alert show];
        [alert release];
        return;
    }

}


-(BOOL)isValidEmail:(NSString *)checkString
{
    NSString *emailRegex = @"[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
    NSPredicate *emailTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", emailRegex];
    return [emailTest evaluateWithObject:checkString];
}


@end