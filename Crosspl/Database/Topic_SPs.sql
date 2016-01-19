----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewTopic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewTopic]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewTopic] 
 @Title nvarchar(400)
,@SeoLink varchar(500)
,@CreatedBy bigint
AS
	declare @newId bigint;
	set @newId = -1;

	-- insert into topic
	INSERT INTO [dbo].[Topic] ([Title],[CreatedBy]) VALUES(@Title, @CreatedBy);
	
	-- get the recently generated auto id	
	set @newId = (SELECT SCOPE_IDENTITY());
	
	-- insert the SEO link
	INSERT INTO [dbo].[SeoLinkTopic]([Link],[TopicId]) VALUES(@SeoLink, @newId);
	
	-- return it
	SELECT @newId;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateTopic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateTopic]');
END
GO

CREATE PROCEDURE [dbo].[UpdateTopic]
 @Id bigint
,@Title nvarchar(400)
,@SeoLink varchar(500)
,@CheckUser bit
,@UserId bigint
AS
	UPDATE [dbo].[Topic] SET [Title] = @Title, [LastUpdateTimeUTC] = GETUTCDATE() 
		WHERE [Id] = @Id AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
	
	IF (@@ROWCOUNT > 0)
	BEGIN
		IF NOT EXISTS(SELECT [TopicId] FROM [dbo].[SeoLinkTopic] WHERE [TopicId] = @Id AND [Link] = @SeoLink)
		BEGIN
			-- insert the new SEO link
			INSERT INTO [dbo].[SeoLinkTopic]([Link],[TopicId]) VALUES(@SeoLink, @Id);
		END

	END
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteTopicIfNoEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteTopicIfNoEntry]');
END
GO

CREATE PROCEDURE [dbo].[DeleteTopicIfNoEntry]
 @Id bigint
,@CheckUser bit
,@UserId bigint
AS
	-- It is not necessary to begin a transaction: It is not worth it since transaction is costly.
	-- A raise condition might occur between two statements; i.e. another user might add an entry 
	-- while we decide that # of entries is 0, however, after this decision, we will attempt to delete
	-- the entry but it will fail since deleting a topic doesn't cascade the deletion to the entry table.
	-- i.e. we are safe: user will get an exception saying that "entry couldn't be deleted please try again".
	-- The other raise condition is to say that "we cannot delete this topic because there is an entry" msg
	-- although the entry has just been deleted. This is fine as well, user will re-try after refreshing the page.
	declare @deleted bit;
	set @deleted = 0;
	declare @count int;
	set @count = (SELECT COUNT(*)FROM [dbo].[Entry] WHERE [TopicId] = @Id);
	IF(@count = 0)
	BEGIN	
		DELETE FROM [dbo].[Topic] WHERE [Id] = @Id AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
		
		-- caller cannot rely on affected rows because the delete might get cascaded.
		-- However, he/she can rely on the below flag (@deleted=1)... This flag's interpretation
		-- might be a false-positive (no-exception but no-deletion either) but it will 
		-- confuse only malicious users who want to delete entries of other people.
		set @deleted = 1;
	END
	
	SELECT @deleted;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateTopicStatus')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateTopicStatus]');
END
GO

CREATE PROCEDURE [dbo].[UpdateTopicStatus]
 @Id bigint
,@Status tinyint
,@CheckUser bit
,@UserId bigint
AS
	UPDATE [dbo].[Topic] SET [Status] = @Status, [LastUpdateTimeUTC] = GETUTCDATE() 
		WHERE [Id] = @Id AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTopic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTopic]');
END
GO

CREATE PROCEDURE [dbo].[GetTopic]
 @Id bigint
