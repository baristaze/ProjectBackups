CREATE TABLE [dbo].[DinerSettings] (
	[DinerId] uniqueidentifier NOT NULL, 
	[Name] varchar(50) NOT NULL,
	[Value] nvarchar(500) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [DinerSettings_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]),
	-- CONSTRAINT [DinerSettings_Uniqueness] UNIQUE ([DinerId], [Name]),
)
GO

ALTER TABLE [dbo].[DinerSettings] ADD CONSTRAINT [DinerSettings_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]);
GO

ALTER TABLE [dbo].[DinerSettings] ADD CONSTRAINT [DinerSettings_Uniqueness] UNIQUE ([DinerId], [Name]);
GO

-- CREATE INDEX [DinerSettingsIndex1] ON [dbo].[DinerSettings]([DinerId]) INCLUDE ([Name]);
CREATE CLUSTERED INDEX [DinerSettingsIndex1] ON [dbo].[DinerSettings]([DinerId], [Name]);
GO