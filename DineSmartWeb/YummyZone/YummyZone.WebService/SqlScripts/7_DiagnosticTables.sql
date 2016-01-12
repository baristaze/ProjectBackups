
USE [YummyZone];
GO

CREATE TABLE [dbo].[Log] (
	[TimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[App] tinyint NOT NULL,
	[Type] tinyint NOT NULL,
	[Operation] varchar(50) NOT NULL,
	[ContextId] uniqueidentifier NULL,
	[Message] nvarchar(max) NULL
)
GO