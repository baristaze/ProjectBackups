----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetEntries')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetEntries]');
END
GO

CREATE PROCEDURE [dbo].[GetEntries]
 @TopicId bigint
,@CurrentUserId bigint
,@Offset int
,@NextFetchCount int
,@CheckStatus bit
,@Status tinyint
,@IncludedEntryId bigint
,@FromOldToNew bit
AS
	-- ////////////////////////////////////////////////////////////////////////////////////////////////////--
	-- firstly decide which entries will be shown
	-- old to new: 1,2,3 have been served before... now user requests after 3... 4,5,6 will be served
	-- new to old: 6,5,4 have been served before... now user requests after 4... 3,2,1 will be served	
	-- 'top' will be executed after 'order by'
	declare @selectedEntries TABLE( [Id] bigint,
									[Status] tinyint,
									[TopicId] bigint,
									[Content] nvarchar(max),
									[FormatVersion] int,
									[CreatedBy] bigint,
									[CreateTimeUTC] datetime,
									[LastUpdateTimeUTC] datetime);

	INSERT INTO @selectedEntries
		SELECT * FROM
		(
			SELECT 	[Id], 
					[Status], 
					[TopicId], 
					[Content], 
					[FormatVersion], 
					[CreatedBy], 
					[CreateTimeUTC], 
					[LastUpdateTimeUTC]
				FROM [dbo].[Entry]
				WHERE [TopicId] = @TopicId 
					AND ((@CheckStatus = 0) OR ([Status] = @Status))
					AND [Id] = @IncludedEntryId
			UNION ALL
			SELECT 	[Id], 
					[Status], 
					[TopicId], 
					[Content], 
					[FormatVersion], 
					[CreatedBy], 
					[CreateTimeUTC], 
					[LastUpdateTimeUTC]
				FROM [dbo].[Entry]
				WHERE [TopicId] = @TopicId 
					AND ((@CheckStatus = 0) OR ([Status] = @Status))
					AND [Id] <> @IncludedEntryId
				ORDER BY CASE WHEN @FromOldToNew = 1 THEN [Id] ELSE -[Id] END ASC 
					OFFSET @Offset ROWS FETCH NEXT @NextFetchCount ROWS ONLY
		 ) AS [All];


	-- ////////////////////////////////////////////////////////////////////////////////////////////////////--
	-- then calculate the voting results for selected entry IDs
	declare @votings TABLE(	[EntryId] bigint,
							[MyVote] int, 
							[UpVoteCount] int, 
							[DownVoteCount] int, 
							[UpVoteSum] int, 
							[DownVoteSum] int, 
							[NetVoteSum] int);
	
	INSERT INTO @votings
		SELECT [EntryId],
			   SUM(CASE WHEN [UserId] = @CurrentUserId THEN [VoteValue] ELSE 0 END) AS [MyVote],
			   SUM(CASE WHEN [VoteValue] > 0 THEN 1 ELSE 0 END) AS [UpVoteCount],
			   SUM(CASE WHEN [VoteValue] < 0 THEN 1 ELSE 0 END) AS [DownVoteCount],
			   SUM(CASE WHEN [VoteValue] > 0 THEN [VoteValue] ELSE 0 END) AS [UpVoteSum], 
			   SUM(CASE WHEN [VoteValue] < 0 THEN [VoteValue] ELSE 0 END) AS [DownVoteSum],
			   SUM([VoteValue]) AS [NetVoteSum]
			FROM [Vote]
			WHERE [EntryId] IN (SELECT [Id] FROM @selectedEntries)
			GROUP BY [EntryId];
	
	-- ////////////////////////////////////////////////////////////////////////////////////////////////////--
	-- select and calculate reactions
	declare @reactionResults TABLE(
		[EntryId] bigint, 
		[Top1ReactionId] bigint, 
		[Top2ReactionId] bigint,
		[Top3ReactionId] bigint,
		[Top1ReactionCount] int,
		[Top2ReactionCount] int,
		[Top3ReactionCount] int,
		[TotalReactionCount] int,
		[MyReactions] bigint);
		
	;WITH [InnerTable]([EntryId], [ReactionTypeId], [ReactionCount], [MeReacted], [RowNumber])
	AS (
			SELECT	[EntryId], 
					[ReactionTypeId], 
					COUNT(*) AS [ReactionCount],
					-- null is important here since we are counting. do not replace it with 0
					COUNT(CASE WHEN [UserId] = @CurrentUserId THEN 1 ELSE NULL END) AS [MeReacted], 
					-- this help us to rank each sub-group separately
					ROW_NUMBER() OVER(PARTITION BY [EntryId] ORDER BY COUNT(*) DESC) AS [RowNumber]
				FROM [Reaction] WHERE [EntryId] IN (SELECT [Id] FROM @selectedEntries)
				GROUP BY [EntryId], [ReactionTypeId]
	   )
	   INSERT INTO @reactionResults
			SELECT [EntryId], 
				   -- this sum will give the top 1st reaction (id) for each entry. sum is like 0+x+0+0+0+0+0
				   SUM(CASE WHEN [RowNumber] = 1 THEN ReactionTypeId ELSE 0 END) AS [Top1ReactionId],
				   SUM(CASE WHEN [RowNumber] = 2 THEN ReactionTypeId ELSE 0 END) AS [Top2ReactionId],
				   SUM(CASE WHEN [RowNumber] = 3 THEN ReactionTypeId ELSE 0 END) AS [Top3ReactionId],
				   -- this sum will give the reaction count of top 1 reaction for each entry. sum is OK
				   SUM(CASE WHEN [RowNumber] = 1 THEN [ReactionCount] ELSE 0 END) AS [Top1ReactionCount],
				   SUM(CASE WHEN [RowNumber] = 2 THEN [ReactionCount] ELSE 0 END) AS [Top2ReactionCount],
				   SUM(CASE WHEN [RowNumber] = 3 THEN [ReactionCount] ELSE 0 END) AS [Top3ReactionCount],
				   -- this sum will give the total reaction count for each entry.
				   SUM([ReactionCount]) AS [TotalReactionCount],
				   -- this sum will logical OR (|) each reaction that is pushed by me. SUM is OK since values are flag
				   SUM(CASE WHEN [MeReacted] = 1 THEN [ReactionTypeId] ELSE 0 END)AS [MyReactions]
				FROM [InnerTable]
				GROUP BY [EntryId];
	
	-- ////////////////////////////////////////////////////////////////////////////////////////////////////--
	-- now join the results
	SELECT [E].[Id]
		  ,[E].[Status]
		  ,[E].[TopicId]
		  ,[E].[Content]
		  ,[E].[FormatVersion]
		  ,[E].[CreatedBy]
		  ,[E].[CreateTimeUTC]
		  ,[E].[LastUpdateTimeUTC]
		  ,[V].[MyVote]
		  ,[V].[UpVoteCount]
		  ,[V].[DownVoteCount]
		  ,[V].[UpVoteSum]
		  ,[V].[DownVoteSum]
		  ,[V].[NetVoteSum]
		  ,[R].[Top1ReactionId]
		  ,[R].[Top2ReactionId]
		  ,[R].[Top3ReactionId]
		  ,[R].[Top1ReactionCount]
		  ,[R].[Top2ReactionCount]
		  ,[R].[Top3ReactionCount]
		  ,[R].[TotalReactionCount]
		  ,[R].[MyReactions]
	FROM @selectedEntries [E]	
	LEFT JOIN @votings [V] ON [E].[Id] = [V].[EntryId]
	LEFT JOIN @reactionResults [R] ON [E].[Id] = [R].[EntryId];
GO
