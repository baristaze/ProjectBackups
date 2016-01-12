USE [YummyZone];
GO

INSERT INTO [dbo].[Group] ([Id], [Name])
     VALUES ('00000000-0000-0000-0000-000000000001', N'Olive Garden, Inc.');

INSERT INTO [dbo].[Group] ([Id], [Name])
     VALUES ('00000000-0000-0000-0000-000000000002', N'California Pizza Kitchen, Inc.');

INSERT INTO [dbo].[Chain] ([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', N'Olive Garden');
     
INSERT INTO [dbo].[Chain] ([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000022', N'California Pizza Kitchen');

INSERT INTO [dbo].[Venue]([GroupId], [ChainId], [Id], [Name])
     VALUES ('00000000-0000-0000-0000-000000000001', 
			 '00000000-0000-0000-0000-000000000011',
			 '00000000-0000-0000-0000-000000000111',
			 N'Olive Garden - Kirkland');
			 
INSERT INTO [dbo].[Venue]([GroupId], [ChainId], [Id], [Name])
     VALUES ('00000000-0000-0000-0000-000000000002', 
			 '00000000-0000-0000-0000-000000000022',
			 '00000000-0000-0000-0000-000000000222',
			 N'California Pizza Kitchen - Bellevue');

INSERT INTO [dbo].[Address] ([ObjectType],[ObjectId],[AddressType],[AddressLine1],[AddressLine2],[City],[State],[ZipCode])
	VALUES (1, '00000000-0000-0000-0000-000000000111', 1, '11325 NE 124th Street', NULL, 'Kirkland', 'WA', '98034');

INSERT INTO [dbo].[Address] ([ObjectType],[ObjectId],[AddressType],[AddressLine1],[AddressLine2],[City],[State],[ZipCode])
	VALUES (1, '00000000-0000-0000-0000-000000000222', 1, '595 106th Ave NE', NULL, 'Bellevue', 'WA', '98004-5006');

INSERT INTO [dbo].[User] ([GroupId],[Id],[Status],[EmailAddress],[FirstName],[LastName])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001',0,'test@live.com', 'John', 'White');

INSERT INTO [dbo].[Password]([UserId],[Password])VALUES('00000000-0000-0000-0000-000000000001','test12');

INSERT INTO [dbo].[Diner] ([Id], [UserName]) VALUES('00000000-0000-0000-0000-000000000001', 'A0000000-0000-0000-0000-000000000001');
INSERT INTO [dbo].[Diner] ([Id], [UserName]) VALUES('00000000-0000-0000-0000-000000000002', 'A0000000-0000-0000-0000-000000000002');
INSERT INTO [dbo].[Diner] ([Id], [UserName]) VALUES('00000000-0000-0000-0000-000000000003', 'A0000000-0000-0000-0000-000000000003');
INSERT INTO [dbo].[Diner] ([Id], [UserName]) VALUES('00000000-0000-0000-0000-000000000004', 'A0000000-0000-0000-0000-000000000004');
INSERT INTO [dbo].[Diner] ([Id], [UserName]) VALUES('00000000-0000-0000-0000-000000000005', 'A0000000-0000-0000-0000-000000000005');

INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES ('A0000000-0000-0000-0000-000000000001', 'tempPswd1');
INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES ('A0000000-0000-0000-0000-000000000002', 'tempPswd2');
INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES ('A0000000-0000-0000-0000-000000000003', 'tempPswd3');
INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES ('A0000000-0000-0000-0000-000000000004', 'tempPswd4');
INSERT INTO [dbo].[DinerPassword] ([UserName], [Password]) VALUES ('A0000000-0000-0000-0000-000000000005', 'tempPswd5');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000222');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000111');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000004','00000000-0000-0000-0000-000000000222');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000005','00000000-0000-0000-0000-000000000111');

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000006','00000000-0000-0000-0000-000000000111');

-- foo checkins 1
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');
INSERT INTO [dbo].[CheckIn] ([DinerId], [VenueId]) VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000111');

INSERT INTO [dbo].[Menu] ([GroupId], [Id], [ServiceStartTime], [ServiceEndTime], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', 0, 1560, N'Dinner');     
INSERT INTO [dbo].[Menu] ([GroupId], [Id], [ServiceStartTime], [ServiceEndTime], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', 0, 990, N'Lunch');
INSERT INTO [dbo].[Menu] ([GroupId], [Id], [ServiceStartTime], [ServiceEndTime], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000003333', 0, 1410, N'Drinks');
INSERT INTO [dbo].[Menu] ([GroupId], [Id], [ServiceStartTime], [ServiceEndTime], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000004444', 0, 1410, N'Desserts');

INSERT INTO [dbo].[VenueAndMenuMap]([GroupId], [VenueId], [MenuId], [OrderIndex])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000111', '00000000-0000-0000-0000-000000001111', 1);     
INSERT INTO [dbo].[VenueAndMenuMap]([GroupId], [VenueId], [MenuId], [OrderIndex])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000111', '00000000-0000-0000-0000-000000002222', 2);     
INSERT INTO [dbo].[VenueAndMenuMap]([GroupId], [VenueId], [MenuId], [OrderIndex])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000111', '00000000-0000-0000-0000-000000003333', 3);     
INSERT INTO [dbo].[VenueAndMenuMap]([GroupId], [VenueId], [MenuId], [OrderIndex])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000111', '00000000-0000-0000-0000-000000004444', 4);

INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000011111', N'Appetizers');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000022222', N'Soups & Salads');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000033333', N'Pizzas');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000044444', N'Classic Recipies');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000055555', N'Filled Pastas');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000066666', N'Chicken');
INSERT INTO [dbo].[MenuCategory]([GroupId], [Id], [Name])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000077777', N'Fish & Seafood');

INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', '00000000-0000-0000-0000-000000011111', 1);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', '00000000-0000-0000-0000-000000022222', 2);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', '00000000-0000-0000-0000-000000044444', 3);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', '00000000-0000-0000-0000-000000055555', 4);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000001111', '00000000-0000-0000-0000-000000066666', 5);

INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000011111', 1);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000022222', 2);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000044444', 3);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000033333', 4);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000055555', 5);
INSERT INTO [dbo].[MenuAndMenuCategoryMap] ([GroupId],[MenuId],[MenuCategoryId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000002222', '00000000-0000-0000-0000-000000077777', 6);

INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0001-000000000001', N'Sampler Italiano', NULL,
     N'Choose from: calamari, stuffed mushrooms, fried zucchini, chicken fingers, fried mozzarella or toasted beef and pork ravioli.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0001-000000000002', N'Bruschetta', 7.50,
     N'A traditional topping of roma tomatoes, fresh basil and extra-virgin olive oil. Served with toasted ciabatta bread.');     
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0002-000000000001', N'Chicken & Gnocchi', 5.65,
     N'A creamy soup made with roasted chicken, traditional Italian dumplings and spinach.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0002-000000000002', N'Grilled Chicken Caesar Salad', 11.50,
     N'Grilled chicken over romaine in a creamy Caesar dressing topped with imported parmesan cheese and croutons.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0003-000000000001', N'Create Your Own Pizza', 13.25,
     N'Choose up to four toppings. Pepperoni, Italian sausage, mushrooms, onions, bell peppers, black olives or roma tomatoes.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0003-000000000002', N'Chicken Alfredo Pizza', 13.25, 
     N'Pizza topped with grilled chicken, Italian cheeses, alfredo sauce and scallions.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0004-000000000001', N'Lasagna Classico', 14.95,
     N'Layers of pasta, meat sauce and mozzarella, ricotta, parmesan and romano cheese.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0004-000000000002', N'Fettuccine Alfredo', 13.95, 
     N'Parmesan cream sauce with a hint of garlic, served over fettuccine.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0005-000000000001', N'Braised Beef & Tortelloni', 16.25,
     N'Tender sliced short ribs and portobello mushrooms tossed with asiago-filled tortelloni in a basil-marsala sauce.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0005-000000000002', N'Ravioli di Portobello', 13.95,
     N'Portobello mushroom-filled ravioli in a creamy smoked cheese and sun-dried tomato sauce.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0006-000000000001', N'Venetian Apricot Chicken', 15.75, 
     N'Grilled chicken breasts in an apricot citrus sauce. Served with broccoli, asparagus and diced tomatoes.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0006-000000000002', N'Chicken Scampi', 16.25,
     N'Chicken breast tenderloins sautéed with bell peppers, roasted garlic and onions in a garlic cream sauce over angel hair.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0007-000000000001', N'Shrimp Primavera', 16.75,
     N'Shrimp, bell peppers, onions and mushrooms in a bold arrabbiata sauce over penne.');
