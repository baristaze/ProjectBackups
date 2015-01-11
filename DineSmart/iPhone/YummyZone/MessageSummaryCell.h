//
//  MessageSummaryCell.h
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/30/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface MessageSummaryCell : UITableViewCell
{
    UILabel *_subject;
    UILabel *_body;
    UILabel *_from;
    UILabel *_date;
    //UIImageView *_checkImage;
    
    //BOOL _inPseudoEditMode;
    //BOOL _selected;
}

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier;

- (void)setSubject:(NSString*)subject body:(NSString*)body from:(NSString*)from date:(NSDate*)date;

//- (void)setInPseudoEditMode:(BOOL)inPseudoEditMode;
//- (void)setSelected:(BOOL)selected;

@end
