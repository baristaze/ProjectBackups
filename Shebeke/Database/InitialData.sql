INSERT INTO [dbo].[ReactionType]([Id],[Text],[Order]) VALUES
			(1, N'YAY!', 1),
			(2, N'LOL', 2),
			(4, N'WIN', 3),
			(8, N'OMFG', 4),
			(16, N'GR8', 5),
			(32, N'WTF', 6),
			(64, N'EPIC', 7),
			(128, N'FAIL!', 8),
			(256, N'NERDS', 9),
			(512, N'UGH', 10),
			(1024, N'MEH', 11),
			(2048, N'YUCK', 12),
			(4096, N'BOOO!', 13);
GO

/*
UPDATE [dbo].[ReactionType] SET [IsEnabled] = 0 WHERE [Id] IN (512, 2048, 4096);
*/

INSERT INTO [dbo].[Category] VALUES(N'Genel');
INSERT INTO [dbo].[Category] VALUES(N'Tanım');
INSERT INTO [dbo].[Category] VALUES(N'Haber');
INSERT INTO [dbo].[Category] VALUES(N'TV');
INSERT INTO [dbo].[Category] VALUES(N'Popüler Kültür');
GO