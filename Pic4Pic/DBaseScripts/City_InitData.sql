

INSERT INTO [dbo].[Country] ([Id], [Code], [Name], [AlternateNames])
     VALUES (1, 'US', 'United States', 'USA, United States of America');
GO


-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO [dbo].[Region] ([CountryId], [Id], [Name], [Code], [AlternateNames]) VALUES
	(1, 0, '*', '*', NULL), 
	(1, 1, 'Alabama', 'AL', NULL), 
	(1, 2, 'Alaska', 'AK', NULL), 
	(1, 3, 'American Samoa', 'AS', NULL), ----------------------
	(1, 4, 'Arizona', 'AZ', NULL), 
	(1, 5, 'Arkansas', 'AR', NULL), 
	(1, 6, 'California', 'CA', NULL), 
	(1, 7, 'Colorado', 'CO', NULL), 
	(1, 8, 'Connecticut', 'CT', NULL), 
	(1, 9, 'Delaware', 'DE', NULL), 
	(1, 10, 'District of Columbia', 'DC', 'Washington DC, Washington D.C.'), ----------------------
	(1, 11, 'Federated States of Micronesia', 'FM', 'Micronesia, F. S. of Micronesia, F.S. of Micronesia, FS of Micronesia'), ----------------------
	(1, 12, 'Florida', 'FL', NULL), 
	(1, 13, 'Georgia', 'GA', NULL), 
	(1, 14, 'Guam', 'GU', NULL), ----------------------
	(1, 15, 'Hawaii', 'HI', NULL), 
	(1, 16, 'Idaho', 'ID', NULL), 
	(1, 17, 'Illinois', 'IL', NULL), 
	(1, 18, 'Indiana', 'IN', NULL), 
	(1, 19, 'Iowa', 'IA', NULL), 
	(1, 20, 'Kansas', 'KS', NULL), 
	(1, 21, 'Kentucky', 'KY', NULL), 
	(1, 22, 'Louisiana', 'LA', NULL), 
	(1, 23, 'Maine', 'ME', NULL), 
	(1, 24, 'Marshall Islands', 'MH', NULL),	----------------------
	(1, 25, 'Maryland', 'MD', NULL), 
	(1, 26, 'Massachusetts', 'MA', NULL), 
	(1, 27, 'Michigan', 'MI', NULL), 
	(1, 28, 'Minnesota', 'MN', NULL), 
	(1, 29, 'Mississippi', 'MS', NULL), 
	(1, 30, 'Missouri', 'MO', NULL), 
	(1, 31, 'Montana', 'MT', NULL), 
	(1, 32, 'Nebraska', 'NE', NULL), 
	(1, 33, 'Nevada', 'NV', NULL), 
	(1, 34, 'New Hampshire', 'NH', NULL), 
	(1, 35, 'New Jersey', 'NJ', NULL), 
	(1, 36, 'New Mexico', 'NM', NULL), 
	(1, 37, 'New York', 'NY', NULL), 
	(1, 38, 'North Carolina', 'NC', 'N. Carolina, N Carolina'), 
	(1, 39, 'North Dakota', 'ND', 'N. North, N North'), 
	(1, 40, 'Northern Mariana Islands', 'MP', 'Mariana Islands, N. Mariana Islands, N Mariana Islands'), ----------------------
	(1, 41, 'Ohio', 'OH', NULL), 
	(1, 42, 'Oklahoma', 'OK', NULL), 
	(1, 43, 'Oregon', 'OR', NULL), 
	(1, 44, 'Palau', 'PW', NULL), 
	(1, 45, 'Pennsylvania', 'PA', NULL), 
	(1, 46, 'Puerto Rico', 'PR', NULL),		----------------------
	(1, 47, 'Rhode Island', 'RI', NULL),	----------------------
	(1, 48, 'South Carolina', 'SC', 'S. Carolina, S Carolina'), 
	(1, 49, 'South Dakota', 'SD', 'S. Dakota, S Dakota'), 
	(1, 50, 'Tennessee', 'TN', NULL), 
	(1, 51, 'Texas', 'TX', NULL), 
	(1, 52, 'Utah', 'UT', NULL), 
	(1, 53, 'Vermont', 'VT', NULL), 
	(1, 54, 'Virgin Islands', 'VI', NULL),	----------------------
	(1, 55, 'Virginia', 'VA', NULL), 
	(1, 56, 'Washington', 'WA', NULL), 
	(1, 57, 'West Virginia', 'WV', 'W Virginia, W. Virginia'), 
	(1, 58, 'Wisconsin', 'WI', NULL), 
	(1, 59, 'Wyoming', 'WY', NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name], [OrderIndex], [AlternateNames]) VALUES
           (1, 1, 0, '*', 0, NULL),
           (1, 2, 0, '*', 0, NULL),
           (1, 3, 0, '*', 0, NULL),
           (1, 4, 0, '*', 0, NULL),
           (1, 5, 0, '*', 0, NULL),
           (1, 6, 0, '*', 0, NULL),
           (1, 7, 0, '*', 0, NULL),
           (1, 8, 0, '*', 0, NULL),
           (1, 9, 0, '*', 0, NULL),
           (1, 10, 0, '*', 0, NULL),
           (1, 11, 0, '*', 0, NULL),
           (1, 12, 0, '*', 0, NULL),
           (1, 13, 0, '*', 0, NULL),
           (1, 14, 0, '*', 0, NULL),
           (1, 15, 0, '*', 0, NULL),
           (1, 16, 0, '*', 0, NULL),
           (1, 17, 0, '*', 0, NULL),
           (1, 18, 0, '*', 0, NULL),
           (1, 19, 0, '*', 0, NULL),
           (1, 20, 0, '*', 0, NULL),
           (1, 21, 0, '*', 0, NULL),
           (1, 22, 0, '*', 0, NULL),
           (1, 23, 0, '*', 0, NULL),
           (1, 24, 0, '*', 0, NULL),
           (1, 25, 0, '*', 0, NULL),
           (1, 26, 0, '*', 0, NULL),
           (1, 27, 0, '*', 0, NULL),
           (1, 28, 0, '*', 0, NULL),
           (1, 29, 0, '*', 0, NULL),
           (1, 30, 0, '*', 0, NULL),
           (1, 31, 0, '*', 0, NULL),
           (1, 32, 0, '*', 0, NULL),
           (1, 33, 0, '*', 0, NULL),
           (1, 34, 0, '*', 0, NULL),
           (1, 35, 0, '*', 0, NULL),
           (1, 36, 0, '*', 0, NULL),
           (1, 37, 0, '*', 0, NULL),
           (1, 38, 0, '*', 0, NULL),
           (1, 39, 0, '*', 0, NULL),
           (1, 40, 0, '*', 0, NULL),
           (1, 41, 0, '*', 0, NULL),
           (1, 42, 0, '*', 0, NULL),
           (1, 43, 0, '*', 0, NULL),
           (1, 44, 0, '*', 0, NULL),
           (1, 45, 0, '*', 0, NULL),
           (1, 46, 0, '*', 0, NULL),
           (1, 47, 0, '*', 0, NULL),
           (1, 48, 0, '*', 0, NULL),
           (1, 49, 0, '*', 0, NULL),
           (1, 50, 0, '*', 0, NULL),
           (1, 51, 0, '*', 0, NULL),
           (1, 52, 0, '*', 0, NULL),
           (1, 53, 0, '*', 0, NULL),
           (1, 54, 0, '*', 0, NULL),
           (1, 55, 0, '*', 0, NULL),
           (1, 56, 0, '*', 0, NULL),
           (1, 57, 0, '*', 0, NULL),
           (1, 58, 0, '*', 0, NULL),
           (1, 59, 0, '*', 0, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-- California...

-----------------------------------------------------------------------------------------------------------------------------------------
-- san francisco county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 1, 'San Francisco County', 0, 'San Francisco Co, SFO County, SFO Co, Bay Area, SFO Bay Area, San Francisco Bay Area');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
     (1, 6, 1, 100, 'San Francisco', 0, 20, 'SFO');
