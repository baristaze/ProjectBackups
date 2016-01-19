-- delete from [dbo].[WorkerAgentProject] 
-- select * from [dbo].[WorkerAgentProject] 
INSERT INTO [dbo].[WorkerAgentProject]([Id], [IsActive], [Project], [WorkerClassFullName])
     VALUES (1, 1, 'AndroidPushNotifier', 'Pic4Pic.ObjectModel.AndroidNotificationPusher');
GO

INSERT INTO [dbo].[WorkerAgentProject]([Id], [IsActive], [Project], [WorkerClassFullName])
     VALUES (2, 1, 'LogProcessor', 'Pic4Pic.ObjectModel.LogProcessor');
GO

declare @index int;
set @index = 0;

-- delete from [dbo].[WorkerAgentSetup]
-- select * from [dbo].[WorkerAgentSetup]

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'ReferenceTimeZoneWinIndex', '5');

set @index = @index + 1;	
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'StartingTimeForPushes', '0');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EndingTimeForPushes', '1439');

set @index = @index + 1;	
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'DBConnectionString', 'SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-test-dbase');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'GoogleApiKey', 'AIzaSyAYVidqUWTItez8neiLhLblheyAN8S_RWk');

set @index = @index + 1;	
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'SleepSeconds', '5');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'SleepSecondsAfterTransmission', '1');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'HeartbeatLogFreq', '1');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'SendStatusEmailOnPushNotifications', 'true');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_Enabled', 'true');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_SmtpHost', 'smtp.gmail.com');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_SmtpPort', '587');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_SenderName', 'pic4pic Reporting');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_SenderEmail', 'appsicle@gmail.com');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_SenderPswd', '**********');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_UseSSL', '1');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_ToList', 'appsicle@gmail.com');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_CCList', '');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 0, 'EMail_BCCList', '');

GO

declare @index int;
set @index = 1000;

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'ReferenceTimeZoneWinIndex', '5');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'StartingTimeForPushes', '540'); -- 9am
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EndingTimeForPushes', '120'); -- 2am
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'DBConnectionString', 'SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-dbase');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'GoogleApiKey', 'AIzaSyAYVidqUWTItez8neiLhLblheyAN8S_RWk');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'SleepSeconds', '60');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'SleepSecondsAfterTransmission', '5');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'HeartbeatLogFreq', '1');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'SendStatusEmailOnPushNotifications', 'true');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_Enabled', 'true');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_SmtpHost', 'smtp.gmail.com');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_SmtpPort', '587');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_SenderName', 'pic4pic Reporting');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_SenderEmail', 'appsicle@gmail.com');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_SenderPswd', '******');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_UseSSL', '1');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_ToList', 'appsicle@gmail.com');

set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_CCList', '');
	
set @index = @index + 1;
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (@index, 1, 1, 'EMail_BCCList', '');

GO

----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5001, 2, 0, 'LogDBaseConnString', '');

INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5002, 2, 0, 'AzureLogTableConnString', '');

INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5003, 2, 0, 'ChunkSizeForLogRead', '100');


INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5101, 2, 1, 'LogDBaseConnString', 'SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-logs');

INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5102, 2, 1, 'AzureLogTableConnString', 'DefaultEndpointsProtocol=https;AccountName=ginger;AccountKey=ChC10hlx4G7EWz4tVwDgZKjmx56zbyy9TGDkVmUWfgS0EW4lBXbPmsNkDfte44WfDm60cYBRagG89cqFHpI8mA==');

INSERT INTO [dbo].[WorkerAgentSetup] ([Id], [ProjectId], [Environment], [ConfigName], [ConfigValue]) 
	VALUES (5103, 2, 1, 'ChunkSizeForLogRead', '100');

GO