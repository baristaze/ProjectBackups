

/*
CREATE TABLE [dbo].[MessageTemplate] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[OrderIndex] tinyint NOT NULL DEFAULT 255,
	[Title] nvarchar(100) NOT NULL,
	[Content] nvarchar(2000) NOT NULL,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [MessageTemplate_PK] PRIMARY KEY CLUSTERED ([Id]) -- see alter table section
	-- CONSTRAINT [MessageTemplate_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
)
GO

ALTER TABLE [dbo].[MessageTemplate] ADD CONSTRAINT [MessageTemplate_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[MessageTemplate] ADD CONSTRAINT [MessageTemplate_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO
*/

CREATE TABLE [dbo].[Message] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[SenderId] uniqueidentifier NOT NULL, -- user... who sends this message?
	[ChainId] uniqueidentifier NOT NULL, -- on behalf of which restaurant (chain)?
	[ReceiverId] uniqueidentifier NOT NULL,	-- to whom?
	[CheckInId] uniqueidentifier NULL, -- is this upon a check-in? not necessarily...
	[Title] nvarchar(140) NOT NULL,
	[Content] nvarchar(2000) NOT NULL,
	[QueueTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(), -- manager saved it to be sent by the system
	[PushTimeUTC] datetime NULL, -- our notification service pushed it
	[ReadTimeUTC] datetime NULL, -- diner read the message
	[DeleteTimeUTC] datetime NULL, -- diner deletes the message
	
	-- CONSTRAINT [Message_PK] PRIMARY KEY CLUSTERED ([Id]) -- see alter table section
	-- CONSTRAINT [Message_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),
	-- CONSTRAINT [Message_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]),
	-- CONSTRAINT [Message_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]),
	-- CONSTRAINT [Message_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]),
	-- CONSTRAINT [Message_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]),
)
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]);
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]);
GO

