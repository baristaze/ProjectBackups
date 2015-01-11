//
//  MessageSummaryCell.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/30/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "MessageSummaryCell.h"
#import "YummyViewConstants.h"
#import "MyDateTimeFormatter.h"

@implementation MessageSummaryCell

- (id)initWithReuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:UITableViewCellStyleDefault reuseIdentifier:reuseIdentifier];
    
    if (self) 
	{
		NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
		
        _from = [[UILabel alloc] initWithFrame:CGRectZero];
        _from.font = [UIFont boldSystemFontOfSize:17];
        _from.textAlignment = UITextAlignmentLeft;
		_from.lineBreakMode = UILineBreakModeWordWrap;
        _from.backgroundColor = [UIColor clearColor];
		_from.textColor = [UIColor blackColor];
		
        _date = [[UILabel alloc] initWithFrame:CGRectZero];
        _date.font = [UIFont systemFontOfSize:14];
        _date.textAlignment = UITextAlignmentRight;
		_date.lineBreakMode = UILineBreakModeCharacterWrap;
        _date.backgroundColor = [UIColor clearColor];
		_date.textColor = [UIColor colorWithRed:31.0/255.0 green:97.0/255.0 blue:186.0/255.0 alpha:1.0];

        _subject = [[UILabel alloc] initWithFrame:CGRectZero];
		_subject.font = [UIFont systemFontOfSize:14];
        _subject.textAlignment = UITextAlignmentLeft;
        _subject.backgroundColor = [UIColor clearColor];
		_subject.textColor = [UIColor blackColor];
        
        _body = [[UILabel alloc] initWithFrame:CGRectZero];
		_body.font = [UIFont systemFontOfSize:14];
        _body.textAlignment = UITextAlignmentLeft;
		_body.lineBreakMode = UILineBreakModeWordWrap;
        _body.backgroundColor = [UIColor clearColor];
		_body.textColor = [UIColor grayColor];
		_body.numberOfLines = 0; // Make this multi line
        
        //_checkImage = [[UIImageView alloc] initWithImage:[[YummyViewConstants singleton] getMultiEditUnSelectedCheckImage]];
        //_checkImage.hidden = YES;

        [self.contentView addSubview:_from];
        [self.contentView addSubview:_subject];
        [self.contentView addSubview:_body];
        //[self.contentView addSubview:_checkImage];
        [self.contentView addSubview:_date];
		
        //_inPseudoEditMode = NO;
        //_selected = NO;
        
		[pool release];
    }
    return self;
}


- (void)dealloc 
{
    [_date release];
    //[_checkImage release];
    [_from release];
    [_subject release];
	[_body release];
    [super dealloc];
}


- (void)layoutSubviews 
{
    [super layoutSubviews];
    
    //const CGFloat CHECKBOX_WIDTH        = 30.0;
    //const CGFloat CHECKBOX_HEIGHT       = 30.0;
    //const CGFloat CHECKBOX_LEFT_OFFSET  = 5.0;
    const CGFloat LEFT_OFFSET           = 15.0;
    const CGFloat RIGHT_OFFSET          = 15.0;
    const CGFloat VERTICAL_OFFSET       = 7.0;
    const CGFloat FROM_HEIGHT           = 24.0;
    const CGFloat SUBJECT_HEIGHT        = 18.0;
    const CGFloat DATE_WIDTH            = 70.0;
    const CGFloat DATE_HEIGHT           = 18.0;
    
    CGFloat contentWidth = self.contentView.bounds.size.width;
    CGFloat contentHeight = self.contentView.bounds.size.height;
    
    CGFloat leftOffset = LEFT_OFFSET;
    
    /*bool stateChanged = (_inPseudoEditMode == _checkImage.hidden);
    if (stateChanged)
    {
        [UIView beginAnimations:nil context:nil];
        [UIView setAnimationBeginsFromCurrentState:YES];
    }

    if (_inPseudoEditMode)
    {
        leftOffset += (CHECKBOX_WIDTH + CHECKBOX_LEFT_OFFSET);
        _checkImage.hidden = NO;
    }
    else
    {
        _checkImage.hidden = YES;
    }
    
    _checkImage.frame = CGRectMake(CHECKBOX_LEFT_OFFSET, (contentHeight - CHECKBOX_WIDTH) / 2.0 , CHECKBOX_WIDTH, CHECKBOX_HEIGHT);
     */
    
	_from.frame = CGRectMake(leftOffset,
                             VERTICAL_OFFSET, 
                             contentWidth - DATE_WIDTH - RIGHT_OFFSET - leftOffset, 
                             FROM_HEIGHT);
	
	_date.frame = CGRectMake(contentWidth - VERTICAL_OFFSET - DATE_WIDTH,
                             VERTICAL_OFFSET + 5.0, 
                             DATE_WIDTH, 
                             DATE_HEIGHT);

	_subject.frame = CGRectMake(leftOffset,
                                VERTICAL_OFFSET + FROM_HEIGHT,
                                contentWidth - RIGHT_OFFSET - leftOffset, 
                                SUBJECT_HEIGHT);
    
    // Align the body to the top
    CGSize maxBodySize = CGSizeMake(contentWidth - RIGHT_OFFSET - leftOffset, 
                                    contentHeight - FROM_HEIGHT - SUBJECT_HEIGHT - VERTICAL_OFFSET - VERTICAL_OFFSET);
    
    CGSize bodySize = [_body.text sizeWithFont:_date.font 
                            constrainedToSize:maxBodySize 
                                 lineBreakMode:_date.lineBreakMode];
    
	_body.frame = CGRectMake(leftOffset, 
                             VERTICAL_OFFSET + FROM_HEIGHT + SUBJECT_HEIGHT, 
                             bodySize.width, 
                             bodySize.height);
    
    /*if (_selected)
    {
        _checkImage.image = [[YummyViewConstants singleton] getMultiEditSelectedCheckImage];
    }
    else
    {
        _checkImage.image = [[YummyViewConstants singleton] getMultiEditUnSelectedCheckImage];
    }

    if (stateChanged)
    {
        [UIView commitAnimations];
    }*/
}


