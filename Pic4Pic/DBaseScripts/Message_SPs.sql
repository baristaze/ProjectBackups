-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'SaveTextMessage')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[SaveTextMessage]');
END
GO

CREATE PROCEDURE [dbo].[SaveTextMessage]
	 @Id uniqueidentifier
	,@UserId1 uniqueidentifier	-- sender
	,@UserId2 uniqueidentifier	-- receiver
	,@Content nvarchar(max)
AS
	-- prepare variables
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();
	
	-- check @Id
	IF (@Id is NULL)
	BEGIN
		set @Id = NEWID();	
	END	

	-- save message
	INSERT INTO [dbo].[TextMessage]
			   ([Id]
			   ,[UserId1]
			   ,[UserId2]
			   ,[Content]
			   ,[SentTimeUTC])
		 VALUES
			   (@Id
			   ,@UserId1
			   ,@UserId2
			   ,@Content
			   ,@utcNow);

	-- keep track of action
	IF (@@ERROR = 0)
	BEGIN
		-- insert into action table
		INSERT INTO [dbo].[Action]
				([UserId1]
				,[UserId2]
				,[ActionType]
				,[ActionTimeUTC])
			VALUES
				(@UserId1
				,@UserId2
				,3 -- SentText -- see Acion table
				,@utcNow);
	END
	ELSE
	BEGIN
		set @Id = NULL;
	END

	-- return
	SELECT @Id;
GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkTextMessageAsRead')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkTextMessageAsRead]');
END
GO

CREATE PROCEDURE [dbo].[MarkTextMessageAsRead]
	 @Id uniqueidentifier
	,@UserId2 uniqueidentifier	-- receiver
AS

	UPDATE [dbo].[TextMessage]
	   SET [ReadTimeUTC] = GETUTCDATE()
	 WHERE [Id] = @Id AND [UserId2] = @UserId2 AND [ReadTimeUTC] is NULL;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkAllTextMessagesAsRead')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkAllTextMessagesAsRead]');
END
GO

CREATE PROCEDURE [dbo].[MarkAllTextMessagesAsRead]
	 @UserId1 uniqueidentifier -- sender
	,@UserId2 uniqueidentifier -- receiver
AS

	UPDATE [dbo].[TextMessage]
	   SET [ReadTimeUTC] = GETUTCDATE()
	 WHERE [UserId1] = @UserId1 AND [UserId2] = @UserId2 AND [ReadTimeUTC] is NULL;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetConversation')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetConversation]');
END
GO

CREATE PROCEDURE [dbo].[GetConversation]
	 @UserId1 uniqueidentifier
	,@UserId2 uniqueidentifier
    ,@CutOffMinutes int
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());

	SELECT TOP (@MaxCount) 
			   [Id]
			  ,[UserId1]
			  ,[UserId2]
			  ,[Content]
			  ,[SentTimeUTC]
			  ,[ReadTimeUTC]
		FROM [dbo].[TextMessage]
		WHERE ([UserId1] = @UserId1 AND [UserId2] = @UserId2) OR ([UserId1] = @UserId2 AND [UserId2] = @UserId1)
		  AND [SentTimeUTC] > @cutOffTime
		ORDER BY [SentTimeUTC] DESC;

GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetConversationsSummary')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetConversationsSummary]');
END
GO

CREATE PROCEDURE [dbo].[GetConversationsSummary]
	 @UserId uniqueidentifier	
	,@CutOffMinutes int
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());		
	
	;WITH [MessageSummary] ([UserId], [UnreadMessageCount], [LastUpdateUTC])
	AS
	(	
		  -- sent by me
		  SELECT [UserId2] AS [UserId]
				,0 AS [UnreadMessageCount]
				,MAX([SentTimeUTC]) AS [LastUpdateUTC]
			FROM [dbo].[TextMessage]
			WHERE [UserId1] = @UserId AND [SentTimeUTC] > @cutOffTime
			GROUP BY [UserId2]
		-- union		
		UNION
			-- received by me
			SELECT [UserId1] AS [UserId]
				,SUM(CASE WHEN [ReadTimeUTC] is NULL THEN 1 ELSE 0 END) AS [UnreadMessageCount]
				,MAX([SentTimeUTC]) AS [LastUpdateUTC]
			FROM [dbo].[TextMessage]
			WHERE [UserId2] = @UserId AND [SentTimeUTC] > @cutOffTime
			GROUP BY [UserId1]
	)
	 SELECT TOP (@MaxCount)
			[UserId], 
			SUM([UnreadMessageCount]) AS [UnreadMessageCount], 
			MAX([LastUpdateUTC]) AS [LastUpdateUTC]
		FROM [MessageSummary]
		GROUP BY [UserId]
		ORDER BY [LastUpdateUTC] DESC;
GO