GO
-----------------------------------------------------------------------------------------------------------------------------------------
-- Alameda county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 2, 'Alameda County', 0, 'Alameda Co, Bay Area, SFO Bay Area, San Francisco Bay Area');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
     (1, 6, 2, 200, 'Oakland', 0, 8, NULL),
     (1, 6, 2, 201, 'Fremont', 0, 4, NULL),
     (1, 6, 2, 202, 'Hayward', 0, 3, NULL),
     (1, 6, 2, 203, 'Berkeley', 0, 2, NULL),
     (1, 6, 2, 204, 'San Leandro', 0, 2, NULL),
     (1, 6, 2, 205, 'Livermore', 0, 2, NULL),
     (1, 6, 2, 206, 'Alameda', 0, 2, NULL),
     (1, 6, 2, 207, 'Pleasanton', 0, 1, NULL),
     (1, 6, 2, 208, 'Union City', 0, 1, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- San Mateo county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 3, 'San Mateo County', 0, 'San Mateo Co, Bay Area, SFO Bay Area, San Francisco Bay Area');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
     (1, 6, 3, 300, 'San Mateo', 0, 2, NULL),
     (1, 6, 3, 301, 'Daly City', 0, 2, NULL),
     (1, 6, 3, 302, 'Redwood City', 0, 2, NULL),
     (1, 6, 3, 303, 'South San Francisco', 0, 1, NULL),
     (1, 6, 3, 304, 'San Bruno', 0, 1, NULL);
GO
-----------------------------------------------------------------------------------------------------------------------------------------
-- Santa Clara county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 4, 'Santa Clara County', 0, 'Santa Clara Co, Bay Area, SFO Bay Area, San Francisco Bay Area');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
     (1, 6, 4, 401, 'San Jose', 0, 20, NULL),
     (1, 6, 4, 402, 'Sunnyvale', 0, 3, NULL),
     (1, 6, 4, 403, 'Santa Clara', 0, 2, NULL),
     (1, 6, 4, 404, 'Mountain View', 0, 1, NULL),
     (1, 6, 4, 405, 'Milpitas', 0, 1, NULL),
     (1, 6, 4, 406, 'Palo Alto', 0, 1, NULL),
     (1, 6, 4, 407, 'Cupertino', 0, 1, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- San Francisco Bay Area (Cumulative)
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 5, 'San Francisco Bay Area', 0, 'Bay Area, SFO Bay Area');
GO

