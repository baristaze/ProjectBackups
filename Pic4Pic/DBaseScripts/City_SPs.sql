
--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCountries')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCountries]');
END
GO

CREATE PROCEDURE [dbo].[GetCountries]
AS
	SELECT [Id], [Code], [Name], [AlternateNames] 
		FROM [dbo].[Country];
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRegionsByCountryId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRegionsByCountryId]');
END
GO

CREATE PROCEDURE [dbo].[GetRegionsByCountryId]
	@CountryId int
AS
	SELECT [Id], [CountryId], [Code], [Name], [AlternateNames] 
		FROM [dbo].[Region]
		WHERE [CountryId] = @CountryId;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetRegionsByCountryCode')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetRegionsByCountryCode]');
END
GO

CREATE PROCEDURE [dbo].[GetRegionsByCountryCode]
	 @CountryCode nvarchar(5)
AS
	SELECT [Id], [CountryId], [Code], [Name], [AlternateNames] 
		FROM [dbo].[Region]	
		WHERE [CountryId] = (SELECT TOP(1) [Id] FROM [dbo].[Country] WHERE [Code] = @CountryCode);
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSubRegionsByCountryId')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSubRegionsByCountryId]');	
END
GO

CREATE PROCEDURE [dbo].[GetSubRegionsByCountryId]
	@CountryId int
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[AlternateNames]
	  FROM [dbo].[SubRegion]
		WHERE [CountryId] = @CountryId
			ORDER BY [OrderIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSubRegionsByCountryCode')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSubRegionsByCountryCode]');	
END
GO

CREATE PROCEDURE [dbo].[GetSubRegionsByCountryCode]
	@CountryCode nvarchar(5)
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[AlternateNames]
	  FROM [dbo].[SubRegion]
		WHERE [CountryId] = (SELECT [Id] FROM [dbo].[Country] WHERE [Code] = @CountryCode)			
		ORDER BY [OrderIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSubRegionsByIDs')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSubRegionsByIDs]');	
END
GO

CREATE PROCEDURE [dbo].[GetSubRegionsByIDs]
	@CountryId int
   ,@RegionId int
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[AlternateNames]
	  FROM [dbo].[SubRegion]
		WHERE [CountryId] = @CountryId AND [RegionId] = @RegionId				
			ORDER BY [OrderIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetSubRegionsByCodes')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetSubRegionsByCodes]');	
END
GO

CREATE PROCEDURE [dbo].[GetSubRegionsByCodes]
	@CountryCode nvarchar(5)
   ,@RegionCode nvarchar(5)
AS
	declare @countrId int;
	set @countrId = (SELECT [Id] FROM [dbo].[Country] WHERE [Code] = @CountryCode);

	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[AlternateNames]
	  FROM [dbo].[SubRegion]
		WHERE [CountryId] = @countrId
			AND [RegionId] = (SELECT [Id] FROM [dbo].[Region] 
								WHERE [Code] = @RegionCode 
									AND [CountryId] = @countrId)
		ORDER BY [OrderIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCitiesByCountry')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCitiesByCountry]');	
END
GO

CREATE PROCEDURE [dbo].[GetCitiesByCountry]
	@CountryId int
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[SubRegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[WeightIndex]
		  ,[AlternateNames]
	  FROM [dbo].[City]
		WHERE [CountryId] = @CountryId
		ORDER BY [OrderIndex] DESC, [WeightIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCitiesByRegion')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCitiesByRegion]');	
END
GO

CREATE PROCEDURE [dbo].[GetCitiesByRegion]
	@CountryId int
   ,@RegionId int
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[SubRegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[WeightIndex]
		  ,[AlternateNames]
	  FROM [dbo].[City]
		WHERE [CountryId] = @CountryId AND [RegionId] = @RegionId
		ORDER BY [OrderIndex] DESC, [WeightIndex] DESC;
GO

--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------
IF EXISTS (SELECT [Name] FROM sys.objects WHERE [Type] = 'p' AND [Name] = 'GetCitiesBySubRegion')
BEGIN
	EXEC('DROP PROCEDURE [dbo].[GetCitiesBySubRegion]');	
END
GO

CREATE PROCEDURE [dbo].[GetCitiesBySubRegion]
	@CountryId int
   ,@RegionId int
   ,@SubRegionId int
AS
	SELECT [Id]
		  ,[CountryId]
		  ,[RegionId]
		  ,[SubRegionId]
		  ,[Name]
		  ,[OrderIndex]
		  ,[WeightIndex]
		  ,[AlternateNames]
	  FROM [dbo].[City]
		WHERE [CountryId] = @CountryId AND [RegionId] = @RegionId AND [SubRegionId] = @SubRegionId
		ORDER BY [OrderIndex] DESC, [WeightIndex] DESC;
GO