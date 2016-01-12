using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class MenuCategory : YummyZoneEntity, IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[MenuCategory] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[MenuCategory]
                                ([GroupId]
                                ,[Id]
                                ,[Name]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                        VALUES(
                                @GroupId,
                                @Id,
                                @Name,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[MenuCategory]
                            SET [Name] = @Name,
                                [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                            WHERE [Id] = @Id AND [GroupId] = @GroupId;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"
                            declare @childCount int;
                            declare @nonRemovedChildCount int;
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[MenuCategoryAndMenuItemMap] WHERE [MenuCategoryId] = @Id AND [GroupId] = @GroupId);
                            IF(@childCount > 0)
                            BEGIN
                                set @nonRemovedChildCount = (SELECT COUNT(*) FROM [dbo].[MenuCategoryAndMenuItemMap] WHERE [Status] <> {0} AND [MenuCategoryId] = @Id AND [GroupId] = @GroupId);
                                IF (@nonRemovedChildCount > 0)
                                BEGIN
                                    RAISERROR ('Item cannot be deleted since it has children', 16, 1);
                                END
                                ELSE
                                BEGIN
	                                UPDATE [dbo].[MenuAndMenuCategoryMap] SET [Status] = {0} WHERE [MenuCategoryId] = @Id AND [GroupId] = @GroupId;
                                END
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[MenuCategory] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
                            END";

            query = Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, (int)Status.Removed);

            return query;
        }
        
        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@Id", this.Id);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public string SelectQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"SELECT [GroupId],
                                    [Id],
		                            [Name],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[MenuCategory]                            
                            WHERE [Id] = '{0}' AND [GroupId] = '{1}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id, this.GroupId);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"SELECT [GroupId],
                                    [Id],
		                            [Name],
		                            [CreateTimeUTC],
		                            [LastUpdateTimeUTC]
                            FROM [dbo].[MenuCategory]
                            WHERE [GroupId] = '{0}'
                            ORDER BY [CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        /*
        public string SelectAllForVenue(Guid groupId, Guid venueId)
        {
            string query = @"SELECT [GroupId],
                                    [Id],
                                    [Name],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC] 
                            FROM [dbo].[MenuCategory]
                            WHERE [GroupId] = '{0}'
                                AND [Id] IN
                                (SELECT DISTINCT [MenuCategoryId]
                                    FROM [dbo].[MenuAndMenuCategoryMap]
                                    WHERE [GroupId] = '{0}'
                                        AND [MenuId] IN
                                        (SELECT DISTINCT [MenuId]
                                            FROM [dbo].[VenueAndMenuMap]
                                            WHERE [GroupId] = '{0}'
                                                AND [VenueId] = '{1}'))";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, venueId);
        }
        */

        public static string SearchCategoryId(int top, Guid groupId, Guid menuId, Status categoryStatus)
        {
            string query = @"    SELECT TOP {0}
	                                   [Map].[MenuCategoryId]
                                  FROM [dbo].[MenuAndMenuCategoryMap] [Map]
                                  JOIN [dbo].[MenuCategory] [Cat] ON [Map].[GroupId] = [Cat].[GroupId] 
		                                AND [Map].[MenuCategoryId] = [Cat].[Id]
                                  WHERE [Map].[GroupId] = '{1}' 
		                                AND [Map].[MenuId] = '{2}' 
		                                AND [Map].[Status] = {3} 
		                                AND [Cat].[Name] = @Name
                                  ORDER BY [Cat].[LastUpdateTimeUTC] DESC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, top, groupId, menuId, (int)categoryStatus);
        }

        public static void AddSqlParametersForCategoryIdSearch(SqlCommand command, string categoryName)
        {
            command.Parameters.AddWithValue("@Name", categoryName);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.InitFromSqlReader(reader, ref colIndex);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int colIndex)
        {
            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Name = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public static MenuCategoryList GetMenuCategories(SqlConnection connection, Guid groupId, Guid menuId)
        {
            List<Guid> filteredCategoryIds = new List<Guid>();
            MenuCategoryList filteredCategories = new MenuCategoryList();

            MapListMenuToCategory maps = Database.SelectAll<MapMenuToCategory, MapListMenuToCategory>(
                connection, null, groupId, Database.TimeoutSecs);

            foreach (MapMenuToCategory map in maps)
            {
                if (map.MenuId == menuId && map.Status != Status.Removed)
                {
                    filteredCategoryIds.Add(map.MenuCategoryId);
                }
            }

            if (filteredCategoryIds.Count > 0)
            {
                MenuCategoryList categories = Database.SelectAll<MenuCategory, MenuCategoryList>(
                    connection, null, groupId, Database.TimeoutSecs);

                // do not change the order of for loops; it changes the sort order
                foreach (Guid categoryId in filteredCategoryIds)
                {
                    MenuCategory cat = categories[categoryId];
                    if (cat != null)
                    {
                        filteredCategories.Add(cat);
                    }
                }

                // add menu items into the categories
                MapListCategoryToMenuItem innerMaps = Database.SelectAll<MapCategoryToMenuItem, MapListCategoryToMenuItem>(
                    connection, null, groupId, Database.TimeoutSecs);

                MenuItemList menuItems = Database.SelectAll<MenuItem, MenuItemList>(
                    connection, null, groupId, Database.TimeoutSecs);

                foreach (MenuCategory category in filteredCategories)
                {
                    foreach (MapCategoryToMenuItem innerMap in innerMaps)
                    {
                        if (innerMap.Status != Status.Removed && category.Id == innerMap.MenuCategoryId)
                        {
                            MenuItem menuItem = menuItems[innerMap.MenuItemId];
                            if (menuItem != null)
                            {
                                category.Items.Add(menuItem);
                            }
                        }
                    }
                }
            }

            return filteredCategories;
        }

        public static MapListMenuToCategoryEx GetMenuCategoryNames(SqlConnection connection, Guid groupId, Guid venueId, Guid excludedMenuId)
        {
            MenuCategoryList existingCatList = GetMenuCategories(connection, groupId, excludedMenuId);

            MapListMenuToCategoryEx pairs = new MapListMenuToCategoryEx();

            string query = @"SELECT [A].[MenuId], [A].[MenuCategoryId], [B].[Name], [C].[Name]
	                                FROM [dbo].[MenuAndMenuCategoryMap] [A]
	                                JOIN [dbo].[Menu] [B] ON [A].[MenuId] = [B].[Id] AND [B].[GroupId] = @GroupId
	                                JOIN [dbo].[MenuCategory] [C] ON [A].[MenuCategoryId] = [C].[Id] AND [C].[GroupId] = @GroupId
	                                WHERE [A].[GroupId] = @GroupId AND [A].[MenuId] <> @MenuId AND [A].[Status] = 0 AND
		                                  [A].[MenuId] IN ( SELECT [MenuId] FROM [dbo].[VenueAndMenuMap] 
							                                WHERE	[MenuId] <> @MenuId AND 
									                                [VenueId] = @VenueId AND
									                                [GroupId] = @GroupId AND
									                                [Status] = 0) AND
		                                  [A].[MenuCategoryId] NOT IN (
							                                SELECT [MenuCategoryId] FROM [dbo].[MenuAndMenuCategoryMap] 
							                                WHERE	[MenuId] = @MenuId AND
									                                [GroupId] = @GroupId)
                                    ORDER BY [A].[MenuId], [A].[MenuCategoryId];";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@VenueId", venueId);
                command.Parameters.AddWithValue("@MenuId", excludedMenuId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colIndex = 0;
                        Guid mid = reader.GetGuid(colIndex++);
                        Guid cid = reader.GetGuid(colIndex++);
                        string menuName = reader.GetString(colIndex++);
                        string catName = reader.GetString(colIndex++);

                        if (pairs.SearchByMenuCategoryId(cid) == null && existingCatList[catName] == null)
                        {
                            MapMenuToCategoryEx pair = new MapMenuToCategoryEx();
                            pair.MenuId = mid;
                            pair.MenuCategoryId = cid;
                            pair.MenuName = menuName;
                            pair.MenuCategoryName = catName;
                            pairs.Add(pair);
                        }
                    }
                }
            }

            return pairs;
        }
    }
}
