

/*
CREATE TABLE [dbo].[Contact] (
	[ObjectType] tinyint NOT NULL,
	[ObjectId] uniqueidentifier NOT NULL,
	[ContactType] tinyint NOT NULL DEFAULT 0,
	[FirstName] nvarchar(50) NOT NULL,
	[LastName] nvarchar(50) NOT NULL,
	[Title] nvarchar(50) NULL,
)
GO

CREATE CLUSTERED INDEX [ContactIndex1] ON [dbo].[Contact]([ObjectId],[ObjectType],[ContactType]);
GO
*/

CREATE TABLE [dbo].[Address] (
	[ObjectType] tinyint NOT NULL,
	[ObjectId] uniqueidentifier NOT NULL,
	[AddressType] tinyint NOT NULL DEFAULT 0,
	[AddressLine1] nvarchar(300) NOT NULL,
	[AddressLine2] nvarchar(100) NULL,
	[City] nvarchar(50) NOT NULL,
	[State] nvarchar(50) NOT NULL,
	[ZipCode] varchar(10) NOT NULL,
	[Country] nvarchar(50) NOT NULL DEFAULT N'USA',
)
GO

-- CREATE INDEX [AddressIndex1] ON [dbo].[Address]([ObjectId]) INCLUDE ([ObjectType],[AddressType]);
CREATE CLUSTERED INDEX [AddressIndex1] ON [dbo].[Address]([ObjectId],[ObjectType],[AddressType]);
GO

/*
CREATE TABLE [dbo].[Phone] (
	[ObjectType] tinyint NOT NULL,
	[ObjectId] uniqueidentifier NOT NULL,
	[PhoneType] tinyint NOT NULL DEFAULT 0,
	[CountryCode] varchar(4) NOT NULL DEFAULT '1',
	[AreaCode] varchar(5) NOT NULL,
	[Number] varchar(15) NOT NULL,
	[Extension] varchar(5) NULL,
)
GO
*/