USE [YummyZone];
GO

CREATE TABLE [dbo].[Diner] (
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[UserName] nvarchar(100) NOT NULL UNIQUE,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled, Removed
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
)
GO

CREATE INDEX [DinerIndex1] ON [dbo].[Diner]([Id]) INCLUDE ([UserName], [Status]);
GO

CREATE TABLE [dbo].[DinerPassword] (
	[UserName] nvarchar(100) NOT NULL,
	[Password] nvarchar(100) NOT NULL,
	
	CONSTRAINT [DinerPassword_ForeignKey_UserName] FOREIGN KEY([UserName]) 
		REFERENCES [dbo].[Diner]([UserName]) ON UPDATE CASCADE,
)
GO

CREATE INDEX [DinerPasswordIndex1] ON [dbo].[DinerPassword]([UserName]) INCLUDE ([Password]);
GO

CREATE TABLE [dbo].[NotificationClient] (
	[DinerId] uniqueidentifier NOT NULL, 
	[DeviceType] tinyint NOT NULL DEFAULT 0,
	[DeviceToken] varchar(128) NOT NULL PRIMARY KEY NONCLUSTERED,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [NotificationClient_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
)
GO

/*
CREATE TABLE [dbo].[DinerProfile] (
	[DinerId] uniqueidentifier NOT NULL, -- top level owner
	[FirstName] nvarchar(50) NULL,
	[LastName] nvarchar(50) NULL,
	[DisplayName] nvarchar(100) NULL,
	[EmailAddress] nvarchar(200) NULL,
	[PhoneNumber] varchar(30) NULL,
	[PhotoURL] varchar(500) NULL,
	[MobileDeviceType] nvarchar(100) NULL,
	[MobileDeviceOsVersion] varchar(10) NULL,
	[ClientAppVersion] varchar(20) NULL,
	[Language] varchar(10) NULL,
	[FavoritesBar] tinyint NOT NULL DEFAULT 4,
	[AutoFavorites] bit NOT NULL DEFAULT 0,
	[SocialMediaShareFlags] int NOT NULL DEFAULT 0,	-- None, Facebook, Twitter,
	[ReviewSitesShareFlags] int NOT NULL DEFAULT 0,	-- None, YummyZone, Yelp, Zagat, Urbanspoon
	[DietTypeFlags] int NOT NULL DEFAULT 0, -- unspecified, kosher, halal, vegan, vegetarian, gluten free, low fat, low salt, organic
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [DinerProfile_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
)

CREATE TABLE [dbo].[DinerSpecialDay] (
	[DinerId] uniqueidentifier NOT NULL, -- top level owner
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Type] tinyint NOT NULL DEFAULT 0, -- Unspecified, Birthday, WeddingAnniversary,
	[Name] nvarchar(50) NULL,
	[Day] tinyint NOT NULL,
	[Month]	tinyint NOT NULL,
	[Year] smallint NULL, -- for one time only special days
	
	CONSTRAINT [DinerSpecialDay_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
)

CREATE TABLE [dbo].[SpecialDay] (
	[Type] tinyint NOT NULL DEFAULT 0, -- Unspecified, ValentineDay, Thanksgiving, ChristmasEve, NewYear, ForthOfJuly,
	[Name] nvarchar(50) NULL,
	[Day] tinyint NOT NULL,
	[Month]	tinyint NOT NULL,
	[Year] smallint NULL,
)
*/

CREATE TABLE [dbo].[CheckIn] (
	[DinerId] uniqueidentifier NOT NULL, -- top level owner
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[VenueId] uniqueidentifier NOT NULL,
	[TimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[AuditFlag] tinyint NOT NULL DEFAULT 0, -- Not Audited (0), Genuine(1), Suspicious(2), Spam(3), Deleted(4)
	
	CONSTRAINT [CheckIn_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
	CONSTRAINT [CheckIn_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]),
)
GO

CREATE INDEX [CheckInIndex1] ON [dbo].[CheckIn]([Id]) INCLUDE ([DinerId], [VenueId], [TimeUTC]);
GO

CREATE TABLE [dbo].[MenuItemRate] (
	[CheckInId] uniqueidentifier NOT NULL,
	[MenuItemId] uniqueidentifier NOT NULL,
	[Rate] tinyint NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(), -- checkin'den ne kadar sonra feedback vermis
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(), -- verdigi feedback'i degistirmis mi?
	
	CONSTRAINT [MenuItemRate_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]),	
	CONSTRAINT [MenuItemRate_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]),
)
GO

CREATE INDEX [MenuItemRateIndex1] ON [dbo].[MenuItemRate]([MenuItemId]) INCLUDE ([CheckInId], [Rate]);
GO
