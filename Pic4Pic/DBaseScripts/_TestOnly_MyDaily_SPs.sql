
select * from [User];
select * from [FacebookUser]
select * from [ImageMetaFile];
select * from WorkHistory;
select * from EducationHistory;
--select * from [FacebookFriend];
select count(*) from [FacebookFriend];
select * from [Preference]
select *  from [Recommendation]; -- delete from [Recommendation];
select *  from [Action];
select *  from [Pic4Pic];

/*
declare @userId uniqueidentifier;
declare @fbuserId bigint;
set @userId = (SELECT [Id] FROM [User] WHERE [Username] = 'cool hand luke');
set @fbUserId = (SELECT [FacebookId] from [FacebookUser] WHERE [UserId] = @userId);

declare @deleteAllFlag bit;
set @deleteAllFlag = 0;
delete from [Action] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [Pic4Pic] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [Recommendation] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [TextMessage] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
*/

/*
-- select * from [User] where Type <> 0;
declare @userId uniqueidentifier;
declare @fbuserId bigint;
--set @userId = '205AA3A9-BEBE-401C-930E-CB9D57C6387D';
set @userId = (SELECT [Id] FROM [User] WHERE [Username] = 'cool hand luke');
set @fbUserId = (SELECT [FacebookId] from [FacebookUser] WHERE [UserId] = @userId);
--select * from TextMessage

declare @deleteAllFlag bit;
set @deleteAllFlag = 0;
delete from [EducationHistory] where [FacebookId]= @fbUserId OR @deleteAllFlag = 1;
delete from [WorkHistory] where [FacebookId] = @fbUserId OR @deleteAllFlag = 1;
delete from [FacebookFriend] where [FacebookId1] = @fbUserId OR @deleteAllFlag = 1;
delete from [FacebookUser] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [Action] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [CreditAdjustment] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [Pic4Pic] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [ImageMetaFile] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [Preference] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [PurchaseRecord] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [Recommendation] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [TextMessage] where [UserId1] = @userId OR [UserId2] = @userId OR @deleteAllFlag = 1;
delete from [MobileDevice] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [UserDetails] where [UserId] = @userId OR @deleteAllFlag = 1;
delete from [User] where [Id] = @userId OR @deleteAllFlag = 1;
*/

-- @maxCount, @UserId, @ExcludeFacebookFriends, @ExcludeRecentRecommendations, @RecentRecommendationsDayLimit, @MatchState, @InsertToRecommendations
-- EXEC [dbo].[MakeNewRecommendations] 20, '32141998-80F2-4659-83E9-B112C3F649EF', 1, 1, 10, 1, 1;
-- EXEC [dbo].[MakeNewRecommendations] 20, '32141998-80F2-4659-83E9-B112C3F649EF', 0, 0, 10, 1, 0;
-- select * from [dbo].[Recommendation] order by [RecommendTimeUTC] desc;


-----------------------------------------------------------
declare @user1 uniqueidentifier;
declare @user2 uniqueidentifier;
declare @pic1 uniqueidentifier;
declare @pic2 uniqueidentifier;
select * from [ImageMetaFile] where [GroupingId]	= '173D139C-0330-43EB-93D1-9A92D771C4B9';
set @user1 = 'F978D1A5-F074-49A9-8FF1-5C9CA7F22357';
set @user2 = '3208F9D8-AB4C-4E7C-974A-56E47169ABBC';
set @pic1 = (select [Id] from [ImageMetaFile] where [UserId] = @user1 AND [Id] = [GroupingId]);
set @pic2 = (select [Id] from [ImageMetaFile] where [UserId] = @user2 AND [Id] = [GroupingId]);
-- print @pic1;
--print @pic2;
--EXEC [dbo].[RequestPic4Pic] NULL, @user1, @user2, @pic1, NULL;
--EXEC [dbo].[GetPic4PicRequest] @user1, @user2;
-- EXEC [dbo].[GetPic4PicRequest] @user2, @user1;
-- EXEC [dbo].[GetPic4PicRequestById] '36F07CFB-A64C-4E4E-B9B6-57315104D418';

EXEC [dbo].[AcceptPic4Pic] 'D0F248C5-DA6F-47B7-AA72-E6B883F40474', @user2, @pic2;

-- select * from [dbo].[Pic4Pic]; -- delete from [dbo].[Pic4Pic];
-- select * from [dbo].[Action]; -- delete from [dbo].[Action];
-- select * from [dbo].[TextMessage]; -- delete from [dbo].[TextMessage];

