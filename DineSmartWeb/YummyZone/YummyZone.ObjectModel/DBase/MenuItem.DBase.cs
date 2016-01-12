using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class MenuItem : YummyZoneEntity, IEditable
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
                    set @count = (SELECT COUNT(*) FROM [dbo].[MenuItem] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[MenuItem]
                                ([GroupId]
                                ,[Id]                                    
                                ,[Name]
                                ,[Price]
                                ,[Description]
                                ,[LegalNotice]
                                ,[DietTypeFlags]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                            VALUES
                                (
                                @GroupId,
                                @Id,
                                @Name,
                                @Price,
                                @Description,
                                @LegalNotice,
                                @DietTypeFlags,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[MenuItem]
                            SET [Name] = @Name,
                                [Price] = @Price,
                                [Description] = @Description,
                                [LegalNotice] = @LegalNotice,
                                [DietTypeFlags] = @DietTypeFlags,
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

            // no group id in 'customer rate' table...
            string query = @"
                            declare @childCount int;
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[MenuItemRate] WHERE [MenuItemId] = @Id);
                            IF(@childCount > 0)
                            BEGIN
                                UPDATE [dbo].[MenuCategoryAndMenuItemMap] SET [Status] = {0} WHERE [MenuItemId] = @Id AND [GroupId] = @GroupId;
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[MenuItem] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
                            END";

            query = Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, (int)Status.Removed);

            return query;
        }

        public string DeleteQuery(Guid categoryId)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            // no group id in 'customer rate' table...
            string query = @"
                            declare @childCount int;
                            set @childCount = (SELECT COUNT(*) FROM [dbo].[MenuItemRate] WHERE [MenuItemId] = @Id);
                            IF(@childCount > 0)
                            BEGIN
                                UPDATE [dbo].[MenuCategoryAndMenuItemMap] SET [Status] = {0} 
                                    WHERE [MenuItemId] = @Id AND [MenuCategoryId] = '{1}' AND [GroupId] = @GroupId;
                            END
                            ELSE 
                            BEGIN
                                DELETE FROM [dbo].[MenuCategoryAndMenuItemMap]
                                    WHERE [MenuItemId] = @Id AND [MenuCategoryId] = '{1}' AND [GroupId] = @GroupId;
                                
                                declare @mapCount int;
                                set @mapCount = (SELECT COUNT(*) FROM [dbo].[MenuCategoryAndMenuItemMap] WHERE [MenuItemId] = @Id);
                                IF(@mapCount = 0)
                                BEGIN
                                    DELETE FROM [dbo].[MenuItem] WHERE [Id] = @Id AND [GroupId] = @GroupId;	
                                END
                            END";

            query = Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, (int)Status.Removed, categoryId);

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
                command.Parameters.AddWithValue("@Price", this.Price.HasValue ? (object)this.Price.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Description", String.IsNullOrWhiteSpace(this.Description) ? DBNull.Value : (object)this.Description);
                command.Parameters.AddWithValue("@LegalNotice", String.IsNullOrWhiteSpace(this.LegalNotice) ? DBNull.Value : (object)this.LegalNotice);
                command.Parameters.AddWithValue("@DietTypeFlags", (int)this.DietTypeFlags);
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

            string query = @"SELECT [A].[GroupId]
                                   ,[A].[Id]                                   
                                   ,[A].[Name]
                                   ,[A].[Price]
                                   ,[A].[Description]
                                   ,[A].[LegalNotice]
                                   ,[A].[DietTypeFlags]
                                   ,[A].[CreateTimeUTC]
                                   ,[A].[LastUpdateTimeUTC]                                
                                   ,[I].[MenuItemId] AS [ImageId]
                              FROM [dbo].[MenuItem] [A]
                              LEFT JOIN [dbo].[MenuItemImage] [I] ON [I].[MenuItemId] = '{0}' AND [I].[GroupId] = '{1}'                              
                              WHERE [A].[Id] = '{0}' AND [A].[GroupId] = '{1}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id, this.GroupId);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"SELECT [A].[GroupId]
                                   ,[A].[Id]
                                   ,[A].[Name]
                                   ,[A].[Price]
                                   ,[A].[Description]
                                   ,[A].[LegalNotice]
                                   ,[A].[DietTypeFlags]
                                   ,[A].[CreateTimeUTC]
                                   ,[A].[LastUpdateTimeUTC]                                
                                   ,[I].[MenuItemId] AS [ImageId]
                              FROM [dbo].[MenuItem] [A]
                              LEFT JOIN [dbo].[MenuItemImage] [I] ON [A].[Id] = [I].[MenuItemId] AND [I].[GroupId] = '{0}'
                              WHERE [A].[GroupId] = '{0}'
                              ORDER BY [A].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Name = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Price = reader.GetDecimal(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.Description = reader.GetString(colIndex);
            }

            colIndex++;
            if (!reader.IsDBNull(colIndex))
            {
                this.LegalNotice = reader.GetString(colIndex);
            }

            colIndex++;
            this.DietTypeFlags = (DietType)reader.GetInt32(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
                        
            if (!reader.IsDBNull(colIndex))
            {
                this.ImageId = reader.GetGuid(colIndex);
            }
        }

        public static string SelectAllForVenue(Guid groupId, Guid venueId)
        {
            string query = @"SELECT [A].[GroupId]
                                   ,[A].[Id]
                                   ,[A].[Name]
                                   ,[A].[Price]
                                   ,[A].[Description]
                                   ,[A].[LegalNotice]
                                   ,[A].[DietTypeFlags]
                                   ,[A].[CreateTimeUTC]
                                   ,[A].[LastUpdateTimeUTC]                                
                                   ,[I].[MenuItemId] AS [ImageId]
                              FROM [dbo].[MenuItem] [A]
                              LEFT JOIN [dbo].[MenuItemImage] [I] ON [A].[Id] = [I].[MenuItemId]
                              WHERE [A].[GroupId] = '{0}'
	                            AND [A].[Id] IN
	                            (SELECT DISTINCT [MenuItemId]
		                              FROM [dbo].[MenuCategoryAndMenuItemMap]
		                              WHERE [GroupId] = '{0}'
			                            AND [Status] <> {1}
			                            AND [MenuCategoryId] IN
			                            (SELECT DISTINCT [MenuCategoryId]
				                            FROM [dbo].[MenuAndMenuCategoryMap]
				                            WHERE [GroupId] = '{0}'
					                            AND [Status] <> {1}
					                            AND [MenuId] IN
					                            (SELECT DISTINCT [MenuId]
						                            FROM [dbo].[VenueAndMenuMap]
						                            WHERE [GroupId] = '{0}'
							                            AND [VenueId] = '{2}'
							                            AND [Status] <> {1})))
                              ORDER BY [A].[Name], [A].[CreateTimeUTC]";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, (int)Status.Removed, venueId);
        }

        public static MenuItemList LoadAllForVenue(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid groupId,
            Guid venueId,
            int timeoutSeconds)
        {
            string query = SelectAllForVenue(groupId, venueId);

            MenuItemList menuItemList = new MenuItemList();
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
                            MenuItem item = new MenuItem();
                            item.InitFromSqlReader(reader);
                            menuItemList.Add(item);
                        }
                    }
                }
            }

            return menuItemList;
        }
    }
}
