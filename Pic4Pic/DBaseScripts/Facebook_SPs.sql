-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewFacebookUser')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewFacebookUser]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewFacebookUser]
	 @UserId uniqueidentifier
	,@FacebookId bigint
	,@FirstName nvarchar(50)
	,@LastName nvarchar(50)
	,@FullName nvarchar(100)
	,@EmailAddress varchar(100)
	,@BirthDay datetime
	,@Gender tinyint
	,@MaritalStatus tinyint
	,@MaritalStatusAsText nvarchar(20)
	,@Profession nvarchar(100)
	,@EducationLevel tinyint
	,@FacebookLink varchar(200)
	,@FacebookUserName varchar(50)
	,@HometownCity nvarchar(100)
	,@HometownState nvarchar(50)
	,@HometownId bigint
	,@TimeZoneOffset int
	,@CurrentLocationCity nvarchar(100)
	,@CurrentLocationState nvarchar(50)
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
			   ,[MaritalStatus]
			   ,[MaritalStatusAsText]
			   ,[Profession]
			   ,[EducationLevel]
			   ,[FacebookLink]
			   ,[FacebookUserName]
			   ,[HometownCity]
			   ,[HometownState]
			   ,[HometownId]
			   ,[TimeZoneOffset]
			   ,[CurrentLocationCity]
			   ,[CurrentLocationState]
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
               ,@MaritalStatus
               ,@MaritalStatusAsText
               ,@Profession
               ,@EducationLevel
               ,@FacebookLink
               ,@FacebookUserName
               ,@HometownCity
			   ,@HometownState
               ,@HometownId
               ,@TimeZoneOffset
               ,@CurrentLocationCity
               ,@CurrentLocationState
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
	 @UserId uniqueidentifier
	,@FacebookId bigint
	,@FirstName nvarchar(50)
	,@LastName nvarchar(50)
	,@FullName nvarchar(100)
	,@EmailAddress varchar(100)
	,@BirthDay datetime
	,@Gender tinyint
	,@MaritalStatus tinyint
	,@MaritalStatusAsText nvarchar(20)
	,@Profession nvarchar(100)
	,@EducationLevel tinyint
	,@FacebookLink varchar(200)
	,@FacebookUserName varchar(50)
	,@HometownCity nvarchar(100)
	,@HometownState nvarchar(50)
	,@HometownId bigint
	,@TimeZoneOffset int
	,@CurrentLocationCity nvarchar(100)
	,@CurrentLocationState nvarchar(50)
	,@CurrentLocationId bigint
	,@ISOLocale varchar(10)
	,@IsVerified bit
	,@PhotoUrl varchar(200)
