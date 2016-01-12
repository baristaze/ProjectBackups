
CREATE TABLE [dbo].[NotificationClient] (
	[DinerId] uniqueidentifier NOT NULL, 
	[DeviceType] tinyint NOT NULL DEFAULT 0,
	[DeviceToken] varchar(128) NOT NULL PRIMARY KEY CLUSTERED,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [NotificationClient_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
)
GO

ALTER TABLE [dbo].[NotificationClient] ADD CONSTRAINT [NotificationClient_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]);
GO

ALTER TABLE [dbo].[Venue]
	ADD [SendFeedback_RangeLimitInMiles] decimal(18,14) NULL;
GO