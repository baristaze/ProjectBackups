//
//  MenuItemViewController.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/4/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "ImageDownloader.h"

@interface MenuItemViewController : UIViewController <ImageDownloaderDelegate, UITextViewDelegate> 
{
    NSMutableDictionary *_itemDetails;
    ImageDownloader *_imageDownloader;
    UIImage *_image;

    UIImageView *_imageView;
    UITextView *_textView;
}

- (id)initWithItemDetails:(NSDictionary*)itemDetails;

@end
