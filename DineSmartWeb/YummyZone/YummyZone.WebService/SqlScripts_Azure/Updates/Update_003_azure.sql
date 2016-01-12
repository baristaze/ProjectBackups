
-- select * from sys.objects where type = 'F'

ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] DROP CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_GroupId];
ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] DROP CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuCategoryId];
ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] DROP CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuItemId];
ALTER TABLE [dbo].[MenuItemImage] DROP CONSTRAINT [MenuItemImage_ForeignKey_GroupId];
ALTER TABLE [dbo].[MenuItemImage] DROP CONSTRAINT [MenuItemImage_ForeignKey_MenuItemId];
ALTER TABLE [dbo].[DinerPassword] DROP CONSTRAINT [DinerPassword_ForeignKey_UserName];
ALTER TABLE [dbo].[NotificationClient] DROP CONSTRAINT [NotificationClient_ForeignKey_DinerId];
ALTER TABLE [dbo].[CheckIn] DROP CONSTRAINT [CheckIn_ForeignKey_DinerId];
ALTER TABLE [dbo].[CheckIn] DROP CONSTRAINT [CheckIn_ForeignKey_VenueId];
ALTER TABLE [dbo].[MenuItemRate] DROP CONSTRAINT [MenuItemRate_ForeignKey_CheckInId];
ALTER TABLE [dbo].[MenuItemRate] DROP CONSTRAINT [MenuItemRate_ForeignKey_MenuItemId];
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [Question_ForeignKey_GroupId];
ALTER TABLE [dbo].[ChainAndQuestionMap] DROP CONSTRAINT [QuestionAndVenueMap_ForeignKey_GroupId];
ALTER TABLE [dbo].[ChainAndQuestionMap] DROP CONSTRAINT [QuestionAndVenueMap_ForeignKey_ChainId];
ALTER TABLE [dbo].[ChainAndQuestionMap] DROP CONSTRAINT [QuestionAndVenueMap_ForeignKey_QuestionId];
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [Choice_ForeignKey_GroupId];
ALTER TABLE [dbo].[QuestionAndChoiceMap] DROP CONSTRAINT [QuestionAndChoiceMap_ForeignKey_GroupId];
ALTER TABLE [dbo].[Chain] DROP CONSTRAINT [Chain_ForeignKey_GroupId];
ALTER TABLE [dbo].[QuestionAndChoiceMap] DROP CONSTRAINT [QuestionAndChoiceMap_ForeignKey_QuestionId];
ALTER TABLE [dbo].[QuestionAndChoiceMap] DROP CONSTRAINT [QuestionAndChoiceMap_ForeignKey_ChoiceId]; -- some tricks here
ALTER TABLE [dbo].[LogoImage] DROP CONSTRAINT [LogoImage_ForeignKey_GroupId];
ALTER TABLE [dbo].[LogoImage] DROP CONSTRAINT [LogoImage_ForeignKey_ChainId];
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [Answer_ForeignKey_CheckInId];
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [Answer_ForeignKey_QuestionId];
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [Answer_ForeignKey_AnswerChoiceId];
ALTER TABLE [dbo].[MarkedAsRead] DROP CONSTRAINT [MarkedAsRead_ForeignKey_CheckInId];
ALTER TABLE [dbo].[Venue] DROP CONSTRAINT [Venue_ForeignKey_GroupId];
ALTER TABLE [dbo].[MarkedAsRead] DROP CONSTRAINT [MarkedAsRead_ForeignKey_UserId];
ALTER TABLE [dbo].[Venue] DROP CONSTRAINT [Venue_ForeignKey_ChainId];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [Message_ForeignKey_GroupId];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [Message_ForeignKey_SenderId];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [Message_ForeignKey_ChainId];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [Message_ForeignKey_ReceiverId];
ALTER TABLE [dbo].[User] DROP CONSTRAINT [User_ForeignKey_GroupId];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [Message_ForeignKey_CheckInId];
ALTER TABLE [dbo].[Password] DROP CONSTRAINT [Password_ForeignKey_UserId];
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [UserRole_ForeignKey_GroupId];
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [UserRole_ForeignKey_VenueId];
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [UserRole_ForeignKey_UserId];
ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [Campaign_ForeignKey_GroupId];
ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [Campaign_ForeignKey_ChainId];
ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [Campaign_ForeignKey_CreatorId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_GroupId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_SenderId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_ChainId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_ReceiverId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_CheckInId];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [Coupon_ForeignKey_RedeemCheckInId];
ALTER TABLE [dbo].[Menu] DROP CONSTRAINT [Menu_ForeignKey_GroupId];
ALTER TABLE [dbo].[VenueAndMenuMap] DROP CONSTRAINT [VenueAndMenuMap_ForeignKey_GroupId];
ALTER TABLE [dbo].[VenueAndMenuMap] DROP CONSTRAINT [VenueAndMenuMap_ForeignKey_VenueId];
ALTER TABLE [dbo].[VenueAndMenuMap] DROP CONSTRAINT [VenueAndMenuMap_ForeignKey_MenuId];
ALTER TABLE [dbo].[MenuCategory] DROP CONSTRAINT [MenuCategory_ForeignKey_GroupId];
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] DROP CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_GroupId];
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] DROP CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuId];
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] DROP CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuCategoryId];
ALTER TABLE [dbo].[MenuItem] DROP CONSTRAINT [MenuItem_ForeignKey_GroupId];
ALTER TABLE [dbo].[DinerSettings] DROP CONSTRAINT [DinerSettings_ForeignKey_DinerId];


