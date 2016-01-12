using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class MenuItemRate : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.MenuItemId == Guid.Empty)
            {
                throw new ArgumentException("Unknown MenuItem Id");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[MenuItemRate] WHERE [CheckInId] = @CheckInId AND [MenuItemId] = @MenuItemId);
                    IF (@count = 0)
                    BEGIN
                         INSERT INTO [dbo].[MenuItemRate](
				                    [CheckInId],
				                    [MenuItemId],
				                    [Rate],
				                    [CreateTimeUTC],
				                    [LastUpdateTimeUTC])
                         VALUES(
				                    @CheckInId,
				                    @MenuItemId,
				                    @Rate,
				                    @CreateTimeUTC,
				                    @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[MenuItemRate]
                           SET [Rate] = @Rate,
                               [LastUpdateTimeUTC] = @LastUpdateTimeUTC
                         WHERE [CheckInId] = @CheckInId AND [MenuItemId] = @MenuItemId;
                    END";

            return Database.ShortenQuery(query);
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.MenuItemId == Guid.Empty)
            {
                throw new ArgumentException("Unknown MenuItem Id");
            }

            command.Parameters.AddWithValue("@CheckInId", this.CheckInId);
            command.Parameters.AddWithValue("@MenuItemId", this.MenuItemId);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Rate", this.Rate);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public string DeleteQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckIn Id");
            }

            if (this.MenuItemId == Guid.Empty)
            {
                throw new ArgumentException("Unknown MenuItem Id");
            }
                        
            return @"DELETE FROM [dbo].[MenuItemRate] WHERE [CheckInId] = @CheckInId AND [MenuItemId] = @MenuItemId;";
        }

        public string SelectQuery()
        {
            if (this.CheckInId == Guid.Empty)
            {
                throw new ArgumentException("Unknown CheckInId");
            }

            if (this.MenuItemId == Guid.Empty)
            {
                throw new ArgumentException("Unknown MenuItemId");
            }

            string query = @"SELECT [R].[CheckInId],
                                    [R].[MenuItemId],
                                    [I].[Name],
                                    [R].[Rate],
                                    [R].[CreateTimeUTC],
                                    [R].[LastUpdateTimeUTC]
                            FROM [dbo].[MenuItemRate] [R]
                            JOIN [dbo].[MenuItem] [I] ON [R].[MenuItemId] = [I].[Id] AND [I].[Id] = '{0}'
                            WHERE [CheckInId] = '{1}' AND [MenuItemId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.MenuItemId, this.CheckInId);
        }

        public string SelectAllQuery(Guid checkInId)
        {
            if (checkInId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown CheckIn Id");
            }

            string query = @"SELECT [R].[CheckInId],
                                    [R].[MenuItemId],
                                    [I].[Name],
                                    [R].[Rate],
                                    [R].[CreateTimeUTC],
                                    [R].[LastUpdateTimeUTC]
                            FROM [dbo].[MenuItemRate] [R]
                            JOIN [dbo].[MenuItem] [I] ON [R].[MenuItemId] = [I].[Id]
                            WHERE [R].[Rate] > 0 AND [CheckInId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, checkInId);
        }

        public static string SelectAllWithoutNameQuery(Guid checkInId)
        {
            if (checkInId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown CheckIn Id");
            }

            string query = @"SELECT [CheckInId],
                                    [MenuItemId],
                                    ('') AS [Name],
                                    [Rate],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC]
                            FROM [dbo].[MenuItemRate]                            
                            WHERE [CheckInId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, checkInId);
        }

        public static string SelectMultipleQuery(CheckinList checkinList)
        {
            if (checkinList == null || checkinList.Count == 0)
            {
                throw new ArgumentException("CheckinList parameter is null or empty");
            }

            string filter = Checkin.BulkFilter(checkinList, "[R].[CheckInId]");

            string query = @"SELECT [R].[CheckInId],
                                    [R].[MenuItemId],
                                    [I].[Name],
                                    [R].[Rate],
                                    [R].[CreateTimeUTC],
                                    [R].[LastUpdateTimeUTC]
                            FROM [dbo].[MenuItemRate] [R]
                            JOIN [dbo].[MenuItem] [I] ON [R].[MenuItemId] = [I].[Id]
                            WHERE [R].[Rate] > 0 AND ({0})";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, filter);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.CheckInId = reader.GetGuid(colIndex++);
            this.MenuItemId = reader.GetGuid(colIndex++);
            this.MenuItemName = reader.GetString(colIndex++);
            this.Rate = reader.GetByte(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }
}
