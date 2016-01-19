/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[Split]
CREATE TABLE [dbo].[Split]
(
	[Id] int NOT NULL PRIMARY KEY,
	[IsActive] bit NOT NULL DEFAULT 1,
	[Description] nvarchar (200) NULL,
	[IsDefault] bit NOT NULL DEFAULT 0
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[TempSplitMap]
CREATE TABLE [dbo].[TempSplitMap] 
(
	[ClientId] uniqueidentifier NOT NULL PRIMARY KEY,
	[SplitId] int NOT NULL DEFAULT 0,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE()
)
GO