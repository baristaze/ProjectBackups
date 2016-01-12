using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class User : YummyZoneEntity, IEditable
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
                    set @count = (SELECT COUNT(*) FROM [dbo].[User] WHERE [GroupId] = @GroupId AND [Id] = @Id);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[User]
                                ([GroupId]
                                ,[Id]
                                ,[Status]
                                ,[EmailAddress]
                                ,[FirstName]
                                ,[LastName]
                                ,[PhoneNumber]
                                ,[CreateTimeUTC]
                                ,[LastUpdateTimeUTC])
                            VALUES
                                (@GroupId
                                ,@Id
                                ,@Status
                                ,@EmailAddress
                                ,@FirstName
                                ,@LastName
                                ,@PhoneNumber
                                ,@CreateTimeUTC
                                ,@LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[User]
                           SET [Status] = @Status
                              ,[EmailAddress] = @EmailAddress
                              ,[FirstName] = @FirstName
                              ,[LastName] = @LastName
                              ,[PhoneNumber] = @PhoneNumber
                              ,[LastUpdateTimeUTC] = @LastUpdateTimeUTC
                        WHERE [GroupId] = @GroupId AND [Id] = @Id;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            return "DELETE FROM [dbo].[User] WHERE [GroupId] = @GroupId AND [Id] = @Id;";
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
                command.Parameters.AddWithValue("@EmailAddress", this.EmailAddress);
                command.Parameters.AddWithValue("@FirstName", this.FirstName);
                command.Parameters.AddWithValue("@LastName", this.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", String.IsNullOrWhiteSpace(this.PhoneNumber) ? DBNull.Value : (object)this.PhoneNumber);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public static void AddSqlParameterForEmail(SqlCommand command, string email)
        {
            command.Parameters.AddWithValue("@EmailAddress", email);
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
                                      ,[EmailAddress]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[PhoneNumber]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[User]
                                  WHERE [GroupId] = '{0}' AND [Id] = '{1}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.GroupId, this.Id);
        }

        public static string SelectByEmailQuery()
        {
            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[Status]
                                      ,[EmailAddress]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[PhoneNumber]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[User]
                                  WHERE [EmailAddress] = @EmailAddress;";

            return Database.ShortenQuery(query);
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
                                      ,[EmailAddress]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[PhoneNumber]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[User]
                                  WHERE [GroupId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Status = (Status)reader.GetByte(colIndex++);
            this.EmailAddress = reader.GetString(colIndex++);
            this.FirstName = reader.GetString(colIndex++);
            this.LastName = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.PhoneNumber = reader.GetString(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}