- (void)setHighlighted:(BOOL)highlighted animated:(BOOL)animated
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
    [super setHighlighted:highlighted animated:animated];

    if (highlighted) //&& (!_inPseudoEditMode))
    {
        _from.textColor = [UIColor whiteColor];
        _subject.textColor = [UIColor whiteColor];
        _body.textColor = [UIColor whiteColor];
        _date.textColor = [UIColor whiteColor];
    }
    else
    {
        _from.textColor = [UIColor blackColor];
        _subject.textColor = [UIColor blackColor];
        _body.textColor = [UIColor grayColor];
        _date.textColor = [UIColor colorWithRed:31.0/255.0 green:97.0/255.0 blue:186.0/255.0 alpha:1.0];
    }

	[pool release];
}


- (void)setSubject:(NSString*)subject body:(NSString*)body from:(NSString*)from date:(NSDate*)date
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    
    _from.text = from;
    _subject.text = subject;
    
    NSString* bodySum = [body stringByReplacingOccurrencesOfString:@"<br/>" withString:@" "];
    bodySum = [bodySum stringByReplacingOccurrencesOfString:@"\n" withString:@" "];
    _body.text = bodySum;
    
    NSCalendar *currentCalendar = [NSCalendar currentCalendar];

    NSDateComponents *dateComponents = 
    [currentCalendar components:(NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | NSHourCalendarUnit | NSMinuteCalendarUnit) 
                       fromDate:date];

    NSDate *today = [NSDate date];
    NSDateComponents *todayComponents = 
    [currentCalendar components:(NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | NSHourCalendarUnit | NSMinuteCalendarUnit) fromDate:today];
        
    NSTimeInterval secondsPerDay = 24 * 60 * 60;
    
    NSDate *yesterday = NULL;
    if ([today respondsToSelector:@selector(dateByAddingTimeInterval:)]) 
    {
        yesterday = [today dateByAddingTimeInterval:(-1 * secondsPerDay)];
    }
    else 
    {
        // deprecated method
        yesterday = [today addTimeInterval:(-1 * secondsPerDay)];
    }
	
	NSDateComponents *yesterdayComponents = [currentCalendar components:(NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit) fromDate:yesterday];	

    if ([todayComponents day] == [dateComponents day] &&
        [todayComponents month] == [dateComponents month] &&
        [todayComponents year] == [dateComponents year])
    {
        _date.text = [MyDateTimeFormatter shortTimeStringFromDate:date];
	}
    else if ([yesterdayComponents day] == [dateComponents day] &&
            [yesterdayComponents month] == [dateComponents month] &&
            [yesterdayComponents year] == [dateComponents year])
    {
        _date.text = @"Yesterday";
    }
    else
    {
        NSDateFormatter *dateFormatter = [[[NSDateFormatter alloc] init] autorelease];
        [dateFormatter setDateFormat:@"MM/dd"];
        _date.text = [dateFormatter stringFromDate:date];
    }

    [pool release];
}


/*- (void)setSelected:(BOOL)selected
{
    if (_selected != selected)
    {
        _selected = selected;
        
        if (_selected)
        {
            _checkImage.image = [[YummyViewConstants singleton] getMultiEditSelectedCheckImage];
        }
        else
        {
            _checkImage.image = [[YummyViewConstants singleton] getMultiEditUnSelectedCheckImage];
        }
    }
}


- (void)setInPseudoEditMode:(BOOL)inPseudoEditMode
{
    if (_inPseudoEditMode != inPseudoEditMode)
    {
        _inPseudoEditMode = inPseudoEditMode;
        
        [self setNeedsDisplay];
    }
}*/

@end
