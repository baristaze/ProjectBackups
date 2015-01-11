//
//  MenuItemViewController.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/4/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "MenuItemViewController.h"
#import "KeyConstants.h"
#import "UIImage+Resize.h"
#import "YummyZoneUrls.h"
#import "YummyZoneSession.h"
#import "YummyZoneHelper.h"
#import "YummyZoneUtils.h"

@interface MenuItemViewController(private)

- (void)loadData;
- (CGSize) calculateDescSize;

@end


@implementation MenuItemViewController

- (id)initWithItemDetails:(NSDictionary*)itemDetails
{
    self = [super init];
    
    if (self) 
    {
        _itemDetails = [itemDetails copy];
        _imageDownloader = nil;
        _image = nil;
        [self setTitle:[_itemDetails objectForKey:kKeyName]];
    }
    return self;
}


- (void)dealloc
{
    if (_imageDownloader != nil)
    {
        [_imageDownloader setDelegate:nil];
    }
    
    [_itemDetails release];
    [_imageDownloader release];
    [_image release];
    [_imageView release];
    [_textView release];
    [super dealloc];
}


- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
}


#define IMAGE_WIDTH             300.0
#define IMAGE_HEIGHT			260.0
#define TOP_OFFSET              10.0
#define TEXT_IMAGE_SEPARATION   10.0

- (void)loadView 
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	CGRect screenRect = [[UIScreen mainScreen] applicationFrame];
	
	UIView *contentView = [[UIView alloc] initWithFrame:screenRect];
	contentView.autoresizesSubviews = YES;
	contentView.autoresizingMask = (UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight);
	contentView.backgroundColor = [UIColor whiteColor];
	self.view = contentView;
	[contentView release];
	
    _imageView = [[UIImageView alloc] init];
    _imageView.frame = CGRectMake((self.view.bounds.size.width - IMAGE_WIDTH) / 2, TOP_OFFSET, IMAGE_WIDTH, IMAGE_HEIGHT);
    [self.view addSubview:_imageView];
    
    CGFloat visibleDescHeight = self.view.bounds.size.height - IMAGE_HEIGHT - TOP_OFFSET;
    CGSize descSize = self.calculateDescSize;
    if(descSize.height < visibleDescHeight)
    {
        descSize.height = visibleDescHeight;
    }
    
    _textView = [[UITextView alloc] init];
    _textView.frame = CGRectMake(0, 
                                 TOP_OFFSET + IMAGE_HEIGHT + TEXT_IMAGE_SEPARATION, 
                                 self.view.bounds.size.width,
                                 descSize.height);
    _textView.delegate = self;
    [self.view addSubview:_textView];
    
    // Now that we created the controls, we can start populating the data
    [self loadData];

    [pool release];
}


- (void)viewDidLoad
{
	[YummyZoneUtils loadBackButton:[self navigationItem]];
    [super viewDidLoad];
}


- (void)viewWillAppear:(BOOL)animated
{
	self.navigationController.toolbarHidden = YES;
    [super viewWillAppear:animated];
}


- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}


- (BOOL)textViewShouldBeginEditing:(UITextView *)textView
{
    return NO;
}

- (CGSize) calculateDescSize
{
    NSString* desc = nil;
    if ([_itemDetails objectForKey:kKeyLongDescription] != nil)
    {
        desc = [_itemDetails objectForKey:kKeyLongDescription];
    }
    else
    {
        desc = [_itemDetails objectForKey:kKeyShortDesc];
    }
    
    if(desc == nil || desc.length == 0)
    {
        desc = @" ";
    }
    
    CGSize size = [desc sizeWithFont:[UIFont systemFontOfSize:14] 
                  constrainedToSize:CGSizeMake(self.view.bounds.size.width,300.0) 
                       lineBreakMode:UILineBreakModeWordWrap];
    return size;
}

- (void)loadData
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    NSString* urlText = [_itemDetails objectForKey:kKeyImageUrl];
    if(urlText == nil || urlText.length == 0)
    {
        [_imageView setImage:[UIImage imageNamed:@"plateNoPhotoBig.png"]];
    }
    else 
    {
        if (_image == nil)
        {
            if (_imageDownloader == nil)
            {
                [_imageView setImage:[UIImage imageNamed:@"plateDownloadingBig.png"]];
                
                _imageDownloader = [[ImageDownloader alloc] 
                                    initWithUrl:[NSURL URLWithString:urlText] 
                                    delegate:self key:[NSNull null]];
            }
            
            if (![_imageDownloader isImageReady])
            {
                [_imageView setImage:[UIImage imageNamed:@"plateDownloadingBig.png"]];
            }
            
            [_imageDownloader startDownload];
        }
        else
        {
            [_imageView setImage:_image];
        }
    }    
    
    if ([_itemDetails objectForKey:kKeyLongDescription] != nil)
    {
        [_textView setFont:[UIFont systemFontOfSize:14]];
        [_textView setText:[_itemDetails objectForKey:kKeyLongDescription]];
    }
    else
    {
        [_textView setFont:[UIFont systemFontOfSize:14]];
        [_textView setText:[_itemDetails objectForKey:kKeyShortDesc]];
    }
                                         
    [pool release];
}


- (void)imageDidLoad:(UIImage*)image key:(NSObject *)key
{
	NSLog(@"+imageDidLoad");
    
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];

    _image = [[UIImage imageUsingImage:image width:IMAGE_WIDTH height:IMAGE_HEIGHT onlyShrink:NO] retain];
    [_imageView setImage:_image];
    
    [pool release];
}


- (void)imageFailedToLoad:(NSString*)error key:(NSObject *)key
{
	NSLog(@"+imageFailedToLoad: Error: %@", error);
    
    [_imageView setImage:[UIImage imageNamed:@"plateNoPhotoBig.png"]];
}

@end
