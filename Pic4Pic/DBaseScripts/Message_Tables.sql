/************************************************************************************************************/
/************************************************************************************************************/
-------------------------------------------------------------------------------------------------------------
-- do not involve [ActionId] here since a text conversation (multiple messages)
-------------------------------------------------------------------------------------------------------------
-- DROP TABLE [dbo].[TextMessage]
CREATE TABLE [dbo].[TextMessage]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[UserId1] uniqueidentifier NOT NULL, 
	[UserId2]uniqueidentifier NOT NULL,
	[Content] nvarchar(max) NOT NULL,
	[SentTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[ReadTimeUTC] datetime NULL,
	
	CONSTRAINT [TextMessage_FK_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [TextMessage_FK_UserId2] FOREIGN KEY ([UserId2]) REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [TextMessageIndex1] ON [dbo].[TextMessage] 
CREATE INDEX [TextMessageIndex1] ON [dbo].[TextMessage]([UserId1], [UserId2]) INCLUDE ([SentTimeUTC]);
GO