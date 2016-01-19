----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRecentPopularTopics')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRecentPopularTopics]');
END
GO

CREATE PROCEDURE [dbo].[GetRecentPopularTopics] 
 @startTimeUtc datetime
,@endTimeUtc datetime
,@TopX int
,@CheckStatus bit
,@Status tinyint
,@TopWritersLimit int
AS
	declare @recentTopics TABLE([TopicId] bigint, [TimeUTC] datetime);
	
	-- get recently created TOPICS
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[Id] AS [TopicId],
				CASE WHEN [CreateTimeUTC] >= [LastUpdateTimeUTC] THEN [CreateTimeUTC] ELSE [LastUpdateTimeUTC] END AS [TimeUTC]
			FROM [dbo].[Topic]
			WHERE ((@CheckStatus = 0) OR ([Status] = @Status))
				  AND 
				  (
						(([CreateTimeUTC] >= @startTimeUtc) AND ([CreateTimeUTC] <= @endTimeUtc))
							OR 
						(([LastUpdateTimeUTC] >= @startTimeUtc) AND ([LastUpdateTimeUTC] <= @endTimeUtc))
				   )
			ORDER BY [TimeUTC] DESC;
	
	-- get topics with recent ENTRIES
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[TopicId],
				MAX(CASE WHEN [CreateTimeUTC] >= [LastUpdateTimeUTC] THEN [CreateTimeUTC] ELSE [LastUpdateTimeUTC] END) AS [TimeUTC]
			FROM [dbo].[Entry]
			WHERE ((@CheckStatus = 0) OR ([Status] = @Status))
				  AND 
				  (
					(([CreateTimeUTC] >= @startTimeUtc) AND ([CreateTimeUTC] <= @endTimeUtc))
						OR 
					(([LastUpdateTimeUTC] >= @startTimeUtc) AND ([LastUpdateTimeUTC] <= @endTimeUtc))
				  )
			GROUP BY [TopicId]
			ORDER BY [TimeUTC] DESC;
	
	-- get topics with recently VOTEd entries
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[E].[TopicId], 
				MAX([V].[VoteTimeUTC]) AS [TimeUTC]
			FROM [dbo].[Vote] [V]
			JOIN [dbo].[Entry] [E] ON [V].[EntryId] = [E].[Id]
			WHERE ((@CheckStatus = 0) OR ([E].[Status] = @Status))
				  AND ([V].[VoteTimeUTC] >= @startTimeUtc) AND ([V].[VoteTimeUTC] <= @endTimeUtc)
			GROUP BY [E].[TopicId]
			ORDER BY [TimeUTC] DESC;
			
	-- get topics with recently REACTed entries
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[E].[TopicId], 
				MAX([R].[ReactionTimeUTC]) AS [TimeUTC]
			FROM [dbo].[Reaction] [R]
			JOIN [dbo].[Entry] [E] ON [R].[EntryId] = [E].[Id]
			WHERE ((@CheckStatus = 0) OR ([E].[Status] = @Status))
				  AND ([R].[ReactionTimeUTC] >= @startTimeUtc) AND ([R].[ReactionTimeUTC] <= @endTimeUtc)
			GROUP BY [E].[TopicId]
			ORDER BY [TimeUTC] DESC;
	
	-- get topics with recently SHAREd entries
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[S].[TopicId], 
				MAX([S].[ShareTimeUTC]) AS [TimeUTC]
			FROM [dbo].[SocialShare] [S]
			JOIN [dbo].[Topic] [T] ON [T].[Id] = [S].[TopicId]
			WHERE ((@CheckStatus = 0) OR ([T].[Status] = @Status))
				  AND ([S].[ShareTimeUTC] >= @startTimeUtc) AND ([S].[ShareTimeUTC] <= @endTimeUtc)
			GROUP BY [S].[TopicId]
			ORDER BY [TimeUTC] DESC;
	
	-- get topics with recent INVITATIONs
	INSERT INTO @recentTopics
		SELECT TOP(@TopX)
				[I].[TopicId], 
				MAX([I].[SendTimeUTC]) AS [TimeUTC]
			FROM [dbo].[TopicInvitesOnFacebook] [I]
			JOIN [dbo].[Topic] [T] ON [T].[Id] = [I].[TopicId]
			WHERE ((@CheckStatus = 0) OR ([T].[Status] = @Status))
				  AND ([I].[SendTimeUTC] >= @startTimeUtc) AND ([I].[SendTimeUTC] <= @endTimeUtc)
			GROUP BY [I].[TopicId]
			ORDER BY [TimeUTC] DESC;
				
	-- merge recent topics and select based on touch times
	-- define a new table for this purpose
	declare @selectedTopics TABLE([TopicId] bigint, [TimeUTC] datetime);
	INSERT INTO @selectedTopics
		SELECT TOP(@TopX)
				[TopicId], 
				MAX([TimeUTC]) AS [ActionTimeUTC]
			FROM @recentTopics 
			GROUP BY [TopicId]
			ORDER BY [ActionTimeUTC] DESC;
	
	-- get all involved entries to reduce the search cost
	declare @involvedEntries TABLE([TopicId] bigint, [EntryId] bigint);
	INSERT INTO @involvedEntries
		SELECT [TopicId], [Id] 
			FROM [dbo].[Entry]
			WHERE [TopicId] IN (SELECT [TopicId] FROM @selectedTopics)
				AND ((@CheckStatus = 0) OR ([Status] = @Status))
			ORDER BY [TopicId] ASC;
		
	-- prepare topics with extended properties
	;WITH [InnerTable] ([Id], [Status], [Title], [CreatedBy], [EntryCount], [VoteCount], [ReactionCount], [ShareCount], [InvitationCount], [TopWriters], [ActionTimeUTC])
	AS (
		SELECT [T].[Id]
			  ,[T].[Status]
			  ,[T].[Title]
			  ,[T].[CreatedBy]
			  
			  -- calculated column: [EntryCount]
			  ,(
					SELECT COUNT(*) FROM @involvedEntries [IE] WHERE [IE].[TopicId] = [T].[Id]
					
			   ) AS [EntryCount]
			   
			  -- calculated column : [VoteCount]
			  ,(
					SELECT COUNT(*) FROM [dbo].[Vote] [V] 
						WHERE [V].[EntryId] IN (
													SELECT [IE].[EntryId] FROM @involvedEntries [IE] WHERE [IE].[TopicId] = [T].[Id]
												)
			   ) AS [VoteCount]
			   
			   -- calculated column : [ReactionCount]
			   ,(
					SELECT COUNT(*) FROM [dbo].[Reaction] [R]
						WHERE [R].[EntryId] IN (
													SELECT [IE].[EntryId] FROM @involvedEntries [IE] WHERE [IE].[TopicId] = [T].[Id]
												)
			   ) AS [ReactionCount]
			   
			   -- calculated column : [ShareCount]
			   ,(
					SELECT COALESCE(SUM([ShareCount]), 0) FROM [dbo].[SocialShare] [S] WHERE [S].[TopicId] = [T].[Id]				
					
				) AS [ShareCount]
			    
			   -- calculated column : [InvitationCount]
			   ,(
					SELECT COALESCE(SUM([InviteeCount]), 0) FROM [dbo].[TopicInvitesOnFacebook] [I] WHERE [I].[TopicId] = [T].[Id]
					
				) AS [InvitationCount]
				
			   --  calculated column : [TopWriters]
			   ,(
					SELECT TOP (@TopWritersLimit) 
							[E].[CreatedBy]
						FROM [dbo].[Entry] [E]
						WHERE [E].[TopicId] = [T].[Id]
						GROUP BY [E].[CreatedBy] 
						ORDER BY COUNT(*) DESC
						FOR XML PATH('')
			   ) AS [TopWriters]

			   -- last action time
			   , [ST].[TimeUTC] AS [ActionTimeUTC] 

		  FROM [dbo].[Topic] [T]
		  JOIN @selectedTopics [ST] ON [ST].[TopicId] = [T].[Id]
		  WHERE -- check if it is recent
				[T].[Id] IN (SELECT [TopicId] FROM @selectedTopics)
				-- no need to check status filter since previous selects handle it already
				-- no need to check time filter since previous selects handle it already
	)
	SELECT [Id], [Status], [Title], [CreatedBy], [EntryCount], [VoteCount], [ReactionCount], [ShareCount], [InvitationCount],
		(	CAST(([VoteCount] + [ReactionCount] + [ShareCount] + [InvitationCount]) AS numeric(18,5)) 
			/ 
			--(CASE WHEN [EntryCount] > 0 THEN CAST([EntryCount] AS numeric(18,5)) ELSE CAST(1.0 AS numeric(18,5)) END)
			--([EntryCount] + 1)-- one is the topic itself
			(CASE WHEN [EntryCount] > 0 THEN [EntryCount] ELSE 1 END)
		) AS [SocialScore],
		[TopWriters]
		FROM [InnerTable]
		-- server is always UTC... We do +2 for Turkey's time... Social Score works per day
		ORDER BY CAST(dateadd(hour, 2, [ActionTimeUTC]) AS date) DESC, [SocialScore] DESC, [ActionTimeUTC] DESC;		

GO