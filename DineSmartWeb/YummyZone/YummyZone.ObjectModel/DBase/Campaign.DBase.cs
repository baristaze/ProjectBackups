using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class Campaign : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            throw new NotImplementedException();
        }

        public string SelectQuery()
        {
            throw new NotImplementedException();
        }

        public string SelectAllQuery(Guid groupId)
        {
            string query = @"   SELECT [GroupId]
                                      ,[ChainId]
                                      ,[CreatorId]
                                      ,[Id]
                                      ,[Name]
                                      ,[Status]
                                      ,[CampaignType]
                                      ,[Priority]
                                      ,[CouponCount]
                                      ,(SELECT COUNT(*) FROM [dbo].[Coupon] WHERE [CampaignId] = [Id]) AS [DeliveredCouponCount]  
                                      ,[Repeatition]
                                      ,[PublishTimeUTC]
                                      ,[RevocationTimeUTC]
                                      ,[CouponType]
                                      ,[Title]
                                      ,[Content]
                                      ,[ExpiryDays]
                                      ,[FaceValue]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Campaign]
                                  WHERE [GroupId] = '{0}'
                                  ORDER BY [Priority] DESC";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, groupId);
        }

        public static string SelectCampaignsForNewSignupDiners()
        {
            string query = @"   SELECT [GroupId]
                                      ,[ChainId]
                                      ,[CreatorId]
                                      ,[Id]
                                      ,[Name]
                                      ,[Status]
                                      ,[CampaignType]
                                      ,[Priority]
                                      ,[CouponCount]
                                      ,(SELECT COUNT(*) FROM [dbo].[Coupon] WHERE [CampaignId] = [Id]) AS [DeliveredCouponCount]
                                      ,[Repeatition]
                                      ,[PublishTimeUTC]
                                      ,[RevocationTimeUTC]
                                      ,[CouponType]
                                      ,[Title]
                                      ,[Content]
                                      ,[ExpiryDays]
                                      ,[FaceValue]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[Campaign]
                                  WHERE [Status] = {0} 
	                                AND [CampaignType] = {1}
	                                AND [PublishTimeUTC] <= GETUTCDATE()
	                                AND [CouponCount] > 0
	                                AND ([RevocationTimeUTC] is NULL OR [RevocationTimeUTC] > GETUTCDATE())
                                  ORDER BY [Priority] DESC";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture, query, 
                (int)CampaignStatus.Active, 
                (int)CampaignType.AutoCouponOnSignup);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.ChainId = reader.GetGuid(colIndex++);
            this.CreatorId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.Name = reader.GetString(colIndex++);
            this.Status = (CampaignStatus)reader.GetByte(colIndex++);
            this.CampaignType = (CampaignType)reader.GetByte(colIndex++);
            this.Priority = reader.GetInt32(colIndex++);

            this.CouponCount = reader.GetInt32(colIndex++);
            this.DeliveredCouponCount = reader.GetInt32(colIndex++);
            this.Repeatition = (Repeatition)reader.GetByte(colIndex++);
            this.PublishTimeUTC = reader.GetDateTime(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.RevocationTimeUTC = reader.GetDateTime(colIndex++);
            }
            colIndex++;            

            this.CouponType = (CouponType)reader.GetByte(colIndex++);
            this.Title = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Content = reader.GetString(colIndex);
            }
            colIndex++;

            this.ExpiryDays = reader.GetInt32(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.FaceValue = reader.GetDecimal(colIndex);
            }
            colIndex++;
            
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            throw new NotSupportedException();
        }

        public static CampaignList CampaignsForNewSignupDiners(SqlConnection connection, SqlTransaction transaction, int timeoutSeconds)
        {
            string query = SelectCampaignsForNewSignupDiners();
            CampaignList list = Database.SelectAll<Campaign, CampaignList>(connection, transaction, query, timeoutSeconds);
            CampaignList list2 = new CampaignList();
            foreach (Campaign c in list)
            {
                if (c.DeliveredCouponCount < c.CouponCount)
                {
                    list2.Add(c);
                }
            }

            return list2;
        }

        public static CouponList CouponsForNewSignupDiners(CampaignList campaigns, Guid dinerId)
        {
            CouponList coupons = new CouponList();
            foreach (Campaign campaign in campaigns)
            {
                if (campaign.CanCreateCoupon())
                {
                    // create coupon from campaign
                    Coupon coupon = campaign.CreateCoupon(dinerId);

                    // we don't want to send push notifications for these coupons
                    // user will see it since he/she is downloading the app first time
                    // We will set the pushTime to mimic that the notification is already pushed
                    coupon.PushTimeUTC = coupon.QueueTimeUTC;

                    // add the coupons
                    coupons.Add(coupon);
                }
            }

            return coupons;
        }
    }
}
