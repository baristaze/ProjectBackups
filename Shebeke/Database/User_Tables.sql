/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [User]
CREATE TABLE [User] 
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[Type] tinyint NOT NULL DEFAULT 1, /* 0=Guest, 1=Regular, 2=Editor, 3=Moderator, 4=PowerUser, 5=SystemAdmin */
	[Status] tinyint NOT NULL DEFAULT 0, /* New, Suspended, Disabled, Deleted */
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[SplitId] int NULL,
	
	CONSTRAINT [User_FK_SplitId] FOREIGN KEY (SplitId) 
		REFERENCES [dbo].[Split]([Id]) ON UPDATE CASCADE,
)
GO

/*
ALTER TABLE [User]
	ADD [SplitId] int NULL;

ALTER TABLE [User]
	ADD CONSTRAINT [User_FK_SplitId] FOREIGN KEY (SplitId) 
		REFERENCES [dbo].[Split]([Id]) ON UPDATE CASCADE;
*/

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [FacebookUser]
CREATE TABLE [FacebookUser]
(
	[UserId] bigint NOT NULL,
	[FacebookId] bigint NOT NULL PRIMARY KEY,
	
	[FirstName] nvarchar(50) NULL,
	[LastName] nvarchar(50) NULL,
	[FullName] nvarchar(100) NULL,
	
	[EmailAddress] varchar(100) NULL,
	[BirthDay] datetime NULL,
	[Gender] tinyint NOT NULL DEFAULT 0,
	
	[FacebookLink] varchar(200) NULL,
    [FacebookUserName] varchar(50) NULL,
	[Hometown] nvarchar(100) NULL,
    [HometownId] bigint NOT NULL DEFAULT 0,
	[TimeZoneOffset] int NULL,
    [CurrentLocation]  nvarchar(100) NULL,
    [CurrentLocationId] bigint NOT NULL DEFAULT 0,
    [ISOLocale] varchar(10),
    [IsVerified] bit NOT NULL DEFAULT 0,
    
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	[PhotoUrl] varchar(200) NULL,
		
	CONSTRAINT [FacebookUser_ForeignKey_Id] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

/*
ALTER TABLE [FacebookUser]
	ADD [PhotoUrl] varchar(200) NULL;
GO
*/

-- We don't need to include FacebookId since any non-clustered-index has the 
-- clustered-index (e.g. primary key = [FacebookId] here) as a row locator.
CREATE INDEX [FacebookUserIndex1] ON [dbo].[FacebookUser]([UserId]); --INCLUDE ([FacebookId])--
GO

-- we will need this when we add (email + pswd) authentication... if a user signs up with email, 
-- then we need to check if s/he has signed up via facebook already. therefore we are going to 
-- scan FacebookUser table with email-address. We need an index on this to make this search faster.
CREATE INDEX [FacebookUserIndex2] ON [dbo].[FacebookUser]([EmailAddress]);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [FacebookFriend]
CREATE TABLE [FacebookFriend]
(
	[FacebookId1] bigint NOT NULL,
	[FacebookId2] bigint NOT NULL,
	[Friend2Name] nvarchar (100) NULL,
	[IsInvited] bit NOT NULL DEFAULT 0,
	[InvitationTimeUTC] datetime NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [FacebookFriend_PK] PRIMARY KEY ([FacebookId1], [FacebookId2]),
	
	CONSTRAINT [FacebookFriend_ForeignKey_FBId1] FOREIGN KEY([FacebookId1]) 
		REFERENCES [dbo].[FacebookUser]([FacebookId]),
	/*	
	-- When user A signs up, we pull FB IDs of all of his/her friends...
	-- Though, this doesn't mean that these friends are in our system already
	-- i.e. we won't have these users in our parent table (FacebookUser).
	-- therefore we cannot put such a referential integrity here
	CONSTRAINT [FacebookFriend_ForeignKey_FBId2] FOREIGN KEY([FacebookId2]) 
		REFERENCES [dbo].[FacebookUser]([FacebookId])*/
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [UserToken]
CREATE TABLE [UserToken]
(
	[UserId] bigint NOT NULL,
	[OAuthProvider] tinyint NOT NULL DEFAULT 0,
	[OAuthUserId] varchar(50) NULL, 
	[OAuthAccessToken] varchar(500) NULL,
	
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[ExpireTimeUTC] datetime NULL,
	
	CONSTRAINT [UserToken_PrimaryKey] PRIMARY KEY([UserId], [OAuthProvider]),
	CONSTRAINT [UserToken_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [UserActionLog]
CREATE TABLE [UserActionLog]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[UserId] bigint NOT NULL,
	[ActionId] tinyint NOT NULL, -- method codes
	[ActionTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE()
		
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [UserActionLog_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

CREATE INDEX [UserActionLogIndex1] ON [dbo].[UserActionLog]([UserId], [ActionId]) INCLUDE ([ActionTimeUTC]);
GO