AS
	UPDATE [dbo].[FacebookUser]
		SET  [FirstName] = @FirstName
			,[LastName] = @LastName
			,[FullName] = @FullName
			,[EmailAddress] = @EmailAddress
			,[BirthDay] = @BirthDay
			,[Gender] = @Gender
			,[MaritalStatus] = @MaritalStatus
			,[MaritalStatusAsText] = @MaritalStatusAsText
			,[Profession] = @Profession
			,[EducationLevel] = @EducationLevel
			,[FacebookLink] = @FacebookLink
			,[FacebookUserName] = @FacebookUserName
			,[HometownCity] = @HometownCity
			,[HometownState] = @HometownState
			,[HometownId] = @HometownId
			,[TimeZoneOffset] = @TimeZoneOffset
			,[CurrentLocationCity] = @CurrentLocationCity
			,[CurrentLocationState] = @CurrentLocationState
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
	 @UserId uniqueidentifier
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
	  SELECT [FB].[UserId]
			,[FB].[FacebookId]
			,[FB].[FirstName]
			,[FB].[LastName]
			,[FB].[FullName]
			,[FB].[EmailAddress]
			,[FB].[BirthDay]
			,[FB].[Gender]
			,[FB].[MaritalStatus]
			,[FB].[MaritalStatusAsText]
			,[FB].[Profession]
			,[FB].[EducationLevel]
			,[FB].[FacebookLink]
			,[FB].[FacebookUserName]
			,[FB].[HometownCity]
			,[FB].[HometownState]
			,[FB].[HometownId]
			,[FB].[TimeZoneOffset]
			,[FB].[CurrentLocationCity]
			,[FB].[CurrentLocationState]
			,[FB].[CurrentLocationId]
			,[FB].[ISOLocale]
			,[FB].[IsVerified]
			,[FB].[PhotoUrl]
			,[D].[Description]
			,[FB].[CreateTimeUTC]
			,[FB].[LastUpdateTimeUTC]			
		FROM [dbo].[FacebookUser] [FB]
		LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [FB].[UserId]
		WHERE [FB].[FacebookId] = @FacebookId;
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
	  SELECT [FB].[UserId]
			,[FB].[FacebookId]
			,[FB].[FirstName]
			,[FB].[LastName]
			,[FB].[FullName]
			,[FB].[EmailAddress]
			,[FB].[BirthDay]
			,[FB].[Gender]
			,[FB].[MaritalStatus]
			,[FB].[MaritalStatusAsText]
			,[FB].[Profession]
			,[FB].[EducationLevel]
			,[FB].[FacebookLink]
			,[FB].[FacebookUserName]
			,[FB].[HometownCity]
			,[FB].[HometownState]
			,[FB].[HometownId]
			,[FB].[TimeZoneOffset]
			,[FB].[CurrentLocationCity]
			,[FB].[CurrentLocationState]
			,[FB].[CurrentLocationId]
			,[FB].[ISOLocale]
			,[FB].[IsVerified]
			,[FB].[PhotoUrl]
			,[D].[Description]
			,[FB].[CreateTimeUTC]
			,[FB].[LastUpdateTimeUTC]			
		FROM [dbo].[FacebookUser] [FB]
		LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [FB].[UserId]
		WHERE [FB].[UserId] IN (SELECT * FROM TokenizeGuids(@concatenatedUserIdsAsText));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetFacebookUsersByUserIDsWithExtensions')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetFacebookUsersByUserIDsWithExtensions]');
END
GO

CREATE PROCEDURE [dbo].[GetFacebookUsersByUserIDsWithExtensions]
   @concatenatedUserIdsAsText varchar(max)
  ,@userIdToCheckPic4Pics uniqueidentifier
AS	
	  SELECT [FB].[UserId]
			,[FB].[FacebookId]
			,[FB].[FirstName]
			,[FB].[LastName]
			,[FB].[FullName]
			,[FB].[EmailAddress]
			,[FB].[BirthDay]
			,[FB].[Gender]
			,[FB].[MaritalStatus]
			,[FB].[MaritalStatusAsText]
			,[FB].[Profession]
			,[FB].[EducationLevel]
			,[FB].[FacebookLink]
			,[FB].[FacebookUserName]
			,[FB].[HometownCity]
			,[FB].[HometownState]
			,[FB].[HometownId]
			,[FB].[TimeZoneOffset]
			,[FB].[CurrentLocationCity]
			,[FB].[CurrentLocationState]
			,[FB].[CurrentLocationId]
			,[FB].[ISOLocale]
			,[FB].[IsVerified]
			,[FB].[PhotoUrl]
			,[D].[Description]
			,[FB].[CreateTimeUTC]
			,[FB].[LastUpdateTimeUTC]			
			,[U].[Username]
			,[U].[Type]
			,(SELECT COUNT(*) 
				FROM [dbo].[Pic4Pic] [P]
				WHERE (
						([P].[UserId1] = [FB].[UserId] AND [UserId2] = @userIdToCheckPic4Pics) OR ([P].[UserId2] = [FB].[UserId] AND [UserId1] = @userIdToCheckPic4Pics)
					  )
					  AND [P].[AcceptTimeUTC] IS NOT NULL
		   )	
		   AS [AcceptedPic4PicCount]
		FROM [dbo].[FacebookUser] [FB]
		JOIN [dbo].[User] [U] ON [U].[Id] = [FB].[UserId]
		LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [FB].[UserId]
		WHERE [FB].[UserId] IN (SELECT * FROM TokenizeGuids(@concatenatedUserIdsAsText));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetWorkHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetWorkHistory]');
