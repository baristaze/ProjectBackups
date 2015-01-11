#import "TextInputController.h"
#import "TextViewCell.h"
#import "YummyZoneUtils.h"

@implementation TextInputController

#define kUITextViewCellRowHeight 175.0

@synthesize myTableView;

- (id)init
{
	self = [super init];
	if (self)
	{
		self.title = @"";
		inputDialogResultDict = nil;
		inputDialogResultKey = nil;
	}
	
	return self;
}

- (void)dealloc
{
    myTableView.delegate = nil;
    myTableView.dataSource = nil;
    
	[inputDialogResultDict release];
	[inputDialogResultKey release];
	[myTableView release];
	[super dealloc];
}

- (void)viewDidLoad 
{        
    UIButton* doneButton = [YummyZoneUtils createNavBarButtonWithText:@"Done" 
                                                            width:50.0
                                                           target:self 
                                                           action:@selector(doneAction:)];
    
    UIButton* clearButton = [YummyZoneUtils createNavBarButtonWithText:@"Clear" 
                                                                width:50.0
                                                               target:self 
                                                               action:@selector(clearAction:)];
    
    
    UIBarButtonItem* doneItem = [[UIBarButtonItem alloc] initWithCustomView:doneButton];  
    UIBarButtonItem* clearItem = [[UIBarButtonItem alloc] initWithCustomView:clearButton];

	self.navigationItem.leftBarButtonItem = clearItem;
	self.navigationItem.rightBarButtonItem = doneItem;
    
	[doneItem release];
	[clearItem release];
	
	[super viewDidLoad];
}

- (void)updateInputDialogDict
{
	TextViewCell *cell = (TextViewCell*)[myTableView cellForRowAtIndexPath:[NSIndexPath indexPathForRow:0 inSection:0]];
	[inputDialogResultDict setObject:cell.textView.text forKey:inputDialogResultKey];
}

- (void)viewWillDisappear:(BOOL)animated
{
	[self updateInputDialogDict];
	
	[super viewWillDisappear:animated];
}

- (void)setInputDialogResultDictionary:(NSMutableDictionary*)dict inputKey:(NSString*)key; 
{
	[dict retain];
    [inputDialogResultDict release];
    inputDialogResultDict = dict;
	
	[inputDialogResultKey release];
	inputDialogResultKey = [key copy];
	
	[myTableView reloadData];
}

- (void)loadView
{
	// create and configure the table view
	myTableView = [[UITableView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame] style:UITableViewStyleGrouped];	
	myTableView.delegate = self;
	myTableView.dataSource = self;
	myTableView.scrollEnabled = NO; // no scrolling in this case, we don't want to interfere with text view scrolling
	myTableView.autoresizesSubviews = YES;
	
	self.view = myTableView;
}

#define kFontName				@"Arial"
#define kTextViewFontSize		18.0

- (UITextView *)createUITextView
{
	UITextView *textView = [[[UITextView alloc] initWithFrame:CGRectZero] autorelease];
    textView.textColor = [UIColor blackColor];
    textView.font = [UIFont fontWithName:kFontName size:kTextViewFontSize];
    textView.delegate = self;
    textView.backgroundColor = [UIColor whiteColor];
	
	textView.text = @"";
	textView.returnKeyType = UIReturnKeyDefault;
	textView.keyboardType = UIKeyboardTypeDefault;	// use the default type input method (entire keyboard)
	// note: for UITextView, if you don't like autocompletion while typing use:
	// myTextView.autocorrectionType = UITextAutocorrectionTypeNo;
	
	return textView;
}


// UITextView delegate methods

- (void)textViewDidBeginEditing:(UITextView *)textView
{
}


- (void)doneAction:(id)sender
{
	[self.navigationController popViewControllerAnimated:YES];
}

- (void)clearAction:(id)sender
{
	TextViewCell *cell = (TextViewCell*)[myTableView cellForRowAtIndexPath:[NSIndexPath indexPathForRow:0 inSection:0]];
    cell.textView.text = @"";
}

// UITableView delegates

// if you want the entire table to just be re-orderable then just return UITableViewCellEditingStyleNone
//
- (UITableViewCellEditingStyle)tableView:(UITableView *)tableView editingStyleForRowAtIndexPath:(NSIndexPath *)indexPath
{
	return UITableViewCellEditingStyleNone;
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
	return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
	return 1;
}

// to determine specific row height for each cell, override this.  In this example, each row is determined
// by the its subviews that are embedded.
//
- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
	return kUITextViewCellRowHeight;
}

// to determine which UITableViewCell to be used on a given row.
//
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	static NSString* kTextViewCellId = @"TextViewCellId";
    
	TextViewCell *cell = (TextViewCell*)[myTableView dequeueReusableCellWithIdentifier:kTextViewCellId];
	if (cell == nil)
	{
		cell = [[[TextViewCell alloc] initWithReuseIdentifier:kTextViewCellId] autorelease];
		cell.textView = [self createUITextView];
		[cell.textView becomeFirstResponder];
	}
	
	if (inputDialogResultDict != nil && inputDialogResultKey != nil)
	{
		cell.textView.text = [inputDialogResultDict objectForKey:inputDialogResultKey];
	}
	else
	{
		cell.textView.text = @"";
	}
	
	return cell;
}

@end

