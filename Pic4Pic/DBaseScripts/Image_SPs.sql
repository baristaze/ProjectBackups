----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewImageMetaFile] 
	 @Id uniqueidentifier
	,@GroupingId uniqueidentifier
	,@UserId uniqueidentifier
	,@Status tinyint
	,@ContentType varchar(255)
	,@ContentLength int
	,@Width int
	,@Height int
	,@CloudUrl nvarchar(500)
	,@IsBlurred bit
	,@IsThumbnail bit
	,@IsProfilePicture bit
AS
	INSERT INTO [dbo].[ImageMetaFile]
			   ([Id]
			   ,[GroupingId]
			   ,[UserId]
			   ,[Status]
			   ,[ContentType]
			   ,[ContentLength]
			   ,[Width]
			   ,[Height]
			   ,[CloudUrl]
			   ,[IsBlurred]
			   ,[IsThumbnail]
			   ,[IsProfilePicture])
		 VALUES
			   ( @Id
			    ,@GroupingId
				,@UserId
				,@Status
				,@ContentType
				,@ContentLength
				,@Width
				,@Height
				,@CloudUrl
				,@IsBlurred
				,@IsThumbnail
				,@IsProfilePicture);
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetImageMetaFileById')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetImageMetaFileById]');
END
GO

CREATE PROCEDURE [dbo].[GetImageMetaFileById] 
	 @Id uniqueidentifier
	,@CheckUser bit
	,@UserId uniqueidentifier
AS
	SELECT [Id]
		  ,[GroupingId]
		  ,[UserId]
		  ,[Status]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[Width]
		  ,[Height]
		  ,[CloudUrl]
		  ,[IsBlurred]
		  ,[IsThumbnail]
		  ,[IsProfilePicture]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
	  WHERE [Id] = @Id 
		AND ((@CheckUser = 0) OR ([UserId] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetImageMetaFilesByUserId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetImageMetaFilesByUserId]');
END
GO

CREATE PROCEDURE [dbo].[GetImageMetaFilesByUserId] 
	@UserId uniqueidentifier
AS
	SELECT [Id]
		  ,[GroupingId]
		  ,[UserId]
		  ,[Status]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[Width]
		  ,[Height]
		  ,[CloudUrl]
		  ,[IsBlurred]
		  ,[IsThumbnail]
		  ,[IsProfilePicture]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
	  WHERE [UserId] = @UserId
	  ORDER BY [CreateTimeUTC] ASC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetImageMetaFilesByUserIDs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetImageMetaFilesByUserIDs]');
END
GO

CREATE PROCEDURE [dbo].[GetImageMetaFilesByUserIDs] 
	@concatenatedIdsAsText varchar(max)
AS
	SELECT [Id]
		  ,[GroupingId]
		  ,[UserId]
		  ,[Status]
		  ,[ContentType]
		  ,[ContentLength]
		  ,[Width]
		  ,[Height]
		  ,[CloudUrl]
		  ,[IsBlurred]
		  ,[IsThumbnail]
		  ,[IsProfilePicture]
		  ,[CreateTimeUTC]
	  FROM [dbo].[ImageMetaFile]
	  WHERE [UserId] IN (SELECT * FROM TokenizeGuids(@concatenatedIdsAsText))
	  ORDER BY [UserId], [CreateTimeUTC] ASC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkImageMetaFileAsDeleted')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkImageMetaFileAsDeleted]');
END
GO

CREATE PROCEDURE [dbo].[MarkImageMetaFileAsDeleted] 
	 @Id uniqueidentifier
	,@CheckUser bit
	,@UserId uniqueidentifier
AS
	UPDATE [dbo].[ImageMetaFile] 
		SET [Status] = 2
		WHERE [Id] = @Id 
			AND ((@CheckUser = 0) OR ([UserId] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DisableImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DisableImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[DisableImageMetaFile] 
	 @Id uniqueidentifier
	,@CheckUser bit
	,@UserId uniqueidentifier
AS
	UPDATE [dbo].[ImageMetaFile] 
		SET [Status] = 1
		WHERE [Id] = @Id 
			AND ((@CheckUser = 0) OR ([UserId] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateImageOwner')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateImageOwner]');
END
GO

CREATE PROCEDURE [dbo].[UpdateImageOwner] 
	 @Id uniqueidentifier
	,@UserId uniqueidentifier
AS
	UPDATE [dbo].[ImageMetaFile] 
		SET [UserId] = @UserId
		WHERE [Id] = @Id AND [UserId] is NULL;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateAllImageOwners')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateAllImageOwners]');
END
GO

CREATE PROCEDURE [dbo].[UpdateAllImageOwners] 
	 @concatenatedImageIds varchar(max)
	,@UserId uniqueidentifier
AS
	UPDATE [dbo].[ImageMetaFile] 
		SET [UserId] = @UserId
		WHERE [Id] IN (SELECT * FROM TokenizeGuids(@concatenatedImageIds)) AND [UserId] is NULL;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteImageMetaFile')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteImageMetaFile]');
END
GO

CREATE PROCEDURE [dbo].[DeleteImageMetaFile] 
	 @Id uniqueidentifier
	,@CheckUser bit
	,@UserId uniqueidentifier
AS
	DELETE [dbo].[ImageMetaFile] 
		WHERE [Id] = @Id 
			AND ((@CheckUser = 0) OR ([UserId] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ResetProfileFlags')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ResetProfileFlags]');
END
GO

CREATE PROCEDURE [dbo].[ResetProfileFlags] 
	 @concatenatedImageIds varchar(max)
	,@UserId uniqueidentifier
AS
	UPDATE [dbo].[ImageMetaFile] 
		SET [IsProfilePicture] = 0
		WHERE [Id] IN (SELECT * FROM TokenizeGuids(@concatenatedImageIds)) AND [UserId] = @UserId;
GO