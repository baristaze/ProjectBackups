using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class NotificationClient : IEditable
    {
        public string SelectQuery()
        {
            throw new NotSupportedException();
        }

        public string SelectAllQuery(Guid dinerId)
        {
            if (dinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            string query = @"   SELECT [DinerId]
                                      ,[DeviceType]
                                      ,[DeviceToken]
                                      ,[CreateTimeUTC]
                                      ,[LastUpdateTimeUTC]
                                  FROM [dbo].[NotificationClient]
                                  WHERE [DinerId] = '{0}'
                                  ORDER BY [LastUpdateTimeUTC] DESC";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, dinerId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.DinerId = reader.GetGuid(colIndex++);
            this.DeviceType = (MobileDeviceType)reader.GetByte(colIndex++);
            this.DeviceToken = reader.GetString(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        public string DeleteQuery()
        {
            if (String.IsNullOrWhiteSpace(DeviceToken))
            {
                throw new ArgumentException("Unknown DeviceToken");
            }

            return @"DELETE FROM [dbo].[NotificationClient] WHERE [DeviceToken] = @DeviceToken";
        }

        public string InsertOrUpdateQuery()
        {
            string query = @"   IF NOT EXISTS (SELECT * FROM [dbo].[NotificationClient] WHERE [DeviceToken] = @DeviceToken)
                                BEGIN
	                                INSERT INTO [dbo].[NotificationClient]
                                           ([DinerId]
                                           ,[DeviceType]
                                           ,[DeviceToken]
                                           ,[CreateTimeUTC]
                                           ,[LastUpdateTimeUTC])
                                     VALUES
                                           (@DinerId
                                           ,@DeviceType
                                           ,@DeviceToken
                                           ,@CreateTimeUTC
                                           ,@LastUpdateTimeUTC);
                                END
                                ELSE
                                BEGIN
	                                UPDATE [dbo].[NotificationClient]
		                                SET [DinerId] = @DinerId, 
		                                    [DeviceType] = @DeviceType,
		                                    [LastUpdateTimeUTC] = @LastUpdateTimeUTC
		                                WHERE [DeviceToken] = @DeviceToken;
                                END;";

            return Database.ShortenQuery(query);

        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (String.IsNullOrEmpty(this.DeviceToken))
            {
                throw new ArgumentException("Unknown DeviceToken");
            }
            
            command.Parameters.AddWithValue("@DeviceToken", this.DeviceToken);

            if (operation != DBaseOperation.Delete)
            {
                if (this.DinerId == Guid.Empty)
                {
                    throw new ArgumentException("Unknown DinerId");
                }

                if (this.DeviceType == MobileDeviceType.None)
                {
                    throw new ArgumentException("Unknown DeviceType");
                }

                command.Parameters.AddWithValue("@DinerId", this.DinerId);
                command.Parameters.AddWithValue("@DeviceType", (byte)this.DeviceType);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }
    }
}
