/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[Recommendation]
CREATE TABLE [dbo].[Recommendation] 
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[UserId1] uniqueidentifier NOT NULL,
	[UserId2] uniqueidentifier NOT NULL,
	[RecommendTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),

	CONSTRAINT [Recommendation_FK_UserId1] FOREIGN KEY ([UserId1]) 
		REFERENCES [dbo].[User]([Id]),

	CONSTRAINT [Recommendation_FK_UserId2] FOREIGN KEY ([UserId2]) 
		REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [RecommendationUsersIndex] ON [dbo].[Recommendation];
CREATE UNIQUE INDEX [RecommendationUsersIndex] ON [dbo].[Recommendation]([UserId1],[UserId2]) INCLUDE ([RecommendTimeUTC]);
GO