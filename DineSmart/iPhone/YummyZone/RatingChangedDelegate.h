//
//  RatingChangedDelegate.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/22/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@protocol RatingChangedDelegate

- (void)ratingChanged:(int)rating key:(NSObject*)key;

@end
