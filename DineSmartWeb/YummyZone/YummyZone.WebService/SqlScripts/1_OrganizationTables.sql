
USE [YummyZone];
GO

CREATE TABLE [dbo].[Group] (
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Status] tinyint NOT NULL DEFAULT 0, -- Draft, Active, Disabled, Removed (we don't delete phsically)
	[Name] nvarchar(100) NOT NULL,
	[WebURL] varchar(300) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
)
GO

CREATE INDEX [GroupIndex1] ON [dbo].[Group]([Id]) INCLUDE ([Status]);
GO

CREATE TABLE [dbo].[Chain] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Status] tinyint NOT NULL DEFAULT 0, -- Draft, Active, Disabled, Removed (we don't delete phsically)
	[Name] nvarchar(100) NOT NULL,
	[VenueTypeFlags] int NOT NULL DEFAULT 0,
	[CuisineTypeFlags] int NOT NULL DEFAULT 0,
	[WebURL] varchar(500) NULL,
	[LogoURL] varchar(500) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [Chain_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

CREATE INDEX [ChainIndex1] ON [dbo].[Chain]([Id]) INCLUDE ([GroupId], [Status]);
GO

CREATE TABLE [dbo].[LogoImage] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[ChainId] uniqueidentifier NOT NULL,
	[ContentType] varchar(255) NOT NULL,
	[ContentLength] int NOT NULL,
	[InitialFileNameOrUrl] varchar(500) NULL,
	[Data] image NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [LogoImage_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [LogoImage_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]) ON DELETE CASCADE,
)
GO

CREATE INDEX [LogoImageIndex1] ON [dbo].[LogoImage]([ChainId]) INCLUDE ([GroupId]);
GO

CREATE TABLE [dbo].[Venue] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[ChainId] uniqueidentifier NOT NULL,
	[Status] tinyint NOT NULL DEFAULT 0, -- Draft, Active, Disabled, Removed (we don't delete phsically)
	[Name] nvarchar(100) NOT NULL,
	[Latitude] decimal(18,14) NULL,
	[Longitude] decimal(18,14) NULL,
	[MapURL] varchar(500) NULL,
	[WebURL] varchar(500) NULL,	
	[TimeZoneWinIndex] tinyint NULL, -- see TimeZoneInfo.GetSystemTimeZones
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[SearchVenue_LatitudeThreshold] decimal(18,14) NULL,
	[SearchVenue_LongitudeThreshold] decimal(18,14) NULL,
	[SearchVenue_RangeLimitInMiles] decimal(18,14) NULL,
	[RedeemCoupon_LatitudeThreshold] decimal(18,14) NULL,
	[RedeemCoupon_LongitudeThreshold] decimal(18,14) NULL,
	[RedeemCoupon_RangeLimitInMiles] decimal(18,14) NULL,
	[SendFeedback_RangeLimitInMiles] decimal(18,14) NULL,
	
	CONSTRAINT [Venue_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [Venue_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]),
)
GO

CREATE INDEX [VenueIndex1] ON [dbo].[Venue]([Id]) INCLUDE ([GroupId], [ChainId], [Status]);
GO

CREATE INDEX [VenueIndex2] ON [dbo].[Venue]([Id]) INCLUDE (
	[Status], [Latitude], [Longitude], 
	[SearchVenue_LatitudeThreshold], [SearchVenue_LongitudeThreshold], 
	[RedeemCoupon_LatitudeThreshold], [RedeemCoupon_LongitudeThreshold]);
GO

CREATE TABLE [dbo].[User] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled, Removed (we don't delete phsically)
	[EmailAddress] nvarchar(200) NOT NULL UNIQUE,
	[FirstName] nvarchar(100) NOT NULL,
	[LastName] nvarchar(100) NOT NULL,	
	[PhoneNumber] varchar(20) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [User_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),	
)
GO

CREATE INDEX [UserIndex1] ON [dbo].[User]([Id]) INCLUDE ([GroupId], [Status], [EmailAddress]);
GO

CREATE TABLE [dbo].[Password] (
	[UserId] uniqueidentifier NOT NULL,
	[Password] nvarchar(100) NOT NULL,
	
	CONSTRAINT [Password_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

CREATE INDEX [PasswordIndex1] ON [dbo].[Password]([UserId]);
GO


CREATE TABLE [dbo].[UserRole] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[VenueId] uniqueidentifier NOT NULL,
	[UserId] uniqueidentifier NOT NULL,
	[Role] tinyint NOT NULL DEFAULT 0, -- Manager, etc
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled, Removed (we don't delete phsically)
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [UserRole_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),	
	CONSTRAINT [UserRole_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]),	
	CONSTRAINT [UserRole_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),	
)
GO

CREATE INDEX [UserRoleIndex1] ON [dbo].[UserRole]([UserId]) INCLUDE ([GroupId], [VenueId], [Status]);
GO

CREATE TABLE [dbo].[SignupInfo] (
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[VenueName] nvarchar(100) NOT NULL,
	[VenueURL] varchar(500) NULL,	
	[VenueTimeZoneWinIndex] tinyint NULL, -- see TimeZoneInfo.GetSystemTimeZones
	[UserFirstName] nvarchar(100) NOT NULL,
	[UserLastName] nvarchar(100) NOT NULL,	
	[UserPhoneNumber] varchar(20) NULL,
	[UserEmailAddress] nvarchar(200) NOT NULL,
	[UserPassword] nvarchar(100) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
)
GO

CREATE TABLE [dbo].[SystemUser](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Status] [tinyint] NOT NULL DEFAULT 0,
	[IsAdmin] [bit] NOT NULL DEFAULT 0,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[EmailAddress] [nvarchar](200) NOT NULL UNIQUE NONCLUSTERED,
	[UserPassword] [nvarchar](100) NOT NULL,
	[CreateTimeUTC] [datetime] NOT NULL DEFAULT GETUTCDATE(),
)
GO
