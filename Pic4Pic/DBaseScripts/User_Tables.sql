/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[User]
CREATE TABLE [dbo].[User] 
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[Type] tinyint NOT NULL DEFAULT 1, /* 0=Guest, 1=Regular, 2=Editor, 3=Moderator, 4=PowerUser, 5=SystemAdmin */
	[Status] tinyint NOT NULL DEFAULT 0, /* 0=Partial, 1=Active, 2=Suspended, 3=Disabled, 4=Deleted */
	[SplitId] int NOT NULL DEFAULT 0,
	[Username] nvarchar(20) NOT NULL,
	[Password] nvarchar(20) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [User_FK_SplitId] FOREIGN KEY (SplitId) 
		REFERENCES [dbo].[Split]([Id]) ON UPDATE CASCADE,
)
GO

-- DROP INDEX [UsernameUniqueIndex] ON [dbo].[User];
CREATE UNIQUE INDEX [UsernameUniqueIndex] ON [dbo].[User]([Username]);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[UserDetails]
CREATE TABLE [dbo].[UserDetails] 
(
	[UserId] uniqueidentifier NOT NULL PRIMARY KEY,
	[Description] nvarchar(max) NULL,
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [UserDetails_FK_UserId] FOREIGN KEY ([UserId]) 
		REFERENCES [dbo].[User]([Id]),
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[Preference]
CREATE TABLE [dbo].[Preference] 
(
	[UserId] uniqueidentifier NOT NULL PRIMARY KEY,
	[InterestedIn] tinyint NOT NULL DEFAULT 0, /* 0=Unspecified, 1=Man, 2=Woman, 3=ManOrWoman */
	--	[AgeDiffLowerLimit] tinyint NULL, /* -25 */
	--	[AgeDiffUpperLimit] tinyint NULL, /* +25 */
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [Preference_FK_UserId] FOREIGN KEY ([UserId]) 
		REFERENCES [dbo].[User]([Id]),
)
GO


/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[MobileDevice]
CREATE TABLE [dbo].[MobileDevice] 
(
	[ClientId] uniqueidentifier NOT NULL PRIMARY KEY,
	[UserId] uniqueidentifier NULL,
	[OSType] tinyint NOT NULL DEFAULT 0, /* 0=Unknown, 1=Android, 2=iOS, 3=Windows */
	[OSVersion] varchar(50) NULL,
	[AppVersion] varchar(10) NULL,
	[SDKVersion] varchar(10) NULL,
	[DeviceType] varchar(50) NULL,
	[PushNotifRegId] varchar(max) NULL,	/* variable length */

	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [MobileDevice_FK_UserId] FOREIGN KEY ([UserId]) 
		REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [MobileDeviceIndex1] ON [dbo].[MobileDevice];
CREATE INDEX [MobileDeviceIndex1] ON [dbo].[MobileDevice]([UserId]) INCLUDE ([PushNotifRegId], [LastUpdateTimeUTC]);
GO