ALTER TABLE [dbo].[Message] ADD CONSTRAINT [Message_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
GO

CREATE INDEX [MessageIndex1] ON [dbo].[Message]([Id]) INCLUDE ([GroupId],[SenderId],[ChainId],[ReceiverId],[CheckInId]);
GO

CREATE TABLE [dbo].[Campaign] (	
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[ChainId] uniqueidentifier NOT NULL, -- campaign is for
	[CreatorId] uniqueidentifier NOT NULL,	-- user id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[Name] nvarchar(100) NOT NULL, -- only managers see this
	[Status] tinyint NOT NULL DEFAULT 0,	-- Draft, Active, Disabled, Removed. [Disabled=Do not repeat anymore, Expired]
	[CampaignType] tinyint NOT NULL DEFAULT 0,	-- Unspecified, Manual, AutoCouponOnSignup, MassDistribution
	[Priority] int NOT NULL DEFAULT 0,	-- 0 is the lowest priority... to address which campaign to use if there are two campaigns defined in the system.
	[CouponCount] int NOT NULL,
	[Repeatition] tinyint NOT NULL DEFAULT 0, -- None, Daily, Weekly, Bi-weekly, Monthly
	[PublishTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(), -- initial publish time; e.g. next monday... this fictively repeats as well...
	[RevocationTimeUTC] datetime NULL,
	[CouponType] tinyint NOT NULL DEFAULT 0,	-- Unspecified, Discount as a Percentage, Discount as a value, Free Sub Item, Buy1Get1Free
	[Title] nvarchar(140) NOT NULL, -- customers see this
	[Content] nvarchar(2000) NULL,
	[ExpiryDays] int NOT NULL DEFAULT 30, -- if repeatition is None
	[FaceValue] decimal (9,6) NULL,			-- when the type is percentage / value
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	[LastUpdateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	-- CONSTRAINT [Campaign_PK] PRIMARY KEY CLUSTERED ([Id]) -- see alter table section
	-- CONSTRAINT [Campaign_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),	
	-- CONSTRAINT [Campaign_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]),
	-- CONSTRAINT [Campaign_ForeignKey_CreatorId] FOREIGN KEY([CreatorId]) REFERENCES [dbo].[User]([Id]),
)
GO

ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
GO

ALTER TABLE [dbo].[Campaign] ADD CONSTRAINT [Campaign_ForeignKey_CreatorId] FOREIGN KEY([CreatorId]) REFERENCES [dbo].[User]([Id]);
GO

CREATE INDEX [CampaignIndex1] ON [dbo].[Campaign]([Id]) INCLUDE ([Status],[CampaignType],[PublishTimeUTC],[RevocationTimeUTC],[CouponCount]);
GO

-- during the campaign creation, user will choose venues to offer coupons
/*
CREATE TABLE [dbo].[CampaignRegionMap] (	
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[CampaignId] uniqueidentifier NOT NULL,
	[VenueId] uniqueidentifier NOT NULL,
	
	-- CONSTRAINT [CampaignRegionMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),	
	-- CONSTRAINT [CampaignRegionMap_ForeignKey_CampaignId] FOREIGN KEY([CampaignId]) REFERENCES [dbo].[Campaign]([Id]),
	-- CONSTRAINT [CampaignRegionMap_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]),	
)
GO

ALTER TABLE [dbo].[CampaignRegionMap] ADD CONSTRAINT [CampaignRegionMap_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[CampaignRegionMap] ADD CONSTRAINT [CampaignRegionMap_ForeignKey_CampaignId] FOREIGN KEY([CampaignId]) REFERENCES [dbo].[Campaign]([Id]);
GO

ALTER TABLE [dbo].[CampaignRegionMap] ADD CONSTRAINT [CampaignRegionMap_ForeignKey_VenueId] FOREIGN KEY([VenueId]) REFERENCES [dbo].[Venue]([Id]);
GO

CREATE INDEX [CampaignRegionMapIndex1] ON [dbo].[CampaignRegionMap]([VenueId]) INCLUDE ([GroupId],[CampaignId]);
GO
*/

CREATE TABLE [dbo].[Coupon] (
	[GroupId] uniqueidentifier NOT NULL, -- root level tenant id
	[Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[CampaignId] uniqueidentifier NULL,
	[SenderId] uniqueidentifier NOT NULL,	-- user id
	[ChainId] uniqueidentifier NOT NULL, -- on behalf of which restaurant (chain)?
	[ReceiverId] uniqueidentifier NOT NULL,		
	[CheckInId] uniqueidentifier NULL,	-- doesn't have to be upon the latest check-in... this could be a special offer as well	
	[CouponType] tinyint NOT NULL DEFAULT 0, -- Unspecified, Discount as a Percentage, Discount as a value, Free Item, Free Sub Item, Buy1Get1Free
	[FaceValue] decimal (9,6) NULL, -- when the type is percentage / value
	[ExpiryDateUTC] datetime NOT NULL,
	[Code] nvarchar(10) NULL,
	[Title] nvarchar(140) NOT NULL,
	[Content] nvarchar(2000) NULL,
	[QueueTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(), -- user clicked to send
	[PushTimeUTC] datetime NULL, -- our notification service pushed it
	[ReadTimeUTC] datetime NULL, -- diner read the message
	[RedeemCheckInId] uniqueidentifier NULL,
	[RedeemedValue] decimal (9,6) NULL,
	[RedeemTimeUTC] datetime NULL, -- diner redeemed the coupon
	[DeleteTimeUTC] datetime NULL, -- diner deletes the message
	
	-- CONSTRAINT [Coupon_PK] PRIMARY KEY CLUSTERED ([Id]) -- see alter table section
	-- CONSTRAINT [Coupon_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]),	
	-- xxx CONSTRAINT [Coupon_ForeignKey_CampaignId] FOREIGN KEY([CampaignId]) REFERENCES [dbo].[Campaign]([Id]),  -- not included
	-- CONSTRAINT [Coupon_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]),
	-- CONSTRAINT [Coupon_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]),
	-- CONSTRAINT [Coupon_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]),
	-- CONSTRAINT [Coupon_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]),
	-- CONSTRAINT [Coupon_ForeignKey_RedeemCheckInId] FOREIGN KEY([RedeemCheckInId]) REFERENCES [dbo].[CheckIn]([Id]),
)
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_PK] PRIMARY KEY CLUSTERED ([Id])
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_GroupId] FOREIGN KEY([GroupId]) REFERENCES [dbo].[Group]([Id]);
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_SenderId] FOREIGN KEY([SenderId]) REFERENCES [dbo].[User]([Id]);
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_ChainId] FOREIGN KEY([ChainId]) REFERENCES [dbo].[Chain]([Id]);
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_ReceiverId] FOREIGN KEY([ReceiverId]) REFERENCES [dbo].[Diner]([Id]);
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_CheckInId] FOREIGN KEY([CheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
GO

ALTER TABLE [dbo].[Coupon] ADD CONSTRAINT [Coupon_ForeignKey_RedeemCheckInId] FOREIGN KEY([RedeemCheckInId]) REFERENCES [dbo].[CheckIn]([Id]);
GO

CREATE INDEX [CouponIndex1] ON [dbo].[Coupon]([Id]) INCLUDE ([GroupId],[SenderId],[ChainId],[ReceiverId],[CheckInId]);
GO