-- select * from sys.objects where type = 'PK'

ALTER TABLE [dbo].[Menu] DROP CONSTRAINT [PK__Menu__3214EC0602C769E9];
ALTER TABLE [dbo].[MenuCategory] DROP CONSTRAINT [PK__MenuCate__3214EC0611158940];
ALTER TABLE [dbo].[NotificationClient] DROP CONSTRAINT [PK__Notifica__99E86CC71A34DF26];
ALTER TABLE [dbo].[MenuItem] DROP CONSTRAINT [PK__MenuItem__3214EC061F63A897];
ALTER TABLE [dbo].[Diner] DROP CONSTRAINT [PK__Diner__3214EC063552E9B6];
ALTER TABLE [dbo].[CheckIn] DROP CONSTRAINT [PK__CheckIn__3214EC063EDC53F0];
ALTER TABLE [dbo].[Group] DROP CONSTRAINT [PK__Group__3214EC0645BE5BA9];
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [PK__Question__3214EC064C364F0E];
ALTER TABLE [dbo].[Chain] DROP CONSTRAINT [PK__Chain__3214EC064D5F7D71];
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [PK__Choice__3214EC0659904A2C];
ALTER TABLE [dbo].[Venue] DROP CONSTRAINT [PK__Venue__3214EC065BAD9CC8];
ALTER TABLE [dbo].[User] DROP CONSTRAINT [PK__User__3214EC06681373AD];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [PK__Message__3214EC067073AF84];
ALTER TABLE [dbo].[SignupInfo] DROP CONSTRAINT [PK__SignupIn__3214EC0672910220];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [PK__Coupon__3214EC067AF13DF7];
ALTER TABLE [dbo].[SystemUser] DROP CONSTRAINT [PK__SystemUs__3214EC067B264821];
ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [PK__Campaign__3214EC067BB05806];

/*
ALTER TABLE [dbo].[] ADD CONSTRAINT [x_PK] PRIMARY KEY CLUSTERED ([Id])
GO
*/

