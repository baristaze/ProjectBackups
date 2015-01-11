//
//  UIImage+Resize.m
//  Dine Smart
//


#import "UIImage+Resize.h"


@implementation UIImage (Resize)

+ (id)imageUsingImage:(UIImage*)image width:(CGFloat)width height:(CGFloat)height onlyShrink:(BOOL)onlyShrink
{
    BOOL needResize = YES;
    if (image.size.width == width && image.size.height == height)
    {
        needResize = NO;
    }
    else if (onlyShrink && (image.size.width < width && image.size.height < height))
    {
        needResize = NO;
    }
    
    if (needResize)
    {
		UIGraphicsBeginImageContext(CGSizeMake(width, height));
		CGRect imageRect = CGRectMake(0.0, 0.0, width, height);
		[image drawInRect:imageRect];
		UIImage *scaledImage = UIGraphicsGetImageFromCurrentImageContext();
		UIGraphicsEndImageContext();
        return scaledImage;
    }
    else
    {
        return [[image retain] autorelease];
    }
}

@end