,@CheckStatus bit
,@Status tinyint
AS
	-- all system categories (id+name) should be retrieved separately and get cached in mid-tier.
	-- we will only return this topic's category IDs as a concatenated string...
	
	-- positive bigint can be max 19 chars
	-- we are going to retrieve top 10 cats
	-- leave this variable as NULL at the beginning
	declare @categories varchar(200);
	SELECT TOP(10) -- we select [CategoryId] here.. though concatenate the rows
		@categories = COALESCE(@categories + ',', '') + CAST([CategoryId] AS varchar(20))
		FROM [dbo].[TopicCategory] WHERE [TopicId] = @Id;

	SELECT [Id]
		  ,[Status]
		  ,[Title]
		  ,[CreatedBy]
		  ,[CreateTimeUTC]
		  ,[LastUpdateTimeUTC]
		  ,(	
				-- sub query should perform better here since we are talking about 1 topic and approx. ~10 entries
				-- the alternative way is to make a LEFT JOIN and then GROUP BY and SUM over the grouped column
				SELECT COUNT(*) FROM [dbo].[Entry] WHERE [TopicId] = @Id AND ((@CheckStatus = 0) OR ([Status] = @Status))
				
		   )AS [EntryCount]
		  ,@categories AS [CategoryIDs]
	  FROM [dbo].[Topic] 
	  WHERE [Id] = @Id AND ((@CheckStatus = 0) OR ([Status] = @Status));	
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTopicByLink')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTopicByLink]');
END
GO

CREATE PROCEDURE [dbo].[GetTopicByLink]
 @SeoLink varchar(500)
,@CheckStatus bit
,@Status tinyint
AS
	declare @Id bigint;
	set @Id = (SELECT TOP(1) [TopicId] FROM [dbo].[SeoLinkTopic] WHERE [Link] = @SeoLink);
	IF(@Id IS NULL)
	BEGIN
		-- return 0 rows
		SELECT TOP(0) NULL AS [Id]
					 ,NULL AS [Status]
					 ,NULL AS [Title]
					 ,NULL AS [CreatedBy]
					 ,NULL AS [CreateTimeUTC]
					 ,NULL AS [LastUpdateTimeUTC]
					 ,NULL AS [EntryCount]
					 ,NULL AS [CategoryIDs];
	END
	ELSE
	BEGIN
		  EXEC [dbo].[GetTopic] @Id, @CheckStatus, @Status;
	END
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'RelateTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[RelateTopics]');
END
GO

CREATE PROCEDURE [dbo].[RelateTopics]
 @TopicId1 bigint
,@TopicId2 bigint
,@RelevanceRank int
AS
	IF EXISTS(SELECT * FROM [dbo].[RelatedTopic] WHERE [TopicId1] = @TopicId1 AND [TopicId2] = @TopicId2)
	BEGIN
		UPDATE [dbo].[RelatedTopic] SET [RelevanceRank] = @RelevanceRank WHERE [TopicId1] = @TopicId1 AND [TopicId2] = @TopicId2;
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[RelatedTopic] ([TopicId1], [TopicId2], [RelevanceRank]) VALUES (@TopicId1, @TopicId2, @RelevanceRank);
	END
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRelatedTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRelatedTopics]');
END
GO

CREATE PROCEDURE [dbo].[GetRelatedTopics]
 @Id bigint
,@CheckStatus bit
,@Status tinyint
AS
	declare @related TABLE ([TopicId] bigint, [RelevanceRank] int);
	INSERT INTO @related
		SELECT [TopicId2], [RelevanceRank]
			FROM [dbo].[RelatedTopic] 
			WHERE [TopicId1] = @Id;
	
	SELECT [T].[Id]
		  ,[T].[Status]
		  ,[T].[Title]
		  ,[T].[CreatedBy]
		  ,[T].[CreateTimeUTC]
		  ,[T].[LastUpdateTimeUTC]
		  ,(	
				-- sub query should perform better here since we are talking about 1 topic and approx. ~10 entries
				-- the alternative way is to make a LEFT JOIN and then GROUP BY and SUM over the grouped column
				SELECT COUNT(*) FROM [dbo].[Entry] WHERE [TopicId] = @Id AND ((@CheckStatus = 0) OR ([Status] = @Status))
				
		   )AS [EntryCount]
		   ,NULL AS [CategoryIDs] -- to have a single interface at mid-tier
	  FROM [dbo].[Topic] [T]
	  JOIN @related [R] ON [T].[Id] = [R].[TopicId]
		WHERE ((@CheckStatus = 0) OR ([T].[Status] = @Status)) 
			AND [T].[Id] IN (SELECT [TopicId] FROM @related)
		ORDER BY [R].[RelevanceRank] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'SearchTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[SearchTopics]');
