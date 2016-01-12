using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class Venue : YummyZoneEntity, IEditable
    {   
        public string InsertOrUpdateQuery()
        {
            this.Check();

            string query = @"
                    declare @count int;
                    set @count = (SELECT COUNT(*) FROM [dbo].[Venue] WHERE [Id] = @Id AND [GroupId] = @GroupId);
                    IF (@count = 0)
                    BEGIN
                        INSERT INTO [dbo].[Venue] ( [GroupId]
                                                   ,[Id]
                                                   ,[ChainId]
                                                   ,[Status]
                                                   ,[Name]
                                                   ,[Latitude]
                                                   ,[Longitude]
                                                   ,[MapURL]
                                                   ,[WebURL]
                                                   ,[TimeZoneWinIndex]                                                   
                                                   ,[SearchVenue_LatitudeThreshold]
                                                   ,[SearchVenue_LongitudeThreshold]
                                                   ,[SearchVenue_RangeLimitInMiles]
                                                   ,[RedeemCoupon_LatitudeThreshold]
                                                   ,[RedeemCoupon_LongitudeThreshold]
                                                   ,[RedeemCoupon_RangeLimitInMiles]
                                                   ,[SendFeedback_RangeLimitInMiles]
                                                   ,[CreateTimeUTC]
                                                   ,[LastUpdateTimeUTC])
                                             VALUES
                                                   (@GroupId,
                                                    @Id,
                                                    @ChainId,
                                                    @Status,
                                                    @Name,
                                                    @Latitude,
                                                    @Longitude,
                                                    @MapURL,
                                                    @WebURL,
                                                    @TimeZoneWinIndex,
                                                    @SearchVenue_LatitudeThreshold,
                                                    @SearchVenue_LongitudeThreshold,
                                                    @SearchVenue_RangeLimitInMiles,
                                                    @RedeemCoupon_LatitudeThreshold,
                                                    @RedeemCoupon_LongitudeThreshold,
                                                    @RedeemCoupon_RangeLimitInMiles,
                                                    @SendFeedback_RangeLimitInMiles,
                                                    @CreateTimeUTC,
                                                    @LastUpdateTimeUTC);
                    END
                    ELSE
                    BEGIN
                        UPDATE [dbo].[Venue]
                               SET [ChainId] = @ChainId
                                  ,[Status] = @Status
                                  ,[Name] = @Name
                                  ,[Latitude] = @Latitude
                                  ,[Longitude] = @Longitude
                                  ,[MapURL] = @MapURL
                                  ,[WebURL] = @WebURL
                                  ,[TimeZoneWinIndex] = @TimeZoneWinIndex
                                  ,[SearchVenue_LatitudeThreshold] = @SearchVenue_LatitudeThreshold
                                  ,[SearchVenue_LongitudeThreshold] = @SearchVenue_LongitudeThreshold
                                  ,[SearchVenue_RangeLimitInMiles] = @SearchVenue_RangeLimitInMiles
                                  ,[RedeemCoupon_LatitudeThreshold] = @RedeemCoupon_LatitudeThreshold
                                  ,[RedeemCoupon_LongitudeThreshold] = @RedeemCoupon_LongitudeThreshold
                                  ,[RedeemCoupon_RangeLimitInMiles] = @RedeemCoupon_RangeLimitInMiles
                                  ,[SendFeedback_RangeLimitInMiles] = @SendFeedback_RangeLimitInMiles
                                  ,[CreateTimeUTC] = @CreateTimeUTC
                                  ,[LastUpdateTimeUTC] = @LastUpdateTimeUTC
                             WHERE 
	                                [Id] = @Id AND [GroupId] = @GroupId;
                    END;";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();  
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            this.Check();

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@Id", this.Id);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@ChainId", this.ChainId);
                command.Parameters.AddWithValue("@Status", (byte)this.Status);
                command.Parameters.AddWithValue("@Name", this.Name);
                command.Parameters.AddWithValue("@Latitude", this.Latitude.HasValue ? this.Latitude.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Longitude", this.Longitude.HasValue ? this.Longitude.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@MapURL", String.IsNullOrWhiteSpace(this.MapURL) ? DBNull.Value : (object)this.MapURL);
                command.Parameters.AddWithValue("@WebURL", String.IsNullOrWhiteSpace(this.WebURL) ? DBNull.Value : (object)this.WebURL);
                command.Parameters.AddWithValue("@TimeZoneWinIndex", this.TimeZoneWinIndex.HasValue ? this.TimeZoneWinIndex.Value : (object)DBNull.Value);

                command.Parameters.AddWithValue("@SearchVenue_LatitudeThreshold", this.LatitudeThresholdForSearch.HasValue ? this.LatitudeThresholdForSearch.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SearchVenue_LongitudeThreshold", this.LongitudeThresholdForSearch.HasValue ? this.LongitudeThresholdForSearch.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SearchVenue_RangeLimitInMiles", this.RangeLimitInMilesForSearch.HasValue ? this.RangeLimitInMilesForSearch.Value : (object)DBNull.Value);

                command.Parameters.AddWithValue("@RedeemCoupon_LatitudeThreshold", this.LatitudeThresholdForRedeem.HasValue ? this.LatitudeThresholdForRedeem.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@RedeemCoupon_LongitudeThreshold", this.LongitudeThresholdForRedeem.HasValue ? this.LongitudeThresholdForRedeem.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@RedeemCoupon_RangeLimitInMiles", this.RangeLimitInMilesForRedeem.HasValue ? this.RangeLimitInMilesForRedeem.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SendFeedback_RangeLimitInMiles", this.RangeLimitInMilesForFeedback.HasValue ? this.RangeLimitInMilesForFeedback.Value : (object)DBNull.Value);

                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public static string SelectQuery(Guid venueId)
        {
            if (venueId == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"SELECT  [V].[GroupId]
                                    ,[V].[Id]
                                    ,[V].[ChainId]
                                    ,[V].[Status]
                                    ,[V].[Name]
                                    ,[V].[Latitude]
                                    ,[V].[Longitude]
                                    ,[V].[MapURL]
                                    ,[V].[WebURL]
                                    ,[V].[TimeZoneWinIndex]
                                    ,[V].[SearchVenue_LatitudeThreshold]
                                    ,[V].[SearchVenue_LongitudeThreshold]
                                    ,[V].[SearchVenue_RangeLimitInMiles]
                                    ,[V].[RedeemCoupon_LatitudeThreshold]
                                    ,[V].[RedeemCoupon_LongitudeThreshold]
                                    ,[V].[RedeemCoupon_RangeLimitInMiles]
                                    ,[V].[SendFeedback_RangeLimitInMiles]
                                    ,[V].[CreateTimeUTC]
                                    ,[V].[LastUpdateTimeUTC]      
                                    ,[A].[AddressLine1]
                                    ,[A].[AddressLine2]
                                    ,[A].[City]
                                    ,[A].[State]
                                    ,[A].[ZipCode]
                                    ,[A].[Country]
                                FROM [dbo].[Venue] [V]
                                LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND 
			                            [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                WHERE [V].[Id] = '{2}';";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                venueId);
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

            string query = @"SELECT  [V].[GroupId]
                                    ,[V].[Id]
                                    ,[V].[ChainId]
                                    ,[V].[Status]
                                    ,[V].[Name]
                                    ,[V].[Latitude]
                                    ,[V].[Longitude]
                                    ,[V].[MapURL]
                                    ,[V].[WebURL]
                                    ,[V].[TimeZoneWinIndex]
                                    ,[V].[SearchVenue_LatitudeThreshold]
                                    ,[V].[SearchVenue_LongitudeThreshold]
                                    ,[V].[SearchVenue_RangeLimitInMiles]
                                    ,[V].[RedeemCoupon_LatitudeThreshold]
                                    ,[V].[RedeemCoupon_LongitudeThreshold]
                                    ,[V].[RedeemCoupon_RangeLimitInMiles]
                                    ,[V].[SendFeedback_RangeLimitInMiles]
                                    ,[V].[CreateTimeUTC]
                                    ,[V].[LastUpdateTimeUTC]      
                                    ,[A].[AddressLine1]
                                    ,[A].[AddressLine2]
                                    ,[A].[City]
                                    ,[A].[State]
                                    ,[A].[ZipCode]
                                    ,[A].[Country]
                                FROM [dbo].[Venue] [V]
                                LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND 
			                            [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                WHERE [V].[GroupId] = '{2}' AND [V].[Id] = '{3}';";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture, 
                query, 
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                this.GroupId,
                this.Id);
        }

        public string SelectQueryWithIdOnly()
        {
            if (this.Id == Guid.Empty)
            {
                throw new ArgumentException("Unknown Id");
            }

            string query = @"SELECT  [V].[GroupId]
                                    ,[V].[Id]
                                    ,[V].[ChainId]
                                    ,[V].[Status]
                                    ,[V].[Name]
                                    ,[V].[Latitude]
                                    ,[V].[Longitude]
                                    ,[V].[MapURL]
                                    ,[V].[WebURL]
                                    ,[V].[TimeZoneWinIndex]
                                    ,[V].[SearchVenue_LatitudeThreshold]
                                    ,[V].[SearchVenue_LongitudeThreshold]
                                    ,[V].[SearchVenue_RangeLimitInMiles]
                                    ,[V].[RedeemCoupon_LatitudeThreshold]
                                    ,[V].[RedeemCoupon_LongitudeThreshold]
                                    ,[V].[RedeemCoupon_RangeLimitInMiles]
                                    ,[V].[SendFeedback_RangeLimitInMiles]
                                    ,[V].[CreateTimeUTC]
                                    ,[V].[LastUpdateTimeUTC]      
                                    ,[A].[AddressLine1]
                                    ,[A].[AddressLine2]
                                    ,[A].[City]
                                    ,[A].[State]
                                    ,[A].[ZipCode]
                                    ,[A].[Country]
                                FROM [dbo].[Venue] [V]
                                LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND 
			                            [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                WHERE [V].[Id] = '{2}';";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                this.Id);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"   SELECT [V].[GroupId]
                                      ,[V].[Id]
                                      ,[V].[ChainId]
                                      ,[V].[Status]
                                      ,[V].[Name]
                                      ,[V].[Latitude]
                                      ,[V].[Longitude]
                                      ,[V].[MapURL]
                                      ,[V].[WebURL]
                                      ,[V].[TimeZoneWinIndex]
                                      ,[V].[SearchVenue_LatitudeThreshold]
                                      ,[V].[SearchVenue_LongitudeThreshold]
                                      ,[V].[SearchVenue_RangeLimitInMiles]
                                      ,[V].[RedeemCoupon_LatitudeThreshold]
                                      ,[V].[RedeemCoupon_LongitudeThreshold]
                                      ,[V].[RedeemCoupon_RangeLimitInMiles]
                                      ,[V].[SendFeedback_RangeLimitInMiles]
                                      ,[V].[CreateTimeUTC]
                                      ,[V].[LastUpdateTimeUTC]      
                                      ,[A].[AddressLine1]
                                      ,[A].[AddressLine2]
                                      ,[A].[City]
                                      ,[A].[State]
                                      ,[A].[ZipCode]
                                      ,[A].[Country]
                                  FROM [dbo].[Venue] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND 
			                                [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                  WHERE [V].[GroupId] = '{2}'
                                  ORDER BY [V].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                groupId);
        }

        public static string SelectByGeoLocationQuery(double userLatitude, double userLongitude, double latThresold, double longThresold, bool isRedeem)
        {
            string thresholdColLat = isRedeem ? "[RedeemCoupon_LatitudeThreshold]" : "[SearchVenue_LatitudeThreshold]";
            string thresholdColLng = isRedeem ? "[RedeemCoupon_LongitudeThreshold]" : "[SearchVenue_LongitudeThreshold]";

            string query = @"   SELECT [V].[GroupId]
                                      ,[V].[Id]
                                      ,[V].[ChainId]
                                      ,[V].[Status]
                                      ,[V].[Name]
                                      ,[V].[Latitude]
                                      ,[V].[Longitude]
                                      ,[V].[MapURL]
                                      ,[V].[WebURL]
                                      ,[V].[TimeZoneWinIndex]
                                      ,[V].[SearchVenue_LatitudeThreshold]
                                      ,[V].[SearchVenue_LongitudeThreshold]
                                      ,[V].[SearchVenue_RangeLimitInMiles]
                                      ,[V].[RedeemCoupon_LatitudeThreshold]
                                      ,[V].[RedeemCoupon_LongitudeThreshold]
                                      ,[V].[RedeemCoupon_RangeLimitInMiles]
                                      ,[V].[SendFeedback_RangeLimitInMiles]
                                      ,[V].[CreateTimeUTC]
                                      ,[V].[LastUpdateTimeUTC]      
                                      ,[A].[AddressLine1]
                                      ,[A].[AddressLine2]
                                      ,[A].[City]
                                      ,[A].[State]
                                      ,[A].[ZipCode]
                                      ,[A].[Country]
                                  FROM [dbo].[Venue] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                  WHERE [Status] = {2} AND [Latitude] is NOT NULL AND [Longitude] is NOT NULL AND
                                        ABS([Latitude] - {3}) <= COALESCE({7}, {5}) AND
	                                    ABS([Longitude] - {4}) <= COALESCE({8}, {6});";

            
            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                (int)VenueStatus.Active,
                userLatitude,
                userLongitude,
                latThresold,
                longThresold,
                thresholdColLat,
                thresholdColLng);
        }

        public static string SelectAllByUserId(Guid groupId, Guid userId)
        {
            if (groupId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Group Id");
            }

            string query = @"   SELECT [V].[GroupId]
                                      ,[V].[Id]
                                      ,[V].[ChainId]
                                      ,[V].[Status]
                                      ,[V].[Name]
                                      ,[V].[Latitude]
                                      ,[V].[Longitude]
                                      ,[V].[MapURL]
                                      ,[V].[WebURL]
                                      ,[V].[TimeZoneWinIndex]
                                      ,[V].[SearchVenue_LatitudeThreshold]
                                      ,[V].[SearchVenue_LongitudeThreshold]
                                      ,[V].[SearchVenue_RangeLimitInMiles]
                                      ,[V].[RedeemCoupon_LatitudeThreshold]
                                      ,[V].[RedeemCoupon_LongitudeThreshold]
                                      ,[V].[RedeemCoupon_RangeLimitInMiles]
                                      ,[V].[SendFeedback_RangeLimitInMiles]
                                      ,[V].[CreateTimeUTC]
                                      ,[V].[LastUpdateTimeUTC]      
                                      ,[A].[AddressLine1]
                                      ,[A].[AddressLine2]
                                      ,[A].[City]
                                      ,[A].[State]
                                      ,[A].[ZipCode]
                                      ,[A].[Country]
                                  FROM [dbo].[Venue] [V]
                                  LEFT JOIN [dbo].[Address] [A] ON [A].[ObjectType] = {0} AND 
			                                [A].[AddressType] = {1} AND [A].[ObjectId] = [V].[Id]
                                  WHERE [V].[GroupId] = '{2}' AND [V].[Id] IN 
                                        (SELECT DISTINCT [VenueId] FROM [dbo].[UserRole] WHERE [GroupId] = '{2}' AND [UserId] = '{3}')
                                  ORDER BY [V].[Name], [A].[City], [V].[CreateTimeUTC] ASC;";

            query = Database.ShortenQuery(query);

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                (int)ObjectType.Venue,
                (int)AddressType.BusinessAddress,
                groupId,
                userId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.Id = reader.GetGuid(colIndex++);
            this.ChainId = reader.GetGuid(colIndex++);
            this.Status = (VenueStatus)reader.GetByte(colIndex++);
            this.Name = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Latitude = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.Longitude = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.MapURL = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.WebURL = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.TimeZoneWinIndex = reader.GetByte(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.LatitudeThresholdForSearch = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.LongitudeThresholdForSearch = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RangeLimitInMilesForSearch = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.LatitudeThresholdForRedeem = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.LongitudeThresholdForRedeem = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RangeLimitInMilesForRedeem = reader.GetDecimal(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.RangeLimitInMilesForFeedback = reader.GetDecimal(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);

            this.Address = new Address();
            this.Address.ObjectType = ObjectType.Venue;
            this.Address.ObjectId = this.Id;
            this.Address.AddressType = AddressType.BusinessAddress;

            this.Address.InitFromSqlReader(reader, ref colIndex);

            if (this.Address.IsEmpty())
            {
                this.Address = null;
            }
        }

        private void Check()
        {
            if (this.Id == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Unknown Id");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Unknown GroupId");
            }

            if (this.ChainId == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Unknown ChainId");
            }

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new YummyZoneArgumentException("Invalid Name");
            }
        }
    }
}
