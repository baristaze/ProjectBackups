
select * from Menu;
select * from MenuCategory;
select * from MenuItem;
select * from MenuItemImage;


/*
delete from Menu;
delete from MenuCategory;
delete from MenuItem;
delete from MenuItemImage;
delete from MenuItemRate;
*/

/*
select * from VenueAndMenuMap;
select * from MenuAndMenuCategoryMap;
select * from MenuCategoryAndMenuItemMap;
*/

declare @GroupId uniqueidentifier;
set @GroupId = '00000000-0000-0000-0000-000000000001';

declare @VenueId uniqueidentifier;
set @VenueId = '00000000-0000-0000-0000-000000000111';

declare @MenuId uniqueidentifier;
set @MenuId = 'FC02EE58-DCB3-47F4-8D33-4473432AB3C1';