END
GO

CREATE PROCEDURE [dbo].[SearchTopics]
 @Query nvarchar(400)
,@TopX int
,@CheckStatus bit
,@Status tinyint
AS
	SELECT TOP (@TopX)
		   [T].[Id]
		  ,[T].[Status]
		  ,[T].[Title]
		  ,[T].[CreatedBy]
		  ,[T].[CreateTimeUTC]
		  ,[T].[LastUpdateTimeUTC]
		  ,COUNT([E].[Id]) AS [EntryCount]
		  ,NULL AS [CategoryIDs]
	  FROM [dbo].[Topic] [T]
	  LEFT JOIN [dbo].[Entry] [E] ON [T].[Id] = [E].[TopicId] 
				AND ((@CheckStatus = 0) OR ([E].[Status] = @Status))
	  WHERE [T].[Title] LIKE @Query COLLATE LATIN1_GENERAL_CI_AI
				AND ((@CheckStatus = 0) OR ([T].[Status] = @Status))
	  GROUP BY [T].[Id], [T].[Status], [T].[Title], [T].[CreatedBy], [T].[CreateTimeUTC], [T].[LastUpdateTimeUTC]
	  ORDER BY [EntryCount] DESC, [T].[LastUpdateTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetLatestTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetLatestTopics]');
END
GO

CREATE PROCEDURE [dbo].[GetLatestTopics]
 @TopX int
,@CheckStatus bit
,@Status tinyint
AS
	SELECT TOP (@TopX)
		   [T].[Id]
		  ,[T].[Status]
		  ,[T].[Title]
		  ,[T].[CreatedBy]
		  ,[T].[CreateTimeUTC]
		  ,[T].[LastUpdateTimeUTC]
		  ,COUNT([E].[Id]) AS [EntryCount]
		  ,NULL AS [CategoryIDs]
	  FROM [dbo].[Topic] [T]
	  LEFT JOIN [dbo].[Entry] [E] ON [T].[Id] = [E].[TopicId] 
				AND ((@CheckStatus = 0) OR ([E].[Status] = @Status))
	  WHERE (@CheckStatus = 0) OR ([T].[Status] = @Status)
	  GROUP BY [T].[Id], [T].[Status], [T].[Title], [T].[CreatedBy], [T].[CreateTimeUTC], [T].[LastUpdateTimeUTC]
	  ORDER BY [T].[LastUpdateTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetTopicByAppRequestId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetTopicByAppRequestId]');
END
GO

CREATE PROCEDURE [dbo].[GetTopicByAppRequestId]
 @ConcatenatedAppRequestIds varchar(max)
,@CheckStatus bit
,@Status tinyint
AS
	declare @sharedTopics TABLE([TopicId] bigint, [SendTimeUTC] datetime);
	
	INSERT INTO @sharedTopics
		SELECT [TopicId],
			   [SendTimeUTC]
			FROM [dbo].[TopicInvitesOnFacebook] 
			WHERE [AppRequestId] IN (SELECT * FROM TokenizeIds(@ConcatenatedAppRequestIds));
	
	SELECT TOP 1
		   [T].[Id]
		  ,[T].[Status]
		  ,[T].[Title]
		  ,[T].[CreatedBy]
		  ,[T].[CreateTimeUTC]
		  ,[T].[LastUpdateTimeUTC]
		  ,0 AS [EntryCount]
		  ,NULL AS [CategoryIDs]
	  FROM [dbo].[Topic] [T]
	  JOIN @sharedTopics [R] ON [T].[Id] = [R].[TopicId]
	  WHERE ((@CheckStatus = 0) OR ([T].[Status] = @Status)) 
			AND [T].[Id] IN (SELECT DISTINCT [TopicId] FROM @sharedTopics)
	  ORDER BY [R].[SendTimeUTC] DESC;
