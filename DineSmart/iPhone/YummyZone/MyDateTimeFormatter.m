//
//  MyDateTimeFormatter.m
//  YummyZone
//
//  Created by Mustafa Demirhan on 5/30/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "MyDateTimeFormatter.h"


@implementation MyDateTimeFormatter

+ (NSString*)shortTimeStringFromDate:(NSDate*)date
{
	NSCalendar *currentCalendar = [NSCalendar currentCalendar];
	NSDateComponents *dateComponents = [currentCalendar components:(NSYearCalendarUnit | NSMonthCalendarUnit | NSDayCalendarUnit | 
																	NSHourCalendarUnit | NSMinuteCalendarUnit | NSSecondCalendarUnit) fromDate:date];
	
	NSString *minuteStr = nil;
	if ([dateComponents minute] < 10)
		minuteStr = [NSString stringWithFormat:@"0%d", [dateComponents minute]];
	else
		minuteStr = [NSString stringWithFormat:@"%d", [dateComponents minute]];
	
	
	if ([dateComponents hour] == 0)
		return [NSString stringWithFormat:@"12:%@ am", minuteStr];
	else if ([dateComponents hour] < 12)
		return [NSString stringWithFormat:@"%d:%@ am", [dateComponents hour], minuteStr];
	else if ([dateComponents hour] == 12)
		return [NSString stringWithFormat:@"12:%@ pm", minuteStr];
	else
		return [NSString stringWithFormat:@"%d:%@ pm", ([dateComponents hour] - 12), minuteStr];
}

@end
