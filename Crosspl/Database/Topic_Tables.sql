/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [Category]
CREATE TABLE [Category]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[Name] nvarchar(50) NOT NULL UNIQUE,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [Topic]
CREATE TABLE [Topic]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[Status] tinyint NOT NULL DEFAULT 0, /* 0=New, 1=Under Review, 2=Suspended, 3=Disabled, 4=Deleted */
	[Title] nvarchar(400) NOT NULL,	/*do not make this longer than 400. see SEO friendly URL keys*/
	[CreatedBy] bigint NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
		
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [Topic_ForeignKey_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[User]([Id]),
)
GO

-- create index on [LastUpdateTimeUTC] for SORT performance. 
-- include [Status] since it will be part of the get-recent-topics query
CREATE INDEX [TopicIndex1] ON [dbo].[Topic]([LastUpdateTimeUTC]) INCLUDE ([Status]);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [SeoLinkTopic]
CREATE TABLE [SeoLinkTopic]
(
	[Link] varchar(500) NOT NULL UNIQUE, /*this doesn't have to be nvarchar since URL will be ASCII. +100 is fine. we might need it*/
	[TopicId] bigint NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [SeoLinkTopic_PK] PRIMARY KEY([Link], [TopicId]),
	
	CONSTRAINT [SeoLinkTopic_ForeignKey_TopicId] FOREIGN KEY([TopicId]) 
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [TopicCategory]
CREATE TABLE [TopicCategory]
(
	[CategoryId] bigint NOT NULL,
	[TopicId] bigint NOT NULL,
	[Order] int NOT NULL DEFAULT 0,
	
	CONSTRAINT [TopicCategory_PK] PRIMARY KEY ([CategoryId], [TopicId]),
	
	CONSTRAINT [TopicCategory_ForeignKey_CategoryId] FOREIGN KEY([CategoryId]) 
		REFERENCES [dbo].[Category]([Id]) ON DELETE CASCADE,
		
	CONSTRAINT [TopicCategory_ForeignKey_TopicId] FOREIGN KEY([TopicId]) 
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [RelatedTopic]
CREATE TABLE [RelatedTopic]
(
	[TopicId1] bigint NOT NULL,
	[TopicId2] bigint NOT NULL,
	[RelevanceRank] int NOT NULL DEFAULT 0,
	
	-- most of the queries will be 'get' where topicId = x. 
	-- therefore it seems like having a tiny clustered index on Topic1 only is enough.
	-- it is a good saving but 'delete' queries will suffer since we will be checking both IDs
	-- therefore it is OK to have an (int+int) as a clustered index
	CONSTRAINT [RelatedTopic_PK] PRIMARY KEY ([TopicId1], [TopicId2]),
	
	-- the relation ship is one way because relevance might differ
	CONSTRAINT [RelatedTopic_ForeignKey_TopicId1] FOREIGN KEY([TopicId1]) 
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
		
	-- this is not allowed by the SQL because of a potential cyclic dependency
	/*
	CONSTRAINT [RelatedTopic_ForeignKey_TopicId2] FOREIGN KEY([TopicId2])
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
	*/	
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [TopicInvitesOnFacebook]
CREATE TABLE [TopicInvitesOnFacebook]
(
	[AppRequestId] bigint NOT NULL PRIMARY KEY,
	[TopicId] bigint NOT NULL,
	[SentBy] bigint NOT NULL,
	[InviteeCount] smallint NOT NULL,
	[SendTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[EntryId] bigint NULL,
	
	CONSTRAINT [TopicInvitesOnFacebook_ForeignKey_TopicId] FOREIGN KEY([TopicId]) 
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
	
	CONSTRAINT [TopicInvitesOnFacebook_ForeignKey_EntryId] FOREIGN KEY([EntryId]) 
		REFERENCES [dbo].[Entry]([Id]) ON DELETE CASCADE,
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer	
	CONSTRAINT [TopicInvitesOnFacebook_ForeignKey_SentBy] FOREIGN KEY([SentBy]) REFERENCES [dbo].[User]([Id]),
)
GO

/*
--ALTER TABLE [TopicInvitesOnFacebook]
--	ADD [EntryId] bigint NOT NULL DEFAULT 0;

ALTER TABLE [TopicInvitesOnFacebook]
	DROP DF__TopicInvi__Entry__19AACF41
	
ALTER TABLE [TopicInvitesOnFacebook]
	DROP COLUMN [EntryId]
	
ALTER TABLE [TopicInvitesOnFacebook]
	ADD [EntryId] bigint NULL;

ALTER TABLE [TopicInvitesOnFacebook]
	ADD CONSTRAINT [TopicInvitesOnFacebook_ForeignKey_EntryId] FOREIGN KEY([EntryId])
		REFERENCES [dbo].[Entry]([Id]) ON DELETE CASCADE;

*/

GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [PromotedTopic]
CREATE TABLE [PromotedTopic]
(
	[TopicId] bigint NOT NULL,
	[IsActive] bit NOT NULL DEFAULT 0,
	[ViewOrder] int NOT NULL DEFAULT 0,
	[StartTimeUTC] datetime NULL,
	[EndTimeUTC] datetime NULL,
	[PromotedBy] nvarchar(300) NULL,
	[Price] smallmoney NULL,
	
	CONSTRAINT [PromotedTopic_PK] PRIMARY KEY([TopicId]),
	
	CONSTRAINT [PromotedTopic_ForeignKey_TopicId] FOREIGN KEY([TopicId]) 
		REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
)
GO