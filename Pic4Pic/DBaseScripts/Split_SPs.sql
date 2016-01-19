-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTempSplitId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTempSplitId]');
END
GO
CREATE PROCEDURE [dbo].[GetTempSplitId]
	 @ClientId uniqueidentifier
AS
	  SELECT [SplitId] FROM [dbo].[TempSplitMap] WHERE [ClientId] = @ClientId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AddOrUpdateTempSplitId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AddOrUpdateTempSplitId]');
END
GO
CREATE PROCEDURE [dbo].[AddOrUpdateTempSplitId]
	  @ClientId uniqueidentifier
	 ,@SplitId int
AS
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();

	declare @count int;
	set @count = (SELECT COUNT(*) FROM [dbo].[TempSplitMap] WHERE [ClientId] = @ClientId);

	IF (@count = 0)
	BEGIN
			INSERT INTO [dbo].[TempSplitMap]
			   ([ClientId]
			   ,[SplitId]
			   ,[CreateTimeUTC]
			   ,[LastUpdateTimeUTC])
			 VALUES
				   (@ClientId
				   ,@SplitId
				   ,@utcNow
				   ,@utcNow);

	END
	ELSE
	BEGIN
		UPDATE [dbo].[TempSplitMap]
		   SET [SplitId] = @SplitId
			  ,[LastUpdateTimeUTC] = @utcNow
		 WHERE [ClientId] = @ClientId
	END

GO