INSERT INTO [dbo].[MenuItem]([GroupId],[Id],[Name],[Price],[Description])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0007-000000000002', N'Capellini di Mare', 18.50,
     N'Shrimp, clams and mussels sautéed in white wine, garlic and a zesty marinara sauce. Served over capellini and topped with fresh basil.');

INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000011111', '00000000-0000-0000-0001-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000011111', '00000000-0000-0000-0001-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000022222', '00000000-0000-0000-0002-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000022222', '00000000-0000-0000-0002-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000033333', '00000000-0000-0000-0003-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000033333', '00000000-0000-0000-0003-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000044444', '00000000-0000-0000-0004-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000044444', '00000000-0000-0000-0004-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000055555', '00000000-0000-0000-0005-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000055555', '00000000-0000-0000-0005-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000066666', '00000000-0000-0000-0006-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000066666', '00000000-0000-0000-0006-000000000002', 2);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000077777', '00000000-0000-0000-0007-000000000001', 1);
INSERT INTO [dbo].[MenuCategoryAndMenuItemMap]([GroupId],[MenuCategoryId],[MenuItemId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000077777', '00000000-0000-0000-0007-000000000002', 2);

/*
declare @ContentLength int;
declare @MenuItemId uniqueidentifier;
set @MenuItemId = '00000000-0000-0000-0001-000000000001';
INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', @MenuItemId, 'image/jpeg', 0,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Create_a_Sampler_Italiano.jpg', SINGLE_BLOB) AS X));
SET @ContentLength = (SELECT datalength([Data]) FROM [dbo].[MenuItemImage] WHERE [MenuItemId] = @MenuItemId);
UPDATE [dbo].[MenuItemImage] SET [ContentLength] = @ContentLength WHERE [MenuItemId] = @MenuItemId;
*/

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0001-000000000001', 'image/jpeg', 38683,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Create_a_Sampler_Italiano.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0001-000000000002', 'image/jpeg', 34925,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Bruschetta.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0002-000000000001', 'image/jpeg', 39101,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Chicken_Gnocchi.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0002-000000000002', 'image/jpeg', 48467,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Grilled_Chicken_Caesar.jpg', SINGLE_BLOB) AS X));
     
INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0003-000000000001', 'image/jpeg', 38896,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Create_Your_Own_Pizza.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0003-000000000002', 'image/jpeg', 45341,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Chicken_Alfredo_Pizza.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0004-000000000001', 'image/jpeg', 41826,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Lasagna_Classico.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0004-000000000002', 'image/jpeg', 35929,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Fettuccine_Alfredo.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0005-000000000001', 'image/jpeg', 37452,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Braised_Beef_Tortelloni.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0005-000000000002', 'image/jpeg', 39549,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Ravioli_di_Portobello.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0006-000000000001', 'image/jpeg', 49443,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Venetian_Apricot_Chicken.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0006-000000000002', 'image/jpeg', 39392,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Chicken_Scampi.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0007-000000000001', 'image/jpeg', 44624,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Shrimp_Primavera.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemImage] ([GroupId],[MenuItemId],[ContentType],[ContentLength],[Data])
     VALUES ('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0007-000000000002', 'image/jpeg', 45975,
     (SELECT * FROM OPENROWSET(BULK 'C:\_OnlineDrive\YummyZone\YummyZone.VenueAdmin.Web\Images\examples\Capellini_di_Mare.jpg', SINGLE_BLOB) AS X));

INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0001-000000000001', 4);
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0004-000000000002', 3);
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0003-000000000001', 0);

INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000003', '00000000-0000-0000-0001-000000000001', 3);
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000003', '00000000-0000-0000-0004-000000000002', 4);
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000003', '00000000-0000-0000-0003-000000000001', 2);
     
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000005', '00000000-0000-0000-0001-000000000001', 0);
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES('00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0001-000000000002', 0);

INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001', 1, 'Service & Staff');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002', 1, 'Cleanliness');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000003', 1, 'Atmosphere');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000004', 1, 'Overall Satisfaction');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000005', 2, 'Would you consider recommending us to your friends?');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000006', 3, 'How likely is it that you would come back to our restaurant again?');
INSERT INTO [dbo].[Question]([GroupId],[Id],[Type],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000007', 4, 'Other comments?');

INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000001',1);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000002',2);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000003',3);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000004',4);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000005',5);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000006',6);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000007',7);
INSERT INTO [dbo].[ChainAndQuestionMap]([GroupId],[ChainId],[QuestionId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000011','00000000-0000-0000-0000-000000000008',8);


INSERT INTO [dbo].[Choice]([GroupId],[Id],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001','Very Likely');
INSERT INTO [dbo].[Choice]([GroupId],[Id],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002','Likely');
INSERT INTO [dbo].[Choice]([GroupId],[Id],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000003','Maybe');
INSERT INTO [dbo].[Choice]([GroupId],[Id],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000004','Unlikely');
INSERT INTO [dbo].[Choice]([GroupId],[Id],[Wording])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000005','Very unlikely');

INSERT INTO [dbo].[QuestionAndChoiceMap]([GroupId],[QuestionId],[ChoiceId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000001', 1);
INSERT INTO [dbo].[QuestionAndChoiceMap]([GroupId],[QuestionId],[ChoiceId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000002', 2);
INSERT INTO [dbo].[QuestionAndChoiceMap]([GroupId],[QuestionId],[ChoiceId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000003', 3);
INSERT INTO [dbo].[QuestionAndChoiceMap]([GroupId],[QuestionId],[ChoiceId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000004', 4);
INSERT INTO [dbo].[QuestionAndChoiceMap]([GroupId],[QuestionId],[ChoiceId],[OrderIndex])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000005', 5);


INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001', 4);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002', 3);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000003', 3);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000004', 4);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerYesNo])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000005', 1);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerChoiceId])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000002');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerFreeText])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000007', N'I would like to see more vegeterian choices in your restaurant.');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId])
     VALUES('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000008');

INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000001', 3);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002', 4);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000003', 5);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000004', 5);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerYesNo])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000005', 1);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerChoiceId])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000001');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerFreeText])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000007', N'The tables are too close to each other.');

INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId])
     VALUES('00000000-0000-0000-0000-000000000005','00000000-0000-0000-0000-000000000001');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId])
     VALUES('00000000-0000-0000-0000-000000000006','00000000-0000-0000-0000-000000000001');

INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000001', 3);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002', 4);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000003', 2);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000004', 3);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerYesNo])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000005', 0);
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerChoiceId])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000002');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerFreeText])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000007', N'The restaurant is too crowded.');
INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId])
     VALUES('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000008');

     
INSERT INTO [dbo].[Message]([GroupId],[ChainId],[SenderId],[ReceiverId],[CheckInId],[Title],[Content])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', 
            '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001',
            N'Thank you', N'We appreciate your feedback');
            
INSERT INTO [dbo].[Coupon]([GroupId],[ChainId],[SenderId],[ReceiverId],[CheckInId],[ExpiryDateUTC],[Title])
     VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', 
            '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001',
            '2012-12-31 23:59:59.000', N'$10 discount on your next visit');


--------------------------------------------------------
----------------------- more test data -----------------
--------------------------------------------------------
declare @chkTime datetime;
set @chkTime = '2011-09-23 05:30:00.000';
declare @count int;
set @count = 0;
declare @chkId uniqueidentifier;
declare @id varchar(50);
declare @dinerId uniqueidentifier;
declare @menuItemId uniqueidentifier;
while(@count < 21)
begin
	set @chkId = NEWID();
	
	set @id = '00000000-0000-0000-0000-00000000000' + CAST(((@count%5)+1)  as varchar(max));
	set @dinerId = CAST(@id as uniqueidentifier);
	
	INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId], [TimeUTC]) 
		VALUES(@dinerId, @chkId, '00000000-0000-0000-0000-000000000111', @chkTime);
	
	set @id = '00000000-0000-0000-000' + CAST(((@count%7)+1)  as varchar(max)) + '-00000000000' + CAST(((@count%2)+1)  as varchar(max));
	set @menuItemId = CAST(@id as uniqueidentifier);
	
	INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES(@chkId, @menuItemId, ((@count % 5) + 1));
	
	if((@count % 3) = 0)
	begin
		INSERT INTO [dbo].[MarkedAsRead] ([CheckInId],[UserId])
			VALUES (@chkId, '00000000-0000-0000-0000-000000000001');
	end
	
	if((@count % 4) = 0)
	begin
		INSERT INTO [dbo].[Message]([GroupId],[ChainId],[SenderId],[ReceiverId],[CheckInId],[Title],[Content])
		 VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', 
				'00000000-0000-0000-0000-000000000001', @chkId,
				 N'Thank you', N'We appreciate your feedback');
	end
	
	if((@count % 6) = 0)
	begin
		INSERT INTO [dbo].[Coupon]([GroupId],[ChainId],[SenderId],[ReceiverId],[CheckInId],[ExpiryDateUTC],[Title])
		 VALUES('00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001',
				'00000000-0000-0000-0000-000000000001', @chkId,
				'2012-12-31 23:59:59.000', N'$10 discount on your next visit');
	end
		
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000001', (CAST((rand() * 5) as int)+1));
	end
	
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000002', (CAST((rand() * 5) as int)+1));
	end
	
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000003', (CAST((rand() * 5) as int)+1));
	end
	
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerRate])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000004', (CAST((rand() * 5) as int)+1));
	end
	
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerYesNo])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000005', (CAST((rand() * 2) as int)));
	end
	
	if((rand() * 10) > 5)
	begin
		set @id = '00000000-0000-0000-0000-00000000000' + CAST(((@count%5)+1)  as varchar(max));
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerChoiceId])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000006', CAST(@id as uniqueidentifier));
	end
	
	if((rand() * 10) > 5)
	begin
		INSERT INTO [dbo].[Answer]([CheckInId],[QuestionId],[AnswerFreeText])
		VALUES(@chkId,'00000000-0000-0000-0000-000000000007', N'Hello');
	end
	
	set @count = @count + 1;
	set @chkTime = dateadd(day, 1, @chkTime);
end


declare @chkIdNew uniqueidentifier;
set @chkIdNew = NEWID();

INSERT INTO [dbo].[CheckIn] ([DinerId], [Id], [VenueId]) 
	VALUES('00000000-0000-0000-0000-000000000001',@chkIdNew, '00000000-0000-0000-0000-000000000111');
	
INSERT INTO [dbo].[MenuItemRate]([CheckInId],[MenuItemId],[Rate])
     VALUES(@chkIdNew, '00000000-0000-0000-0001-000000000001', 5);