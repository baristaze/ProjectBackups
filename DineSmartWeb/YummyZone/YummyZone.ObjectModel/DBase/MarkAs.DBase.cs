using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class MarkAs : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckInId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[MarkedAsRead] WHERE [CheckInId] = @CheckInId AND [UserId] = @UserId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[MarkedAsRead] ([CheckInId], [UserId], [ReadTimeUTC]) 
	                        VALUES(@CheckInId, @UserId, @ReadTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[MarkedAsRead]
                            SET [ReadTimeUTC] = @ReadTimeUTC
                            WHERE [CheckInId] = @CheckInId AND [UserId] = @UserId;
                    END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            return "DELETE FROM [dbo].[MarkedAsRead] WHERE [CheckInId] = @CheckInId AND [UserId] = @UserId;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckInId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            command.Parameters.AddWithValue("@CheckInId", this.CheckInId);
            command.Parameters.AddWithValue("@UserId", this.UserId);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@ReadTimeUTC", this.ReadTimeUTC);
            }
        }

        public string SelectQuery()
        {
            string query = @"SELECT [CheckInId], [UserId], [ReadTimeUTC] FROM [dbo].[MarkedAsRead] WHERE [CheckInId] = '{0}' AND [UserId] = '{1}';";
            return String.Format(CultureInfo.InvariantCulture, query, this.CheckInId, this.UserId);
        }

        public string SelectAllQuery(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.CheckInId = reader.GetGuid(colIndex++);
            this.UserId = reader.GetGuid(colIndex++);
            this.ReadTimeUTC = reader.GetDateTime(colIndex++);
        }        
    }
}
