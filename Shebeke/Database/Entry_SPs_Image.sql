----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewImageMetaFile] 
 @CloudId uniqueidentifier
,@CreatedBy bigint
,@AssetId bigint
,@AssetType tinyint
,@ContentType varchar(255)
,@ContentLength int
,@OriginalUrl nvarchar(500)
,@CloudUrl nvarchar(500)
,@Width int
,@Height int
AS
	declare @newId bigint;
	set @newId = -1;
	
	-- insert item
	INSERT INTO [dbo].[ImageMetaFile]
			   ([CloudId]
			   ,[CreatedBy]
			   ,[AssetId]
			   ,[AssetType]
			   ,[ContentType]
			   ,[ContentLength]
			   ,[OriginalUrl]
			   ,[CloudUrl]
			   ,[Width]
			   ,[Height])
		 VALUES
			   (@CloudId
			   ,@CreatedBy
			   ,@AssetId
			   ,@AssetType
			   ,@ContentType
			   ,@ContentLength
			   ,@OriginalUrl
			   ,@CloudUrl
			   ,@Width
			   ,@Height);

	-- get the recently generated auto id	
	set @newId = (SELECT SCOPE_IDENTITY());
	
	-- return it
	SELECT @newId;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[GetImageMetaFile] 
 @Id bigint
,@CheckUser bit
,@UserId bigint
AS
	SELECT [Id]
		  ,[CloudId]
		  ,[CreatedBy]
		  ,[AssetId]
		  ,[AssetType]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[OriginalUrl]
		  ,[CloudUrl]
		  ,[Width]
		  ,[Height]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
	  WHERE [Id] = @Id 
		AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetImageMetaFilesByAssetIDs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetImageMetaFilesByAssetIDs]');
END
GO

CREATE PROCEDURE [dbo].[GetImageMetaFilesByAssetIDs] 
 @AssetType tinyint
,@AssetIdListAsText varchar(max)
AS
	SELECT [Id]
		  ,[CloudId]
		  ,[CreatedBy]
		  ,[AssetId]
		  ,[AssetType]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[OriginalUrl]
		  ,[CloudUrl]
		  ,[Width]
		  ,[Height]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
	  WHERE [AssetType] = @AssetType 
		AND [AssetId] IN (SELECT * FROM TokenizeIds(@AssetIdListAsText))
	  ORDER BY [AssetId] ASC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[DeleteImageMetaFile] 
 @Id bigint
,@CheckUser bit
,@UserId bigint
AS
	DELETE FROM [dbo].[ImageMetaFile] 
		WHERE [Id] = @Id 
			AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetUnAssociatedImages')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetUnAssociatedImages]');
END
GO

CREATE PROCEDURE [dbo].[GetUnAssociatedImages] 
 @UserId bigint
,@AssetType tinyint
AS
	SELECT [Id]
		  ,[CloudId]
		  ,[CreatedBy]
		  ,[AssetId]
		  ,[AssetType]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[OriginalUrl]
		  ,[CloudUrl]
		  ,[Width]
		  ,[Height]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
		WHERE [CreatedBy] = @UserId AND [AssetType] = @AssetType AND [AssetId] <= 0;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AssociateImageToAsset')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AssociateImageToAsset]');
END
GO

CREATE PROCEDURE [dbo].[AssociateImageToAsset] 
 @Id bigint
,@AssetId bigint
,@AssetType tinyint
,@CheckUser bit
,@UserId bigint
AS
	declare @update bit;
	set @update = 1;
	
	IF (@CheckUser = 1)
	BEGIN
		IF(@AssetType = 3) -- entry
		BEGIN
			IF NOT EXISTS(SELECT * FROM [dbo].[Entry] WHERE [Id] = @AssetId AND [CreatedBy] = @UserId)
			BEGIN
				set @update = 0;
				RAISERROR('Entry does not belong to this user', 16, 1);
			END
		END
	END
	
	IF(@update = 1)
	BEGIN
		UPDATE [dbo].[ImageMetaFile] 
			SET [AssetId] = @AssetId, 
				[AssetType] = @AssetType
			WHERE [Id] = @Id 
				AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
	END
GO
