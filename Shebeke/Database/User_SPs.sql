-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewUser]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewUser]
@SplitId int
AS
	declare @newUserId bigint;
	set @newUserId = -1;

	-- insert into AutoId to get a new checkin Id
	INSERT INTO [dbo].[User] ([SplitId]) VALUES (@SplitId);
	
	-- get the recently generated auto id	
	-- MySQL equivalent of this is LAST_INSERT_ID()
	set @newUserId = (SELECT SCOPE_IDENTITY());
	
	-- return it
	SELECT @newUserId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateUsersSplit')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateUsersSplit]');
END
GO

CREATE PROCEDURE [dbo].[UpdateUsersSplit]
 @Id bigint
,@SplitId int
AS
	IF (@SplitId is NULL)
	BEGIN
		UPDATE [dbo].[User] SET [SplitId] = NULL WHERE [Id] = @Id;
		SELECT 0;
	END
	ELSE
	BEGIN
	
		IF EXISTS(SELECT [Id] FROM [dbo].[Split] WHERE [Id] = @SplitId)
		BEGIN
			UPDATE [dbo].[User] SET [SplitId] = @SplitId WHERE [Id] = @Id;			
			SELECT @SplitId;
		END
		ELSE
		BEGIN
			SELECT COALESCE([SplitId], 0) FROM [dbo].[User] WHERE [Id] = @Id;
		END
	END
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteUser]');
END
GO

CREATE PROCEDURE [dbo].[DeleteUser]
 @Id bigint
AS
	DELETE FROM [dbo].[User] WHERE [Id] = @Id;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewFacebookUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewFacebookUser]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewFacebookUser]
	 @UserId bigint
	,@FacebookId bigint
	,@FirstName nvarchar(50)
	,@LastName nvarchar(50)
	,@FullName nvarchar(100)
	,@EmailAddress varchar(100)
	,@BirthDay datetime
	,@Gender tinyint
	,@FacebookLink varchar(200)
	,@FacebookUserName varchar(50)
	,@Hometown nvarchar(100)
	,@HometownId bigint
	,@TimeZoneOffset int
	,@CurrentLocation nvarchar(100)
	,@CurrentLocationId bigint
	,@ISOLocale varchar(10)
	,@IsVerified bit
	,@PhotoUrl varchar(200)
AS
	INSERT INTO [dbo].[FacebookUser]
			   ([UserId]
			   ,[FacebookId]
			   ,[FirstName]
			   ,[LastName]
			   ,[FullName]
			   ,[EmailAddress]
			   ,[BirthDay]
			   ,[Gender]
			   ,[FacebookLink]
			   ,[FacebookUserName]
			   ,[Hometown]
			   ,[HometownId]
			   ,[TimeZoneOffset]
			   ,[CurrentLocation]
			   ,[CurrentLocationId]
			   ,[ISOLocale]
			   ,[IsVerified]
			   ,[PhotoUrl])
		 VALUES
			   (@UserId
               ,@FacebookId
               ,@FirstName
               ,@LastName
               ,@FullName
               ,@EmailAddress
               ,@BirthDay
               ,@Gender
               ,@FacebookLink
               ,@FacebookUserName
               ,@Hometown
               ,@HometownId
               ,@TimeZoneOffset
               ,@CurrentLocation
               ,@CurrentLocationId
               ,@ISOLocale
               ,@IsVerified
               ,@PhotoUrl);
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateFacebookUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateFacebookUser]');
END
GO

CREATE PROCEDURE [dbo].[UpdateFacebookUser]
	 @UserId bigint
	,@FacebookId bigint
	,@FirstName nvarchar(50)
	,@LastName nvarchar(50)
	,@FullName nvarchar(100)
	,@EmailAddress varchar(100)
	,@BirthDay datetime
	,@Gender tinyint
	,@FacebookLink varchar(200)
	,@FacebookUserName varchar(50)
	,@Hometown nvarchar(100)
	,@HometownId bigint
	,@TimeZoneOffset int
	,@CurrentLocation nvarchar(100)
	,@CurrentLocationId bigint
	,@ISOLocale varchar(10)
	,@IsVerified bit
	,@PhotoUrl varchar(200)