EXEC [dbo].[GetActionsToBeNotified] 1000, 100;

declare @utcNow datetime;
set @utcNow = GETUTCDATE();
EXEC [dbo].[ScheduleNotifications] '30239EA0-E739-4BDB-A051-72E82BD3E6A7,C63D0C2E-177B-47CD-BE94-DE2BCECE5035', @utcNow;
-- UPDATE Action SET Status = 0, [NotifScheduleTimeUTC] = NULL, [NotifSentTimeUTC] = NULL, [NotifViewTimeUTC] = NULL;

EXEC [dbo].[MarkNotificationsAsOmitted] '30239EA0-E739-4BDB-A051-72E82BD3E6A7,C63D0C2E-177B-47CD-BE94-DE2BCECE5035';

EXEC [dbo].[MarkNotificationsAsSent] '30239EA0-E739-4BDB-A051-72E82BD3E6A7,C63D0C2E-177B-47CD-BE94-DE2BCECE5035';

EXEC [dbo].[MarkNotificationAsViewed] '30239EA0-E739-4BDB-A051-72E82BD3E6A7', 'E0A69473-DF5F-4C6C-86F6-06FA838545C1';
EXEC [dbo].[MarkNotificationAsViewed] 'C63D0C2E-177B-47CD-BE94-DE2BCECE5035', '897EA85D-BBF9-4CB7-805B-004090EECEF9';


-------------------------------------

select * from TextMessage;
-- delete from TextMessage;

declare @user1 uniqueidentifier;
declare @user2 uniqueidentifier;
declare @user3 uniqueidentifier;
set @user1 = '75816F7A-5D1C-4942-B2D6-72BCAD2E8B2C';
set @user2 = 'D88F478D-58FE-4A0F-8480-000E7A880083';
set @user3 = '6E366C2D-3F31-4A3F-800D-0014857C7D41';

--EXEC [dbo].[GetConversation] @user1, @user2, 1000, 50;
EXEC [dbo].[GetConversationsSummary] @user1, 1000, 50;

declare @id uniqueidentifier;
set @id = NEWID();
EXEC [dbo].[SaveTextMessage] @id, @user1, @user2, 'hello there';
EXEC [dbo].[MarkTextMessageAsRead] @id, @user2;

set @id = NEWID();
EXEC [dbo].[SaveTextMessage] @id, @user2, @user1, 'hi! thank you for the message';
EXEC [dbo].[MarkTextMessageAsRead] @id, @user1;

set @id = NEWID();
EXEC [dbo].[SaveTextMessage] @id, @user1, @user2, 'how are you doing? looking for p4p?';
EXEC [dbo].[MarkTextMessageAsRead] @id, @user2;
/*
set @id = NEWID();
EXEC [dbo].[SaveTextMessage] @id, @user2, @user1, 'mmm not yet';
*/
set @id = NEWID();
EXEC [dbo].[SaveTextMessage] @id, @user1, @user3, 'hi';
EXEC [dbo].[MarkTextMessageAsRead] @id, @user3;

EXEC [dbo].[SaveTextMessage] NULL, @user3, @user1, 'hi<-';
EXEC [dbo].[SaveTextMessage] NULL, @user3, @user1, '???';
EXEC [dbo].[SaveTextMessage] NULL, @user3, @user1, 'Are you there?';
-- EXEC [dbo].[MarkAllTextMessagesAsRead] @user3, @user1;

select * from [User] where [Id] = '32141998-80F2-4659-83E9-B112C3F649EF'
select * from Facebookuser where [UserId] = '32141998-80F2-4659-83E9-B112C3F649EF'
select * from Recommendation ORDER BY RecommendTimeUTC DESC;

select * from Action
select * from Pic4Pic

'32141998-80F2-4659-83E9-B112C3F649EF'
'59C9211A-16E3-4E04-8F62-1487FAFAC912'
'4922D083-E9D1-4C7B-9CE4-027AC9AD58F7'

select * from [User];

-- notifications sent to me
select * from [Action] where [UserId2] = '32141998-80F2-4659-83E9-B112C3F649EF'; // => '84CFB5BF-11B4-4349-8FA5-EF2A9E466944', '177DDCCA-B171-4368-92CB-F464F0A0A296'

-- notifications triggered by me
select * from [Action] where [UserId1] = '32141998-80F2-4659-83E9-B112C3F649EF';

