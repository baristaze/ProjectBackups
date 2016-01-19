/************************************************************************************************************/
/************************************************************************************************************/
-------------------------------------------------------------------------------------------------------------
-- DROP TABLE [dbo].[PurchaseRecord]
CREATE TABLE [dbo].[PurchaseRecord]
(
	[ExternalId] varchar(100) NOT NULL PRIMARY KEY,	-- PurchaseReferenceToken
	[UserId] uniqueidentifier NOT NULL,
	[ItemIDsInOfferredPackage] varchar(100) NOT NULL,
	[InternalOfferId] int NOT NULL,
	[AppStoreId] tinyint NOT NULL,
	[PurchaseTimeUTC] datetime NOT NULL,
	[EarnedCredit] int NOT NULL,
	[ExtraData1] nvarchar(max) NULL,		-- PurchaseInstanceId
	[ExtraData2] nvarchar(max) NULL,		-- InternalPurchasePayLoad
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),
	
	CONSTRAINT [PurchaseRecord_FK_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [PurchaseRecordIndex1] ON [dbo].[PurchaseRecord]
CREATE INDEX [PurchaseRecordIndex1] ON [dbo].[PurchaseRecord]([UserId]);
GO


/************************************************************************************************************/
/************************************************************************************************************/
/*	[Reason]
	------------
	Unknown(0),
	Complimentary(1),
	VisitedApp(2),
	ReferredFriend(3),
	Refund(4),
	LikedBio(5),	
	LikedPhoto(6),	
	RequestedP4P(7),
	AcceptedP4P(8),
	PremiumMatch(9),		// -
	PaidMatch(10),			// - 
	CustomBioEdit(11),		// -
*/
-------------------------------------------------------------------------------------------------------------
-- DROP TABLE [dbo].[CreditAdjustment]
CREATE TABLE [dbo].[CreditAdjustment]
(
	[AutoId] bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[UserId] uniqueidentifier NOT NULL,
	[Credit] int NOT NULL,
	[Reason] tinyint NOT NULL DEFAULT 0,
	[CreateTimeUTC] datetime NOT NULL DEFAULT GETUTCDATE(),

	CONSTRAINT [CreditAdjustment_FK_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
)
GO

-- DROP INDEX [CreditAdjustmentIndex1] ON [dbo].[CreditAdjustment]
CREATE INDEX [CreditAdjustmentIndex1] ON [dbo].[CreditAdjustment]([UserId]) INCLUDE ([Credit]);
GO