----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'InsertCssSplitters')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[InsertCssSplitters]');
END
GO

CREATE PROCEDURE [dbo].[InsertCssSplitters] 
 @SectionId int
,@VariationId int
,@FriendlyName nvarchar(100)
,@CssClass varchar(100)
,@CssTemplate varchar(500)
,@Value nvarchar(500)
AS
	INSERT INTO [dbo].[SplitProperty]
			   ([SectionId]
			   ,[VariationId]
			   ,[FriendlyName]
			   ,[SplitType]
			   ,[CssClass]
			   ,[CssTemplate]
			   ,[Value])
		 VALUES
			   (@SectionId
			   ,@VariationId
			   ,@FriendlyName
			   ,0 -- css
			   ,@CssClass
			   ,@CssTemplate
			   ,@Value);
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'InsertTextSplitters')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[InsertTextSplitters]');
END
GO

CREATE PROCEDURE [dbo].[InsertTextSplitters]
 @SectionId int
,@VariationId int
,@FriendlyName nvarchar(100)
,@SideToApply tinyint
,@JQSelector varchar(100)
,@Value nvarchar(500)
AS
	INSERT INTO [dbo].[SplitProperty]
			   ([SectionId]
			   ,[VariationId]
			   ,[FriendlyName]
			   ,[SplitType]
			   ,[SideToApply]
			   ,[JQSelector]
			   ,[Value])
		 VALUES
			   (@SectionId
			   ,@VariationId
			   ,@FriendlyName
			   ,1 -- text
			   ,@SideToApply
			   ,@JQSelector
			   ,@Value);
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ChangeSplitterStatus')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ChangeSplitterStatus]');
END
GO

CREATE PROCEDURE [dbo].[ChangeSplitterStatus] 
 @SplitterPropertyId int
,@IsEnabled bit
AS
	UPDATE [dbo].[SplitProperty]
	   SET [IsEnabled] = @IsEnabled,
		   [LastUpdateTimeUTC] = GetUtcDate()
	 WHERE [Id] = @SplitterPropertyId;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ChangeSplitterStatus2')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ChangeSplitterStatus2]');
END
GO

CREATE PROCEDURE [dbo].[ChangeSplitterStatus2] 
 @SectionId int
 -- no variation id here since we better enable all variations at once
,@FriendlyName nvarchar(100)
,@IsEnabled bit
AS
	UPDATE [dbo].[SplitProperty]
	   SET [IsEnabled] = @IsEnabled,
		   [LastUpdateTimeUTC] = GetUtcDate()
	  WHERE [SectionId] = @SectionId
		AND [FriendlyName] = @FriendlyName;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateSplitter')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateSplitter]');
END
GO

CREATE PROCEDURE [dbo].[UpdateSplitter] 
 @SplitterPropertyId int
,@Value nvarchar(500)
AS
	UPDATE [dbo].[SplitProperty]
	   SET [Value] = @Value,
		   [LastUpdateTimeUTC] = GetUtcDate()
	 WHERE [Id] = @SplitterPropertyId;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateSplitter2')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateSplitter2]');
END
GO

-- this updates both disabled and enabled versions...
CREATE PROCEDURE [dbo].[UpdateSplitter2] 
 @SectionId int
,@VariationId int
,@FriendlyName nvarchar(100)
,@Value nvarchar(500)
AS
	UPDATE [dbo].[SplitProperty]
	   SET [Value] = @Value,
		   [LastUpdateTimeUTC] = GetUtcDate()
	 WHERE [SectionId] = @SectionId
		AND [VariationId] = @VariationId
		AND [FriendlyName] = @FriendlyName;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CopySplitters')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CopySplitters]');
END
GO

CREATE PROCEDURE [dbo].[CopySplitters] 
 @SectionId int
,@ExistingVariationId int
,@NewVariationId int
AS
	;WITH [OriginalRows] ([IsEnabled], [SectionId], [VariationId], [FriendlyName], [SplitType], [SideToApply], [JQSelector], [CssClass], [CssTemplate], [Value])
	AS
	(	SELECT [IsEnabled], [SectionId], @NewVariationId AS [VariationId], [FriendlyName], [SplitType], [SideToApply], [JQSelector], [CssClass], [CssTemplate], [Value]
			FROM [dbo].[SplitProperty]
			WHERE [SectionId] = @SectionId AND [VariationId] = @ExistingVariationId
	)
	INSERT INTO [dbo].[SplitProperty] ([IsEnabled], [SectionId], [VariationId], [FriendlyName], [SplitType], [SideToApply], [JQSelector], [CssClass], [CssTemplate], [Value])
		SELECT [IsEnabled], [SectionId], [VariationId], [FriendlyName], [SplitType], [SideToApply], [JQSelector], [CssClass], [CssTemplate], [Value]
			FROM [OriginalRows];
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCssSplitters')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCssSplitters]');
END
GO

CREATE PROCEDURE [dbo].[GetCssSplitters] 
 @SectionId int
,@VariationId int
,@IncludeDisableds bit
AS
	SELECT [Id]
		  ,[IsEnabled]
		  ,[SectionId]
		  ,[VariationId]
		  ,[FriendlyName]
		  ,[Value]
		  ,[CreateTimeUTC]
		  ,[LastUpdateTimeUTC]
		  ,[CssClass]
		  ,[CssTemplate]
	  FROM [dbo].[SplitProperty]
	  WHERE [SplitType] = 0 -- css
		AND [SectionId] = @SectionId
		AND [VariationId] = @VariationId
		AND ( @IncludeDisableds = 1 OR [IsEnabled] = 1)
	  ORDER BY [FriendlyName];
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTextSplitters')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTextSplitters]');
END
GO

