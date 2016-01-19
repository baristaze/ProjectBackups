-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSetup')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSetup]');
END
GO

CREATE PROCEDURE [dbo].[GetSetup]
	 @ProjectId tinyint
	,@EnvironmentId tinyint
AS
	 SELECT [ConfigName],
			[ConfigValue]
		FROM [dbo].[WorkerAgentSetup] 
		WHERE [ProjectId] = @ProjectId AND [Environment] = @EnvironmentId;

GO