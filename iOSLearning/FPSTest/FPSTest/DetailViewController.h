//
//  DetailViewController.h
//  FPSTest
//
//  Created by Baris Taze on 3/18/15.
//  Copyright (c) 2015 Baris Taze. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DetailViewController : UIViewController

@property (strong, nonatomic) id detailItem;
@property (weak, nonatomic) IBOutlet UILabel *detailDescriptionLabel;

@end