GO


----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'InsertAppRequestForTopic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[InsertAppRequestForTopic]');
END
GO

CREATE PROCEDURE [dbo].[InsertAppRequestForTopic]
 @AppRequestId bigint
,@TopicId bigint
,@EntryId bigint
,@SentBy bigint
,@InviteeCount smallint
AS
	INSERT INTO [dbo].[TopicInvitesOnFacebook] (
		[AppRequestId], [TopicId], [EntryId], [SentBy], [InviteeCount]) 
			VALUES (@AppRequestId, @TopicId, @EntryId, @SentBy, @InviteeCount);
GO


----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetAppRequest')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetAppRequest]');
END
GO

CREATE PROCEDURE [dbo].[GetAppRequest]
 @ConcatenatedAppRequestIds varchar(max)
,@CheckStatus bit
,@Status tinyint
AS
	declare @sharedTopics TABLE([TopicId] bigint, [EntryId] bigint, [SentBy] bigint, [SendTimeUTC] datetime);
	
	INSERT INTO @sharedTopics
		SELECT [TopicId],
			   [EntryId],
			   [SentBy],
			   [SendTimeUTC]
			FROM [dbo].[TopicInvitesOnFacebook] 
			WHERE [AppRequestId] IN (SELECT * FROM TokenizeIds(@ConcatenatedAppRequestIds));
	
	SELECT TOP 1
		   [T].[Id] AS [TopicId]
		  ,[T].[Title] AS [TopicTitle]
		  ,[R].[EntryId]
		  ,[R].[SentBy]
		  ,[F].[FacebookId] AS [SenderFacebookId]
		  ,[F].[FirstName] AS [SenderName]
		  ,[U].[SplitId] AS [SenderSplitId]
		  ,[F].[PhotoUrl] AS [SenderPhotoUrl]
	  FROM [dbo].[Topic] [T]
	  JOIN @sharedTopics [R] ON [T].[Id] = [R].[TopicId]
	  LEFT JOIN [FacebookUser] [F] ON [R].[SentBy] = [F].[UserId]
	  LEFT JOIN [User] [U] ON [U].[Id] = [R].[SentBy]
	  WHERE ((@CheckStatus = 0) OR ([T].[Status] = @Status)) 
			AND [T].[Id] IN (SELECT DISTINCT [TopicId] FROM @sharedTopics)
	  ORDER BY [R].[SendTimeUTC] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPromotedTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPromotedTopics]');
END
GO

CREATE PROCEDURE [dbo].[GetPromotedTopics]
 @CheckTopicStatus bit
,@TopicStatus tinyint
,@CheckPromotionStatus bit
,@IsPromotionActive bit
,@CheckPromotionDate bit
AS
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();
	
	SELECT [T].[Id]
		  ,[T].[Status]
		  ,[T].[Title]
		  ,[T].[CreatedBy]
		  ,[T].[CreateTimeUTC]
		  ,[T].[LastUpdateTimeUTC]
		  ,0 AS [EntryCount]
		  ,NULL AS [CategoryIDs]
	  FROM [dbo].[Topic] [T]
	  JOIN [dbo].[PromotedTopic] [P] ON [P].[TopicId] = [T].[Id]
	  WHERE ((@CheckTopicStatus = 0) OR ([T].[Status] = @TopicStatus))
			AND ((@CheckPromotionStatus = 0) OR ([P].[IsActive] = @IsPromotionActive))
			AND (
					(@CheckPromotionDate = 0) 
					OR 
					(
						([P].[StartTimeUTC] is NULL OR [P].[StartTimeUTC] <= @utcNow) 
						AND 
						([P].[EndTimeUTC] is NULL OR [P].[EndTimeUTC] >= @utcNow)
					)
				)
	  ORDER BY [P].[ViewOrder] ASC, [P].[StartTimeUTC] ASC;
GO