using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Menu : YummyZoneEntity, IEditable
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
                    set @count = (SELECT COUNT(*) FROM [dbo].[Menu] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Menu]
                                ([GroupId]
                                ,[Id]                                   
                                ,[Name]
                                ,[ServiceStartTime]
                                ,[ServiceEndTime]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                            VALUES(
                                @GroupId,
                                @Id,                                   
                                @Name,
                                @ServiceStartTime,
                                @ServiceEndTime,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                        
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Menu]
                            SET [Name] = @Name,
                                [ServiceStartTime] = @ServiceStartTime,
                                [ServiceEndTime] = @ServiceEndTime,
                                [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                            WHERE 
                                [Id] = @Id AND [GroupId] = @GroupId;
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
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[MenuAndMenuCategoryMap] WHERE [MenuId] = @Id AND [GroupId] = @GroupId);
                            IF(@childCount > 0)
                            BEGIN
                                set @nonRemovedChildCount = (SELECT COUNT(*) FROM [dbo].[MenuAndMenuCategoryMap] WHERE [Status] <> {0} AND [MenuId] = @Id AND [GroupId] = @GroupId);
                                IF (@nonRemovedChildCount > 0)
                                BEGIN
                                    RAISERROR ('Item cannot be deleted since it has children', 16, 1);
                                END
                                ELSE
                                BEGIN
	                                UPDATE [dbo].[VenueAndMenuMap] SET [Status] = {0} WHERE [MenuId] = @Id AND [GroupId] = @GroupId;
                                END
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[Menu] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
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
                command.Parameters.AddWithValue("@ServiceStartTime", this.ServiceStartTime.HasValue ? this.ServiceStartTime.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ServiceEndTime", this.ServiceEndTime.HasValue ? this.ServiceEndTime.Value : (object)DBNull.Value);
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
                                    [ServiceStartTime],
                                    [ServiceEndTime],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC]
                          FROM [dbo].[Menu]                          
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
                                    [ServiceStartTime],
                                    [ServiceEndTime],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC]
                          FROM [dbo].[Menu]
                          WHERE [GroupId] = '{0}'
                          ORDER BY [CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public static string SelectAllForVenue(Guid groupId, Guid venueId)
        {
            string query = @"   SELECT [VM].[GroupId]
	                                  ,[M].[Id]
	                                  ,[M].[Name]
	                                  ,[M].[ServiceStartTime]
	                                  ,[M].[ServiceEndTime]
	                                  ,[M].[CreateTimeUTC]
	                                  ,[M].[LastUpdateTimeUTC]
                                  FROM [dbo].[VenueAndMenuMap] [VM]
                                  JOIN [dbo].[Menu] [M] ON [M].[Id] = [VM].[MenuId] 
                                  WHERE [VM].[GroupId] = '{0}'
	                                AND [VM].[VenueId] = '{1}'
	                                AND [VM].[Status] <> {2}
                                  ORDER BY [VM].[Status] ASC, 
		                                   [VM].[OrderIndex] ASC,
		                                   [M].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, venueId, (int)Status.Removed);
        }

        public static string SelectAllForVenueWithCategories(Guid groupId, Guid venueId)
        {
            string query = @"SELECT [M].[GroupId]
                                  ,[M].[Id]
                                  ,[M].[Name]
                                  ,[M].[ServiceStartTime]
                                  ,[M].[ServiceEndTime]
                                  ,[M].[CreateTimeUTC]
                                  ,[M].[LastUpdateTimeUTC]
                                  ,[C].[GroupId]
                                  ,[C].[Id]
                                  ,[C].[Name]
                                  ,[C].[CreateTimeUTC]
                                  ,[C].[LastUpdateTimeUTC]
                              FROM [dbo].[MenuAndMenuCategoryMap] [CM]
                              JOIN [dbo].[Menu] [M] ON [M].[Id] = [CM].[MenuId]
                              JOIN [dbo].[MenuCategory] [C] ON [C].[Id] = [CM].[MenuCategoryId]
                              WHERE [CM].[GroupId] = '{0}'
		                            AND [CM].[Status] {1}
		                            AND [CM].[MenuId] IN
		                            (SELECT DISTINCT [MenuId]
			                            FROM [dbo].[VenueAndMenuMap]
			                            WHERE [GroupId] = '{0}'
				                            AND [VenueId] = '{2}'
				                            AND [Status] {1})
	                            ORDER BY [M].[Id], [C].[Id]";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, (int)Status.Removed, venueId);
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

            if (!reader.IsDBNull(colIndex))
            {
                this.ServiceStartTime = reader.GetInt16(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.ServiceEndTime = reader.GetInt16(colIndex);
            }
            colIndex++;            

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public static MenuList LoadMenusWithCategories(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid groupId,
            Guid venueId,
            int timeoutSeconds)
        {
            string query = SelectAllForVenueWithCategories(groupId, venueId);

            MenuList menuList = new MenuList();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = transaction;
                command.CommandTimeout = timeoutSeconds;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int colIndex = 0;

                            Menu newMenu = new Menu();
                            newMenu.InitFromSqlReader(reader, ref colIndex);
                            Menu menu = menuList[newMenu.Id];
                            if (menu == null)
                            {
                                menu = newMenu;

                                // add if it doesn't exist
                                menuList.Add(menu);
                            }

                            MenuCategory newCategory = new MenuCategory();
                            newCategory.InitFromSqlReader(reader, ref colIndex);
                            MenuCategory category = menuList.SearchByCategoryId(newCategory.Id);
                            if (category == null)
                            {
                                category = newCategory;
                            }

                            // always add
                            menu.Categories.Add(category);
                        }
                    }
                }
            }

            return menuList;
        }
    }
}
