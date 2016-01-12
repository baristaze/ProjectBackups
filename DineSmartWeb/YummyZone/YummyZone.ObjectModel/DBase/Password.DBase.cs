using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{   
    public partial class Password : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            if (String.IsNullOrWhiteSpace(this.PasswordText))
            {
                throw new ArgumentException("Empty Password");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[Password] WHERE [UserId] = @UserId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Password]([UserId], [Password]) VALUES (@UserId, @Password);
                    END
                    ELSE
                    BEGIN
                       UPDATE [dbo].[Password] SET [Password] = @Password WHERE [UserId] = @UserId;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            return "DELETE FROM [dbo].[Password] WHERE [UserId] = @UserId;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            command.Parameters.AddWithValue("@UserId", this.UserId);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Password", this.PasswordText);
            }
        }

        public string SelectQuery()
        {
            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"SELECT [UserId], [Password] FROM [dbo].[Password] WHERE [UserId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.UserId);
        }

        public string SelectAllQuery(Guid foo)
        {
            throw new NotSupportedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.UserId = reader.GetGuid(colIndex++);
            this.PasswordText = reader.GetString(colIndex++);
        }
    }
}


