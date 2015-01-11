#import "MyPathHelper.h"

static NSString *_rootDataFolder = nil;

@implementation MyPathHelper

+ (void)initialize
{
	BOOL needInitialization = (_rootDataFolder == nil);
	if (!needInitialization)
		return;
	
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];	
	NSFileManager *fileManager = [NSFileManager defaultManager];
    
    if (_rootDataFolder == nil)
	{
		NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
		_rootDataFolder = [[paths objectAtIndex:0] stringByAppendingPathComponent:@"Data"];
		
		// Need to retain it otherwise pool will get rid of it
		[_rootDataFolder retain];
		NSLog(@"_rootDataPath is %@", _rootDataFolder);
		
		BOOL isDirectory;
		if (![fileManager fileExistsAtPath:_rootDataFolder isDirectory:&isDirectory] || !isDirectory) 
		{
            [fileManager createDirectoryAtPath:_rootDataFolder withIntermediateDirectories:YES attributes:nil error:NULL];
		}
		
		// Set the current directory to the _rootDataPath
		[fileManager changeCurrentDirectoryPath:_rootDataFolder];
		//NSLog(@"Current directory is %@", [fileManager currentDirectoryPath]);
	}
	
	[pool release];
}


+(id)alloc
{
	return [super alloc];
}


+ (NSString*)rootRepository { return _rootDataFolder; }


+ (NSString *)filePathForAccountInfo
{
	return [NSString stringWithFormat:@"%@/AccountInfo.xml", [MyPathHelper rootRepository]];
}

+ (NSString *)tempFilePathForHttpPost
{
	return [NSString stringWithFormat:@"%@/HttpPostTemp.xml", [MyPathHelper rootRepository]];
}

@end
