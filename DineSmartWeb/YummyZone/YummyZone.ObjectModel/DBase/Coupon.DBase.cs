using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Coupon : IEditable
    {
        public const int PageSize = 10;

        public string InsertOrUpdateQuery()
        {
            string query =
                @"INSERT INTO [dbo].[Coupon](
                     [GroupId]
                    ,[Id]
                    ,[CampaignId]
                    ,[SenderId]
                    ,[ChainId]
                    ,[ReceiverId]
                    ,[CheckInId]
                    ,[CouponType]
                    ,[FaceValue]
                    ,[ExpiryDateUTC]
                    ,[Code]
                    ,[Title]
                    ,[Content]
                    ,[QueueTimeUTC]
                    ,[PushTimeUTC]
                    ,[ReadTimeUTC]
                    ,[RedeemCheckInId]
                    ,[RedeemedValue]
                    ,[RedeemTimeUTC]
                    ,[DeleteTimeUTC])
                VALUES(
                     @GroupId
                    ,@Id
                    ,@CampaignId
                    ,@SenderId
                    ,@ChainId
                    ,@ReceiverId
                    ,@CheckInId
                    ,@CouponType
                    ,@FaceValue
                    ,@ExpiryDateUTC
                    ,@Code
                    ,@Title
                    ,@Content
                    ,@QueueTimeUTC
                    ,@PushTimeUTC
                    ,@ReadTimeUTC
                    ,@RedeemCheckInId
                    ,@RedeemedValue
                    ,@RedeemTimeUTC
                    ,@DeleteTimeUTC);";

            query = Database.ShortenQuery(query);

            return query;
        }

        public string SelectQuery()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"   SELECT [GroupId]
                                      ,[Id]
                                      ,[CampaignId]
                                      ,[SenderId]
                                      ,[ChainId]
                                      ,[ReceiverId]
                                      ,[CheckInId]
                                      ,[CouponType]
                                      ,[FaceValue]
                                      ,[ExpiryDateUTC]
                                      ,[Code]
                                      ,[Title]
                                      ,[Content]
                                      ,[QueueTimeUTC]
                                      ,[PushTimeUTC]
                                      ,[ReadTimeUTC]
                                      ,[RedeemCheckInId]
                                      ,[RedeemedValue]
                                      ,[RedeemTimeUTC]
                                      ,[DeleteTimeUTC]
                                      ,('') AS [FirstName]
                                      ,('') AS [LastName]
                                      ,(0) AS [ReceiverCheckinCountInTotal]
                                  FROM [dbo].[Coupon]              
                                  WHERE [Id] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.Id);
        }

        public string SelectAllQuery(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            this.InitFromSqlReader(reader, 0);
        }

        private void InitFromSqlReader(SqlDataReader reader, int colIndex)
        {
            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.CampaignId = reader.GetGuid(colIndex);
            }
            colIndex++;

            this.SenderId = reader.GetGuid(colIndex++);
            this.ChainId = reader.GetGuid(colIndex++);
            this.ReceiverId = reader.GetGuid(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.CheckInId = reader.GetGuid(colIndex);
            }
            colIndex++;

            this.CouponType = (CouponType)reader.GetByte(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.FaceValue = reader.GetDecimal(colIndex);
            }
            colIndex++;

            this.ExpiryDateUTC = reader.GetDateTime(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Code = reader.GetString(colIndex);
            }
            colIndex++;

            this.Title = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Content = reader.GetString(colIndex);
            }
            colIndex++;

            this.QueueTimeUTC = reader.GetDateTime(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.PushTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.ReadTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RedeemCheckInId = reader.GetGuid(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RedeemedValue = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RedeemTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.DeleteTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;

            this.SenderFirstName = reader.GetString(colIndex++);
            this.SenderLastName = reader.GetString(colIndex++);
            this.ReceiverCheckinCountInTotal = reader.GetInt32(colIndex++);
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();
        }

        public static int CountPerChannel(SqlConnection connection, Guid groupId, Guid chainId, Guid receiverId, DateTime sinceTimeUtc)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = CountPerChannelQuery(groupId, chainId, receiverId, sinceTimeUtc);
                command.CommandTimeout = Database.TimeoutSecs;
                int count = (int)command.ExecuteScalar();
                return count;
            }
        }

        public static int CountPerCheckin(SqlConnection connection, Guid checkinId)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = CountPerCheckinQuery(checkinId);
                command.CommandTimeout = Database.TimeoutSecs;
                int count = (int)command.ExecuteScalar();
                return count;
            }
        }

        private static string CountPerChannelQuery(Guid groupId, Guid chainId, Guid receiverId, DateTime sinceTimeUtc)
        {
            string query = "SELECT COUNT (*) FROM [dbo].[Coupon] WHERE [GroupId] = '{0}' AND [ChainId] = '{1}' AND [ReceiverId] = '{2}' AND [QueueTimeUTC] >= '{3}'";
            return String.Format(CultureInfo.InvariantCulture, query, groupId, chainId, receiverId, sinceTimeUtc.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }

        private static string CountPerCheckinQuery(Guid checkinId)
        {
            string query = "SELECT COUNT (*) FROM [dbo].[Coupon] WHERE [CheckInId] = '{0}'";
            return String.Format(CultureInfo.InvariantCulture, query, checkinId);
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            this.Validate();

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@Id", this.Id);
            command.Parameters.AddWithValue("@CampaignId", this.CampaignId.HasValue ? (object)this.CampaignId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@SenderId", this.SenderId);
            command.Parameters.AddWithValue("@ChainId", this.ChainId);
            command.Parameters.AddWithValue("@ReceiverId", this.ReceiverId);
            command.Parameters.AddWithValue("@CheckInId", this.CheckInId.HasValue ? (object)this.CheckInId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@CouponType", (byte)this.CouponType);
            command.Parameters.AddWithValue("@FaceValue", this.FaceValue.HasValue ? (object)this.FaceValue.Value : DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDateUTC", this.ExpiryDateUTC);
            command.Parameters.AddWithValue("@Code", String.IsNullOrWhiteSpace(this.Code) ? DBNull.Value : (object)this.Code);
            command.Parameters.AddWithValue("@Title", this.Title);
            command.Parameters.AddWithValue("@Content", String.IsNullOrWhiteSpace(this.Content) ? DBNull.Value : (object)this.Content);
            command.Parameters.AddWithValue("@QueueTimeUTC", this.QueueTimeUTC);
            command.Parameters.AddWithValue("@PushTimeUTC", this.PushTimeUTC.HasValue ? (object)this.PushTimeUTC.Value : DBNull.Value);
            command.Parameters.AddWithValue("@ReadTimeUTC", this.ReadTimeUTC.HasValue ? (object)this.ReadTimeUTC.Value : DBNull.Value);
            command.Parameters.AddWithValue("@RedeemCheckInId", this.RedeemCheckInId.HasValue ? (object)this.RedeemCheckInId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@RedeemedValue", this.RedeemedValue.HasValue ? (object)this.RedeemedValue.Value : DBNull.Value);
            command.Parameters.AddWithValue("@RedeemTimeUTC", this.RedeemTimeUTC.HasValue ? (object)this.RedeemTimeUTC.Value : DBNull.Value);
            command.Parameters.AddWithValue("@DeleteTimeUTC", this.DeleteTimeUTC.HasValue ? (object)this.DeleteTimeUTC.Value : DBNull.Value);
        }

        private void Validate()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            if (this.SenderId == Guid.Empty)
            {
                throw new ArgumentException("Unknown SenderId");
            }

            if (this.ChainId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChainId");
            }

            if (this.ReceiverId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ReceiverId");
            }

            if (String.IsNullOrWhiteSpace(this.Title))
            {
                throw new ArgumentException("Invalid Title");
            }
        }

        public static CouponList GetRecentCouponNews(SqlConnection connection, Guid chainId, DateTime tillUTC, int timeOutSec)
        {
            CouponList couponList = new CouponList();
            string query = GetRecentCouponNewsQuery(chainId, tillUTC);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = timeOutSec;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Coupon coupon = new Coupon();
                            // DateTime maxTimeUtc = reader.GetDateTime(0); // skip the first column
                            coupon.InitFromSqlReader(reader, 1);
                            couponList.Add(coupon);
                        }
                    }
                }
            }

            return couponList;
        }

        private static string GetRecentCouponNewsQuery(Guid chainId, DateTime tillUTC)
        {
            string query =
              @"SELECT TOP {0} 
	               CASE WHEN [C].[RedeemTimeUTC] is NOT NULL AND [C].[QueueTimeUTC] < [C].[RedeemTimeUTC] 
	               THEN [C].[RedeemTimeUTC] ELSE [C].[QueueTimeUTC] END AS [MaxTimeUTC]
	              ,[C].[GroupId]
                  ,[C].[Id]
                  ,[C].[CampaignId]
                  ,[C].[SenderId]
                  ,[C].[ChainId]
                  ,[C].[ReceiverId]
                  ,[C].[CheckInId]
                  ,[C].[CouponType]
                  ,[C].[FaceValue]
                  ,[C].[ExpiryDateUTC]
                  ,[C].[Code]
                  ,[C].[Title]
                  ,[C].[Content]
                  ,[C].[QueueTimeUTC]
                  ,[C].[PushTimeUTC]
                  ,[C].[ReadTimeUTC]
                  ,[C].[RedeemCheckInId]
                  ,[C].[RedeemedValue]
                  ,[C].[RedeemTimeUTC]
                  ,[C].[DeleteTimeUTC]
                  ,[U].[FirstName]
                  ,[U].[LastName]
                  ,[ReceiverCheckinCountInTotal]
              FROM [dbo].[Coupon] [C]
              JOIN [dbo].[User] [U] ON [C].[SenderId] = [U].[Id]
              CROSS APPLY
	                (
		                SELECT [ReceiverCheckinCountInTotal] = (SELECT COUNT(*) FROM [dbo].[CheckIn] WHERE [DinerId] = [C].[ReceiverId])
	                )
	                AS [CheckinCountInTotalQuery]
              WHERE [ChainId] = '{1}' AND 
		            [QueueTimeUTC] < '{2}' AND 
                    (
                        [RedeemTimeUTC] < '{2}'  OR 
                        ([RedeemTimeUTC] is NULL AND [ExpiryDateUTC] > GETUTCDATE())
                    )
              ORDER BY [MaxTimeUTC] DESC;";

            query = Database.ShortenQuery(query);
            string cutOffTime = tillUTC.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            query = String.Format(CultureInfo.InvariantCulture, query, PageSize, chainId, cutOffTime);

            return query;
        }

        public static int GetCouponCount(SqlConnection connection, Guid chainId, DateTime startUtcInclude, DateTime endUtcExcluded, bool isRedeemed, int timeOutSec)
        {
            string query = GetCouponCountQuery(isRedeemed);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = timeOutSec;
                command.Parameters.AddWithValue("@ChainId", chainId);
                command.Parameters.AddWithValue("@TimeMin", startUtcInclude);
                command.Parameters.AddWithValue("@TimeMax", endUtcExcluded);

                return (int)command.ExecuteScalar();
            }
        }

        private static string GetCouponCountQuery(bool isRedeemed)
        {
            string query = String.Empty;

            if (isRedeemed)
            {
                query = @"SELECT COUNT (*) FROM [dbo].[Coupon]
                                WHERE [ChainId] = @ChainId AND
		                            [RedeemTimeUTC] is NOT NULL AND
		                            [RedeemTimeUTC] >= @TimeMin AND 
		                            [RedeemTimeUTC] < @TimeMax";
            }
            else
            {
                query = @"SELECT COUNT (*) FROM [dbo].[Coupon]
                              WHERE [ChainId] = @ChainId AND
		                            [QueueTimeUTC] >= @TimeMin AND 
		                            [QueueTimeUTC] < @TimeMax";
            }

            return Database.ShortenQuery(query);
        }
    }
}