UPDATE [dbo].[City] SET [SubRegionId] = 5 WHERE [CountryId] = 1 AND [RegionId] = 6 AND [SubRegionId] IN (1, 2, 3, 4);
-- UPDATE [dbo].[City] SET [SubRegionId] = ([Id]/100) WHERE [CountryId] = 1 AND [RegionId] = 6 AND [SubRegionId] = 5;
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- los angeles county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 6, 'Los Angeles County', 0, 'Los Angeles Co');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES 
	 (1, 6, 6, 600, 'Los Angeles', 0, 70, NULL),
	 (1, 6, 6, 601, 'Long Beach', 0, 10, NULL),
	 (1, 6, 6, 602, 'Glendale', 0, 4, NULL),
	 (1, 6, 6, 603, 'Santa Clarita', 0, 4, NULL),
	 (1, 6, 6, 604, 'Lancaster', 0, 3, NULL),
	 (1, 6, 6, 605, 'Palmdale', 0, 3, NULL),
	 (1, 6, 6, 606, 'Pomona', 0, 3, NULL),
	 (1, 6, 6, 607, 'Torrance', 0, 3, NULL),
	 (1, 6, 6, 608, 'Pasadena', 0, 3, NULL),
	 (1, 6, 6, 609, 'El Monte', 0, 2, NULL),
	 (1, 6, 6, 610, 'Downey', 0, 2, NULL),
	 (1, 6, 6, 611, 'Inglewood', 0, 2, NULL),
	 (1, 6, 6, 612, 'West Covina', 0, 2, NULL),
	 (1, 6, 6, 613, 'Norwalk', 0, 2, NULL),
	 (1, 6, 6, 614, 'Burbank', 0, 2, NULL),
	 (1, 6, 6, 615, 'Compton', 0, 2, NULL),
	 (1, 6, 6, 616, 'South Gate', 0, 2, NULL),
	 (1, 6, 6, 617, 'Carson', 0, 2, NULL),
	 (1, 6, 6, 618, 'Santa Monica', 0, 2, NULL),
	 (1, 6, 6, 619, 'Whittier', 0, 2, NULL),
	 (1, 6, 6, 620, 'Hawthorne', 0, 2, NULL),
	 (1, 6, 6, 621, 'Alhambra', 0, 2, NULL),
	 (1, 6, 6, 622, 'Lakewood', 0, 2, NULL),
	 (1, 6, 6, 623, 'Bellflower', 0, 2, NULL),
	 (1, 6, 6, 624, 'Baldwin Park', 0, 2, NULL),
	 (1, 6, 6, 625, 'Lynwood', 0, 1, NULL),
	 (1, 6, 6, 626, 'Redondo Beach', 0, 1, NULL),
	 (1, 6, 6, 627, 'Pico Rivera', 0, 1, NULL),
	 (1, 6, 6, 628, 'Montebello', 0, 1, NULL),
	 (1, 6, 6, 629, 'Monterey Park', 0, 1, NULL),
	 (1, 6, 6, 630, 'Gardena', 0, 1, NULL),
	 (1, 6, 6, 631, 'Huntington Park', 0, 1, NULL),
	 (1, 6, 6, 632, 'Arcadia', 0, 1, NULL),
	 (1, 6, 6, 633, 'Diamond Bar', 0, 1, NULL),
	 (1, 6, 6, 634, 'Paramount', 0, 1, NULL),
	 (1, 6, 6, 635, 'Rosemead', 0, 1, NULL),
	 (1, 6, 6, 636, 'Glendora', 0, 1, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- orange county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 7, 'Orange County', 0, 'OC, O.C., Orange Co');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES 
	 (1, 6, 7, 701, 'Anaheim', 0, 8, NULL),
	 (1, 6, 7, 702, 'Santa Ana', 0, 7, NULL),
	 (1, 6, 7, 703, 'Irvine', 0, 5, NULL),
	 (1, 6, 7, 704, 'Huntington Beach', 0, 4, NULL),
	 (1, 6, 7, 705, 'Garden Grove', 0, 4, NULL),
	 (1, 6, 7, 706, 'Fullerton', 0, 3, NULL),
	 (1, 6, 7, 707, 'Orange', 0, 3, NULL),
	 (1, 6, 7, 708, 'Costa Mesa', 0, 2, NULL),
	 (1, 6, 7, 709, 'Mission Viejo', 0, 1, NULL),
	 (1, 6, 7, 710, 'Westminster', 0, 1, NULL),
	 (1, 6, 7, 711, 'Newport Beach', 0, 1, NULL),
	 (1, 6, 7, 712, 'Buena Park', 0, 1, NULL),
	 (1, 6, 7, 713, 'Lake Forest', 0, 1, NULL),
	 (1, 6, 7, 714, 'Tustin', 0, 1, NULL),
	 (1, 6, 7, 715, 'Yorba Linda', 0, 1, NULL),
	 (1, 6, 7, 716, 'San Clemente', 0, 1, NULL),
	 (1, 6, 7, 717, 'Laguna Niguel', 0, 1, NULL),
	 (1, 6, 7, 718, 'La Habra', 0, 1, NULL),
	 (1, 6, 7, 719, 'Fountain Valley', 0, 1, NULL),
	 (1, 6, 7, 720, 'Placentia', 0, 1, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- san diego county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 8, 'San Diego County', 0, 'San Diego Co');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
     (1, 6, 8, 800, 'San Diego', 0, 26, NULL),
	 (1, 6, 8, 801, 'Chula Vista', 0, 5, NULL),
	 (1, 6, 8, 802, 'Oceanside', 0, 3, NULL),
	 (1, 6, 8, 803, 'Escondido', 0, 3, NULL),
	 (1, 6, 8, 804, 'Carlsbad', 0, 2, NULL),
	 (1, 6, 8, 805, 'El Cajon', 0, 2, NULL),
	 (1, 6, 8, 806, 'Vista', 0, 2, NULL),
	 (1, 6, 8, 807, 'San Marcos', 0, 2, NULL),
	 (1, 6, 8, 808, 'Encinitas', 0, 1, NULL),
	 (1, 6, 8, 809, 'National City', 0, 1, NULL),
	 (1, 6, 8, 810, 'La Mesa', 0, 1, NULL),
	 (1, 6, 8, 811, 'Santee', 0, 1, NULL);
