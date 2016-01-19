-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPreviewRecommendations')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPreviewRecommendations]');
END
GO

CREATE PROCEDURE [dbo].[GetPreviewRecommendations]
	 @maxCount int
	,@HomeTownState nvarchar(max)
	,@ConcatenatedCitiesInRange nvarchar(max)	
AS
	-- select recommendations and insert them into recommendations
	SELECT TOP (@maxCount) 
			 [FB].[UserId]
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
		WHERE [FB].[UserId] <> '6C3CF22D-4726-466A-AAA5-F2DDB2D0E0B6'
			AND [FB].[Gender] > 0
			AND [FB].[HometownState] = @HomeTownState
			AND [FB].[HometownCity] IN (SELECT * FROM TokenizeStrings(@ConcatenatedCitiesInRange))
		ORDER BY NEWID();
GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MakeNewRecommendations')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MakeNewRecommendations]');
END
GO

CREATE PROCEDURE [dbo].[MakeNewRecommendations]
	 @maxCount int
	,@UserId uniqueidentifier
	,@ExcludeFacebookFriends bit
	,@ExcludeRecentRecommendations bit
	,@RecentRecommendationsMinuteLimit int
	,@MatchState bit
	,@HomeTownState nvarchar(max)
	,@ConcatenatedCitiesInRange nvarchar(max)
	,@InsertToRecommendations bit
	
AS
	-- prepare cutoff time	
	declare @RecentRecommendationsCutOffTimeUTC datetime;
	set @RecentRecommendationsCutOffTimeUTC = dateadd(minute, -1 * @RecentRecommendationsMinuteLimit, GETUTCDATE());

	-- prepare my settings
	declare @FacebookId bigint;
	declare @InterestedIn tinyint;
	declare @State nvarchar(50);

	-- get my settings
	SELECT	@FacebookId = [FU].[FacebookId], 
			@State = [FU].[HometownState], 
			@InterestedIn = [UP].[InterestedIn]
		FROM [dbo].[FacebookUser] [FU]
		JOIN [dbo].[Preference] [UP] ON [FU].[UserId] = [UP].[UserId]
		WHERE [FU].[UserId] = @UserId;
	
	-- adjust state
	IF((@HomeTownState is NOT NULL) AND (LEN(@HomeTownState) > 0))
	BEGIN
		set @State = @HomeTownState;	
	END

	-- do we need to match cities?
	declare @MatchCities bit;
	set @MatchCities = 0;
	IF((@ConcatenatedCitiesInRange is NOT NULL) AND (LEN(@ConcatenatedCitiesInRange) > 0))
	BEGIN
		set @MatchCities = 1;	
	END

	-- adjust interest
	IF(@InterestedIn = 0)
	BEGIN
		set @InterestedIn = 3; -- both man and woman
	END
	
	-- prepare a temporaty table to keep the recommended Facebook users
	declare @facebookTable TABLE 
	(
		[UserId] [uniqueidentifier] NULL,
		[FacebookId] [bigint] NULL,
		[FirstName] [nvarchar](50) NULL,
		[LastName] [nvarchar](50) NULL,
		[FullName] [nvarchar](100) NULL,
		[EmailAddress] [varchar](100) NULL,
		[BirthDay] [datetime] NULL,
		[Gender] [tinyint] NULL,
		[MaritalStatus] [tinyint] NULL,
		[MaritalStatusAsText] [nvarchar](20) NULL,
		[Profession] [nvarchar](100) NULL,
		[EducationLevel] [tinyint] NULL,
		[FacebookLink] [varchar](200) NULL,
		[FacebookUserName] [varchar](50) NULL,
		[HometownCity] [nvarchar](100) NULL,
		[HometownState] [nvarchar](50) NULL,
		[HometownId] [bigint] NULL,
		[TimeZoneOffset] [int] NULL,
		[CurrentLocationCity] [nvarchar](100) NULL,
		[CurrentLocationState] [nvarchar](100) NULL,
		[CurrentLocationId] [bigint] NULL,
		[ISOLocale] [varchar](10) NULL,
		[IsVerified] [bit] NULL,
		[PhotoUrl] [varchar](200) NULL,
		[Description] nvarchar(max) NULL,
		[CreateTimeUTC] [datetime] NULL,
		[LastUpdateTimeUTC] [datetime] NULL
	);
	
	-- select recommendations and insert them into recommendations
	INSERT INTO @facebookTable
		SELECT TOP (@maxCount) 
				 [FB].[UserId]
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
			WHERE [FB].[UserId] <> @UserId AND [FB].[UserId] <> '6C3CF22D-4726-466A-AAA5-F2DDB2D0E0B6'
			  AND ((@InterestedIn & [FB].[Gender]) > 0)
			  AND (@ExcludeFacebookFriends = 0 OR [FB].[FacebookId] NOT IN (SELECT [FacebookId2] FROM [dbo].[FacebookFriend] WHERE [FacebookId1] = @FacebookId))	  
			  AND (@ExcludeRecentRecommendations = 0 OR [FB].[UserId] NOT IN (SELECT [UserId2] FROM [dbo].[Recommendation] WHERE [UserId1] = @UserId AND [RecommendTimeUTC] > @RecentRecommendationsCutOffTimeUTC))
			  AND (@MatchState = 0 OR ([FB].[HometownState] = @State))
			  AND (@MatchCities = 0 OR ([FB].[HometownCity] IN (SELECT * FROM TokenizeStrings(@ConcatenatedCitiesInRange))))
			ORDER BY NEWID();

	-- insert them into the table
	IF(@InsertToRecommendations = 1)
	BEGIN
		MERGE [dbo].[Recommendation] AS [Target]
		USING (SELECT @UserId AS [UserId1], [FT].[UserId] AS [UserId2] FROM @facebookTable [FT]) AS [Source]([UserId1], [UserId2])
		ON [Target].[UserId1] = [Source].[UserId1] AND [Target].[UserId2] = [Source].[UserId2]
		WHEN MATCHED THEN
			UPDATE SET [RecommendTimeUTC] = GETUTCDATE()
		WHEN NOT MATCHED BY TARGET THEN
			INSERT ([UserId1], [UserId2]) VALUES ([Source].[UserId1], [Source].[UserId2]);
	END

	-- return 
	SELECT * FROM @facebookTable;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRecentRecommendations')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRecentRecommendations]');
END
GO

CREATE PROCEDURE [dbo].[GetRecentRecommendations]
	 @maxCount int
	,@UserId uniqueidentifier
	,@RecentRecommendationsMinuteLimit int
AS
	-- prepare cutoff time	
	declare @RecentRecommendationsCutOffTimeUTC datetime;
	set @RecentRecommendationsCutOffTimeUTC = dateadd(minute, -1 * @RecentRecommendationsMinuteLimit, GETUTCDATE());
	
	SELECT TOP (@maxCount) 
				 [FB].[UserId]
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
		        ,[R].[RecommendTimeUTC]
	  FROM [dbo].[Recommendation] [R]
	   LEFT JOIN [dbo].[FacebookUser] [FB] ON [R].[UserId2] = [FB].[UserId]
	   LEFT JOIN [dbo].[UserDetails] [D] ON [D].[UserId] = [FB].[UserId]
	  WHERE [R].[UserId1] = @UserId 
		AND [R].[RecommendTimeUTC] > @RecentRecommendationsCutOffTimeUTC
	  ORDER BY [R].[RecommendTimeUTC] DESC;
GO