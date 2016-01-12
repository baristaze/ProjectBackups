

CREATE TABLE [dbo].[Question] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[Type] tinyint NOT NULL, -- Undefined, FiveStarRate, Yes-No, MultipleChoice, FreeText
	[Wording] nvarchar(300) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),

	-- CONSTRAINT [Question_PK] PRIMARY KEY CLUSTERED ([Id]) -- see alter table section	
	-- CONSTRAINT [Question_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

ALTER TABLE [dbo].[Question] ADD CONSTRAINT [Question_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[Question] ADD CONSTRAINT [Question_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

CREATE INDEX [QuestionIndex1] ON [dbo].[Question]([Id]) INCLUDE ([GroupId], [Type]);
GO

CREATE TABLE [dbo].[ChainAndQuestionMap](
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[ChainId] uniqueidentifier NOT NULL,
	[QuestionId] uniqueidentifier NOT NULL,
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Removed (don't delete physically)
	
	-- CONSTRAINT [QuestionAndVenueMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	-- CONSTRAINT [QuestionAndVenueMap_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]),
	-- CONSTRAINT [QuestionAndVenueMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE,
)
GO

ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
GO

ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE;
GO

-- CREATE INDEX [ChainAndQuestionMapIndex1] ON [dbo].[ChainAndQuestionMap]([ChainId]) INCLUDE ([GroupId], [QuestionId], [Status]);
CREATE CLUSTERED INDEX [ChainAndQuestionMapIndex1] ON [dbo].[ChainAndQuestionMap]([GroupId], [ChainId],[QuestionId], [Status]);
GO

CREATE TABLE [dbo].[Choice] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[Wording] nvarchar(100) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [Choice_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

ALTER TABLE [dbo].[Choice] ADD CONSTRAINT [Choice_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[Choice] ADD CONSTRAINT [Choice_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

CREATE INDEX [ChoiceIndex1] ON [dbo].[Choice]([Id]) INCLUDE ([GroupId]);
GO

CREATE TABLE [dbo].[QuestionAndChoiceMap](
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[QuestionId] uniqueidentifier NOT NULL,
	[ChoiceId] uniqueidentifier NOT NULL,
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Status] tinyint NOT NULL DEFAULT 0, -- Active, Removed (don't delete physically)
	
	-- CONSTRAINT [QuestionAndChoiceMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	-- CONSTRAINT [QuestionAndChoiceMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE,
	-- CONSTRAINT [QuestionAndChoiceMap_ForeignKey_ChoiceId] FOREIGN KEY([ChoiceId]) REFERENCES [dbo].[Choice]([Id]) ON DELETE CASCADE,
)
GO

ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_ChoiceId] FOREIGN KEY([ChoiceId]) REFERENCES [dbo].[Choice]([Id]) ON DELETE CASCADE;
GO

-- CREATE INDEX [QuestionAndChoiceMapIndex1] ON [dbo].[QuestionAndChoiceMap]([QuestionId]) INCLUDE ([GroupId], [ChoiceId], [Status]);
CREATE CLUSTERED INDEX [QuestionAndChoiceMapIndex1] ON [dbo].[QuestionAndChoiceMap]([GroupId], [QuestionId], [ChoiceId], [Status]);
GO

CREATE TABLE [dbo].[Answer] (
	[CheckInId] uniqueidentifier NOT NULL,
	[QuestionId] uniqueidentifier NOT NULL,
	[AnswerYesNo] bit NULL,
	[AnswerRate] tinyint NULL,
	[AnswerChoiceId] uniqueidentifier NULL,
	[AnswerFreeText] nvarchar(500) NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [Answer_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]),
	-- CONSTRAINT [Answer_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]),
	-- CONSTRAINT [Answer_ForeignKey_AnswerChoiceId] FOREIGN KEY([AnswerChoiceId]) REFERENCES [dbo].[Choice]([Id]),
)
GO

ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
GO

ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]);
GO

ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_AnswerChoiceId] FOREIGN KEY([AnswerChoiceId]) REFERENCES [dbo].[Choice]([Id]);
GO

-- CREATE INDEX [AnswerIndex1] ON [dbo].[Answer]([CheckInId]) INCLUDE ([QuestionId]);
CREATE CLUSTERED INDEX [AnswerIndex1] ON [dbo].[Answer]([QuestionId], [CheckInId]);
GO

CREATE TABLE [dbo].[MarkedAsRead] (
	[CheckInId] uniqueidentifier NOT NULL,
	[UserId] uniqueidentifier NOT NULL,
	[ReadTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),	
	
	-- CONSTRAINT [MarkedAsRead_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]),	
	-- CONSTRAINT [MarkedAsRead_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),	
)
GO

-- CREATE INDEX [MarkedAsReadIndex1] ON [dbo].[MarkedAsRead]([UserId]) INCLUDE ([CheckInId]);
CREATE CLUSTERED INDEX [MarkedAsReadIndex1] ON [dbo].[MarkedAsRead]([UserId], [CheckInId]);
GO

ALTER TABLE [dbo].[MarkedAsRead] ADD CONSTRAINT [MarkedAsRead_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
GO

ALTER TABLE [dbo].[MarkedAsRead] ADD CONSTRAINT [MarkedAsRead_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]);
GO
