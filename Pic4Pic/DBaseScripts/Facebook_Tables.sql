/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [FacebookUser]
CREATE TABLE [FacebookUser]
(
	[UserId] uniqueidentifier NOT NULL,
	[FacebookId] bigint NOT NULL PRIMARY KEY,	

	[FirstName] nvarchar(50) NULL,
	[LastName] nvarchar(50) NULL,
	[FullName] nvarchar(100) NULL,	
	[EmailAddress] varchar(100) NULL,
	[BirthDay] datetime NULL,
	[Gender] tinyint NOT NULL DEFAULT 0,
	[MaritalStatus] tinyint NOT NULL DEFAULT 0,
	[MaritalStatusAsText] nvarchar(20) NULL,
	[Profession] nvarchar(100) NULL,
	[EducationLevel] tinyint NOT NULL DEFAULT 0,

	[FacebookLink] varchar(200) NULL,
    [FacebookUserName] varchar(50) NULL,
	[HometownCity] nvarchar(100) NULL,
	[HometownState] nvarchar(50) NULL,
    [HometownId] bigint NOT NULL DEFAULT 0,
	[TimeZoneOffset] int NULL,
    [CurrentLocationCity]  nvarchar(100) NULL,
	[CurrentLocationState]  nvarchar(100) NULL,
    [CurrentLocationId] bigint NOT NULL DEFAULT 0,
    [ISOLocale] varchar(10),
    [IsVerified] bit NOT NULL DEFAULT 0,    
	[PhotoUrl] varchar(200) NULL,

	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
		
	CONSTRAINT [FacebookUser_ForeignKey_Id] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

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
-- DROP TABLE [WorkHistory]
CREATE TABLE [WorkHistory]
(
	[FacebookId] bigint NOT NULL,	
	[Order] tinyint NOT NULL,
	[EmployerId] bigint NOT NULL DEFAULT 0,
	[EmployerName] nvarchar(100) NULL,
	[PositionId] bigint NOT NULL DEFAULT 0,
	[PositionName] nvarchar(100) NOT NULL, -- this is not null
	[StartDate] datetime NULL,
	[EndDate] datetime NULL,
	
	CONSTRAINT [WorkHistory_PrimaryKey] PRIMARY KEY([FacebookId], [Order]),	
	CONSTRAINT [WorkHistory_ForeignKey_FacebookId] FOREIGN KEY([FacebookId]) 
		REFERENCES [dbo].[FacebookUser]([FacebookId]) ON DELETE CASCADE,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [EducationHistory]
CREATE TABLE [EducationHistory]
(
	[FacebookId] bigint NOT NULL,	
	[Order] tinyint NOT NULL,
	[Type] varchar(50) NOT NULL, -- this is not null
	[SchoolId] bigint NOT NULL DEFAULT 0,
	[SchoolName] nvarchar(100) NULL,
	[ConcentrationId] bigint NOT NULL DEFAULT 0,
	[ConcentrationName] nvarchar(100) NULL,
	[DegreeId] bigint NOT NULL DEFAULT 0,
	[DegreeName] nvarchar(100) NULL,
	[Year] int NOT NULL DEFAULT 0,
	
	CONSTRAINT [EducationHistory_PrimaryKey] PRIMARY KEY([FacebookId], [Order]),	
	CONSTRAINT [EducationHistory_ForeignKey_FacebookId] FOREIGN KEY([FacebookId]) 
		REFERENCES [dbo].[FacebookUser]([FacebookId]) ON DELETE CASCADE,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [FacebookFriend]
CREATE TABLE [FacebookFriend]
(
	[FacebookId1] bigint NOT NULL,
	[FacebookId2] bigint NOT NULL,
	[Friend2Name] nvarchar (100) NULL,
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