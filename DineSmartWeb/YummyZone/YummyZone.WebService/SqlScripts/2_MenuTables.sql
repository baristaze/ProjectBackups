USE [YummyZone];
GO

CREATE TABLE [dbo].[Menu] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[ServiceStartTime] smallint NULL, -- as minutes of a 24-hour [0, 1440]
	[ServiceEndTime] smallint NULL, -- as minutes of a 24-hour [0, 1440]
	[Name] nvarchar(100) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [Menu_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

CREATE INDEX [MenuIndex1] ON [dbo].[Menu]([Id]) INCLUDE ([GroupId]);
GO

CREATE TABLE [dbo].[VenueAndMenuMap](
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[VenueId] uniqueidentifier NOT NULL,
	[MenuId] uniqueidentifier NOT NULL,
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled
	
	CONSTRAINT [VenueAndMenuMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [VenueAndMenuMap_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]),	
	CONSTRAINT [VenueAndMenuMap_ForeignKey_MenuId] FOREIGN KEY([MenuId]) REFERENCES [dbo].[Menu]([Id]) ON DELETE CASCADE,
	CONSTRAINT [VenueAndMenuMap_Uniqueness] UNIQUE ([VenueId], [MenuId]),
)
GO

CREATE INDEX [VenueAndMenuMapIndex1] ON [dbo].[VenueAndMenuMap]([VenueId]) INCLUDE ([GroupId], [MenuId], [Status]);
GO

CREATE TABLE [dbo].[MenuCategory] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Name] nvarchar(100) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [MenuCategory_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

CREATE INDEX [MenuCategoryIndex1] ON [dbo].[MenuCategory]([Id]) INCLUDE ([GroupId]);
GO

CREATE TABLE [dbo].[MenuAndMenuCategoryMap](
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[MenuId] uniqueidentifier NOT NULL,
	[MenuCategoryId] uniqueidentifier NOT NULL,
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled, Removed (we don't delete physically)
	
	CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuId] FOREIGN KEY([MenuId]) REFERENCES [dbo].[Menu]([Id]),
	CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuCategoryId] FOREIGN KEY([MenuCategoryId]) REFERENCES [dbo].[MenuCategory]([Id]) ON DELETE CASCADE,
	CONSTRAINT [MenuAndMenuCategoryMap_Uniqueness] UNIQUE ([MenuId], [MenuCategoryId]),
)
GO

CREATE INDEX [MenuAndMenuCategoryMapIndex1] ON [dbo].[MenuAndMenuCategoryMap]([MenuId]) INCLUDE ([GroupId], [MenuCategoryId], [Status]);
GO

CREATE TABLE [dbo].[MenuItem] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED DEFAULT NEWSEQUENTIALID(),
	[Name] nvarchar(100) NOT NULL,
	[Price] smallmoney NULL,
	[Description] nvarchar(1000) NULL,
	[LegalNotice] nvarchar(1000) NULL,
	[DietTypeFlags] int NOT NULL DEFAULT 0, -- unspecified, kosher, halal, vegan, vegetarian, gluten free, low fat, low salt, organic
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [MenuItem_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

CREATE INDEX [MenuItemIndex1] ON [dbo].[MenuItem]([Id]) INCLUDE ([GroupId]);
GO

CREATE TABLE [dbo].[MenuCategoryAndMenuItemMap](
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[MenuCategoryId] uniqueidentifier NOT NULL,
	[MenuItemId] uniqueidentifier NOT NULL,
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Disabled, Removed (we don't delete physically)
	
	CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuCategoryId] FOREIGN KEY([MenuCategoryId]) REFERENCES [dbo].[MenuCategory]([Id]),
	CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]) ON DELETE CASCADE,
	CONSTRAINT [MenuCategoryAndMenuItemMap_Uniqueness] UNIQUE ([MenuCategoryId], [MenuItemId]),
)
GO

CREATE INDEX [MenuCategoryAndMenuItemMapIndex1] ON [dbo].[MenuCategoryAndMenuItemMap]([MenuCategoryId]) INCLUDE ([GroupId], [MenuItemId], [Status]);
GO

CREATE TABLE [dbo].[MenuItemImage] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[MenuItemId] uniqueidentifier NOT NULL,
	[ContentType] varchar(255) NOT NULL,
	[ContentLength] int NOT NULL,
	[InitialFileNameOrUrl] varchar(500) NULL,
	[Data] image NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [MenuItemImage_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [MenuItemImage_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]) ON DELETE CASCADE,
)
GO

CREATE INDEX [MenuItemImageIndex1] ON [dbo].[MenuItemImage]([MenuItemId]) INCLUDE ([GroupId]);
GO

/*
CREATE TABLE [dbo].[NutritionFact] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[MenuItemId] uniqueidentifier NOT NULL,
	[Status] tinyint NOT NULL DEFAULT 0, -- Draft, Active, Disabled
	[Calories] smallint NOT NULL,
	[CaloriesFromFat] smallint NULL,
	[TotalFatAsGram] decimal (9,4) NULL,
	[TotalFatDailyPercentage] tinyint NULL,
	[CholesterolAsMilliGram] decimal (9,4) NULL,
	[CholesterolDailyPercentage] tinyint NULL,
	[SodiumAsMilliGram] decimal (9,4) NULL,
	[SodiumDailyPercentage] tinyint NULL,
	[TotalCarbohydratesAsGram] decimal (9,4) NULL,
	[TotalCarbohydratesDailyPercentage] tinyint NULL,
	[ProteinAsGram] decimal (9,4) NULL,
	[ProteinDailyPercentage] tinyint NULL,
	[DailyCalorieDietBasedOn] smallint NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [NutritionFact_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	CONSTRAINT [NutritionFact_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]) ON DELETE CASCADE,
)
GO
*/