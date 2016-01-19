/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [Entry]
CREATE TABLE [Entry]
(
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[Status] tinyint NOT NULL DEFAULT 0, /*New, Under Review, Suspended, Disabled, Deleted*/
	[TopicId] bigint NOT NULL,
	[Content] nvarchar(max) NOT NULL,
	[FormatVersion] int NOT NULL DEFAULT 0,
	[CreatedBy] bigint NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [Entry_ForeignKey_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[User]([Id]),
	
	-- we don't cascade on update (bcs: auto-integer) or delete because topic won't be deleted if there is any entry;
	CONSTRAINT [Entry_ForeignKey_TopicId] FOREIGN KEY([TopicId]) REFERENCES [dbo].[Topic]([Id]),
)
GO

-- foreign keys are not indexed by default in SQL Server. we need to create them explicitly.
-- foreign keys are good candidates for an index because constraints are checked in any update/delete action
-- we need to fetch the children of a parent in most of the time. therefore this needs to be fast.
-- We don't need to include Id; i.e. [Id, TopicId] is not needed since we won't use both in a where clause
-- UPDATE: We don't need it for sure since any unclustered index has the primary key or the clustered index
-- as row locator. Therefore, the below non-clustered index on [TopicId] will have the [Id] already.
-- though we better include [LastUpdateTimeUTC] for sort performance. Briefly
-- create index on [TopicId] for JOIN performance and include [LastUpdateTimeUTC] for SORT performance
-- also include [Status] since it will be part of the get-recent-topics-with-top-entries query
CREATE INDEX [EntryIndex1] ON [dbo].[Entry]([TopicId]) INCLUDE ([LastUpdateTimeUTC], [Status]);
GO


/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[ImageMetaFile]
-- an image can belong to a entry or topic or user, etc...
CREATE TABLE [dbo].[ImageMetaFile] (
	-- Cloud Id which is being generated during the upload to Azure
	[Id] bigint NOT NULL PRIMARY KEY IDENTITY,
	[CloudId] uniqueidentifier NOT NULL UNIQUE NONCLUSTERED,
	[CreatedBy] bigint NOT NULL,
	
	-- Id of topic or entry or user, etc. this can be NULL though, 
	-- because we don't know the entry id yet; i.e. it is still being 
	-- typed by the user when image gets uploaded. this will be set later
	[AssetId] bigint NULL, 	
	-- Unknown(not associated yet), Entry, Topic, Image, etc
	[AssetType] tinyint NOT NULL DEFAULT 0, 
	
	[ContentType] varchar(255) NOT NULL,
	[ContentLength] int NOT NULL,
	[OriginalUrl] nvarchar(500) NULL,
	[CloudUrl] nvarchar(500) NULL,
	
	-- we might give up to calculate width and height
	-- therefore these are NULL-able
	[Width] int NULL,
	[Height] int NULL,
	
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [ImageMetaFile_ForeignKey_CreatedBy] FOREIGN KEY([CreatedBy]) REFERENCES [dbo].[User]([Id]),
)
GO

-- we need this because we are going to JOIN the image meta files with the entry IDs although its PK is something else.
-- this is not unique... one asset can have multiple images...
CREATE INDEX [ImageMetaFileIndex1] ON [dbo].[ImageMetaFile]([AssetId], [AssetType]);
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [Vote]
CREATE TABLE [Vote]
(
	[EntryId] bigint NOT NULL,
	[UserId] bigint NOT NULL,
	[VoteValue] smallint NOT NULL DEFAULT 0,
	[VoteTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- having entryID first in the primary key is important
	CONSTRAINT [Vote_PK] PRIMARY KEY ([EntryId], [UserId]),
	
	-- we don't cascade on update because primary key of entry is auto-integer; i.e. not updatable
	CONSTRAINT [Vote_ForeignKey_EntryId] FOREIGN KEY([EntryId]) REFERENCES [dbo].[Entry]([Id]) ON DELETE CASCADE,
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [Vote_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [ReactionType]
CREATE TABLE [ReactionType]
(
	-- this is not identity, it is a FLAG, but reactions are saved separately
	[Id] bigint NOT NULL PRIMARY KEY CHECK(([Id] & ([Id]-1)) = 0), 
	[IsEnabled] bit NOT NULL DEFAULT 1,
	[Text] nvarchar(20) NOT NULL,
	[Order] tinyint NOT NULL DEFAULT 0,
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [Reaction]
CREATE TABLE [Reaction]
(
	[EntryId] bigint NOT NULL,
	[UserId] bigint NOT NULL,
	[ReactionTypeId] bigint NOT NULL,
	[ReactionTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- having entryID first in the primary key is important
	CONSTRAINT [Reaction_PK] PRIMARY KEY ([EntryId], [ReactionTypeId], [UserId]),
	
	-- we don't cascade on update because primary key of entry is auto-integer; i.e. not updatable
	CONSTRAINT [Reaction_ForeignKey_EntryId] FOREIGN KEY([EntryId]) REFERENCES [dbo].[Entry]([Id]) ON DELETE CASCADE,
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [Reaction_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
	
	-- we don't cascade on update or delete because reaction type won't be deleted; and it is an auto-integer
	CONSTRAINT [Reaction_ForeignKey_ReactionTypeId] FOREIGN KEY([ReactionTypeId]) REFERENCES [dbo].[ReactionType]([Id]),
)
GO

/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [SocialShare]
CREATE TABLE [SocialShare]
(
	-- we have a separate PK here since a user can share an entry multiple times.
	[Id] bigint NOT NULL IDENTITY PRIMARY KEY,
	
	[TopicId] bigint NOT NULL,
	[EntryId] bigint NULL, -- this can be null if the share was about a created topic
	[UserId] bigint NULL, -- this can be null if the user was anonymous
	[SocialChannel] tinyint NOT NULL DEFAULT 0,
	[ShareCount] int NOT NULL DEFAULT 0,
	[ShareTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- we don't cascade on update because primary key of topic is auto-integer; i.e. not updatable
	CONSTRAINT [SocialShare_ForeignKey_TopicId] FOREIGN KEY([TopicId]) REFERENCES [dbo].[Topic]([Id]) ON DELETE CASCADE,
		
	-- we don't cascade on update because primary key of entry is auto-integer; i.e. not updatable
	CONSTRAINT [SocialShare_ForeignKey_EntryId] FOREIGN KEY([EntryId]) REFERENCES [dbo].[Entry]([Id]) ON DELETE CASCADE,
	
	-- we don't cascade on update or delete because user won't be deleted; and it is an auto-integer
	CONSTRAINT [SocialShare_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

-- create an index on social share to make sure that join operations are fast
CREATE INDEX [SocialShareIndex1] ON [dbo].[SocialShare]([TopicId], [EntryId], [UserId]);
GO