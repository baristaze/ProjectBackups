----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSignupActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSignupActs]');
END
GO

CREATE PROCEDURE [dbo].[GetSignupActs] 
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
		   CAST(1 AS tinyint) AS [AssetType], -- user
		   CAST(2 AS tinyint) AS [Action], -- signup
		   [U].[LastUpdateTimeUTC] AS [ActionTimeUTC],	-- now
		   ([F].[FirstName] + ' ' + [F].[LastName]) AS [ActionValue], -- baris taze
		   CAST(0 AS bigint) AS [TopicId],
		   CAST(NULL AS nvarchar(400)) AS [TopicTitle],
		   CAST(0 AS bigint) AS [EntryId],
		   CAST(NULL AS nvarchar(max)) AS [EntryContent]		   
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
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTopicActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTopicActs]');
END
GO

CREATE PROCEDURE [dbo].[GetTopicActs] 
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
		   CAST(2 AS tinyint) AS [AssetType], -- Topic
		   CAST(6 AS tinyint) AS [Action], -- Create
		   [T].[CreateTimeUTC] AS [ActionTimeUTC],
		   CAST(NULL AS nvarchar(100)) AS [ActionValue],
		   [T].[Id] AS [TopicId],
		   [T].[Title] AS [TopicTitle],
		   CAST(0 AS bigint) AS [EntryId],
		   CAST(NULL AS nvarchar(max)) AS [EntryContent]
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
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetEntryActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetEntryActs]');
END
GO

CREATE PROCEDURE [dbo].[GetEntryActs] 
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
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   CAST(6 AS tinyint) AS [Action], -- Create
		   [E].[CreateTimeUTC] AS [ActionTimeUTC],		   
		   CAST(NULL AS nvarchar(100)) AS [ActionValue],
		   [T].[Id] AS [TopicId],
		   [T].[Title] AS [TopicTitle],
		   [E].[Id] AS [EntryId],
		   [E].[Content] AS [EntryContent]
		FROM [dbo].[Entry] [E]
		JOIN [dbo].[User] [U] ON [U].[Id] = [E].[CreatedBy]
		JOIN [dbo].[Topic] [T] ON [T].[Id] = [E].[TopicId]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [E].[LastUpdateTimeUTC] >= @startTimeUtc 
			AND [E].[LastUpdateTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [E].[LastUpdateTimeUTC] DESC;
GO
----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetVoteActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetVoteActs]');
END
GO

CREATE PROCEDURE [dbo].[GetVoteActs] 
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
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   CAST(11 AS tinyint) AS [Action], -- Vote
		   [V].[VoteTimeUTC] AS [ActionTimeUTC],
		   CAST([V].[VoteValue] AS nvarchar(100)) AS [ActionValue],
		   [T].[Id] AS [TopicId],
		   [T].[Title] AS [TopicTitle],
		   [E].[Id] AS [EntryId],
		   [E].[Content] AS [EntryContent]
		FROM [dbo].[Vote] [V]
		JOIN [dbo].[User] [U] ON [U].[Id] = [V].[UserId]
		JOIN [dbo].[Entry] [E] ON [V].[EntryId] = [E].[Id]
		JOIN [dbo].[Topic] [T] ON [T].[Id] = [E].[TopicId]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [V].[VoteTimeUTC] >= @startTimeUtc 
			AND [V].[VoteTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [V].[VoteTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetReactionActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetReactionActs]');
END
GO

CREATE PROCEDURE [dbo].[GetReactionActs] 
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
		   CAST(3 AS tinyint) AS [AssetType], -- Entry
		   CAST(12 AS tinyint) AS [Action], -- React
		   [R].[ReactionTimeUTC] AS [ActionTimeUTC],		   
		   CAST([R].[ReactionTypeId] AS nvarchar(100)) AS [ActionValue],
		   [T].[Id] AS [TopicId],
		   [T].[Title] AS [TopicTitle],
		   [E].[Id] AS [EntryId],
		   [E].[Content] AS [EntryContent]
		FROM [dbo].[Reaction] [R]
		JOIN [dbo].[User] [U] ON [U].[Id] = [R].[UserId]
		JOIN [dbo].[Entry] [E] ON [R].[EntryId] = [E].[Id]
		JOIN [dbo].[Topic] [T] ON [T].[Id] = [E].[TopicId]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		WHERE [R].[ReactionTimeUTC] >= @startTimeUtc 
			AND [R].[ReactionTimeUTC] < @endTimeUtc
			AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
			ORDER BY [R].[ReactionTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetInvitationActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetInvitationActs]');
END
GO

CREATE PROCEDURE [dbo].[GetInvitationActs] 
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
		   CAST(2 AS tinyint) AS [AssetType], -- Topic
		   CAST(5 AS tinyint) AS [Action], -- Invite
		   [I].[SendTimeUTC] AS [ActionTimeUTC],		   
		   CAST([I].[InviteeCount] AS nvarchar(100)) AS [ActionValue],
		   [T].[Id] AS [TopicId],
		   [T].[Title] AS [TopicTitle],
		   CAST(0 AS bigint) AS [EntryId],
		   CAST(NULL AS nvarchar(max)) AS [EntryContent]
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
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetOtherSocialShareActs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetOtherSocialShareActs]');
END
GO

