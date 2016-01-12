using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{   
    public partial class Group : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[Group] WHERE [Id] = @Id);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Group]
                                ([Id]
                                ,[Status]
                                ,[Name]
                                ,[WebURL]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                            VALUES(
                                @Id,
                                @Status,
                                @Name,
                                @WebURL,
                                @CreateTimeUTC,
                                @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                       UPDATE [dbo].[Group]
                           SET [Status] = @Status,
                               [Name] = @Name,
                               [WebURL] = @WebURL,
                               [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                         WHERE [Id] = @Id;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            return "DELETE FROM [dbo].[Group] WHERE [Id] = @Id;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }
                        
            command.Parameters.AddWithValue("@Id", this.Id);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Status", (byte)this.Status);
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@WebURL", String.IsNullOrWhiteSpace(this.WebURL) ? DBNull.Value : (object)this.WebURL);
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

            string query = @"   SELECT [Id]
                                      ,[Status]
                                      ,[Name]
                                      ,[WebURL]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Group]
                                  WHERE [Id] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id);
        }

        public string SelectAllQuery(Guid foo)
        {
            string query = @"   SELECT [Id]
                                      ,[Status]
                                      ,[Name]
                                      ,[WebURL]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Group]";

            return Database.ShortenQuery(query);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.Id = reader.GetGuid(colIndex++);
            this.Status = (VenueStatus)reader.GetByte(colIndex++);
            this.Name = reader.GetString(colIndex++);
            
            if (!reader.IsDBNull(colIndex))
            {
                this.WebURL = reader.GetString(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}

