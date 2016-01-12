using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class SystemUser : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[SystemUser] WHERE [Id] = @Id);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[SystemUser]
                                (  [Id]
                                  ,[Status]
                                  ,[IsAdmin]
                                  ,[FirstName]
                                  ,[LastName]
                                  ,[EmailAddress]
                                  ,[UserPassword]
                                  ,[CreateTimeUTC])
                            VALUES(
                                @Id,
                                @Status,
                                @IsAdmin,
                                @FirstName,
                                @LastName,
                                @EmailAddress,
                                @UserPassword,
                                @CreateTimeUTC);
                    END
                    ELSE
                    BEGIN
                       UPDATE [dbo].[SystemUser]
                           SET [Status] = @Status
                              ,[IsAdmin] = @IsAdmin
                              ,[FirstName] = @FirstName
                              ,[LastName] = @LastName
                              ,[EmailAddress] = @EmailAddress
                              ,[UserPassword] = @UserPassword
                              ,[CreateTimeUTC] = @CreateTimeUTC
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

            return "DELETE FROM [dbo].[SystemUser] WHERE [Id] = @Id;";
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
                command.Parameters.AddWithValue("@IsAdmin", this.IsAdmin);
                command.Parameters.AddWithValue("@FirstName", this.FirstName);
                command.Parameters.AddWithValue("@LastName", this.LastName);
                command.Parameters.AddWithValue("@EmailAddress", this.EmailAddress);
                command.Parameters.AddWithValue("@UserPassword", this.UserPassword);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
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
                                      ,[IsAdmin]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[EmailAddress]
                                      ,[UserPassword]
                                      ,[CreateTimeUTC]
                                  FROM [dbo].[SystemUser]
                                  WHERE [Id] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id);
        }

        public static string SelectByEmailAndPasswordQuery()
        {
            string query = @"   SELECT [Id]
                                      ,[Status]
                                      ,[IsAdmin]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[EmailAddress]
                                      ,[UserPassword]
                                      ,[CreateTimeUTC]
                                  FROM [dbo].[SystemUser]
                                  WHERE [EmailAddress] = @EmailAddress AND 
                                        [UserPassword] = @UserPassword;";

            return Database.ShortenQuery(query);
        }

        public static void AddSqlParametersForEmailAndPassword(SqlCommand command, string email, string pswd)
        {
            command.Parameters.AddWithValue("@EmailAddress", email);
            command.Parameters.AddWithValue("@UserPassword", pswd);
        }

        public string SelectAllQuery(Guid foo)
        {
            string query = @"   SELECT [Id]
                                      ,[Status]
                                      ,[IsAdmin]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[EmailAddress]
                                      ,[UserPassword]
                                      ,[CreateTimeUTC]
                                  FROM [dbo].[SystemUser]";

            return Database.ShortenQuery(query);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.Id = reader.GetGuid(colIndex++);
            this.Status = (Status)reader.GetByte(colIndex++);
            this.IsAdmin = reader.GetBoolean(colIndex++);
            this.FirstName = reader.GetString(colIndex++);
            this.LastName = reader.GetString(colIndex++);
            this.EmailAddress = reader.GetString(colIndex++);
            this.UserPassword = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}
