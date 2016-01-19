/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[SplitSection]
CREATE TABLE [dbo].[SplitSection]
(
	[Id] int NOT NULL PRIMARY KEY,
	[Name] varchar(100) NOT NULL,
);
GO

CREATE UNIQUE INDEX [SplitSectionIndex1] ON [dbo].[SplitSection]([Name]);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[SplitVariation]
CREATE TABLE [dbo].[SplitVariation]
(
	[SectionId] int NOT NULL,
	[Id] int NOT NULL,
	[IsDefault] bit NOT NULL DEFAULT 0,
	[Description] nvarchar(200) NOT NULL,
	
	CONSTRAINT [SplitVariation_PK] PRIMARY KEY ([SectionId], [Id]),
	
	CONSTRAINT [SplitVariation_FK_SectionId] FOREIGN KEY ([SectionId]) 
		REFERENCES [dbo].[SplitSection]([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[SplitProperty]
CREATE TABLE [dbo].[SplitProperty]
(	
	[Id] int NOT NULL PRIMARY KEY IDENTITY,
	[IsEnabled] bit NOT NULL DEFAULT 0, /*this is for authoring purposes. disable one; copy it; alter it;*/
	[SectionId] int NOT NULL,
	[VariationId] int NOT NULL,
	[FriendlyName] nvarchar(100) NOT NULL,
	[SplitType]	tinyint NOT NULL,	/*0=css, 1=text */
	[SideToApply] tinyint NULL, /*0=client, 1=server */
	[JQSelector] varchar(100) NULL,
	[CssClass] varchar(100) NULL,
	[CssTemplate] varchar(500) NULL,
	[Value] nvarchar(500) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GetUtcDate(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GetUtcDate(),
	
	CONSTRAINT [SplitProperty_FK_ParentIds] FOREIGN KEY ([SectionId], [VariationId]) 
		REFERENCES [dbo].[SplitVariation]([SectionId], [Id]) ON DELETE CASCADE ON UPDATE CASCADE,
)
GO

CREATE UNIQUE INDEX [SplitPropertyUniqueIndex1] ON [SplitProperty]([IsEnabled], [SectionId], [VariationId], [FriendlyName]);
GO

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
/*
ALTER TABLE [dbo].[Split]
	ADD [IsDefault] bit NOT NULL DEFAULT 0;
*/

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[SplitBag]
CREATE TABLE [dbo].[SplitBag]
(
	[SplitId] int NOT NULL,
	[SectionId] int NOT NULL,
	[VariationId] int NOT NULL,
	
	-- for each section & split, we can only have one or none variation
	CONSTRAINT [SplitBag_PK] PRIMARY KEY ([SplitId], [SectionId]),
	
	CONSTRAINT [SplitBag_FK_VarIds] FOREIGN KEY ([SectionId], [VariationId]) 
		REFERENCES [dbo].[SplitVariation]([SectionId], [Id]) ON DELETE CASCADE ON UPDATE CASCADE,
		
	CONSTRAINT [SplitBag_FK_SplitId] FOREIGN KEY (SplitId) 
		REFERENCES [dbo].[Split]([Id]) ON DELETE CASCADE ON UPDATE CASCADE,
)
GO