GO

-----------------------------------------------------------------------------------------------------------------------------------------
-- sacramento county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 6, 9, 'Sacramento County', 0, 'Sacramento Co');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES 
	 (1, 6, 9, 900, 'Sacramento', 0, 10, NULL),
	 (1, 6, 9, 901, 'Elk Grove', 0, 3, NULL),
	 (1, 6, 9, 902, 'Citrus Heights', 0, 2, NULL),
	 (1, 6, 9, 903, 'Folsom', 0, 1, NULL),
	 (1, 6, 9, 904, 'Rancho Cordova', 0, 1, NULL);

GO

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Nevada state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 33, 0, 100, 'Las Vegas', 0, 25, NULL),
	(1, 33, 0, 101, 'Henderson', 0, 10, NULL),
	(1, 33, 0, 102, 'Reno', 0, 9, NULL),
	(1, 33, 0, 103, 'North Las Vegas', 0, 9, NULL),
	(1, 33, 0, 104, 'Sparks', 0, 4, NULL),
	(1, 33, 0, 105, 'Carson City', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-- washington

-----------------------------------------------------------------------------------------------------------------------------------------
-- King county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 1, 'King County', 0, 'King Co');
GO

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 1, 100, 'Seattle', 0, 26, NULL),
	 (1, 56, 1, 101, 'Bellevue', 0, 5, NULL),
	 (1, 56, 1, 102, 'Kent', 0, 5, NULL),
	 (1, 56, 1, 103, 'Renton', 0, 4, NULL),
	 (1, 56, 1, 104, 'Federal Way', 0, 4, NULL),
	 (1, 56, 1, 105, 'Kirkland', 0, 3, NULL),
	 (1, 56, 1, 106, 'Auburn', 0, 3, NULL),
	 (1, 56, 1, 107, 'Redmond', 0, 2, NULL),
	 (1, 56, 1, 108, 'Shoreline', 0, 2, NULL),
	 (1, 56, 1, 109, 'Sammamish', 0, 2, NULL),
	 (1, 56, 1, 110, 'Burien', 0, 2, NULL),
	 (1, 56, 1, 111, 'Bothell', 0, 1, NULL),
	 (1, 56, 1, 112, 'Issaquah', 0, 1, NULL);	 