AS
	UPDATE [dbo].[FacebookUser]
					   SET [FirstName] = @FirstName
						  ,[LastName] = @LastName
						  ,[FullName] = @FullName
						  ,[EmailAddress] = @EmailAddress
						  ,[BirthDay] = @BirthDay
						  ,[Gender] = @Gender
						  ,[FacebookLink] = @FacebookLink
						  ,[FacebookUserName] = @FacebookUserName
						  ,[Hometown] = @Hometown
						  ,[HometownId] = @HometownId
						  ,[TimeZoneOffset] = @TimeZoneOffset
						  ,[CurrentLocation] = @CurrentLocation
						  ,[CurrentLocationId] = @CurrentLocationId
						  ,[ISOLocale] = @ISOLocale
						  ,[IsVerified] = @IsVerified
						  ,[PhotoUrl] = @PhotoUrl
						  ,[LastUpdateTimeUTC] = GETUTCDATE()
					 WHERE [UserId] = @UserId AND [FacebookId] = @FacebookId;

GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteFacebookUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteFacebookUser]');
END
GO

CREATE PROCEDURE [dbo].[DeleteFacebookUser]
	 @UserId bigint
	,@FacebookId bigint
AS
	DELETE FROM [dbo].[FacebookUser] WHERE [UserId] = @UserId AND [FacebookId] = @FacebookId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetFacebookUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetFacebookUser]');
END
GO

CREATE PROCEDURE [dbo].[GetFacebookUser]
 @FacebookId bigint
AS	
	  SELECT [F].[UserId]
			,[F].[FacebookId]
			,[U].[Type] AS [UserType]
			,[U].[Status] AS [UserStatus]
			,[U].[SplitId]
			,[F].[FirstName]
			,[F].[LastName]
			,[F].[FullName]
			,[F].[EmailAddress]
			,[F].[BirthDay]
			,[F].[Gender]
			,[F].[FacebookLink]
			,[F].[FacebookUserName]
			,[F].[Hometown]
			,[F].[HometownId]
			,[F].[TimeZoneOffset]
			,[F].[CurrentLocation]
			,[F].[CurrentLocationId]
			,[F].[ISOLocale]
			,[F].[IsVerified]
			,[F].[CreateTimeUTC]
			,[F].[LastUpdateTimeUTC]
			,[F].[PhotoUrl]
		FROM [dbo].[FacebookUser] [F]
		JOIN [dbo].[User] [U] ON [U].[Id] = [F].[UserId]
		WHERE [F].[FacebookId] = @FacebookId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetFacebookUsersByID')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetFacebookUsersByID]');
END
GO

CREATE PROCEDURE [dbo].[GetFacebookUsersByID]
 @concatenatedUserIdsAsText varchar(max)
AS	
	  SELECT [F].[UserId]
			,[F].[FacebookId]
			,[U].[Type] AS [UserType]
			,[U].[Status] AS [UserStatus]
			,[U].[SplitId]
			,[F].[FirstName]
			,[F].[LastName]
			,[F].[FullName]
			,[F].[EmailAddress]
			,[F].[BirthDay]
			,[F].[Gender]
			,[F].[FacebookLink]
			,[F].[FacebookUserName]
			,[F].[Hometown]
			,[F].[HometownId]
			,[F].[TimeZoneOffset]
			,[F].[CurrentLocation]
			,[F].[CurrentLocationId]
			,[F].[ISOLocale]
			,[F].[IsVerified]
			,[F].[CreateTimeUTC]
			,[F].[LastUpdateTimeUTC]
			,[F].[PhotoUrl]
		FROM [dbo].[FacebookUser] [F]
		JOIN [dbo].[User] [U] ON [U].[Id] = [F].[UserId]
		WHERE [F].[UserId] IN (SELECT * FROM TokenizeIds(@concatenatedUserIdsAsText));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CheckUserActivity')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CheckUserActivity]');
END
GO
 
CREATE PROCEDURE [dbo].[CheckUserActivity] 
 @UserId bigint 
,@ActionId tinyint
,@TimeWindowInSeconds int
,@ActionCountLimit int
AS
	declare @now datetime;
	declare @pre datetime;
	set @now = GETUTCDATE();
	set @pre = dateadd(second, -1 * @TimeWindowInSeconds, @now);

	declare @count int;
	set @count = (SELECT COUNT(*) FROM [dbo].[UserActionLog] 
					WHERE [UserId] = @UserId AND [ActionId] = @ActionId AND [ActionTimeUTC] > @pre);

	declare @result int;
	IF(@count >= @ActionCountLimit)
	BEGIN
		UPDATE [User] set [Status] = 1 where [Id] = @UserId;
		set @result = -1;
	END
	ELSE
	BEGIN		
		set @result = 1;
	END

	SELECT @result;
GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'LogUserActivity')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[LogUserActivity]');
END
GO
CREATE PROCEDURE [dbo].[LogUserActivity] 
 @UserId bigint 
,@ActionId tinyint
AS
	INSERT INTO [dbo].[UserActionLog] ([UserId], [ActionId]) VALUES (@UserId, @ActionId);
GO