EXEC [dbo].[GetRecentActions] '32141998-80F2-4659-83E9-B112C3F649EF', 10000, 100;


declare @id uniqueidentifier;
set @id = NEWID();
--EXEC [dbo].[InsertAction] @id, '0E4795C2-2B42-4C38-903F-6A0FD50EC6D1', '32141998-80F2-4659-83E9-B112C3F649EF', 4;
--EXEC [dbo].[InsertAction] @id, '2F7D442C-4B7D-4C4B-ABD5-6FBA19A303A9', '32141998-80F2-4659-83E9-B112C3F649EF', 4;

-----------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------

----------------------------------------
-- delete p4p requests from me (they were for test)
select * from Action;
-- delete from Action where ActionType <> 4 AND ActionType <> 6;
-- delete from Action where ActionTimeUTC > '2014-05-13 00:00:00.000';
-- delete from Action where UserId1 = '32141998-80F2-4659-83E9-B112C3F649EF';
-- update Action set Status = 0, NotifViewTimeUTC = NULL;

----------------------------------------
-- delete p4p requests from me (they were for test)
select * from Pic4Pic;
-- delete from Pic4Pic where UserId1 = '32141998-80F2-4659-83E9-B112C3F649EF';
-- update Pic4Pic set PicId2 = NULL, AcceptTimeUTC = NULL;

----------------------------------------
-- add some 'viewed' actions for myself
declare @table table(Id uniqueidentifier);
insert into @table
	select distinct UserId2 from Recommendation where UserId1 = 'E2E415E6-E11C-4C64-B181-96D0E3EBA304';
;with Helper(ActionId, UserId) AS
(
	select top(5) NEWID() as ActionId, Id as UserId from @table order by NEWID()
)
INSERT INTO [dbo].[Action] ([Id],[UserId1],[UserId2],[ActionType])
	SELECT ActionId, UserId, 'E2E415E6-E11C-4C64-B181-96D0E3EBA304', 1 FROM Helper;
----------------------------------------------------------------------------------------------------------------
----------------- RECOMMEND ME to OTHERS -----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
declare @meUserId uniqueidentifier;
set @meUserId = '86100B21-6B28-43A3-A6D8-14BB5003A53F';
-- firstly recommend me to some candidates
declare @table1 table(RowNumber int, UserId uniqueidentifier);
insert into @table1
	select TOP(20) ROW_NUMBER() OVER(ORDER BY UserId) AS RowNumber, UserId FROM [FacebookUser]
		WHERE [UserId] <> @meUserId AND [Gender] = 2 AND [HometownCity] = 'Seattle'
			AND [UserId] NOT IN (SELECT [UserId1] FROM [Recommendation] WHERE [UserId2] = @meUserId)
			AND [UserId] NOT IN (SELECT [UserId2] FROM [Recommendation] WHERE [UserId1] = @meUserId);
declare @j int;
declare @c int;
declare @uid uniqueidentifier;
set @j = 0;
set @c = (select count(*) from @table1);
while (@j < @c)
BEGIN
	set @j = @j + 1;
	set @uid = (select UserId from @table1 where RowNumber = @j);
	INSERT INTO [dbo].[Recommendation] ([UserId1], [UserId2]) VALUES (@uid, @meUserId);
END

----------------------------------------------------------------------------------------------------------------
----------------- P4P requests from ME to OTHERS ---------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
declare @meUserId uniqueidentifier;
set @meUserId = '86100B21-6B28-43A3-A6D8-14BB5003A53F';

-- request some 'p4p'
declare @table2 table(RowNumber int, UserId uniqueidentifier, PictureId uniqueidentifier);
insert into @table2
	select TOP(5) ROW_NUMBER() OVER(ORDER BY UserId) AS RowNumber, UserId, GroupingId from [ImageMetaFile] 
		Where IsBlurred = 0 AND IsThumbnail = 0 AND IsProfilePicture = 1 
		AND [UserId] IN (select distinct UserId1 from Recommendation where UserId2 = @meUserId
							AND UserId1 NOT IN (select distinct [UserId1] FROM [dbo].[Pic4Pic] WHERE [UserId1] = @meUserId OR [UserId2] = @meUserId))
		ORDER BY UserId;

