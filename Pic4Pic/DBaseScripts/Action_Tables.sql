/************************************************************************************************************/
/************************************************************************************************************/

/*	[ActionType]
	------------
	Undefined(0),
	ViewedProfile(1),
	Poked(2),		
	SentText(3),	
	LikedBio(4),	
	LikedPhoto(5),	
	RequestingP4P(6),
	AcceptedP4P(7);
*/

-------------------------------------------------------------------------------------------------------------
-- Records all actions including Pic4Pic, even if it is special
-- Do not include data here since we might trim records in this table (i.e. delete old ones)
-- A Pic4Pic or SendText actions will be recorded here but their data will be saved separately.
-------------------------------------------------------------------------------------------------------------

-- DROP TABLE [dbo].[Action]
CREATE TABLE [dbo].[Action]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
	[UserId1] uniqueidentifier NOT NULL, 
	[UserId2]uniqueidentifier NOT NULL,
	[ActionType] tinyint NOT NULL, /* see above */
	[ActionTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[Status] tinyint NOT NULL DEFAULT 0, /* 0=Created, 1=NotificationOmitted, 2=NotificationScheduled, 3=NotificationSent, 4=NotificationViewed */
	[NotifScheduleTimeUTC] datetime NULL, 
	[NotifSentTimeUTC] datetime NULL,
	[NotifViewTimeUTC] datetime NULL
	
	CONSTRAINT [Action_FK_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [Action_FK_UserId2] FOREIGN KEY ([UserId2]) REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [ActionIndex] ON [dbo].[Action];
CREATE INDEX [ActionIndex] ON [dbo].[Action]([Id], [UserId1], [UserId2], [ActionTimeUTC], [Status]);
GO