CREATE PROCEDURE [dbo].[GetOtherSocialShareActs] 
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
		   CAST((CASE WHEN [EntryId] > 0 THEN 3 ELSE 2 END) AS tinyint) AS [AssetType], -- Topic | Entry
		   CAST(10 AS tinyint) AS [Action], -- Share
		   [S].[ShareTimeUTC] AS [ActionTimeUTC],
		   CAST([S].[SocialChannel] AS nvarchar(100)) AS [ActionValue],
			[T].[Id] AS [TopicId],
			[T].[Title] AS [TopicTitle],
			[E].[Id] AS [EntryId],
			[E].[Content] AS [EntryContent]
		FROM [dbo].[SocialShare] [S]
		-- this is left join since we have userId=null if the user is anonymous
		LEFT JOIN [dbo].[User] [U] ON [U].[Id] = [S].[UserId]
		LEFT JOIN [dbo].[FacebookUser] [F] ON [U].[Id] = [F].[UserId]
		LEFT JOIN [dbo].[Topic] [T] ON [T].[Id] = [S].[TopicId]
		LEFT JOIN [dbo].[Entry] [E] ON [E].[Id] = [S].[EntryId]
			WHERE [S].[ShareTimeUTC] >= @startTimeUtc 
				AND [S].[ShareTimeUTC] < @endTimeUtc
				AND (@ApplyUserFilter = 0 OR [U].[Id] = @UserId)
		ORDER BY [S].[ShareTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetActions')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetActions]');
END
GO

CREATE PROCEDURE [dbo].[GetActions] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@ApplyUserFilter bit
,@UserId bigint
AS
	declare @ActionTable TABLE ( [UserId] bigint, 
								 [UserType] tinyint, 
								 [UserSplit] int,
								 [FacebookId] bigint,
								 [PhotoUrl] varchar(200),
								 [FirstName] nvarchar(50),
								 [LastName] nvarchar(50),
								 [Gender] tinyint,
								 [FacebookLink] varchar(200),
								 [Hometown] nvarchar(100),
								 [AssetType] tinyint,
								 [Action] tinyint,
								 [ActionTimeUTC] datetime,
								 [ActionValue] nvarchar(100),
								 [TopicId] bigint,
								 [TopicTitle] nvarchar(400),
								 [EntryId] bigint,
								 [EntryContent] nvarchar(max));
							 
	INSERT INTO @ActionTable
		EXEC [dbo].[GetSignupActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
	
	INSERT INTO @ActionTable	
		EXEC [dbo].[GetTopicActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @ActionTable
		EXEC [dbo].[GetEntryActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @ActionTable
		EXEC [dbo].[GetVoteActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;

	INSERT INTO @ActionTable
		EXEC [dbo].[GetReactionActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
	
	INSERT INTO @ActionTable
		EXEC [dbo].[GetInvitationActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
		
	INSERT INTO @ActionTable
		EXEC [dbo].[GetOtherSocialShareActs] @startTimeUtc, @endTimeUtc, @TopX, @ApplyUserFilter, @UserId;
		
	SELECT TOP (@TopX) * FROM @ActionTable ORDER BY [ActionTimeUTC] DESC;
GO
