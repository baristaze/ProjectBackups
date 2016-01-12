using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class MemberUser
    {
        public static string SelectByVenueQuery(Guid groupId, Guid venueId, Guid excludedUserId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            if (venueId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Venue Id");
            }

            string query = @"   SELECT [R].[UserId]
                                      ,[U].[FirstName]
                                      ,[U].[LastName]
                                      ,[U].[EmailAddress]
                                      ,[R].[Status]
                                      ,[R].[Role]
                                      ,(SELECT COUNT(*) FROM [dbo].[UserRole] WHERE [UserId] = [R].[UserId]) AS [MembershipCount]
                                      ,(SELECT COUNT(*) FROM [dbo].[Message] WHERE [SenderId] = [R].[UserId]) AS [SentMessageCount]
                                      ,(SELECT COUNT(*) FROM [dbo].[Coupon] WHERE [SenderId] = [R].[UserId]) AS [SentCouponCount]
                                  FROM [dbo].[UserRole] [R]
                                  JOIN [dbo].[User] [U] ON [R].[UserId] = [U].[Id] AND [R].[GroupId] = [U].[GroupId]
                                  WHERE [R].[GroupId] = '{0}' AND [R].[VenueId] = '{1}' AND [R].[UserId] != '{2}' 
                                        AND [R].[Status] != {3} AND [U].[Status] = {4}
                                  ORDER BY [R].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId, venueId, excludedUserId, (int)Status.Removed, (int)Status.Active);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.UserId = reader.GetGuid(colIndex++);
            this.FirstName = reader.GetString(colIndex++);
            this.LastName = reader.GetString(colIndex++);
            this.EmailAddress = reader.GetString(colIndex++);
            this.Status = (Status)reader.GetByte(colIndex++);
            this.Role = (Role)reader.GetByte(colIndex++);
            this.MembershipCount = reader.GetInt32(colIndex++);
            this.SentMessageCount = reader.GetInt32(colIndex++);
            this.SentCouponCount = reader.GetInt32(colIndex++);
        }

        public static MemberUserList Select(SqlConnection connection, SqlTransaction trans, Guid groupId, Guid venueId, Guid excludedUserId)
        {
            MemberUserList entities = new MemberUserList();

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = SelectByVenueQuery(groupId, venueId, excludedUserId);
                command.Transaction = trans;
                command.CommandTimeout = Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            MemberUser item = new MemberUser();
                            item.InitFromSqlReader(reader);
                            entities.Add(item);
                        }
                    }
                }
            }

            return entities;
        }
    }
}
