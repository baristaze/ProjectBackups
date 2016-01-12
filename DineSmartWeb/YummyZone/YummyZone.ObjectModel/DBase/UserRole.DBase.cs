using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class UserRole : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[UserRole] WHERE [GroupId] = @GroupId AND [VenueId] = @VenueId AND [UserId] = @UserId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[UserRole]
                            ([GroupId]
                            ,[VenueId]
                            ,[UserId]
                            ,[Role]
                            ,[Status]
                            ,[CreateTimeUTC]
                            ,[LastUpdateTimeUTC])
                        VALUES
                            (@GroupId
                            ,@VenueId
                            ,@UserId
                            ,@Role
                            ,@Status
                            ,@CreateTimeUTC
                            ,@LastUpdateTimeUTC);
                        
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[UserRole]
                           SET [Role] = @Role
                              ,[Status] = @Status
                              ,[LastUpdateTimeUTC] = @LastUpdateTimeUTC
                         WHERE [GroupId] = @GroupId 
		                        AND [VenueId] = @VenueId 
		                        AND [UserId] = @UserId
                                            END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            return @"DELETE FROM [dbo].[UserRole] WHERE [GroupId] = @GroupId AND [VenueId] = @VenueId AND [UserId] = @UserId";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@VenueId", this.VenueId);
            command.Parameters.AddWithValue("@UserId", this.UserId);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Role", (byte)this.Role);
                command.Parameters.AddWithValue("@Status", (byte)this.Status);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public string SelectQuery()
        {
            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"SELECT  [GroupId]
                                    ,[VenueId]
                                    ,[UserId]
                                    ,[Role]
                                    ,[Status]
                                    ,[CreateTimeUTC]
                                    ,[LastUpdateTimeUTC]
                                FROM [dbo].[UserRole]
                                WHERE [GroupId] = '{0}'
		                            AND [VenueId] = '{1}'
		                            AND [UserId] = '{2}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.GroupId, this.VenueId, this.UserId);
        }

        public string SelectForLoginQuery()
        {
            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.UserId == Guid.Empty)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"SELECT TOP (1) 
	                               [R].[GroupId]
                                  ,[R].[VenueId]
                                  ,[R].[UserId]
                                  ,[R].[Role]
                                  ,[R].[Status]
                                  ,[R].[CreateTimeUTC]
                                  ,[R].[LastUpdateTimeUTC]
                              FROM [dbo].[UserRole] [R]
                              JOIN [dbo].[User] [U] ON [U].[Id] = [R].[UserId]
                              WHERE [R].[GroupId] = '{0}'
		                            AND [R].[UserId] = '{1}'
                                    AND [R].[Status] = {2}
                                    AND [U].[Status] = {2}
		                            AND [R].[VenueId] IN (SELECT [Id] FROM [dbo].[Venue] WHERE [GroupId] = '{0}')
                              ORDER BY [R].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.GroupId, this.UserId, (int)Status.Active);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"SELECT  [GroupId]
                                    ,[VenueId]
                                    ,[UserId]
                                    ,[Role]
                                    ,[Status]
                                    ,[CreateTimeUTC]
                                    ,[LastUpdateTimeUTC]
                                FROM [dbo].[UserRole]
                                WHERE [GroupId] = '{0}'
                            ORDER BY [CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.VenueId = reader.GetGuid(colIndex++);
            this.UserId = reader.GetGuid(colIndex++);

            this.Role = (Role)reader.GetByte(colIndex++);
            this.Status = (Status)reader.GetByte(colIndex++);

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public static string QueryForBranchCountForUser(Guid groupId, Guid userId)
        {
            string query = @"SELECT COUNT(DISTINCT VenueId) FROM [dbo].[UserRole] WHERE [GroupId] = '{0}' AND [UserId] = '{1}'";
            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, userId);
        }

        public static int ReadBranchCountForUser(SqlConnection connection, Guid groupId, Guid userId)
        { 
            string query = QueryForBranchCountForUser(groupId, userId);
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                var count = command.ExecuteScalar();
                if (count != null)
                {
                    int result = 0;
                    Int32.TryParse(count.ToString(), out result);
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
