/************************************************************************************************************/
/************************************************************************************************************/
-- DROP TABLE [dbo].[ImageMetaFile]
CREATE TABLE [dbo].[ImageMetaFile] (
	
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[GroupingId] uniqueidentifier NOT NULL,
	[UserId] uniqueidentifier NULL, /* pictures might be uploaded anonymously during signup */
	[Status] tinyint NOT NULL DEFAULT 0, /* 0=New, 1=Disabled, 2=Deleted */
	[ContentType] varchar(255) NOT NULL,
	[ContentLength] int NOT NULL,
	[Width] int NULL,
	[Height] int NULL,
	[CloudUrl] nvarchar(500) NOT NULL,
	[IsBlurred] bit NOT NULL DEFAULT 0,
	[IsThumbnail] bit NOT NULL DEFAULT 0,
	[IsProfilePicture] bit NOT NULL DEFAULT 0,	
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- we don't cascade on update or delete because user won't be deleted; and user id won't be updaded
	CONSTRAINT [ImageMetaFile_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

-- we need this because we are going to JOIN the image meta files with the User IDs
-- this is not unique... one user can have multiple images...
CREATE INDEX [ImageMetaFileIndex1] ON [dbo].[ImageMetaFile]([UserId]);
GO