CREATE PROCEDURE [dbo].[GetTextSplitters] 
 @SectionId int
,@VariationId int
,@IncludeDisableds bit
AS
	SELECT [Id]
		  ,[IsEnabled]
		  ,[SectionId]
		  ,[VariationId]
		  ,[FriendlyName]
		  ,[Value]
		  ,[CreateTimeUTC]
		  ,[LastUpdateTimeUTC]
		  ,[SideToApply]
		  ,[JQSelector]
	  FROM [dbo].[SplitProperty]
	  WHERE [SplitType] = 1 -- text
		AND [SectionId] = @SectionId
		AND [VariationId] = @VariationId
		AND ( @IncludeDisableds = 1 OR [IsEnabled] = 1)
	  ORDER BY [FriendlyName];
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetDefaultSplitId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetDefaultSplitId]');
END
GO

CREATE PROCEDURE [dbo].[GetDefaultSplitId] 
AS
	SELECT TOP (1) [Id] FROM [dbo].[Split] WHERE [IsActive] = 1 AND [IsDefault] = 1;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSplitInfo')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSplitInfo]');
END
GO

CREATE PROCEDURE [dbo].[GetSplitInfo] 
 @SplitType tinyint
,@SplitId int
,@SectionFilters varchar(max)
AS
	declare @Sections TABLE([SectionId] int, [RowNumber] int);
	INSERT INTO @Sections
	SELECT	 [Id]
			,ROW_NUMBER() OVER (ORDER BY [Id] ASC) AS [RowNumber]
		FROM [dbo].[SplitSection]
		WHERE [Id] IN (SELECT DISTINCT [SectionId] FROM [dbo].[SplitVariation])
			AND [Id] IN (SELECT * FROM [dbo].[TokenizeIDs] (@SectionFilters));
	
	declare @sectionIterator int;
	set @sectionIterator = 1;
	declare @sectionCount int;
	set @sectionCount = (SELECT COUNT(*) FROM @Sections);
	declare @sectionId int;
	
	declare @variationId int;
	
	declare @Variations TABLE ([SectionId] int, [VariationId] int);
	
	-- for each section
	WHILE(@sectionIterator <= @sectionCount)
	BEGIN
		-- for current section
		set @sectionId = (SELECT [SectionId] FROM @Sections WHERE [RowNumber] = @sectionIterator);		
		-- PK (split_id, section_id): there can be only one or none variation for a given (split_id, section_id) combination
		set @variationId = (
			SELECT [VariationId] 
				FROM [dbo].[SplitBag] [SB] 
				JOIN [dbo].[Split] [S] ON [S].[Id] = [SB].[SplitId]
				WHERE [S].[IsActive] = 1 
					AND [SB].[SplitId] = @SplitId 
					AND [SB].[SectionId] = @sectionId);
		IF (@variationId is NULL)
		BEGIN
			-- PK(section_id, variation_id): there might be multiple defaults by mistake. notice that we don't specify variation_id.
			set @variationId = (SELECT TOP (1) [Id] FROM [dbo].[SplitVariation] WHERE [SectionId] = @sectionId AND [IsDefault] = 1);
		END
		
		-- save the variation for current section: it is either the specified one or the default one
		INSERT INTO @Variations
			SELECT @sectionId, @variationId;
		
		-- print CAST (@sectionId AS varchar(20)) + ', ' + CAST (@variationId AS varchar(20));
		
		set @sectionIterator = @sectionIterator + 1;
	END
	
	IF(@SplitType = 0) -- text
	BEGIN
		SELECT [P].[Id]
			  ,[P].[IsEnabled]
			  ,[P].[SectionId]
			  ,[P].[VariationId]
			  ,[P].[FriendlyName]
			  ,[P].[Value]
			  ,[P].[CreateTimeUTC]
			  ,[P].[LastUpdateTimeUTC]
			  ,[P].[CssClass]
		      ,[P].[CssTemplate]
		  FROM [dbo].[SplitProperty] [P]
		  JOIN @Variations [V] ON [P].[SectionId] = [V].[SectionId] AND [P].[VariationId] = [V].[VariationId]
		  WHERE [P].[SplitType] = @SplitType
				AND [P].[IsEnabled] = 1
		  ORDER BY [P].[SectionId], [P].[VariationId], [P].[FriendlyName];
	END
	ELSE IF (@SplitType = 1) -- text
	BEGIN
		SELECT [P].[Id]
			  ,[P].[IsEnabled]
			  ,[P].[SectionId]
			  ,[P].[VariationId]
			  ,[P].[FriendlyName]
			  ,[P].[Value]
			  ,[P].[CreateTimeUTC]
			  ,[P].[LastUpdateTimeUTC]
			  ,[P].[SideToApply]
			  ,[P].[JQSelector]
		  FROM [dbo].[SplitProperty] [P]
		  JOIN @Variations [V] ON [P].[SectionId] = [V].[SectionId] AND [P].[VariationId] = [V].[VariationId]
		  WHERE [P].[SplitType] = @SplitType
				AND [P].[IsEnabled] = 1
		  ORDER BY [P].[SectionId], [P].[VariationId], [P].[FriendlyName];
	END
	ELSE
	BEGIN
		SELECT [Id] FROM [dbo].[SplitProperty] WHERE [Id] = -1;
	END
GO