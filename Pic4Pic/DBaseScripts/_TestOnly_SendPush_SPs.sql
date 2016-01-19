/*
select * from [User]
*/

declare @userId1 uniqueidentifier;
declare @userId2 uniqueidentifier;
declare @userId3 uniqueidentifier;
declare @userId4 uniqueidentifier;
declare @userId5 uniqueidentifier;

set @userId1 = '4922D083-E9D1-4C7B-9CE4-027AC9AD58F7';
set @userId2 = '346BA033-C7F8-41F1-97FC-035D039CD6E7';
set @userId3 = 'DF1D8700-F0D9-48CA-B62E-0640B52E61FA';
set @userId4 = '456A1716-6B73-4A65-B268-084BCEC3A23B';
set @userId5 = 'DBF24718-CE8A-443D-A811-0919273FDF43';

declare @deviceKey nvarchar(max);
declare @id uniqueidentifier;

/*
set @id = NEWID();
set @deviceKey = 'device_key_' + CONVERT(nvarchar(max), (select ROUND(RAND() * 1000000, 0)));
EXEC [dbo].[AddOrUpdateMobileDevice] @id, @userId1, 1, '12.9.1', '14.0', '5.3', 'Samsung Galaxy S4', @deviceKey; 

set @id = NEWID();
set @deviceKey = 'device_key_' + CONVERT(nvarchar(max), (select ROUND(RAND() * 1000000, 0)));
EXEC [dbo].[AddOrUpdateMobileDevice] @id, @userId2, 1, '12.9.1', '14.0', '5.3', 'Samsung Galaxy S4', @deviceKey; 

set @id = NEWID();
set @deviceKey = 'device_key_' + CONVERT(nvarchar(max), (select ROUND(RAND() * 1000000, 0)));
EXEC [dbo].[AddOrUpdateMobileDevice] @id, @userId3, 1, '12.9.1', '14.0', '5.3', 'Samsung Galaxy S4', @deviceKey; 

set @id = NEWID();
set @deviceKey = 'device_key_' + CONVERT(nvarchar(max), (select ROUND(RAND() * 1000000, 0)));
EXEC [dbo].[AddOrUpdateMobileDevice] @id, @userId4, 1, '12.9.1', '14.0', '5.3', 'Samsung Galaxy S4', @deviceKey; 

set @id = NEWID();
set @deviceKey = 'device_key_' + CONVERT(nvarchar(max), (select ROUND(RAND() * 1000000, 0)));
EXEC [dbo].[AddOrUpdateMobileDevice] @id, @userId5, 1, '12.9.1', '14.0', '5.3', 'Samsung Galaxy S4', @deviceKey; 
*/

-- select * from MobileDevice
-- delete from MobileDevice

/*	ViewedProfile(1),
	SentText(3),	
	LikedBio(4),	
	RequestingP4P(6),
	AcceptedP4P(7);
*/

delete from [Action];

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId3, 1;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId4, 1;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId3, 1;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId4, 1;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId3, @userId5, 1;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId2, 6;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId3, 6;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId4, 6;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId5, 6;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId1, 7;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId5, @userId2, 7;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId1, 3;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId4, 3;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId3, @userId2, 3;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId2, @userId3, 4;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId1, @userId3, 4;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId4, @userId2, 4;

set @id = NEWID();
EXEC [dbo].[InsertAction] @id, @userId3, @userId4, 4;

-- select * from [Action]