END
GO

CREATE PROCEDURE [dbo].[GetWorkHistory]
	@FacebookId bigint
AS	
	SELECT [EmployerId]
		  ,[EmployerName]
		  ,[PositionId]
		  ,[PositionName]
		  ,[StartDate]
		  ,[EndDate]
	  FROM [dbo].[WorkHistory]
	  WHERE [FacebookId] = @FacebookId
	  ORDER BY [Order] ASC; 
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteWorkHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteWorkHistory]');
END
GO

CREATE PROCEDURE [dbo].[DeleteWorkHistory]
	@FacebookId bigint
AS	
	DELETE FROM [dbo].[WorkHistory] WHERE [FacebookId] = @FacebookId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AddToWorkHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AddToWorkHistory]');
END
GO

CREATE PROCEDURE [dbo].[AddToWorkHistory]
	 @FacebookId bigint
	,@Order tinyint
	,@EmployerId bigint
	,@EmployerName nvarchar(100)
	,@PositionId bigint
	,@PositionName nvarchar(100)
	,@StartDate datetime
	,@EndDate datetime
AS	
	INSERT INTO [dbo].[WorkHistory]
			   ([FacebookId]
			   ,[Order]
			   ,[EmployerId]
			   ,[EmployerName]
			   ,[PositionId]
			   ,[PositionName]
			   ,[StartDate]
			   ,[EndDate])
		 VALUES
			   (@FacebookId
			   ,@Order
			   ,@EmployerId
			   ,@EmployerName
			   ,@PositionId
			   ,@PositionName
			   ,@StartDate
			   ,@EndDate);

GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetEducationHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetEducationHistory]');
END
GO

CREATE PROCEDURE [dbo].[GetEducationHistory]
	@FacebookId bigint
AS	
	SELECT [Type]
		  ,[SchoolId]
		  ,[SchoolName]
		  ,[ConcentrationId]
		  ,[ConcentrationName]
		  ,[DegreeId]
		  ,[DegreeName]
		  ,[Year]
	  FROM [dbo].[EducationHistory]
	  WHERE [FacebookId] = @FacebookId
	  ORDER BY [Order] ASC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteEducationHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteEducationHistory]');
END
GO

CREATE PROCEDURE [dbo].[DeleteEducationHistory]
	@FacebookId bigint
AS	
	DELETE FROM [dbo].[EducationHistory] WHERE [FacebookId] = @FacebookId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AddToEducationHistory')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AddToEducationHistory]');
END
GO

CREATE PROCEDURE [dbo].[AddToEducationHistory]
	 @FacebookId bigint
	,@Order tinyint
	,@Type varchar(50)
	,@SchoolId bigint
	,@SchoolName nvarchar(100)
	,@ConcentrationId bigint
	,@ConcentrationName nvarchar(100)
	,@DegreeId bigint
	,@DegreeName nvarchar(100)
	,@Year int
AS	
	INSERT INTO [dbo].[EducationHistory]
           ([FacebookId]
           ,[Order]
           ,[Type]
           ,[SchoolId]
           ,[SchoolName]
           ,[ConcentrationId]
           ,[ConcentrationName]
           ,[DegreeId]
           ,[DegreeName]
           ,[Year])
     VALUES
           (@FacebookId
           ,@Order
           ,@Type
           ,@SchoolId
           ,@SchoolName
           ,@ConcentrationId
           ,@ConcentrationName
           ,@DegreeId
           ,@DegreeName
           ,@Year);
GO

