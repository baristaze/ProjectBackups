----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'CreateNewEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[CreateNewEntry]');
END
GO

CREATE PROCEDURE [dbo].[CreateNewEntry] 
 @TopicId bigint 
,@Content nvarchar(max)
,@FormatVersion int
,@CreatedBy bigint
AS
	declare @newId bigint;
	set @newId = -1;

	-- insert item
	INSERT INTO [dbo].[Entry] ([TopicId],[Content],[FormatVersion],[CreatedBy])
		VALUES (@TopicId, @Content, @FormatVersion, @CreatedBy);
	
	-- get the recently generated auto id	
	set @newId = (SELECT SCOPE_IDENTITY());
	
	-- return it
	SELECT @newId;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdateEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdateEntry]');
END
GO

CREATE PROCEDURE [dbo].[UpdateEntry]
 @Id bigint 
,@Content nvarchar(max)
,@FormatVersion int
,@CheckUser bit
,@UserId bigint
AS
	-- caller can rely on affected rows but this might not be necessary
	-- because returning true (yes, query is a success; i.e. updated) will 
	-- confuse only malicious users who want to update entries of other people
	UPDATE [dbo].[Entry] 
		SET [Content] = @Content, 
			[FormatVersion] = @FormatVersion, 
			[LastUpdateTimeUTC] = GETUTCDATE() 
		WHERE [Id] = @Id AND 
		((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO


----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'DeleteEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[DeleteEntry]');
END
GO

CREATE PROCEDURE [dbo].[DeleteEntry]
 @Id bigint
,@CheckUser bit
,@UserId bigint
AS
	-- caller cannot rely on affected rows because Vote and Reactions will be deleted as well.
	-- However, he/she can rely on exception... Returning true if there is not such an entry is fine as well...
	-- We might interpret the result as a false-positive (no-exception but no-deletion either) but will 
	-- confuse only malicious users who want to delete entries of other people
	DELETE FROM [dbo].[Entry] WHERE [Id] = @Id AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'UpdatEntryStatus')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[UpdatEntryStatus]');
END
GO

CREATE PROCEDURE [dbo].[UpdatEntryStatus]
 @Id bigint
,@Status tinyint
,@CheckUser bit
,@UserId bigint
AS
	UPDATE [dbo].[Entry] SET [Status] = @Status, [LastUpdateTimeUTC] = GETUTCDATE() 
		WHERE [Id] = @Id AND ((@CheckUser = 0) OR ([CreatedBy] = @UserId));
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'VoteForEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[VoteForEntry]');
END
GO

CREATE PROCEDURE [dbo].[VoteForEntry]
 @EntryId bigint
,@UserId bigint
,@VoteValue smallint
AS
	-- vote
	IF(@VoteValue = 0)
	BEGIN
		-- revert
		DELETE FROM [dbo].[Vote] WHERE [EntryId] = @EntryId AND [UserId] = @UserId;
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT * FROM [dbo].[Vote] WHERE [EntryId] = @EntryId AND [UserId] = @UserId)
		BEGIN
			-- update
			UPDATE [dbo].[Vote] SET [VoteValue] = @VoteValue WHERE [EntryId] = @EntryId AND [UserId] = @UserId;
		END
		ELSE
		BEGIN
			-- insert
			INSERT INTO [dbo].[Vote]([EntryId], [UserId], [VoteValue]) VALUES (@EntryId, @UserId, @VoteValue);
		END
	END
	
	-- return current vote statistics
	SELECT SUM(CASE WHEN [UserId] = @UserId THEN [VoteValue] ELSE 0 END) AS [MyVote],
		   SUM(CASE WHEN [VoteValue] > 0 THEN 1 ELSE 0 END) AS [UpVoteCount],
		   SUM(CASE WHEN [VoteValue] < 0 THEN 1 ELSE 0 END) AS [DownVoteCount],
		   SUM(CASE WHEN [VoteValue] > 0 THEN [VoteValue] ELSE 0 END) AS [UpVoteSum], 
		   SUM(CASE WHEN [VoteValue] < 0 THEN [VoteValue] ELSE 0 END) AS [DownVoteSum],
		   SUM([VoteValue]) AS [NetVoteSum]
		FROM [Vote]
		WHERE [EntryId] = @EntryId
		GROUP BY [EntryId];
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ReactToEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ReactToEntry]');
END
GO

CREATE PROCEDURE [dbo].[ReactToEntry]
 @EntryId bigint
,@UserId bigint
,@ReactionTypeId bigint
AS
	-- vote
	IF(@ReactionTypeId < 0)
	BEGIN
		-- revert
		set @ReactionTypeId = @ReactionTypeId * -1;
		DELETE FROM [dbo].[Reaction] WHERE [EntryId] = @EntryId AND [UserId] = @UserId AND [ReactionTypeId] = @ReactionTypeId;
	END
	ELSE
	BEGIN
		IF NOT EXISTS (SELECT * FROM [dbo].[Reaction] WHERE [EntryId] = @EntryId AND [UserId] = @UserId AND [ReactionTypeId] = @ReactionTypeId)
		BEGIN
			-- insert
			INSERT INTO [dbo].[Reaction] ([EntryId], [UserId], [ReactionTypeId]) VALUES (@EntryId, @UserId, @ReactionTypeId);
		END
	END
	
	-- return current vote statistics for this entry
	SELECT	[ReactionTypeId], 
			COUNT(*) AS [ReactionCount],
			-- null is important here since we are counting. do not replace it with 0
			COUNT(CASE WHEN [UserId] = @UserId THEN 1 ELSE NULL END) AS [MeReacted]
		FROM [Reaction] 
			WHERE [EntryId] = @EntryId
		GROUP BY [ReactionTypeId]
		ORDER BY [ReactionCount] DESC;
GO

----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetEntry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetEntry]');
END
GO

CREATE PROCEDURE [dbo].[GetEntry]
 @Id bigint
,@CheckStatus bit
,@Status tinyint
AS

	SELECT [Id]
		  ,[Status]
		  ,[TopicId]
		  ,[Content]
		  ,[FormatVersion]
		  ,[CreatedBy]
		  ,[CreateTimeUTC]
		  ,[LastUpdateTimeUTC]
	FROM [Entry]	
	WHERE [Id] = @Id AND ((@CheckStatus = 0) OR ([Status] = @Status));
GO
----------------------------------------------------------------------------
----------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'LogSocialShare')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[LogSocialShare]');
END
GO

CREATE PROCEDURE [dbo].[LogSocialShare]
 @TopicId bigint
,@EntryId bigint
,@UserId bigint
,@SocialChannel tinyint
,@ShareCount smallint
AS
	-- don't do this
	--IF(@UserId = 0)
	--BEGIN
	--	set @UserId = NULL;
	--END
	
	-- log
	INSERT INTO [dbo].[SocialShare]
           ([TopicId]
           ,[EntryId]
           ,[UserId]
           ,[SocialChannel]
           ,[ShareCount])
     VALUES
           (@TopicId
           ,@EntryId
           ,@UserId
           ,@SocialChannel
           ,@ShareCount);

GO
