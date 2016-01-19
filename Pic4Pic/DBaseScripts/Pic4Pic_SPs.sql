-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPic4PicRequest')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPic4PicRequest]');
END
GO

CREATE PROCEDURE [dbo].[GetPic4PicRequest]
     @maxCount int
	,@UserId1 uniqueidentifier
	,@UserId2 uniqueidentifier
AS
	SELECT TOP (@maxCount)
	       [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[PicId1]
		  ,[PicId2]
		  ,[RequestTimeUTC]
		  ,[AcceptTimeUTC]
	  FROM [dbo].[Pic4Pic]
	  WHERE ([UserId1] = @UserId1 AND [UserId2] = @UserId2) OR ([UserId1] = @UserId2 AND [UserId2] = @UserId1)
	  ORDER BY [RequestTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPic4PicRequests')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPic4PicRequests]');
END
GO

CREATE PROCEDURE [dbo].[GetPic4PicRequests]
     @maxCount int
	,@UserId uniqueidentifier
	,@ConcatenatedUserIDs varchar(max)
AS
	declare @userIDs TABLE ([Id] uniqueidentifier);
	INSERT INTO @userIDs SELECT * FROM TokenizeGuids(@ConcatenatedUserIDs);

	SELECT TOP (@maxCount)
	       [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[PicId1]
		  ,[PicId2]
		  ,[RequestTimeUTC]
		  ,[AcceptTimeUTC]
	  FROM [dbo].[Pic4Pic]
	  WHERE ([UserId1] = @UserId AND [UserId2] IN (SELECT * FROM @userIDs))
	     OR ([UserId2] = @UserId AND [UserId1] IN (SELECT * FROM @userIDs))
	  ORDER BY [RequestTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPendingReceivedPic4PicRequests')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPendingReceivedPic4PicRequests]');
END
GO

CREATE PROCEDURE [dbo].[GetPendingReceivedPic4PicRequests]
     @maxCount int
	,@UserId uniqueidentifier
AS
	SELECT TOP (@maxCount)
	       [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[PicId1]
		  ,[PicId2]
		  ,[RequestTimeUTC]
		  ,[AcceptTimeUTC]
	  FROM [dbo].[Pic4Pic]
	  WHERE [UserId2] = @UserId AND [AcceptTimeUTC] is NULL
	  ORDER BY [RequestTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPic4PicRequestById')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPic4PicRequestById]');
END
GO

CREATE PROCEDURE [dbo].[GetPic4PicRequestById]
	 @Id uniqueidentifier
AS
	SELECT [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[PicId1]
		  ,[PicId2]
		  ,[RequestTimeUTC]
		  ,[AcceptTimeUTC]
	  FROM [dbo].[Pic4Pic]
	  WHERE [Id] = @Id;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'RequestPic4Pic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[RequestPic4Pic]');
END
GO

CREATE PROCEDURE [dbo].[RequestPic4Pic]
	 @Id uniqueidentifier
	,@UserId1 uniqueidentifier	-- requester
	,@UserId2 uniqueidentifier	-- receiver
	,@PicId1 uniqueidentifier
	,@PicId2 uniqueidentifier
AS
	-- prepare variables
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();
	
	-- check @Id
	IF (@Id is NULL)
	BEGIN
		set @Id = NEWID();	
	END	

	-- insert into P4P
	INSERT INTO [dbo].[Pic4Pic]
			   ([Id]
			   ,[UserId1]
			   ,[UserId2]
			   ,[PicId1]
			   ,[PicId2]
			   ,[RequestTimeUTC])
		 VALUES
			   (@Id
			   ,@UserId1
			   ,@UserId2
			   ,@PicId1
			   ,@PicId2
			   ,@utcNow);
	
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
				,6 -- RequestingP4P -- see Acion table
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
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'AcceptPic4Pic')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[AcceptPic4Pic]');
END
GO

CREATE PROCEDURE [dbo].[AcceptPic4Pic]
	 @Id uniqueidentifier
	,@UserId2 uniqueidentifier -- accepting user
	,@PicId2 uniqueidentifier  -- photo of accepting user
AS
	-- prepare variables
	declare @utcNow datetime;
	set @utcNow = GETUTCDATE();
	
	-- get user id 1
	declare @UserId1 uniqueidentifier;
	set @UserId1 = (SELECT [UserId1] FROM [dbo].[Pic4Pic] WHERE [Id] = @Id AND [UserId2] = @UserId2);
	IF (@UserId1 is NULL)
	BEGIN
		RAISERROR(N'Pic4Pic Request does not exist or you do not have access', 16, 1);
		RETURN;
	END

	-- update P4P
	UPDATE [dbo].[Pic4Pic]
	   SET [PicId2] = COALESCE(@PicId2, [PicId2])
		  ,[AcceptTimeUTC] = @utcNow
	 WHERE [Id] = @Id AND [UserId2] = @UserId2;
	
	-- insert into action table
	IF (@@ERROR = 0)
	BEGIN
		INSERT INTO [dbo].[Action]
				([UserId1]
				,[UserId2]
				,[ActionType]
				,[ActionTimeUTC])
			VALUES
				(@UserId2	-- reverse
				,@UserId1	-- reverse
				,7 -- AcceptedP4P -- see Acion table
				,@utcNow);
	END
GO
