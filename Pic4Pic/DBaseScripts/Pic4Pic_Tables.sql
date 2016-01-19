/************************************************************************************************************/
/************************************************************************************************************/
-------------------------------------------------------------------------------------------------------------
-- Records Pic4Pic data => which photos are involved?
-- Do not involve [ActionId] here since multiple action IDs are involved during 
-- lifetime of a single Pic4Pic; e.g. { requested, accepted; [lifted?] }
-------------------------------------------------------------------------------------------------------------
-- DROP TABLE [dbo].[Pic4Pic]
CREATE TABLE [dbo].[Pic4Pic]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[UserId1] uniqueidentifier NOT NULL, 
	[UserId2] uniqueidentifier NOT NULL,
	[PicId1] uniqueidentifier NOT NULL, -- this is [GroupingId] indeed -- use pic when P4P is sent
	[PicId2] uniqueidentifier NULL, -- this is [GroupingId] indeed  -- use pic when P4P is accepted
	[RequestTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[AcceptTimeUTC] datetime NULL,
	
	CONSTRAINT [Pic4Pic_FK_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [Pic4Pic_FK_UserId2] FOREIGN KEY ([UserId2]) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [Pic4Pic_FK_PicId1] FOREIGN KEY ([PicId1]) REFERENCES [dbo].[ImageMetaFile]([Id]), -- even if it is [GroupingId]
	CONSTRAINT [Pic4Pic_FK_PicId2] FOREIGN KEY ([PicId2]) REFERENCES [dbo].[ImageMetaFile]([Id]), -- even if it is [GroupingId]
)
GO

-- DROP INDEX [Pic4PicIndex1] ON [dbo].[Pic4Pic]
CREATE INDEX [Pic4PicIndex1] ON [dbo].[Pic4Pic]([UserId1], [UserId2]) INCLUDE ([AcceptTimeUTC]);
GO