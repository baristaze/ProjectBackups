-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetUserByUsername')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetUserByUsername]');
END
GO

CREATE PROCEDURE [dbo].[GetUserByUsername]
@Username nvarchar(20)
AS
	SELECT [U].[Id]
		  ,[U].[Type]
		  ,[U].[Status]
		  ,[U].[SplitId]
		  ,[U].[Username]
		  ,[U].[Password]
		  ,[D].[Description]
		  ,[U].[CreateTimeUTC]
		  ,[U].[LastUpdateTimeUTC]
	  FROM [dbo].[User] [U]
	  LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [U].[Id]
	  WHERE [U].[Username] = @Username;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetUserByID')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetUserByID]');
END
GO

CREATE PROCEDURE [dbo].[GetUserByID]
  @UserId uniqueidentifier
AS
	SELECT [U].[Id]
		  ,[U].[Type]
		  ,[U].[Status]
		  ,[U].[SplitId]
		  ,[U].[Username]
		  ,N'' AS [Password]
		  ,[D].[Description]
		  ,[U].[CreateTimeUTC]
		  ,[U].[LastUpdateTimeUTC]
	  FROM [dbo].[User] [U]
	  LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [U].[Id]
	  WHERE [U].[Id] = @UserId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetUsersByID')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetUsersByID]');
END
GO

CREATE PROCEDURE [dbo].[GetUsersByID]
  @concatenatedIdsAsText varchar(max)
AS
	SELECT [U].[Id]
		  ,[U].[Type]
		  ,[U].[Status]
		  ,[U].[SplitId]
		  ,[U].[Username]
		  ,N'' AS [Password]
		  ,[D].[Description]
		  ,[U].[CreateTimeUTC]
		  ,[U].[LastUpdateTimeUTC]
	  FROM [dbo].[User] [U]
	  LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [U].[Id]
	  WHERE [U].[Id] IN (SELECT * FROM TokenizeGuids(@concatenatedIdsAsText));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetUsersByIDWithPic4PicCounts')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetUsersByIDWithPic4PicCounts]');
END
GO

CREATE PROCEDURE [dbo].[GetUsersByIDWithPic4PicCounts]
  @concatenatedIdsAsText varchar(max)
 ,@userIdToCheckPic4Pics uniqueidentifier
AS
	SELECT [U].[Id]
		  ,[U].[Type]
		  ,[U].[Status]
		  ,[U].[SplitId]
		  ,[U].[Username]
		  ,N'' AS [Password]
		  ,[D].[Description]
		  ,[U].[CreateTimeUTC]
		  ,[U].[LastUpdateTimeUTC]
		  ,(SELECT COUNT(*) 
				FROM [dbo].[Pic4Pic] [P]
				WHERE (
						([P].[UserId1] = [U].[Id] AND [P].[UserId2] = @userIdToCheckPic4Pics) OR ([P].[UserId2] = [U].[Id] AND [P].[UserId1] = @userIdToCheckPic4Pics)
					  )
					  AND [P].[AcceptTimeUTC] IS NOT NULL
		   )	
		   AS [AcceptedPic4PicCount]
	  FROM [dbo].[User] [U]
	  LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [U].[Id]
	  WHERE [U].[Id] IN (SELECT * FROM TokenizeGuids(@concatenatedIdsAsText));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewUser]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewUser]
		@Id uniqueidentifier,
		@Type tinyint,
		@Status tinyint,
		@SplitId int,
		@Username nvarchar(20),
		@Password nvarchar(20)
AS
	INSERT INTO [dbo].[User]
			   ([Id]
			   ,[Type]
			   ,[Status]
			   ,[SplitId]
			   ,[Username]
			   ,[Password])
		 VALUES
			   (@Id
			   ,@Type
			   ,@Status
			   ,@SplitId
			   ,@Username
			   ,@Password);
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ActivateUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ActivateUser]');
END
GO

CREATE PROCEDURE [dbo].[ActivateUser]
		@Id uniqueidentifier,
		@OnlyIfPartial bit
AS
	UPDATE [dbo].[User] SET [Status] = 1 WHERE [Id] = @Id AND (@OnlyIfPartial = 0 OR [Status] < 2); -- partial | active
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AddOrUpdateUserDetails')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AddOrUpdateUserDetails]');
END
GO

CREATE PROCEDURE [dbo].[AddOrUpdateUserDetails]
		@UserId uniqueidentifier,
		@Description nvarchar(max)
AS
	IF EXISTS (SELECT * FROM [dbo].[UserDetails] WHERE [UserId] = @UserId)
		BEGIN
			UPDATE [dbo].[UserDetails] 
				SET [Description] = @Description, 
					[LastUpdateTimeUTC] = GETUTCDATE() 
				WHERE [UserId] = @UserId;
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[UserDetails] ([UserId], [Description]) 
				VALUES (@UserId, @Description);
		END	
GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewPreference')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewPreference]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewPreference]
		@UserId uniqueidentifier,
		@InterestedIn tinyint
AS
	INSERT INTO [dbo].[Preference]
			   ([UserId]
			   ,[InterestedIn])
		 VALUES
			   (@UserId
			   ,@InterestedIn);
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdatePreference')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdatePreference]');
END
GO

CREATE PROCEDURE [dbo].[UpdatePreference]
		@UserId uniqueidentifier,
		@InterestedIn tinyint
AS

	UPDATE [dbo].[Preference]
	   SET [InterestedIn] = @InterestedIn
		  ,[LastUpdateTimeUTC] = GETUTCDATE()
	 WHERE [UserId] = @UserId;
GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AddOrUpdateMobileDevice')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AddOrUpdateMobileDevice]');
END
GO

CREATE PROCEDURE [dbo].[AddOrUpdateMobileDevice]
	 @ClientId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@OSType tinyint
    ,@OSVersion varchar(50)
    ,@AppVersion varchar(10)
    ,@SDKVersion varchar(10)
    ,@DeviceType varchar(50)
    ,@PushNotifRegId varchar(max)
AS
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();

	declare @count int;
	set @count = (SELECT COUNT(*) FROM [dbo].[MobileDevice] WHERE [ClientId] = @ClientId);

	IF (@count = 0)
	BEGIN
			INSERT INTO [dbo].[MobileDevice]
			   ([ClientId]
			   ,[UserId]
			   ,[OSType]
			   ,[OSVersion]
			   ,[AppVersion]
			   ,[SDKVersion]
			   ,[DeviceType]
			   ,[PushNotifRegId]
			   ,[CreateTimeUTC]
			   ,[LastUpdateTimeUTC])
			 VALUES
				   (@ClientId
				   ,@UserId
				   ,@OSType
				   ,@OSVersion
				   ,@AppVersion
				   ,@SDKVersion
				   ,@DeviceType
				   ,@PushNotifRegId
				   ,@utcNow
				   ,@utcNow);

	END
	ELSE
	BEGIN
		UPDATE [dbo].[MobileDevice]
		   SET [UserId] = COALESCE(@UserId, [UserId])
			  ,[OSType] = COALESCE(@OSType, [OSType])
			  ,[OSVersion] = COALESCE(@OSVersion, [OSVersion])
			  ,[AppVersion] = COALESCE(@AppVersion, [AppVersion])
			  ,[SDKVersion] = COALESCE(@SDKVersion, [SDKVersion])
			  ,[DeviceType] = COALESCE(@DeviceType, [DeviceType])
			  ,[PushNotifRegId] = COALESCE(@PushNotifRegId, [PushNotifRegId])
			  ,[LastUpdateTimeUTC] = @utcNow
		 WHERE [ClientId] = @ClientId
	END
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetMobileDevices')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetMobileDevices]');
END
GO
-- you need to check duplicate [PushNotifRegId]s and also eliminate the same [DeviceType]s and same [ClientId]s
CREATE PROCEDURE [dbo].[GetMobileDevices]
	 @UserId uniqueidentifier
AS
	  SELECT [ClientId]
			,[UserId]
			,[OSType]
			,[OSVersion]
			,[AppVersion]
			,[SDKVersion]
			,[DeviceType]
			,[PushNotifRegId]
			,[CreateTimeUTC]
			,[LastUpdateTimeUTC]
		FROM [dbo].[MobileDevice] 
		WHERE [UserId] = @UserId AND
			  ([PushNotifRegId] IS NOT NULL AND [PushNotifRegId] <> '')
		ORDER BY [LastUpdateTimeUTC] DESC
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetMobileDevicesBulk')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetMobileDevicesBulk]');
END
GO
-- you need to check duplicate [PushNotifRegId]s and also eliminate the same [DeviceType]s and same [ClientId]s
CREATE PROCEDURE [dbo].[GetMobileDevicesBulk]
	 @concatenatedUserIds varchar(max)
AS
	  SELECT [ClientId]
			,[UserId]
			,[OSType]
			,[OSVersion]
			,[AppVersion]
			,[SDKVersion]
			,[DeviceType]
			,[PushNotifRegId]
			,[CreateTimeUTC]
			,[LastUpdateTimeUTC]
		FROM [dbo].[MobileDevice] 
		WHERE [UserId] IN (SELECT * FROM TokenizeGuids(@concatenatedUserIds)) AND
			  ([PushNotifRegId] IS NOT NULL AND [PushNotifRegId] <> '')
		ORDER BY [UserId], [LastUpdateTimeUTC] DESC
GO