ALTER TABLE [dbo].[Group] ADD CONSTRAINT [Group_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Chain] ADD CONSTRAINT [Chain_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Venue] ADD CONSTRAINT [Venue_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[User] ADD CONSTRAINT [User_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[SignupInfo] ADD CONSTRAINT [SignupInfo_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[SystemUser] ADD CONSTRAINT [SystemUser_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Menu] ADD CONSTRAINT [Menu_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[MenuCategory] ADD CONSTRAINT [MenuCategory_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[MenuItem] ADD CONSTRAINT [MenuItem_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Diner] ADD CONSTRAINT [Diner_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[NotificationClient] ADD CONSTRAINT [NotificationClient_PK] PRIMARY KEY CLUSTERED ([DeviceToken]);
ALTER TABLE [dbo].[CheckIn] ADD CONSTRAINT [CheckIn_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Question] ADD CONSTRAINT [Question_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Choice] ADD CONSTRAINT [Choice_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_PK] PRIMARY KEY CLUSTERED ([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_PK] PRIMARY KEY CLUSTERED ([Id]);


ALTER TABLE [dbo].[Chain] ADD CONSTRAINT [Chain_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[LogoImage] ADD CONSTRAINT [LogoImage_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[LogoImage] ADD CONSTRAINT [LogoImage_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[Venue] ADD CONSTRAINT [Venue_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[Venue] ADD CONSTRAINT [Venue_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
ALTER TABLE [dbo].[User] ADD CONSTRAINT [User_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[Password] ADD CONSTRAINT [Password_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [UserRole_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [UserRole_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]);
ALTER TABLE [dbo].[UserRole] ADD CONSTRAINT [UserRole_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[Menu] ADD CONSTRAINT [Menu_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[VenueAndMenuMap] ADD CONSTRAINT [VenueAndMenuMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[VenueAndMenuMap] ADD CONSTRAINT [VenueAndMenuMap_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]);
ALTER TABLE [dbo].[VenueAndMenuMap] ADD CONSTRAINT [VenueAndMenuMap_ForeignKey_MenuId] FOREIGN KEY([MenuId]) REFERENCES [dbo].[Menu]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[MenuCategory] ADD CONSTRAINT [MenuCategory_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] ADD CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] ADD CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuId] FOREIGN KEY([MenuId]) REFERENCES [dbo].[Menu]([Id]);
ALTER TABLE [dbo].[MenuAndMenuCategoryMap] ADD CONSTRAINT [MenuAndMenuCategoryMap_ForeignKey_MenuCategoryId] FOREIGN KEY([MenuCategoryId]) REFERENCES [dbo].[MenuCategory]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[MenuItem] ADD CONSTRAINT [MenuItem_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] ADD CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] ADD CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuCategoryId] FOREIGN KEY([MenuCategoryId]) REFERENCES [dbo].[MenuCategory]([Id]);
ALTER TABLE [dbo].[MenuCategoryAndMenuItemMap] ADD CONSTRAINT [MenuCategoryAndMenuItemMap_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[MenuItemImage] ADD CONSTRAINT [MenuItemImage_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[MenuItemImage] ADD CONSTRAINT [MenuItemImage_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[DinerPassword] ADD CONSTRAINT [DinerPassword_ForeignKey_UserName] FOREIGN KEY([UserName]) REFERENCES [dbo].[Diner]([UserName]) ON UPDATE CASCADE;
ALTER TABLE [dbo].[NotificationClient] ADD CONSTRAINT [NotificationClient_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]);
ALTER TABLE [dbo].[CheckIn] ADD CONSTRAINT [CheckIn_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]);
ALTER TABLE [dbo].[CheckIn] ADD CONSTRAINT [CheckIn_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]);
ALTER TABLE [dbo].[MenuItemRate] ADD CONSTRAINT [MenuItemRate_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[MenuItemRate] ADD CONSTRAINT [MenuItemRate_ForeignKey_MenuItemId] FOREIGN KEY([MenuItemId]) REFERENCES [dbo].[MenuItem]([Id]);
ALTER TABLE [dbo].[Question] ADD CONSTRAINT [Question_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
ALTER TABLE [dbo].[ChainAndQuestionMap] ADD CONSTRAINT [QuestionAndVenueMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[Choice] ADD CONSTRAINT [Choice_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[QuestionAndChoiceMap] ADD CONSTRAINT [QuestionAndChoiceMap_ForeignKey_ChoiceId] FOREIGN KEY([ChoiceId]) REFERENCES [dbo].[Choice]([Id]) ON DELETE CASCADE;
ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_QuestionId] FOREIGN KEY([QuestionId]) REFERENCES [dbo].[Question]([Id]);
ALTER TABLE [dbo].[Answer] ADD CONSTRAINT [Answer_ForeignKey_AnswerChoiceId] FOREIGN KEY([AnswerChoiceId]) REFERENCES [dbo].[Choice]([Id]);
ALTER TABLE [dbo].[MarkedAsRead] ADD CONSTRAINT [MarkedAsRead_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[MarkedAsRead] ADD CONSTRAINT [MarkedAsRead_ForeignKey_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]);
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_CreatorId] FOREIGN KEY([CreatorId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_RedeemCheckInId] FOREIGN KEY([RedeemCheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
ALTER TABLE [dbo].[DinerSettings] ADD CONSTRAINT [DinerSettings_ForeignKey_DinerId] FOREIGN KEY([DinerId]) REFERENCES [dbo].[Diner]([Id]);

----

ALTER TABLE [dbo].[Log] ADD [Id] bigint NOT NULL PRIMARY KEY IDENTITY;

DROP INDEX [AddressIndex1] ON [dbo].[Address];
CREATE CLUSTERED INDEX [AddressIndex1] ON [dbo].[Address]([ObjectId],[ObjectType],[AddressType]);

DROP INDEX [VenueAndMenuMapIndex1] ON [dbo].[VenueAndMenuMap];
CREATE CLUSTERED INDEX [VenueAndMenuMapIndex1] ON [dbo].[VenueAndMenuMap]([GroupId], [VenueId], [MenuId], [Status]);

DROP INDEX [UserRoleIndex1] ON [dbo].[UserRole];
CREATE CLUSTERED INDEX [UserRoleIndex1] ON [dbo].[UserRole]([GroupId], [UserId], [VenueId], [Status]);

DROP INDEX [MenuAndMenuCategoryMapIndex1] ON [dbo].[MenuAndMenuCategoryMap];
CREATE CLUSTERED INDEX [MenuAndMenuCategoryMapIndex1] ON [dbo].[MenuAndMenuCategoryMap]([GroupId], [MenuId], [MenuCategoryId], [Status]);

DROP INDEX [DinerSettingsIndex1] ON [DinerSettings];
CREATE CLUSTERED INDEX [DinerSettingsIndex1] ON [dbo].[DinerSettings]([DinerId], [Name]);

DROP INDEX [MenuCategoryAndMenuItemMapIndex1] ON [dbo].[MenuCategoryAndMenuItemMap];
CREATE CLUSTERED INDEX [MenuCategoryAndMenuItemMapIndex1] ON [dbo].[MenuCategoryAndMenuItemMap]([GroupId], [MenuCategoryId], [MenuItemId], [Status]);

DROP INDEX [MenuItemImageIndex1] ON [dbo].[MenuItemImage];
CREATE CLUSTERED INDEX [MenuItemImageIndex1] ON [dbo].[MenuItemImage]([GroupId], [MenuItemId]);

DROP INDEX [DinerPasswordIndex1] ON [dbo].[DinerPassword];
CREATE CLUSTERED INDEX [DinerPasswordIndex1] ON [dbo].[DinerPassword]([UserName]);

DROP INDEX [MenuItemRateIndex1] ON [dbo].[MenuItemRate];
CREATE CLUSTERED INDEX [MenuItemRateIndex1] ON [dbo].[MenuItemRate]([MenuItemId], [CheckInId], [Rate]);

DROP INDEX [ChainAndQuestionMapIndex1] ON [dbo].[ChainAndQuestionMap];
CREATE CLUSTERED INDEX [ChainAndQuestionMapIndex1] ON [dbo].[ChainAndQuestionMap]([GroupId], [ChainId],[QuestionId], [Status]);

DROP INDEX [LogoImageIndex1] ON [dbo].[LogoImage];
CREATE CLUSTERED INDEX [LogoImageIndex1] ON [dbo].[LogoImage]([GroupId], [ChainId]);

DROP INDEX [AnswerIndex1] ON [dbo].[Answer];
CREATE CLUSTERED INDEX [AnswerIndex1] ON [dbo].[Answer]([QuestionId], [CheckInId]);

DROP INDEX [QuestionAndChoiceMapIndex1] ON [dbo].[QuestionAndChoiceMap];
CREATE CLUSTERED INDEX [QuestionAndChoiceMapIndex1] ON [dbo].[QuestionAndChoiceMap]([GroupId], [QuestionId], [ChoiceId], [Status]);

DROP INDEX [MarkedAsReadIndex1] ON [dbo].[MarkedAsRead];
CREATE CLUSTERED INDEX [MarkedAsReadIndex1] ON [dbo].[MarkedAsRead]([UserId], [CheckInId]);

DROP INDEX [PasswordIndex1] ON [dbo].[Password];
CREATE CLUSTERED INDEX [PasswordIndex1] ON [dbo].[Password]([UserId]);

------

-- select * from sys.objects where type = 'D' and name like '%__Id__%'

ALTER TABLE [dbo].[Menu] DROP CONSTRAINT [DF__Menu__Id__04AFB25B];
ALTER TABLE [dbo].[MenuCategory] DROP CONSTRAINT [DF__MenuCategory__Id__12FDD1B2];
ALTER TABLE [dbo].[MenuItem] DROP CONSTRAINT [DF__MenuItem__Id__214BF109];
ALTER TABLE [dbo].[Diner] DROP CONSTRAINT [DF__Diner__Id__373B3228];
ALTER TABLE [dbo].[CheckIn] DROP CONSTRAINT [DF__CheckIn__Id__40C49C62];
ALTER TABLE [dbo].[Group] DROP CONSTRAINT [DF__Group__Id__47A6A41B];
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [DF__Question__Id__4E1E9780];
ALTER TABLE [dbo].[Chain] DROP CONSTRAINT [DF__Chain__Id__4F47C5E3];
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [DF__Choice__Id__5B78929E];
ALTER TABLE [dbo].[Venue] DROP CONSTRAINT [DF__Venue__Id__5D95E53A];
ALTER TABLE [dbo].[User] DROP CONSTRAINT [DF__User__Id__69FBBC1F];
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [DF__Message__Id__725BF7F6];
ALTER TABLE [dbo].[SignupInfo] DROP CONSTRAINT [DF__SignupInfo__Id__74794A92];
ALTER TABLE [dbo].[Coupon] DROP CONSTRAINT [DF__Coupon__Id__7CD98669];
ALTER TABLE [dbo].[SystemUser] DROP CONSTRAINT [DF__SystemUser__Id__7D0E9093];
ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [DF__Campaign__Id__7D98A078];


ALTER TABLE [dbo].[Menu] ADD CONSTRAINT [DF__Menu__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[MenuCategory] ADD CONSTRAINT [DF__MenuCategory__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[MenuItem] ADD CONSTRAINT [DF__MenuItem__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Diner] ADD CONSTRAINT [DF__Diner__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[CheckIn] ADD CONSTRAINT [DF__CheckIn__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Group] ADD CONSTRAINT [DF__Group__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Question] ADD CONSTRAINT [DF__Question__Id] DEFAULT NEWID() FOR [Id];;
ALTER TABLE [dbo].[Chain] ADD CONSTRAINT [DF__Chain__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Choice] ADD CONSTRAINT [DF__Choice__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Venue] ADD CONSTRAINT [DF__Venue__Id] DEFAULT NEWID() FOR [Id];;
ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF__User__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Message] ADD CONSTRAINT [DF__Message__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[SignupInfo] ADD CONSTRAINT [DF__SignupInfo__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [DF__Coupon__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[SystemUser] ADD CONSTRAINT [DF__SystemUser__Id] DEFAULT NEWID() FOR [Id];
ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [DF__Campaign__Id] DEFAULT NEWID() FOR [Id];