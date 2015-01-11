#import "SingleSelectKeyedListViewController.h"
#import "YummyZoneUtils.h"


@interface SingleSelectKeyedListViewController(private)

- (int)getSelectionIndexFromKey:(NSString*)key;

@end


@implementation SingleSelectKeyedListViewController

- (id)init
{
    self = [super init];
	if (self)
	{
		self.title = @"";
        _inputArray = nil;
        _inputDisplayKey = nil;
        _inputResultKey = nil;
        _outputDict = nil;
        _outputKey = nil;

		_selectionIndex = -1;
	}
	return self;
}

- (void)dealloc 
{
    _tableView.delegate = nil;
    _tableView.dataSource = nil;
	
	[_inputArray release];
	[_inputDisplayKey release];
	[_inputResultKey release];
	[_outputDict release];
    [_outputKey release];
	[_tableView release];
    [super dealloc];
}


- (void)setInputArray:(NSArray*)inputArray inputDisplayKey:(NSString*)inputDisplayKey 
       inputResultKey:(NSString*)inputResultKey outputDict:(NSMutableDictionary*)outputDict
            outputKey:(NSString*)outputKey title:(NSString*)title
{
	self.title = title;
	
	[inputArray retain]; [_inputArray release]; _inputArray = inputArray;
	[inputDisplayKey retain]; [_inputDisplayKey release]; _inputDisplayKey = inputDisplayKey;
	[inputResultKey retain]; [_inputResultKey release]; _inputResultKey = inputResultKey;
	[outputDict retain]; [_outputDict release]; _outputDict = outputDict;
	[outputKey retain]; [_outputKey release]; _outputKey = outputKey;
    
    _selectionIndex = [self getSelectionIndexFromKey:[outputDict objectForKey:outputKey]];
    
	[_tableView reloadData];
}


- (int)getSelectionIndexFromKey:(NSString*)key
{
    for (int i = 0; i < [_inputArray count]; i++)
    {
        NSDictionary *item = [_inputArray objectAtIndex:i];
        if ([[item objectForKey:_inputResultKey] caseInsensitiveCompare:key] == NSOrderedSame)
        {
            return i;
        }
    }
    return -1;
}


- (void)viewWillAppear:(BOOL)animated
{
	[super viewWillAppear:animated];
}


- (void)viewDidLoad 
{
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
	
	self.view = _tableView;
}


- (void)viewWillDisappear:(BOOL)animated
{
    if (_selectionIndex >= 0)
    {
        NSDictionary *selectedItemData = [_inputArray objectAtIndex:_selectionIndex];
        NSString *resultToWrite = [selectedItemData objectForKey:_inputResultKey];
        [_outputDict setValue:resultToWrite forKey:_outputKey];
    }
	
	[super viewWillDisappear:animated];
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
	return 1;
}


- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
	if (_inputArray != nil)
    {
		return [_inputArray count];
    }
	else
    {
		return 0;
    }
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	static NSString *kTextOnlyCellId = @"TextOnlyCellId";
	
	UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:kTextOnlyCellId];
    if (cell == nil) 
	{
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:kTextOnlyCellId] autorelease];
    }
	
    NSDictionary *selectedItemData = [_inputArray objectAtIndex:indexPath.row];
    cell.textLabel.text = [selectedItemData objectForKey:_inputDisplayKey];
    
    if (_selectionIndex == indexPath.row)
    {
        cell.accessoryType = UITableViewCellAccessoryCheckmark;
    }
    else
    {
        cell.accessoryType = UITableViewCellAccessoryNone;
    }
    
	return cell;
}


- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath 
{
	[tableView deselectRowAtIndexPath:indexPath animated:NO];
	
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	if (_selectionIndex != indexPath.row)
	{
        if (_selectionIndex >= 0)
        {
            NSIndexPath *oldIndexPath = [NSIndexPath indexPathForRow:_selectionIndex inSection:0];
            UITableViewCell *oldCell = [tableView cellForRowAtIndexPath:oldIndexPath];
            oldCell.accessoryType = UITableViewCellAccessoryNone;
        }
        
		_selectionIndex = indexPath.row;

        UITableViewCell *newCell = [tableView cellForRowAtIndexPath:indexPath];
        newCell.accessoryType = UITableViewCellAccessoryCheckmark;
	}
    
	[self.navigationController popViewControllerAnimated:YES];
    
	[pool release];
}


@end
