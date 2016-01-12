using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class DinerSettingsItem : IEditable
    {
        public string SelectQuery()
        {
            throw new NotSupportedException();

            /*
            if (this.DinerId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Diner Id");
            }

            string query = @"SELECT [DinerId],
                                    [Name],
                                    [Value],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC]
                          FROM [dbo].[DinerSettings]
                          WHERE [DinerId] = '{0}' AND [Name] = '{1}'"; // do not do that!

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.DinerId, this.Name);
            */
        }

        public string SelectAllQuery(Guid dinerId)
        {
            if (dinerId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Diner Id");
            }

            string query = @"SELECT [DinerId],
                                    [Name],
                                    [Value],
                                    [CreateTimeUTC],
                                    [LastUpdateTimeUTC]
                          FROM [dbo].[DinerSettings]
                          WHERE [DinerId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, dinerId);
        }

        public string DeleteQuery()
        {
            if (this.DinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new ArgumentException("Unknown Settings Name");
            }

            return "DELETE FROM [dbo].[DinerSettings] WHERE [DinerId] = @DinerId AND [Name] = @Name";
        }

        public static string DeleteByDinerIdQuery(Guid dinerId)
        {
            if (dinerId == Guid.Empty)
            {
                throw new YummyZoneException("Unknown Diner Id");
            }

            string query = @"DELETE FROM [dbo].[DinerSettings] WHERE [DinerId] = '{0}';";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, dinerId);
        }

        public string InsertOrUpdateQuery()
        {
            string query = @"   IF NOT EXISTS(SELECT * FROM [dbo].[DinerSettings] 
			                                WHERE [DinerId] = @DinerId AND [Name] = @Name)
                                BEGIN
	                                INSERT INTO [dbo].[DinerSettings]
			                                   ([DinerId]
			                                   ,[Name]
			                                   ,[Value]
			                                   ,[CreateTimeUTC]
			                                   ,[LastUpdateTimeUTC])
		                                 VALUES
			                                   (@DinerId
			                                   ,@Name
			                                   ,@Value
			                                   ,@CreateTimeUTC
			                                   ,@LastUpdateTimeUTC);
                                END
                                ELSE
                                BEGIN
	                                UPDATE [dbo].[DinerSettings]
	                                   SET [Value] = @Value
		                                  ,[LastUpdateTimeUTC] = @LastUpdateTimeUTC
	                                 WHERE [DinerId] = @DinerId AND [Name] = @Name;
                                END;";

            return Database.ShortenQuery(query);    
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.DinerId == Guid.Empty)
            {
                throw new ArgumentException("Unknown DinerId");
            }

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new ArgumentException("Unknown Settings Name");
            }

            command.Parameters.AddWithValue("@DinerId", this.DinerId);
            command.Parameters.AddWithValue("@Name", this.Name);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                command.Parameters.AddWithValue("@Value", this.Value == null ? (object)DBNull.Value : this.Value);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@LastUpdateTimeUTC", this.LastUpdateTimeUTC);
            }
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.DinerId = reader.GetGuid(colIndex++);
            this.Name = reader.GetString(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.Value = reader.GetString(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }
    }

    public partial class DinerSettings : List<DinerSettingsItem>
    {
        public static DinerSettings ReadFromDBase(SqlConnection connection, SqlTransaction trans, Guid dinerId)
        {
            return Database.SelectAll<DinerSettingsItem, DinerSettings>(connection, trans, dinerId, Database.TimeoutSecs);
        }

        public static DinerSettings Save(SqlConnection connection, SqlTransaction trans, DinerSettingsItem item)
        {
            Database.InsertOrUpdate(item, connection, trans, Database.TimeoutSecs);
            return Database.SelectAll<DinerSettingsItem, DinerSettings>(connection, trans, item.DinerId, Database.TimeoutSecs);
        }

        public static int Delete(SqlConnection connection, SqlTransaction trans, Guid dinerId)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = DinerSettingsItem.DeleteByDinerIdQuery(dinerId);
                command.Transaction = trans;
                command.CommandTimeout = Database.TimeoutSecs;

                return command.ExecuteNonQuery();
            }
        }
    }
}