using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Checkin
    {
        public string InsertQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.DinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            string query = @"INSERT INTO [dbo].[CheckIn](
			                    [DinerId],
                                [Id],
                                [VenueId],
                                [TimeUTC],
                                [AuditFlag])
                         VALUES(
			                    @DinerId,
                                @Id,
                                @VenueId,
                                @TimeUTC,
                                @AuditFlag);";

            return Database.ShortenQuery(query);
        }
                
        public void InsertParameters(SqlCommand command)
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.DinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            if (this.VenueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            command.Parameters.AddWithValue("@DinerId", this.DinerId);
            command.Parameters.AddWithValue("@VenueId", this.VenueId);            
            command.Parameters.AddWithValue("@Id", this.Id);
            command.Parameters.AddWithValue("@TimeUTC", this.TimeUTC);
            command.Parameters.AddWithValue("@AuditFlag", (byte)this.AuditFlag);
        }

        public static Checkin SelectCoreBy(SqlConnection connection, Guid groupId, Guid checkinId)
        {
            string query = @"SELECT [C].[DinerId], [C].[Id], [C].[VenueId], [C].[TimeUTC], [C].[AuditFlag]
	                            FROM [dbo].[CheckIn] [C]
	                            JOIN [dbo].[Venue] [V] ON [C].[VenueId] = [V].[Id] AND [V].[GroupId] = '{0}'
	                            WHERE [C].[Id] = '{1}';";

            query = Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, groupId, checkinId);

            Checkin checkin = null;
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null && reader.Read())
                    {
                        int colIndex = 0;

                        checkin = new Checkin();
                        checkin.DinerId = reader.GetGuid(colIndex++);
                        checkin.Id = reader.GetGuid(colIndex++);
                        checkin.VenueId = reader.GetGuid(colIndex++);
                        checkin.TimeUTC = reader.GetDateTime(colIndex++);
                        checkin.AuditFlag = (AuditFlag)reader.GetByte(colIndex++);
                    }
                }
            }

            return checkin;
        }

        public static int MarkAsSpam(SqlConnection connection, Guid checkinId)
        {
            return MarkAs(connection, checkinId, 3);
        }

        public static int MarkAsDeleted(SqlConnection connection, Guid checkinId)
        {
            return MarkAs(connection, checkinId, 4);
        }

        private static int MarkAs(SqlConnection connection, Guid checkinId, int marker)
        {
            string query = @"UPDATE	[dbo].[CheckIn] SET [AuditFlag] = {0} WHERE [Id] = '{1}';";

            query = String.Format(CultureInfo.InvariantCulture, query, marker, checkinId);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = Database.TimeoutSecs;
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// This is to be used to calculate the type of the diner; e.g.
        /// Occasional Diner
        /// Frequent Diner
        /// Gourmet
        /// </summary>
        /// <param name="dinerId"></param>
        /// <returns></returns>
        /*
        public static string TotalCheckinQuery(Guid dinerId)
        {
            if (dinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            string query = @"SELECT COUNT(*) FROM [dbo].[CheckIn] WHERE [DinerId] = '{0}' AND [AuditFlag] < 2;";
            return String.Format(CultureInfo.InvariantCulture, query, dinerId);
        }
        */

        /// <summary>
        /// This is to be used to calculate the type of the customer; e.g.
        /// New Customer
        /// Occasional Customer
        /// Frequent Customer
        /// </summary>
        /// <param name="dinerId"></param>
        /// <param name="venueId"></param>
        /// <returns></returns>
        /*
        public static string TotalCheckinQuery(Guid dinerId, Guid venueId)
        {
            if (dinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            if (venueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            string query = @"SELECT COUNT(*) FROM [dbo].[CheckIn] WHERE [DinerId] = '{0}' AND [VenueId] = '{1}' AND [AuditFlag] < 2;";
            return String.Format(CultureInfo.InvariantCulture, query, dinerId, venueId);
        }
        */ 

        /// <summary>
        /// This is to determine the checkin ID for the user who checkins into a restaurant
        /// We will return the last record for that user in that level. The service will check
        /// the check-in time to see if it has happened within last 2 hours. If yes, this checkin
        /// ID will be used instead of creating a new one.
        /// ...
        /// Notice that this will just receive the core properties like Id, DinerId, VenueId, TimeUtc, etc.
        /// Other calculated items won't be retrieved. Instead, they will be populated as '0' to fulfill
        /// InitFromReader method. For example, the phrase "(0) AS [RatedMenuItemCount]" simply means
        /// that "return 0 for the column in the SqlDataReader instead of making any calculation"
        /// </summary>
        /// <param name="dinerId"></param>
        /// <param name="venueId"></param>
        /// <returns></returns>
        public static string LastCheckinsQuery(Guid dinerId, int topX, DateTime cutoffTimeUTC)
        {
            if (dinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            // no audit flag here
            string query = @"SELECT TOP {0} 
                                    [DinerId],
                                    [Id],
                                    [VenueId],
                                    [TimeUTC],
                                    [AuditFlag],
                                    (0) AS [RatedMenuItemCount],
                                    (0) AS [AnswerCountToSurvey],
                                    (0) AS [CheckinCountInTotal],
                                    (0) AS [CheckinCountAtVenue],
                                    (0) AS [SentCouponCount],
                                    (0) AS [SentMessageCount],
                                    (0) AS [ReadByManager]
                                FROM [dbo].[CheckIn]
                                WHERE [DinerId] = '{1}' AND [TimeUTC] >= '{2}'
                                ORDER BY [TimeUTC] DESC;";

            query = Database.ShortenQuery(query);

            string cutOffTime = cutoffTimeUTC.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            return String.Format(CultureInfo.InvariantCulture, query, topX, dinerId, cutOffTime);
        }

        /// <summary>
        /// This is to be called when we first show the Feedbacks page.
        /// It will be also called when  user clicks 'Show More' link.
        /// In that case 'tillUTC' needs to be filled out.
        /// </summary>
        /// <param name="venueId">Target venue</param>
        /// <param name="top">Page Size to be used in SELECT TOP clause</param>
        /// <param name="tillUTC">
        /// This is to be called when user clicks 'Show More' link on the Feedbacks page
        /// </param>
        /// <returns>SQL Select Query</returns>
        public static string RecentCheckinsQuery(Guid venueId, int top, DateTime? tillUTC, Guid currentVenueUser)
        {
            if (venueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown VenueId");
            }

            if (top <= 0)
            {
                throw new ArgumentException("Invalid TOP clause parameter");
            }

            string query = @"SELECT TOP {0}
		                            [C].[DinerId],
                                    [C].[Id],
                                    [C].[VenueId],
                                    [C].[TimeUTC],
                                    [C].[AuditFlag],
                                    [RatedMenuItemCount],
                                    [AnswerCountToSurvey],
                                    [CheckinCountInTotal],
                                    [CheckinCountAtVenue],
                                    [SentCouponCount],
                                    [SentMessageCount],
                                    [ReadByManager]
                                FROM [dbo].[CheckIn] [C]
                                CROSS APPLY
                                (
		                            SELECT [RatedMenuItemCount] = (SELECT COUNT(*) FROM [dbo].[MenuItemRate] WHERE [CheckInId] = [C].[Id] AND [Rate] > 0 )
                                ) 
                                AS [RatedMenuItemCountQuery]
                                CROSS APPLY
                                (
		                            SELECT [AnswerCountToSurvey] = (SELECT COUNT(*) FROM [dbo].[Answer] 
														                            WHERE [CheckInId] = [C].[Id] AND 
														                            (	[AnswerYesNo] is NOT NULL OR 
															                            [AnswerRate] is NOT NULL OR 
															                            [AnswerChoiceId] is NOT NULL OR 
															                            [AnswerFreeText] is NOT NULL))
                                )
                                AS [AnswerCountToSurveyQuery]
                                CROSS APPLY
                                (
		                            SELECT [CheckinCountInTotal] = (SELECT COUNT(*) FROM [dbo].[CheckIn] WHERE [DinerId] = [C].[DinerId])
                                )
                                AS [CheckinCountInTotalQuery]
                                CROSS APPLY
                                (
		                            SELECT [CheckinCountAtVenue] = (SELECT COUNT(*) FROM [dbo].[CheckIn] WHERE [DinerId] = [C].[DinerId] AND [VenueId] = [C].[VenueId])
                                )
                                AS [CheckinCountAtVenueQuery]
                                CROSS APPLY
                                (
		                            SELECT [SentCouponCount] = (SELECT COUNT(*) FROM [dbo].[Coupon] WHERE [CheckInId] = [C].[Id])
                                )
                                AS [SentCouponCountQuery]
                                CROSS APPLY
                                (
		                            SELECT [SentMessageCount] = (SELECT COUNT(*) FROM [dbo].[Message] WHERE [CheckInId] = [C].[Id])
                                )
                                AS [SentMessageCountQuery]
                                CROSS APPLY
                                (
		                            SELECT [ReadByManager] = (SELECT COUNT(*) FROM [dbo].[MarkedAsRead] WHERE [CheckInId] = [C].[Id] AND [UserId] = '{3}')
                                )
                                AS [ReadByManagerQuery]
                                WHERE [C].[VenueId] = '{1}' AND [C].[AuditFlag] < 2 AND {2} ([RatedMenuItemCount] > 0 OR [AnswerCountToSurvey] > 0)
                                ORDER BY [C].[TimeUTC] DESC;";
                        
            string cutOffTimeQuery = String.Empty;
            if(tillUTC.HasValue)
            {
                string cutOffTime = tillUTC.Value.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                cutOffTimeQuery = String.Format(CultureInfo.InvariantCulture, "[C].[TimeUTC] < '{0}' AND", cutOffTime);                
            }

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, top, venueId, cutOffTimeQuery, currentVenueUser);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.DinerId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.VenueId = reader.GetGuid(colIndex++);            
            this.TimeUTC = reader.GetDateTime(colIndex++);
            this.AuditFlag = (AuditFlag)reader.GetByte(colIndex++);
            this.RatedMenuItemCount = reader.GetInt32(colIndex++);
            this.AnswerCountToSurvey = reader.GetInt32(colIndex++);
            this.CheckinCountInTotal = reader.GetInt32(colIndex++);
            this.CheckinCountAtVenue = reader.GetInt32(colIndex++);
            this.SentCouponCount = reader.GetInt32(colIndex++);
            this.SentMessageCount = reader.GetInt32(colIndex++);
            this.ReadByManager = reader.GetInt32(colIndex++);
        }

        /// <summary>
        /// This method returns the recent checkins for a restaurant.
        /// It skips those checkins which don't have any feedback in it.
        /// ...
        /// This is to be called when we first show the Feedbacks page.
        /// It will be also called when  user clicks 'Show More' link.
        /// In that case 'tillUTC' needs to be filled out.
        /// </summary>
        /// <param name="connection">Opened database connection</param>
        /// <param name="venueId">Target venue</param>
        /// <param name="top">Page Size to be used in SELECT TOP clause</param>
        /// <param name="tillUTC">This is to be called when user clicks 'Show More' link on the Feedbacks page</param>
        /// <param name="timeoutSec">Max time for the SQL execution</param>
        /// <returns></returns>
        public static CheckinList GetRecentCheckins(SqlConnection connection, Guid venueId, int top, DateTime? tillUTC, Guid currentVenueUser, int timeoutSec)
        {
            CheckinList checkinList = new CheckinList();
            string query = Checkin.RecentCheckinsQuery(venueId, top, tillUTC, currentVenueUser);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = timeoutSec;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Checkin checkin = new Checkin();
                            checkin.InitFromSqlReader(reader);
                            checkinList.Add(checkin);
                        }
                    }
                }
            }

            return checkinList;
        }

        public static string BulkFilter(CheckinList checkinList, string colName)
        {
            string clause = String.Empty;

            for (int x = 0; x < checkinList.Count; x++)
            {
                clause += colName + " = '" + checkinList[x].Id.ToString() + "'";
                if (x != checkinList.Count - 1)
                {
                    clause += " OR ";
                }
            }

            return clause;
        }
    }
}