declare @i int;
declare @count int;
set @i = 0;
set @count = (select count(*) from @table2);
declare @userId uniqueidentifier;
declare @picId uniqueidentifier;
declare @pic4picId uniqueidentifier;
declare @mePhotoGroupId uniqueidentifier;
set @mePhotoGroupId = (SELECT GroupingId from [ImageMetaFile] 
						Where IsBlurred = 0 AND IsThumbnail = 0 AND IsProfilePicture = 1 
							AND [UserId] = @meUserId);
WHILE(@i < @count)
BEGIN
	set @userId = (select UserId from @table2 where RowNumber = (@i+1));
	set @picId = (select PictureId from @table2 where RowNumber = (@i+1));
	set @pic4picId = NEWID();
	EXEC [dbo].[RequestPic4Pic] @pic4picId, @meUserId, @userId, @mePhotoGroupId, null;
	set @i = @i + 1;
	-- accept one of the p4p requests.	
	/*
	IF(@i = 1)
	BEGIN
		EXEC [dbo].[AcceptPic4Pic] @pic4picId, @userId, @picId;
		-- sent a message from me
		EXEC [dbo].[SaveTextMessage] null, @userId, @meUserId, N'Hi, Thank you for pinging me. Wanna date?';
		-- sent a message from x 
		EXEC [dbo].[SaveTextMessage] null, @meUserId, @userId, N'You are fast! :)';		
	END
	*/
END

----------------------------------------------------------------------------------------------------------------
----------------- P4P requests from OTHERS to me ---------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------
declare @meUserId uniqueidentifier;
set @meUserId = '86100B21-6B28-43A3-A6D8-14BB5003A53F';
-- request some 'p4p' from others to me.
declare @table2 table(RowNumber int, UserId uniqueidentifier, PictureId uniqueidentifier);
insert into @table2
	select TOP(5) ROW_NUMBER() OVER(ORDER BY UserId) AS RowNumber, UserId, GroupingId from [ImageMetaFile] 
		Where IsBlurred = 0 AND IsThumbnail = 0 AND IsProfilePicture = 1 
		AND [UserId] IN (select distinct UserId2 from Recommendation where UserId1 = @meUserId
							AND UserId2 NOT IN (select distinct [UserId1] FROM [dbo].[Pic4Pic] WHERE [UserId2] = @meUserId))
		ORDER BY UserId;
declare @i int;
declare @count int;
set @i = 0;
set @count = (select count(*) from @table2);
declare @userId uniqueidentifier;
declare @picId uniqueidentifier;
declare @pic4picId uniqueidentifier;
declare @mePhotoGroupId uniqueidentifier;
set @mePhotoGroupId = (SELECT GroupingId from [ImageMetaFile] 
						Where IsBlurred = 0 AND IsThumbnail = 0 AND IsProfilePicture = 1 
							AND [UserId] = @meUserId);
WHILE(@i < @count)
BEGIN
	set @userId = (select UserId from @table2 where RowNumber = (@i+1));
	set @picId = (select PictureId from @table2 where RowNumber = (@i+1));
	set @pic4picId = NEWID();
	EXEC [dbo].[RequestPic4Pic] @pic4picId, @userId, @meUserId, @picId, null;
	set @i = @i + 1;
	-- accept one of the p4p requests.	
	IF(@i = 1)
	BEGIN
		EXEC [dbo].[AcceptPic4Pic] @pic4picId, @meUserId, @mePhotoGroupId;
		-- sent a message from me
		EXEC [dbo].[SaveTextMessage] null, @meUserId, @userId, N'Hi, Thank you for pinging me. Wanna date?';
		-- sent a message from x 
		EXEC [dbo].[SaveTextMessage] null, @userId, @meUserId, N'You are fast! :)';
	END
END

-- select * from TextMessage


-- select * from Region where Code = 'nv';
-- select * from SubRegion where RegionId = (select [Id] from Region where Code = 'dc');
-- select distinct SubRegionId from City where RegionId = (select [Id] from Region where Code = 'dc');

select [F].FacebookId, [U].[username],
		(YEAR(GETUTCDATE()) - YEAR([F].BirthDay)) as [Age],
		[F].MaritalStatusAsText, [F].Profession, [F].EducationLevel, [F].HometownCity	
	from [FacebookUser] [F]
	join [User] [U] ON [U].[Id] = [F].[UserId]
	where [F].Gender = 1
		AND HometownCity IN (Select Name from City where CountryId = 1 and RegionId = 33 and SubRegionId = 0)
		AND HometownState IN (Select Name from Region where CountryId = 1 and Id = 33)
	--Order By Age Asc
	Order By HometownCity