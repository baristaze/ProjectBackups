
-- DROP TABLE [dbo].[Country]
CREATE TABLE [dbo].[Country]
(
	[Id] int NOT NULL PRIMARY KEY,
	[Code] nvarchar(5) NOT NULL UNIQUE, 
	[Name] nvarchar(50) NOT NULL UNIQUE,
	[AlternateNames] nvarchar(255) NULL -- comma separated
)
GO

-- DROP TABLE [dbo].[Region]
CREATE TABLE [dbo].[Region]	-- state
(
	[Id] int NOT NULL,
	[CountryId] int NOT NULL,
	[Code] nvarchar(5) NOT NULL, 
	[Name] nvarchar(100) NOT NULL,
	[AlternateNames] nvarchar(500) NULL, -- comma separated

	CONSTRAINT [Region_PK] PRIMARY KEY ([CountryId], [Id]),

	CONSTRAINT [Region_FK_CountryId] FOREIGN KEY ([CountryId]) 
		REFERENCES [dbo].[Country]([Id]) ON UPDATE CASCADE,

	CONSTRAINT [Unique_Name_per_Country] UNIQUE ([CountryId], [Name]),
	CONSTRAINT [Unique_Code_per_Country] UNIQUE ([CountryId], [Code])
)
GO

-- DROP TABLE [dbo].[SubRegion]
CREATE TABLE [dbo].[SubRegion]	-- county
(
	[Id] int NOT NULL,
	[CountryId] int NOT NULL,
	[RegionId] int NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	[OrderIndex] int NOT NULL DEFAULT 0,
	[AlternateNames] nvarchar(500) NULL, -- comma separated

	CONSTRAINT [SubRegion_PK] PRIMARY KEY ([CountryId], [RegionId], [Id]),

	CONSTRAINT [SubRegion_FK_RegionId] FOREIGN KEY ([CountryId], [RegionId]) 
		REFERENCES [dbo].[Region]([CountryId], [Id]) ON UPDATE CASCADE,

	CONSTRAINT [Unique_Name_per_Region] UNIQUE ([CountryId], [RegionId], [Name])
)
GO

-- DROP TABLE [dbo].[City]
CREATE TABLE [dbo].[City]
(
	[Id] int NOT NULL,
	[CountryId] int NOT NULL,
	[RegionId] int NOT NULL,
	[SubRegionId] int NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	[OrderIndex] int NOT NULL DEFAULT 0,
	[WeightIndex] int NOT NULL DEFAULT 1,
	[AlternateNames] nvarchar(500) NULL,

	CONSTRAINT [City_PK] PRIMARY KEY ([CountryId], [RegionId], [SubRegionId], [Id]),

	CONSTRAINT [City_FK_SubRegionId] FOREIGN KEY ([CountryId], [RegionId], [SubRegionId]) 
		REFERENCES [dbo].[SubRegion]([CountryId], [RegionId], [Id]) ON UPDATE CASCADE,

	CONSTRAINT [Unique_Name_per_SubRegion] UNIQUE ([CountryId], [RegionId], [SubRegionId], [Name])
)
GO