-----------------------------------------------------------------------------------------------------------------------------------------
-- Snohomish county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 2, 'Snohomish County', 0, 'Snohomish Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 2, 201, 'Everett', 0, 4, NULL),
	 (1, 56, 2, 202, 'Marysville', 0, 2, NULL),
	 (1, 56, 2, 203, 'Edmonds', 0, 1, NULL),
	 (1, 56, 2, 204, 'Lynnwood', 0, 1, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-- Spokane county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 3, 'Spokane County', 0, 'Spokane Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 3, 300, 'Spokane', 0, 8, NULL),
	 (1, 56, 3, 301, 'Spokane Valley', 0, 4, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-- Pierce county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 4, 'Pierce County', 0, 'Pierce Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 4, 401, 'Tacoma', 0, 8, NULL),
	 (1, 56, 4, 402, 'Lakewood', 0, 2, NULL),
	 (1, 56, 4, 403, 'Puyallup', 0, 1, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-- Clark county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 5, 'Clark County', 0, 'Clark Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 5, 501, 'Vancouver', 0, 6, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-- Yakima county
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 56, 6, 'Yakima County', 0, 'Yakima Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 56, 6, 600, 'Yakima', 0, 4, NULL);


-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- georgia state

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 13, 0, 1, 'Atlanta', 0, 20, NULL),
	 (1, 13, 0, 2, 'Columbus', 0, 8, NULL),
	 (1, 13, 0, 3, 'Augusta', 0, 8, NULL),
	 (1, 13, 0, 4, 'Savannah', 0, 6, NULL),
	 (1, 13, 0, 5, 'Athens', 0, 5, NULL),
	 (1, 13, 0, 6, 'Sandy Springs', 0, 4, NULL),
	 (1, 13, 0, 7, 'Roswell', 0, 4, NULL),
	 (1, 13, 0, 8, 'Macon', 0, 4, NULL),
	 (1, 13, 0, 9, 'Johns Creek', 0, 3, NULL),
	 (1, 13, 0, 10, 'Albany', 0, 3, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- texas state

-----------------------------------------------------------------------------------------------------------------------------------------

-- Greater Houston area (Harris County, Fort Bend County, Montgomery County, Brazoria County, Galveston County)
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 51, 1, 'Greater Houston Area', 0, 'Greater Houston, Harris County, Fort Bend County, Montgomery County, Brazoria County, Galveston County, Harris Co, Fort Bend Co, Montgomery Co, Brazoria Co, Galveston Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 51, 1, 100, 'Houston', 0, 120, NULL),
	 (1, 51, 1, 101, 'Pasadena', 0, 6, NULL),
	 (1, 51, 1, 102, 'Pearland', 0, 4, NULL),
	 (1, 51, 1, 103, 'Baytown', 0, 3, NULL),
	 (1, 51, 1, 104, 'Conroe', 0, 2, NULL),
	 (1, 51, 1, 105, 'Deer Park', 0, 1, NULL),
	 (1, 51, 1, 106, 'Friendswood', 0, 1, NULL),
	 (1, 51, 1, 107, 'Galveston', 0, 2, NULL),
	 (1, 51, 1, 108, 'Lake Jackson', 0, 1, NULL),
	 (1, 51, 1, 109, 'La Porte', 0, 1, NULL),
	 (1, 51, 1, 110, 'League City', 0, 3, NULL),
	 (1, 51, 1, 111, 'Missouri City', 0, 2, NULL),
	 (1, 51, 1, 112, 'Rosenberg', 0, 1, NULL),
	 (1, 51, 1, 113, 'Sugar Land', 0, 3, NULL),
	 (1, 51, 1, 114, 'Texas City', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------

-- Greater Austin area
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 51, 2, 'Greater Austin Area', 0, 'Greater Austin');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 51, 2, 200, 'Austin', 0, 32, NULL),
	 (1, 51, 2, 201, 'Round Rock', 0, 4, NULL),
	 (1, 51, 2, 202, 'New Braunfels', 0, 2, NULL),
	 (1, 51, 2, 203, 'Cedar Park', 0, 2, NULL),
	 (1, 51, 2, 204, 'Georgetown', 0, 2, NULL),
	 (1, 51, 2, 205, 'Pflugerville', 0, 2, NULL),
	 (1, 51, 2, 206, 'San Marcos', 0, 2, NULL),
	 (1, 51, 2, 207, 'Kyle', 0, 1, NULL),
	 (1, 51, 2, 208, 'Leander', 0, 1, NULL);


-----------------------------------------------------------------------------------------------------------------------------------------

-- Greater Dallas area (Dallas County, Fort Worth county, Tarrant County)
INSERT INTO [dbo].[SubRegion]([CountryId], [RegionId], [Id], [Name],[OrderIndex], [AlternateNames]) VALUES
           (1, 51, 3, 'Greater Dallas Area', 0, 'Greater Dallas, Dallas County, Fort Worth county, Tarrant County, Dallas Co, Fort Worth Co, Tarrant Co');

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	 (1, 51, 3, 300, 'Dallas', 0, 50, NULL),
	 (1, 51, 3, 301, 'Garland', 0, 9, NULL),
	 (1, 51, 3, 302, 'Plano', 0, 11, NULL),
	 (1, 51, 3, 303, 'Arlington', 0, 15, NULL),
	 (1, 51, 3, 304, 'Fort Worth', 0, 32, NULL),
	 (1, 51, 3, 305, 'University Park', 0, 1, NULL),
	 (1, 51, 3, 306, 'Irving', 0, 9, NULL),
	 (1, 51, 3, 307, 'Balch Springs', 0, 1, NULL),
	 (1, 51, 3, 308, 'Duncanville', 0, 1, NULL),
	 (1, 51, 3, 309, 'Farmers Branch', 0, 1, NULL),
	 (1, 51, 3, 310, 'Mesquite', 0, 6, NULL),
	 (1, 51, 3, 311, 'Grand Prairie', 0, 7, NULL),
	 (1, 51, 3, 312, 'Richardson', 0, 4, NULL),
	 (1, 51, 3, 313, 'Carrollton', 0, 5, NULL),
	 (1, 51, 3, 314, 'Lancaster', 0, 1, NULL),
	 (1, 51, 3, 315, 'DeSoto', 0, 2, NULL),
	 (1, 51, 3, 316, 'Rowlett', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Massachusetts state

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 26, 0, 100, 'Boston', 0, 25, NULL),
	(1, 26, 0, 101, 'Worcester', 0, 7, NULL),
	(1, 26, 0, 102, 'Springfield', 0, 6, NULL),
	(1, 26, 0, 103, 'Lowell', 0, 4, NULL),
	(1, 26, 0, 104, 'Cambridge', 0, 4, NULL),
	(1, 26, 0, 105, 'New Bedford', 0, 4, NULL),
	(1, 26, 0, 106, 'Brockton', 0, 4, NULL),
	(1, 26, 0, 107, 'Quincy', 0, 4, NULL),
	(1, 26, 0, 108, 'Lynn', 0, 4, NULL),
	(1, 26, 0, 109, 'Fall River', 0, 4, NULL),
	(1, 26, 0, 110, 'Newton', 0, 3, NULL),
	(1, 26, 0, 111, 'Lawrence', 0, 3, NULL),
	(1, 26, 0, 112, 'Somerville', 0, 3, NULL),
	(1, 26, 0, 113, 'Framingham', 0, 2, NULL),
	(1, 26, 0, 114, 'Haverhill', 0, 2, NULL),
	(1, 26, 0, 115, 'Waltham', 0, 2, NULL),
	(1, 26, 0, 116, 'Malden', 0, 2, NULL),
	(1, 26, 0, 117, 'Brookline', 0, 2, NULL),
	(1, 26, 0, 118, 'Plymouth', 0, 2, NULL),
	(1, 26, 0, 119, 'Medford', 0, 2, NULL),
	(1, 26, 0, 120, 'Taunton', 0, 2, NULL),
	(1, 26, 0, 121, 'Chicopee', 0, 2, NULL),
	(1, 26, 0, 122, 'Weymouth', 0, 2, NULL),
	(1, 26, 0, 123, 'Revere', 0, 2, NULL),
	(1, 26, 0, 124, 'Peabody', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Illinois state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 17, 0, 100 , 'Chicago', 0, 108 , NULL),
	(1, 17, 0, 101 , 'Aurora', 0, 8 , NULL),
	(1, 17, 0, 102 , 'Rockford', 0, 6 , NULL),
	(1, 17, 0, 103 , 'Joliet', 0, 6 , NULL),
	(1, 17, 0, 104 , 'Naperville', 0, 6 , NULL),
	(1, 17, 0, 105 , 'Springfield', 0, 4 , NULL),
	(1, 17, 0, 106 , 'Peoria', 0, 4 , NULL),
	(1, 17, 0, 107 , 'Elgin', 0, 4 , NULL),
	(1, 17, 0, 108 , 'Waukegan', 0, 3 , NULL),
	(1, 17, 0, 109 , 'Champaign', 0, 3 , NULL),
	(1, 17, 0, 110 , 'Bloomington', 0, 3 , NULL),
	(1, 17, 0, 111 , 'Decatur', 0, 3 , NULL),
	(1, 17, 0, 112 , 'Evanston', 0, 3 , NULL),
	(1, 17, 0, 113 , 'Des Plaines', 0, 2 , NULL),
	(1, 17, 0, 114 , 'Berwyn', 0, 2 , NULL),
	(1, 17, 0, 115 , 'Wheaton', 0, 2 , NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Colorado state

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 7, 0, 100, 'Denver', 0, 24 , NULL),
	(1, 7, 0, 101, 'Colorado Springs', 0, 17 , NULL),
	(1, 7, 0, 102, 'Aurora', 0, 13 , NULL),
	(1, 7, 0, 103, 'Fort Collins', 0, 6 , NULL),
	(1, 7, 0, 104, 'Lakewood', 0, 6 , NULL),
	(1, 7, 0, 105, 'Thornton', 0, 4 , NULL),
	(1, 7, 0, 106, 'Pueblo', 0, 4 , NULL),
	(1, 7, 0, 107, 'Arvada', 0, 4 , NULL),
	(1, 7, 0, 108, 'Westminster', 0, 4 , NULL),
	(1, 7, 0, 109, 'Centennial', 0, 4 , NULL),
	(1, 7, 0, 110, 'Boulder', 0, 4 , NULL),
	(1, 7, 0, 111, 'Greeley', 0, 4 , NULL),
	(1, 7, 0, 112, 'Longmont', 0, 3 , NULL),
	(1, 7, 0, 113, 'Loveland', 0, 2 , NULL),
	(1, 7, 0, 114, 'Grand Junction', 0, 2 , NULL),
	(1, 7, 0, 115, 'Broomfield', 0, 2 , NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Michigan state

INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 27, 0, 100, 'Detroit', 0, 30, NULL),
	(1, 27, 0, 101, 'Grand Rapids', 0, 7, NULL),
	(1, 27, 0, 102, 'Warren', 0, 5, NULL),
	(1, 27, 0, 103, 'Sterling Heights', 0, 5, NULL),
	(1, 27, 0, 104, 'Lansing', 0, 4, NULL),
	(1, 27, 0, 105, 'Ann Arbor', 0, 4, NULL),
	(1, 27, 0, 106, 'Flint', 0, 4, NULL),
	(1, 27, 0, 107, 'Dearborn', 0, 4, NULL),
	(1, 27, 0, 108, 'Livonia', 0, 4, NULL),
	(1, 27, 0, 109, 'Westland', 0, 3, NULL),
	(1, 27, 0, 110, 'Troy', 0, 3, NULL),
	(1, 27, 0, 111, 'Farmington Hills', 0, 3, NULL),
	(1, 27, 0, 112, 'Kalamazoo', 0, 3, NULL),
	(1, 27, 0, 113, 'Wyoming', 0, 3, NULL),
	(1, 27, 0, 114, 'Southfield', 0, 3, NULL),
	(1, 27, 0, 115, 'Rochester Hills', 0, 3, NULL),
	(1, 27, 0, 116, 'Taylor', 0, 2, NULL),
	(1, 27, 0, 117, 'St. Clair Shores', 0, 2, 'Saint Clair Shores'),
	(1, 27, 0, 118, 'Pontiac', 0, 2, NULL),
	(1, 27, 0, 119, 'Dearborn Heights', 0, 2, NULL),
	(1, 27, 0, 120, 'Royal Oak', 0, 2, NULL),
	(1, 27, 0, 121, 'Novi', 0, 2, NULL),
	(1, 27, 0, 122, 'Battle Creek', 0, 2, NULL),
	(1, 27, 0, 123, 'Saginaw', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-- Florida state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 12, 0, 100, 'Jacksonville', 0, 33, NULL),
	(1, 12, 0, 101, 'Miami', 0, 16, NULL),
	(1, 12, 0, 102, 'Tampa', 0, 13, NULL),
	(1, 12, 0, 103, 'St. Petersburg', 0, 10, 'Saint Petersburg'),
	(1, 12, 0, 104, 'Orlando', 0, 10, NULL),
	(1, 12, 0, 105, 'Hialeah', 0, 9, NULL),
	(1, 12, 0, 106, 'Tallahassee', 0, 7, NULL),
	(1, 12, 0, 107, 'Fort Lauderdale', 0, 6, NULL),
	(1, 12, 0, 108, 'Port St. Lucie', 0, 6, 'Port Saint Lucie'),
	(1, 12, 0, 109, 'Pembroke Pines', 0, 6, NULL),
	(1, 12, 0, 110, 'Cape Coral', 0, 6, NULL),
	(1, 12, 0, 111, 'Hollywood', 0, 6, NULL),
	(1, 12, 0, 112, 'Gainesville', 0, 5, NULL),
	(1, 12, 0, 113, 'Miramar', 0, 5, NULL),
	(1, 12, 0, 114, 'Coral Springs', 0, 5, NULL),
	(1, 12, 0, 115, 'Clearwater', 0, 4, NULL),
	(1, 12, 0, 116, 'Miami Gardens', 0, 4, NULL),
	(1, 12, 0, 117, 'Palm Bay', 0, 4, NULL),
	(1, 12, 0, 118, 'West Palm Beach', 0, 4, NULL),
	(1, 12, 0, 119, 'Pompano Beach', 0, 4, NULL),
	(1, 12, 0, 120, 'Lakeland', 0, 4, NULL),
	(1, 12, 0, 121, 'Miami Beach', 0, 4, NULL),
	(1, 12, 0, 122, 'Deltona', 0, 4, NULL),
	(1, 12, 0, 123, 'Plantation', 0, 3, NULL),
	(1, 12, 0, 124, 'Sunrise', 0, 3, NULL),
	(1, 12, 0, 125, 'Boca Raton', 0, 3, NULL),
	(1, 12, 0, 126, 'Largo', 0, 3, NULL),
	(1, 12, 0, 127, 'Melbourne', 0, 3, NULL),
	(1, 12, 0, 128, 'Palm Coast', 0, 3, NULL),
	(1, 12, 0, 129, 'Deerfield Beach', 0, 3, NULL),
	(1, 12, 0, 130, 'Boynton Beach', 0, 2, NULL),
	(1, 12, 0, 131, 'Lauderhill', 0, 2, NULL),
	(1, 12, 0, 132, 'Weston', 0, 2, NULL),
	(1, 12, 0, 133, 'Fort Myers', 0, 2, NULL),
	(1, 12, 0, 134, 'Daytona Beach', 0, 2, NULL),
	(1, 12, 0, 135, 'Delray Beach', 0, 2, NULL),
	(1, 12, 0, 136, 'Homestead', 0, 2, NULL),
	(1, 12, 0, 137, 'Tamarac', 0, 2, NULL),
	(1, 12, 0, 138, 'Kissimmee', 0, 2, NULL),
	(1, 12, 0, 139, 'North Miami', 0, 2, NULL),
	(1, 12, 0, 140, 'North Port', 0, 2, NULL),
	(1, 12, 0, 141, 'Ocala', 0, 2, NULL),
	(1, 12, 0, 142, 'Port Orange', 0, 2, NULL),
	(1, 12, 0, 143, 'Sanford', 0, 2, NULL),
	(1, 12, 0, 144, 'Margate', 0, 2, NULL),
	(1, 12, 0, 145, 'Coconut Creek', 0, 2, NULL),
	(1, 12, 0, 146, 'Pensacola', 0, 2, NULL),
	(1, 12, 0, 147, 'Sarasota', 0, 2, NULL);



-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Minnesota state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 28, 0, 100, 'Minneapolis', 0, 16, NULL),
	(1, 28, 0, 101, 'St. Paul', 0, 12, NULL),
	(1, 28, 0, 102, 'Rochester', 0, 4, NULL),
	(1, 28, 0, 103, 'Duluth', 0, 4, NULL),
	(1, 28, 0, 104, 'Bloomington', 0, 3, NULL),
	(1, 28, 0, 105, 'Brooklyn Park', 0, 3, NULL),
	(1, 28, 0, 106, 'Plymouth', 0, 3, NULL),
	(1, 28, 0, 107, 'Eagan', 0, 2, NULL),
	(1, 28, 0, 108, 'Woodbury', 0, 2, NULL),
	(1, 28, 0, 109, 'Maple Grove', 0, 2, NULL),
	(1, 28, 0, 110, 'Coon Rapids', 0, 2, NULL),
	(1, 28, 0, 111, 'Eden Prairie', 0, 2, NULL),
	(1, 28, 0, 112, 'Burnsville', 0, 2, NULL),
	(1, 28, 0, 113, 'Lakeville', 0, 2, NULL),
	(1, 28, 0, 114, 'Minnetonka', 0, 2, NULL),
	(1, 28, 0, 115, 'Apple Valley', 0, 2, NULL),
	(1, 28, 0, 116, 'Edina', 0, 2, NULL),
	(1, 28, 0, 117, 'St. Louis Park', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Pennsylvania state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 45, 0, 100, 'Philadelphia', 0, 61, NULL),
	(1, 45, 0, 101, 'Pittsburgh', 0, 12, NULL),
	(1, 45, 0, 102, 'Allentown', 0, 4, NULL),
	(1, 45, 0, 103, 'Erie', 0, 4, NULL),
	(1, 45, 0, 104, 'Reading', 0, 3, NULL),
	(1, 45, 0, 105, 'Scranton', 0, 3, NULL),
	(1, 45, 0, 106, 'Bethlehem', 0, 3, NULL),
	(1, 45, 0, 107, 'Lancaster', 0, 2, NULL),
	(1, 45, 0, 108, 'Harrisburg', 0, 2, NULL),
	(1, 45, 0, 109, 'Altoona', 0, 2, NULL),
	(1, 45, 0, 110, 'York', 0, 2, NULL),
	(1, 45, 0, 111, 'State College', 0, 2, NULL),
	(1, 45, 0, 112, 'Wilkes-Barre', 0, 2, NULL),
	(1, 45, 0, 113, 'Chester', 0, 1, NULL),
	(1, 45, 0, 114, 'Williamsport', 0, 1, NULL),
	(1, 45, 0, 115, 'Easton', 0, 1, NULL),
	(1, 45, 0, 116, 'Lebanon', 0, 1, NULL),
	(1, 45, 0, 117, 'Hazleton', 0, 1, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Arizona state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 4, 0, 100, 'Phoenix', 0, 58, NULL),
	(1, 4, 0, 101, 'Tucson', 0, 21, NULL),
	(1, 4, 0, 102, 'Mesa', 0, 18, NULL),
	(1, 4, 0, 103, 'Chandler', 0, 10, NULL),
	(1, 4, 0, 104, 'Glendale', 0, 9, NULL),
	(1, 4, 0, 105, 'Scottsdale', 0, 9, NULL),
	(1, 4, 0, 106, 'Gilbert', 0, 8, NULL),
	(1, 4, 0, 107, 'Tempe', 0, 6, NULL),
	(1, 4, 0, 108, 'Peoria', 0, 6, NULL),
	(1, 4, 0, 109, 'Surprise', 0, 4, NULL),
	(1, 4, 0, 110, 'Yuma', 0, 4, NULL),
	(1, 4, 0, 111, 'Avondale', 0, 3, NULL),
	(1, 4, 0, 112, 'Flagstaff', 0, 2, NULL),
	(1, 4, 0, 113, 'Goodyear', 0, 2, NULL),
	(1, 4, 0, 114, 'Lake Havasu City', 0, 2, NULL),
	(1, 4, 0, 115, 'Buckeye', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Oregon state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 43, 0, 100, 'Portland', 0, 24, NULL),
	(1, 43, 0, 101, 'Salem', 0, 6, NULL),
	(1, 43, 0, 102, 'Eugene', 0, 6, NULL),
	(1, 43, 0, 103, 'Gresham', 0, 4, NULL),
	(1, 43, 0, 104, 'Hillsboro', 0, 4, NULL),
	(1, 43, 0, 105, 'Beaverton', 0, 4, NULL),
	(1, 43, 0, 106, 'Bend', 0, 3, NULL),
	(1, 43, 0, 107, 'Medford', 0, 3, NULL),
	(1, 43, 0, 108, 'Springfield', 0, 2, NULL),
	(1, 43, 0, 109, 'Corvallis', 0, 2, NULL),
	(1, 43, 0, 110, 'Albany', 0, 2, NULL),
	(1, 43, 0, 111, 'Tigard', 0, 2, NULL),
	(1, 43, 0, 112, 'Lake Oswego', 0, 1, NULL),
	(1, 43, 0, 113, 'Keizer', 0, 1, NULL),
	(1, 43, 0, 114, 'Grants Pass', 0, 1, NULL),
	(1, 43, 0, 115, 'Oregon City', 0, 1, NULL),
	(1, 43, 0, 116, 'McMinnville', 0, 1, NULL),
	(1, 43, 0, 117, 'Redmond', 0, 1, NULL),
	(1, 43, 0, 118, 'Tualatin', 0, 1, NULL),
	(1, 43, 0, 119, 'West Linn', 0, 1, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- North Carolina state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 38, 0, 100, 'Charlotte', 0, 29, NULL),
	(1, 38, 0, 101, 'Raleigh', 0, 16, NULL),
	(1, 38, 0, 102, 'Greensboro', 0, 10, NULL),
	(1, 38, 0, 103, 'Winston-Salem', 0, 9, 'Winston Salem, WinstonSalem'),
	(1, 38, 0, 104, 'Durham', 0, 9, NULL),
	(1, 38, 0, 105, 'Fayetteville', 0, 8, NULL),
	(1, 38, 0, 106, 'Cary', 0, 5, NULL),
	(1, 38, 0, 107, 'Wilmington', 0, 4, NULL),
	(1, 38, 0, 108, 'High Point', 0, 4, NULL),
	(1, 38, 0, 109, 'Greenville', 0, 3, NULL),
	(1, 38, 0, 110, 'Asheville', 0, 3, NULL),
	(1, 38, 0, 111, 'Concord', 0, 3, NULL),
	(1, 38, 0, 112, 'Gastonia', 0, 3, NULL),
	(1, 38, 0, 113, 'Jacksonville', 0, 3, NULL),
	(1, 38, 0, 114, 'Rocky Mount', 0, 2, 'Rocky Mt, Rocky Mt.'),
	(1, 38, 0, 115, 'Chapel Hill', 0, 2, NULL),
	(1, 38, 0, 116, 'Burlington', 0, 2, NULL),
	(1, 38, 0, 117, 'Wilson', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- New York state
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 37, 0, 100, 'New York City', 0, 330, 'New York, NY City, N.Y. City'),
	(1, 37, 0, 101, 'Buffalo', 0, 10, NULL),
	(1, 37, 0, 102, 'Rochester', 0, 8, NULL),
	(1, 37, 0, 103, 'Yonkers', 0, 8, NULL),
	(1, 37, 0, 104, 'Syracuse', 0, 6, NULL),
	(1, 37, 0, 105, 'Albany', 0, 4, NULL),
	(1, 37, 0, 106, 'New Rochelle', 0, 3, NULL),
	(1, 37, 0, 107, 'Mount Vernon', 0, 2,  'Mt Vernon, Mt. Vernon'),
	(1, 37, 0, 108, 'Schenectady', 0, 2, NULL),
	(1, 37, 0, 109, 'Utica', 0, 2, NULL),
	(1, 37, 0, 110, 'White Plains', 0, 2, NULL),
	(1, 37, 0, 111, 'Troy', 0, 2, NULL),
	(1, 37, 0, 112, 'Niagara Falls', 0, 2, NULL);

-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------

-- Washington DC
INSERT INTO [dbo].[City] ([CountryId], [RegionId], [SubRegionId], [Id], [Name], [OrderIndex], [WeightIndex], [AlternateNames]) VALUES
	(1, 10, 0, 100, 'Washington DC', 0, 25, 'Washington D.C.');

