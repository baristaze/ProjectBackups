-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'SavePurchaseRecord')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[SavePurchaseRecord]');
END
GO

CREATE PROCEDURE [dbo].[SavePurchaseRecord]
	 @ExternalId varchar(100)
	,@UserId uniqueidentifier
	,@ItemIDsInOfferredPackage varchar(100)
	,@InternalOfferId int
	,@AppStoreId tinyint
	,@PurchaseTimeUTC datetime
	,@EarnedCredit int
	,@ExtraData1 nvarchar(max)
	,@ExtraData2 nvarchar(max)
AS
	INSERT INTO [dbo].[PurchaseRecord]
			   ([ExternalId]
			   ,[UserId]
			   ,[ItemIDsInOfferredPackage]
			   ,[InternalOfferId]
			   ,[AppStoreId]
			   ,[PurchaseTimeUTC]
			   ,[EarnedCredit]
			   ,[ExtraData1]
			   ,[ExtraData2])
		 VALUES
			   ( @ExternalId
				,@UserId
				,@ItemIDsInOfferredPackage
				,@InternalOfferId
				,@AppStoreId
				,@PurchaseTimeUTC
				,@EarnedCredit
				,@ExtraData1
				,@ExtraData2);

GO


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPurchaseRecordByExternalId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPurchaseRecordByExternalId]');
END
GO

CREATE PROCEDURE [dbo].[GetPurchaseRecordByExternalId]
	 @ExternalId varchar(100)
	,@UserId uniqueidentifier
AS
		SELECT [ExternalId]
			  ,[UserId]
			  ,[ItemIDsInOfferredPackage]
			  ,[InternalOfferId]
			  ,[AppStoreId]
			  ,[PurchaseTimeUTC]
			  ,[EarnedCredit]
			  ,[ExtraData1]
			  ,[ExtraData2]
			  ,[CreateTimeUTC]
		  FROM [dbo].[PurchaseRecord]
		  WHERE [ExternalId] = @ExternalId AND [UserId] = @UserId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetPurchaseRecordsByUserId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetPurchaseRecordsByUserId]');
END
GO

CREATE PROCEDURE [dbo].[GetPurchaseRecordsByUserId]
	 @UserId uniqueidentifier
AS
		SELECT [ExternalId]
			  ,[UserId]
			  ,[ItemIDsInOfferredPackage]
			  ,[InternalOfferId]
			  ,[AppStoreId]
			  ,[PurchaseTimeUTC]
			  ,[EarnedCredit]
			  ,[ExtraData1]
			  ,[ExtraData2]
			  ,[CreateTimeUTC]
		  FROM [dbo].[PurchaseRecord]
		  WHERE [UserId] = @UserId;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetAllPurchaseRecords')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetAllPurchaseRecords]');
END
GO

CREATE PROCEDURE [dbo].[GetAllPurchaseRecords]
	 @CutOffMinutes int
	,@MaxCount int
AS
	declare @cutOffTime datetime;
	set @cutOffTime = DATEADD(MINUTE, -1 * @CutOffMinutes, GETUTCDATE());

	SELECT TOP (@MaxCount) 
			   [ExternalId]
			  ,[UserId]
			  ,[ItemIDsInOfferredPackage]
			  ,[InternalOfferId]
			  ,[AppStoreId]
			  ,[PurchaseTimeUTC]
			  ,[EarnedCredit]
			  ,[ExtraData1]
			  ,[ExtraData2]
			  ,[CreateTimeUTC]
		  FROM [dbo].[PurchaseRecord]
		  WHERE [PurchaseTimeUTC] >= @cutOffTime
		  ORDER BY [PurchaseTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'MakeCreditAdjustment')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[MakeCreditAdjustment]');
END
GO

CREATE PROCEDURE [dbo].[MakeCreditAdjustment]
	 @UserId uniqueidentifier
	,@Credit int
	,@Reason tinyint
AS
	INSERT INTO [dbo].[CreditAdjustment]
			   ([UserId]
			   ,[Credit]
			   ,[Reason])
		 VALUES
			   (@UserId
			   ,@Credit
			   ,@Reason);

GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCreditAdjustments')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCreditAdjustments]');
END
GO

CREATE PROCEDURE [dbo].[GetCreditAdjustments]
	 @UserId uniqueidentifier
	,@MaxCount int
AS
	SELECT TOP (@MaxCount) 
		   [UserId]
		  ,[Credit]
		  ,[Reason]
		  ,[CreateTimeUTC]
	  FROM [dbo].[CreditAdjustment]
	  WHERE [UserId] = @UserId
	  ORDER BY [CreateTimeUTC] DESC;
GO

-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCurrentCredit')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCurrentCredit]');
END
GO

CREATE PROCEDURE [dbo].[GetCurrentCredit]
	 @UserId uniqueidentifier
AS
	declare @adjustments int;
	set @adjustments = (SELECT SUM([Credit]) FROM [dbo].[CreditAdjustment] WHERE [UserId] = @UserId);
	IF(@adjustments is NULL)
	BEGIN
		set @adjustments = 0;
	END

	declare @purchases int;
	set @purchases = (SELECT SUM([EarnedCredit]) FROM [dbo].[PurchaseRecord] WHERE [UserId] = @UserId);
	IF(@purchases is NULL)
	BEGIN
		set @purchases = 0;
	END

	SELECT (@adjustments + @purchases);
GO