using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{   
    public partial class Chain : YummyZoneEntity, IEditable
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
                    set @count = (SELECT COUNT(*) FROM [dbo].[Chain] WHERE [GroupId] = @GroupId AND [Id] = @Id);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Chain]
                                ([GroupId]
                                ,[Id]
                                ,[Status]
                                ,[Name]
                                ,[VenueTypeFlags]
                                ,[CuisineTypeFlags]
                                ,[WebURL]
                                ,[LogoURL]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                            VALUES(
                                @GroupId,
                                @Id,
                                @Status,
                                @Name,
                                @VenueTypeFlags,
                                @CuisineTypeFlags,
                                @WebURL,
                                @LogoURL,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                       UPDATE [dbo].[Chain]
                           SET [Status] = @Status,
                               [Name] = @Name,
                               [VenueTypeFlags] = @VenueTypeFlags,
                               [CuisineTypeFlags] = @CuisineTypeFlags,
                               [WebURL] = @WebURL,
                               [LogoURL] = @LogoURL,
                               [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                         WHERE [GroupId] = @GroupId AND [Id] = @Id;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            return "DELETE FROM [dbo].[Chain] WHERE [GroupId] = @GroupId AND [Id] = @Id;";
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
                command.Parameters.AddWithValue("@Status", (byte)this.Status);
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@VenueTypeFlags", (int)this.VenueTypeFlags);
                command.Parameters.AddWithValue("@CuisineTypeFlags", (int)this.CuisineTypeFlags);
                command.Parameters.AddWithValue("@WebURL", String.IsNullOrWhiteSpace(this.WebURL) ? DBNull.Value : (object)this.WebURL);
                command.Parameters.AddWithValue("@LogoURL", String.IsNullOrWhiteSpace(this.LogoURL) ? DBNull.Value : (object)this.LogoURL);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public static string SelectQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[Status]
                                      ,[Name]
                                      ,[VenueTypeFlags]
                                      ,[CuisineTypeFlags]
                                      ,[WebURL]
                                      ,[LogoURL]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Chain]
                                  WHERE [Id] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, id);
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

            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[Status]
                                      ,[Name]
                                      ,[VenueTypeFlags]
                                      ,[CuisineTypeFlags]
                                      ,[WebURL]
                                      ,[LogoURL]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Chain]
                                  WHERE [GroupId] = '{0}' AND [Id] = '{1}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.GroupId, this.Id);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[Status]
                                      ,[Name]
                                      ,[VenueTypeFlags]
                                      ,[CuisineTypeFlags]
                                      ,[WebURL]
                                      ,[LogoURL]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Chain]
                                  WHERE [GroupId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Status = (VenueStatus)reader.GetByte(colIndex++);
            this.Name = reader.GetString(colIndex++);
            this.VenueTypeFlags = (VenueType)reader.GetInt32(colIndex++);
            this.CuisineTypeFlags = (CuisineType)reader.GetInt32(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.WebURL = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.LogoURL = reader.GetString(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}
