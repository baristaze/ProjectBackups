
-- DROP TABLE [dbo].[ProcessedLogs]
CREATE TABLE [dbo].[ProcessedLogs]
(
	[LogId] uniqueidentifier NOT NULL PRIMARY KEY,
	[TimeUTC] datetime NOT NULL,
	[ClientId] uniqueidentifier NULL,
	[UserId] uniqueidentifier NULL,
	[SplitId] int NULL,
	[Level] varchar(10) NULL,
	[Platform] varchar(20) NULL,
	[Version] varchar(5) NULL,
	[File] varchar(255) NULL,
	[Class] varchar(255) NULL,
	[Method] varchar(255) NULL,
	[Line] int NULL,
	[Message] nvarchar(max) NULL,
	[Exception] nvarchar(max) NULL,
	[ElapsedTimeMSec] int NULL,
	[Funnel] varchar(50) NULL,
	[Page] varchar(50) NULL,
	[Action] varchar(50) NULL,
	[Step] varchar(50) NULL,
	[Full] nvarchar(max) NULL,
)
GO

-- DROP INDEX [ProcessedLogsIndex] ON [dbo].[ProcessedLogs];
CREATE INDEX [ProcessedLogsIndex] ON [dbo].[ProcessedLogs]([TimeUTC], [ClientId], [UserId], [Level]);
GO

---------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------
-- DROP TABLE [dbo].[UnProcessedLogs]
CREATE TABLE [dbo].[UnProcessedLogs]
(
	[RowKey] varchar(255) NOT NULL PRIMARY KEY,
	[TimeUTC] datetime NOT NULL,
	[Message] nvarchar(max) NULL,
)
GO

-- DROP INDEX [UnprocessedLogsIndex] ON [dbo].[UnProcessedLogs];
CREATE INDEX [UnprocessedLogsIndex] ON [dbo].[UnProcessedLogs]([TimeUTC]);
GO
