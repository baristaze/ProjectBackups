EXEC('DROP PROCEDURE [dbo].[GetActionLogs]');
EXEC('DROP PROCEDURE [dbo].[GetEntryLogs]');
EXEC('DROP PROCEDURE [dbo].[GetOtherSocialShareLogs]');
EXEC('DROP PROCEDURE [dbo].[GetInvitationLogs]');
EXEC('DROP PROCEDURE [dbo].[GetReactionLogs]');
EXEC('DROP PROCEDURE [dbo].[GetVoteLogs]');
EXEC('DROP PROCEDURE [dbo].[GetTopicLogs]');
EXEC('DROP PROCEDURE [dbo].[GetSignupLogs]');
/*
----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSignupLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSignupLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetSignupLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(2 AS tinyint) AS [Action], -- signup
		   [U].[LastUpdateTimeUTC] AS [ActionTimeUTC],
		   CAST(1 AS tinyint) AS [AssetType], -- user
		   [U].[Id] AS [AssetId],
		   CAST(0 AS bigint) AS [AssetParentId],
		   ([F].[FirstName] + ' ' + [F].[LastName]) AS [AssetData],
		   CAST(NULL AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[User] [U]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [U].[Status] = 0 -- active/new
			AND [U].[LastUpdateTimeUTC] >= @startTimeUtc 
			AND [U].[LastUpdateTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [U].[LastUpdateTimeUTC] DESC; 
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTopicLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTopicLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetTopicLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(6 AS tinyint) AS [Action], -- Create
		   [T].[CreateTimeUTC] AS [ActionTimeUTC],
		   CAST(2 AS tinyint) AS [AssetType], -- Topic
		   [T].[Id] AS [AssetId],	   
		   CAST(0 AS bigint) AS [AssetParentId],
		   [T].[Title] AS [AssetData],
		   CAST(NULL AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[Topic] [T]
		JOIN [dbo].[User] [U] ON [U].[Id] = [T].[CreatedBy]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
			WHERE [T].[LastUpdateTimeUTC] >= @startTimeUtc 
				AND [T].[LastUpdateTimeUTC] < @endTimeUtc
				AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [T].[LastUpdateTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetEntryLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetEntryLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetEntryLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(6 AS tinyint) AS [Action], -- Create
		   [E].[CreateTimeUTC] AS [ActionTimeUTC],
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   [E].[Id] AS [AssetId],
		   [E].[TopicId] AS [AssetParentId],
		   [E].[Content] AS [AssetData],
		   CAST(NULL AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[Entry] [E]
		JOIN [dbo].[User] [U] ON [U].[Id] = [E].[CreatedBy]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [E].[LastUpdateTimeUTC] >= @startTimeUtc 
			AND [E].[LastUpdateTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [E].[LastUpdateTimeUTC] DESC;
GO
----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetVoteLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetVoteLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetVoteLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(11 AS tinyint) AS [Action], -- Vote
		   [V].[VoteTimeUTC] AS [ActionTimeUTC],
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   [V].[EntryId] AS [AssetId],
		   [E].[TopicId] AS [AssetParentId],
		   [E].[Content] AS [AssetData],
		   CAST([V].[VoteValue] AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[Vote] [V]
		JOIN [dbo].[User] [U] ON [U].[Id] = [V].[UserId]
		JOIN [dbo].[Entry] [E] ON [V].[EntryId] = [E].[Id]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [V].[VoteTimeUTC] >= @startTimeUtc 
			AND [V].[VoteTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [V].[VoteTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetReactionLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetReactionLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetReactionLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(12 AS tinyint) AS [Action], -- React
		   [R].[ReactionTimeUTC] AS [ActionTimeUTC],
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   [R].[EntryId] AS [AssetId],
		   [E].[TopicId] AS [AssetParentId],
		   [E].[Content] AS [AssetData],
		   CAST([R].[ReactionTypeId] AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[Reaction] [R]
		JOIN [dbo].[User] [U] ON [U].[Id] = [R].[UserId]
		JOIN [dbo].[Entry] [E] ON [R].[EntryId] = [E].[Id]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [R].[ReactionTimeUTC] >= @startTimeUtc 
			AND [R].[ReactionTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
			ORDER BY [R].[ReactionTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetInvitationLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetInvitationLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetInvitationLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(5 AS tinyint) AS [Action], -- Invite
		   [I].[SendTimeUTC] AS [ActionTimeUTC],
		   CAST(2 AS tinyint) AS [AssetType], -- Topic
		   [I].[TopicId] AS [AssetId],
		   CAST(0 AS bigint) AS [AssetParentId],
		   [T].[Title] AS [AssetData],
		   CAST([I].[InviteeCount] AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[TopicInvitesOnFacebook] [I]
		JOIN [dbo].[User] [U] ON [U].[Id] = [I].[SentBy]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		LEFT JOIN [dbo].[Topic] [T] ON [T].[Id] = [I].[TopicId]
			WHERE [I].[SendTimeUTC] >= @startTimeUtc 
				AND [I].[SendTimeUTC] < @endTimeUtc
				AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [I].[SendTimeUTC] DESC;
GO


----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetOtherSocialShareLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetOtherSocialShareLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetOtherSocialShareLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
  	SELECT TOP (@TopX)
		   [U].[Id] AS [UserId],
		   [U].[Type] AS [UserType],
		   COALESCE([U].[SplitId], 0) AS [UserSplit],
		   [F].[FacebookId],
		   [F].[PhotoUrl],
		   [F].[FirstName],
		   [F].[LastName],
		   [F].[Gender],
		   [F].[FacebookLink],
		   [F].[Hometown],
		   CAST(10 AS tinyint) AS [Action], -- Share
		   [S].[ShareTimeUTC] AS [ActionTimeUTC],
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   [S].[EntryId] AS [AssetId],
		   [E].[TopicId] AS [AssetParentId],
		   [E].[Content] AS [AssetData],
		   CAST([S].[SocialChannel] AS nvarchar(100)) AS [AssetData2]
		FROM [dbo].[SocialShare] [S]
		-- this is left join since we have userId=null if the user is anonymous
		LEFT JOIN [dbo].[User] [U] ON [U].[Id] = [S].[UserId]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		LEFT JOIN [dbo].[Entry] [E] ON [S].[EntryId] = [E].[Id]
			WHERE [S].[ShareTimeUTC] >= @startTimeUtc 
				AND [S].[ShareTimeUTC] < @endTimeUtc
				AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [S].[ShareTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetActionLogs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetActionLogs]');
END
GO

CREATE PROCEDURE [dbo].[GetActionLogs] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	declare @LogTable TABLE ([UserId] bigint, 
							 [UserType] tinyint, 
							 [UserSplit] int,
							 [FacebookId] bigint,
							 [PhotoUrl] varchar(200),
							 [FirstName] nvarchar(50),
							 [LastName] nvarchar(50),
							 [Gender] tinyint,
							 [FacebookLink] varchar(200),
							 [Hometown] nvarchar(100),
							 [Action] tinyint,
							 [ActionTimeUTC] datetime,
							 [AssetType] tinyint,
							 [AssetId] bigint,
							 [AssetParentId] bigint,
							 [AssetData] nvarchar(max),
							 [AssetData2] nvarchar(100));
							 
	INSERT INTO @LogTable
		EXEC [dbo].[GetSignupLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
	
	INSERT INTO @LogTable	
		EXEC [dbo].[GetTopicLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @LogTable
		EXEC [dbo].[GetEntryLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @LogTable
		EXEC [dbo].[GetVoteLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @LogTable
		EXEC [dbo].[GetReactionLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
	
	INSERT INTO @LogTable
		EXEC [dbo].[GetInvitationLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
		
	INSERT INTO @LogTable
		EXEC [dbo].[GetOtherSocialShareLogs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
		
	SELECT TOP (@TopX) * FROM @LogTable ORDER BY [ActionTimeUTC] DESC;
GO
*/