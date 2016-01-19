--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'SaveProcessedAzureLog')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[SaveProcessedAzureLog]');
END
GO

CREATE PROCEDURE [dbo].[SaveProcessedAzureLog]
	 @LogId uniqueidentifier
	,@TimeUTC datetime
	,@ClientId uniqueidentifier
	,@UserId uniqueidentifier
	,@SplitId int
	,@Level varchar(max)
	,@Platform varchar(max)
	,@Version varchar(max)
	,@File varchar(max)
	,@Class varchar(max)
	,@Method varchar(max)
	,@Line int
	,@Message nvarchar(max)
	,@Exception nvarchar(max)
	,@ElapsedTimeMSec int
	,@Funnel varchar(max)
	,@Page varchar(max)
	,@Action varchar(max)
	,@Step varchar(max)
	,@Full nvarchar(max)
AS

	IF EXISTS (SELECT * FROM [dbo].[ProcessedLogs] WHERE [LogId] = @LogId)
	BEGIN
		DELETE FROM [dbo].[ProcessedLogs] WHERE [LogId] = @LogId;
	END

	INSERT INTO [dbo].[ProcessedLogs]
			   ([LogId]
			   ,[TimeUTC]
			   ,[ClientId]
			   ,[UserId]
			   ,[SplitId]
			   ,[Level]
			   ,[Platform]
			   ,[Version]
			   ,[File]
			   ,[Class]
			   ,[Method]
			   ,[Line]
			   ,[Message]
			   ,[Exception]
			   ,[ElapsedTimeMSec]
			   ,[Funnel]
			   ,[Page]
			   ,[Action]
			   ,[Step]
			   ,[Full])
		 VALUES
			   (@LogId
			   ,@TimeUTC
			   ,@ClientId
			   ,@UserId
			   ,@SplitId
			   ,@Level
			   ,@Platform
			   ,@Version
			   ,@File
			   ,@Class
			   ,@Method
			   ,@Line
			   ,@Message
			   ,@Exception
			   ,@ElapsedTimeMSec
			   ,@Funnel
			   ,@Page
			   ,@Action
			   ,@Step
			   ,@Full);
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'SaveUnprocessedAzureLog')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[SaveUnprocessedAzureLog]');
END
GO

CREATE PROCEDURE [dbo].[SaveUnprocessedAzureLog]
	 @RowKey varchar(max)
	,@TimeUTC datetime
	,@Message nvarchar(max)
	
AS
	IF EXISTS (SELECT * FROM [dbo].[UnProcessedLogs] WHERE [RowKey] = @RowKey)
	BEGIN
		DELETE FROM [dbo].[UnProcessedLogs] WHERE [RowKey] = @RowKey;
	END

	INSERT INTO [dbo].[UnProcessedLogs]
			   ([RowKey]
			   ,[TimeUTC]
			   ,[Message])
		 VALUES
			   (@RowKey
			   ,@TimeUTC
			   ,@Message);
GO
