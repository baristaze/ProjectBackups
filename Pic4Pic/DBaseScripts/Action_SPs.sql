-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'InsertAction')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[InsertAction]');
END
GO

CREATE PROCEDURE [dbo].[InsertAction]
	 @Id uniqueidentifier
	,@UserId1 uniqueidentifier
	,@UserId2 uniqueidentifier
	,@ActionType tinyint
AS
	-- insert
	INSERT INTO [dbo].[Action]
			   ([Id]
			   ,[UserId1]
			   ,[UserId2]
			   ,[ActionType])
		 VALUES
			   (@Id
			   ,@UserId1
			   ,@UserId2
			   ,@ActionType);
GO

/*
-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetAction')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetAction]');
END
GO

CREATE PROCEDURE [dbo].[GetAction]
	 @UserId1 uniqueidentifier
	,@UserId2 uniqueidentifier
AS
	SELECT [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[ActionType]
		  ,[ActionTimeUTC]
		  ,[Status]
		  ,[NotifScheduleTimeUTC]
		  ,[NotifSentTimeUTC]
		  ,[NotifViewTimeUTC]
	  FROM [dbo].[Action]
	  WHERE ([UserId1] = @UserId1 AND [UserId2] = @UserId2) OR ([UserId1] = @UserId2 AND [UserId2] = @UserId1);

GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetActionById')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetActionById]');
END
GO

CREATE PROCEDURE [dbo].[GetActionById]
	 @Id uniqueidentifier
AS
	SELECT [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[ActionType]
		  ,[ActionTimeUTC]
		  ,[Status]
		  ,[NotifScheduleTimeUTC]
		  ,[NotifSentTimeUTC]
		  ,[NotifViewTimeUTC]
	  FROM [dbo].[Action]
	  WHERE [Id] = @Id;

GO
*/

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRecentActions')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRecentActions]');
END
GO

CREATE PROCEDURE [dbo].[GetRecentActions]
	 @UserId uniqueidentifier
	,@CutOffMinutes int
	,@ActionTypeFilter tinyint
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());

	SELECT TOP (@MaxCount) 
		   [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[ActionType]
		  ,[ActionTimeUTC]
		  ,[Status]
		  ,[NotifScheduleTimeUTC]
		  ,[NotifSentTimeUTC]
		  ,[NotifViewTimeUTC]
		FROM [dbo].[Action]
		WHERE [UserId2] = @UserId 
		  AND [ActionTimeUTC] >= @cutOffTime
		  AND ([ActionType] = @ActionTypeFilter OR @ActionTypeFilter = 0 OR @ActionTypeFilter is NULL)
		ORDER BY [ActionTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRecentActionsByMe')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRecentActionsByMe]');
END
GO

CREATE PROCEDURE [dbo].[GetRecentActionsByMe]
	 @UserId uniqueidentifier
	,@ConcatenatedUserIDs varchar(max)
	,@CutOffMinutes int
	,@ActionTypeFilter tinyint
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());

	SELECT TOP (@MaxCount) 
		   [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[ActionType]
		  ,[ActionTimeUTC]
		  ,[Status]
		  ,[NotifScheduleTimeUTC]
		  ,[NotifSentTimeUTC]
		  ,[NotifViewTimeUTC]
		FROM [dbo].[Action]
		WHERE [UserId1] = @UserId 
		  AND [ActionTimeUTC] >= @cutOffTime
		  AND ([ActionType] = @ActionTypeFilter OR @ActionTypeFilter = 0 OR @ActionTypeFilter is NULL)
		  AND (@ConcatenatedUserIDs is NULL OR @ConcatenatedUserIDs = '' OR [UserId2] IN (SELECT * FROM TokenizeGUIDs(@ConcatenatedUserIDs)))
		ORDER BY [ActionTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetActionsToBeNotified')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetActionsToBeNotified]');
END
GO

CREATE PROCEDURE [dbo].[GetActionsToBeNotified]
	 @CutOffMinutes int
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());

	SELECT TOP (@MaxCount) 
		   [Id]
		  ,[UserId1]
		  ,[UserId2]
		  ,[ActionType]
		  ,[ActionTimeUTC]
		  ,[Status]
		  ,[NotifScheduleTimeUTC]
		  ,[NotifSentTimeUTC]
		  ,[NotifViewTimeUTC]
		FROM [dbo].[Action]
		WHERE [Status] = 0 
		  AND [ActionTimeUTC] >= @cutOffTime
		ORDER BY [ActionTimeUTC] ASC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'ScheduleNotifications')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[ScheduleNotifications]');
END
GO

CREATE PROCEDURE [dbo].[ScheduleNotifications]
	 @ConcatenatedActionIDs varchar(max)
	,@ScheduleTimeUTC datetime
AS
	UPDATE [dbo].[Action]
	   SET [Status] = 2
		  ,[NotifScheduleTimeUTC] = @ScheduleTimeUTC
	 WHERE [Status] = 0 
	   AND [Id] IN (SELECT * FROM TokenizeGUIDs(@ConcatenatedActionIDs));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkNotificationsAsOmitted')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkNotificationsAsOmitted]');
END
GO

CREATE PROCEDURE [dbo].[MarkNotificationsAsOmitted]
	 @ConcatenatedActionIDs varchar(max)
AS
	UPDATE [dbo].[Action]
	   SET [Status] = 1
	 WHERE [Status] = 0
	   AND [Id] IN (SELECT * FROM TokenizeGUIDs(@ConcatenatedActionIDs));
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkNotificationsAsSent')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkNotificationsAsSent]');
END
GO

CREATE PROCEDURE [dbo].[MarkNotificationsAsSent]
	 @ConcatenatedActionIDs varchar(max)
AS
	UPDATE [dbo].[Action]
	   SET [Status] = 3
		  ,[NotifSentTimeUTC] = GETUTCDATE()
	 WHERE [Status] <> 4 
	   AND [Id] IN (SELECT * FROM TokenizeGUIDs(@ConcatenatedActionIDs));
			
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MarkNotificationAsViewed')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MarkNotificationAsViewed]');
END
GO

CREATE PROCEDURE [dbo].[MarkNotificationAsViewed]
	 @ActionId uniqueidentifier
	,@UserId2 uniqueidentifier	-- receiver
AS
	UPDATE [dbo].[Action]			
		   SET [Status] = 4
			  ,[NotifViewTimeUTC] = GETUTCDATE()
		 WHERE [Id] = @ActionId AND [UserId2] = @UserId2;
GO