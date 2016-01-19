
-- DROP TABLE [dbo].[WorkerAgentProject]
CREATE TABLE [dbo].[WorkerAgentProject](
	[Id] tinyint NOT NULL PRIMARY KEY,
	[IsActive] bit NOT NULL DEFAULT 1,
	[Project] varchar(100) NOT NULL UNIQUE,
	[WorkerClassFullName] varchar(255) NOT NULL,
)
GO

-- DROP TABLE [dbo].[WorkerAgentSetup]
CREATE TABLE [dbo].[WorkerAgentSetup] (
	[Id] int NOT NULL PRIMARY KEY, 
	[ProjectId] tinyint NOT NULL, 
	[Environment] tinyint NOT NULL DEFAULT 0, 
	[ConfigName] varchar(100), 
	[ConfigValue] nvarchar(max),

	CONSTRAINT [WorkerAgentSetup_FK_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[WorkerAgentProject]([Id]),
)
GO
