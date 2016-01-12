using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class Address : IEditable
    {
        public string SelectQuery()
        {
            this.Check();

            string query = @"SELECT [AddressLine1] 
                                   ,[AddressLine2]
                                   ,[City]
                                   ,[State]
                                   ,[ZipCode]
                                   ,[Country]
                               FROM [dbo].[Address]
                               WHERE [ObjectType] = {0} AND 
		                             [AddressType] = {1} AND 
		                             [ObjectId] = '{2}';";

            query = Database.ShortenQuery(query);

            query = String.Format(
                CultureInfo.InvariantCulture, 
                query, 
                (int)this.ObjectType, 
                (int)this.AddressType, 
                this.ObjectId);

            return query;
        }

        public string SelectAllQuery(Guid groupId)
        {
            throw new NotSupportedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            this.Check();

            int colIndex = 0;
            this.InitFromSqlReader(reader, ref colIndex);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
            {
                this.AddressLine1 = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.AddressLine2 = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.City = reader.GetString(colIndex);
                this.City = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(this.City.ToLower());
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.State = reader.GetString(colIndex).ToUpper();
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.ZipCode = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.Country = reader.GetString(colIndex);
            }
            colIndex++;
        }

        public string InsertOrUpdateQuery()
        {
            this.Check();

            string query = @"   declare @count int;
                                set @count = (SELECT COUNT(*) FROM [dbo].[Address]
					                                WHERE [ObjectType] = @ObjectType AND
						                                  [ObjectId] = @ObjectId AND
						                                  [AddressType] = @AddressType);
                                IF(@count = 0)
                                BEGIN
	                                INSERT INTO [dbo].[Address]
                                               ([ObjectType]
                                               ,[ObjectId]
                                               ,[AddressType]
                                               ,[AddressLine1]
                                               ,[AddressLine2]
                                               ,[City]
                                               ,[State]
                                               ,[ZipCode]
                                               ,[Country])
                                         VALUES
                                               (@ObjectType,
                                                @ObjectId,
                                                @AddressType,
                                                @AddressLine1,
                                                @AddressLine2,
                                                @City,
                                                @State,
                                                @ZipCode,
                                                @Country);
                                END
                                ELSE
                                BEGIN
                                    UPDATE [dbo].[Address] SET 
			                                   [AddressLine1] = @AddressLine1,
			                                   [AddressLine2] = @AddressLine2,
			                                   [City] = @City,
			                                   [State] = @State,
			                                   [ZipCode] = @ZipCode,
			                                   [Country] = @Country
		                                 WHERE [ObjectType] = @ObjectType AND
			                                   [ObjectId] = @ObjectId AND
			                                   [AddressType] = @AddressType;
                                END;";

            return Database.ShortenQuery(query);
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            this.Check();

            command.Parameters.AddWithValue("@ObjectType", (byte)this.ObjectType);
            command.Parameters.AddWithValue("@ObjectId", this.ObjectId);
            command.Parameters.AddWithValue("@AddressType", (byte)this.AddressType);

            if (operation != DBaseOperation.Delete)
            {
                command.Parameters.AddWithValue("@AddressLine1", this.AddressLine1);
                command.Parameters.AddWithValue("@AddressLine2", String.IsNullOrWhiteSpace(this.AddressLine2) ? (object)DBNull.Value : this.AddressLine2);
                command.Parameters.AddWithValue("@City", this.City);
                command.Parameters.AddWithValue("@State", this.State);
                command.Parameters.AddWithValue("@ZipCode", this.ZipCode);
                command.Parameters.AddWithValue("@Country", this.Country);
            }
        }

        public string DeleteQuery()
        {
            string query = @"DELETE FROM [dbo].[Address] 
		                         WHERE [ObjectType] = @ObjectType AND
			                           [ObjectId] = @ObjectId AND
			                           [AddressType] = @AddressType;";

            return Database.ShortenQuery(query);
        }

        private void Check()
        {
            if (this.ObjectType == ObjectType.Unspecified)
            {
                throw new YummyZoneArgumentException("Address Object Type has not been specified yet");
            }
            if (this.AddressType == AddressType.Unspecified)
            {
                throw new YummyZoneArgumentException("Address Type has not been specified yet");
            }
            if (this.ObjectId == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Address Object Id is empty");
            }
        }

        private static string QueryForVenueCountPerCity(VenueStatus status)
        {
            string query = @"   SELECT [Table1].[State], [Table1].[City], [Table1].[CityCount], [Table2].[StateCount] FROM
                                (
	                                SELECT [State], [City], COUNT(*) [CityCount] FROM [dbo].[Address] WHERE [ObjectId] IN 
	                                ( SELECT [Id] FROM [dbo].[Venue] WHERE [Status] = {0}) GROUP BY [State], [City]
                                ) [Table1]
                                JOIN 
                                (
	                                SELECT [State], COUNT(*) [StateCount] FROM [dbo].[Address] WHERE [ObjectId] IN 
	                                ( SELECT [B].[Id] FROM [dbo].[Venue] [B] WHERE [B].[Status] = {0}) GROUP BY [State]
                                ) [Table2] ON [Table1].[State] = [Table2].[State]
                                ORDER BY [Table2].[StateCount] DESC, [Table1].[CityCount] DESC, [Table1].[State], [Table1].[City]
                                ";

            query = Database.ShortenQuery(query);

            return String.Format(CultureInfo.InvariantCulture, query, (int)status);
        }

        public static List<string> Cities(SqlConnection connection, SqlTransaction transaction, int timeouts)
        {
            List<string> cities = new List<string>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = QueryForVenueCountPerCity(VenueStatus.Active);
                command.Transaction = transaction;
                command.CommandTimeout = timeouts;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string state = reader.GetString(0);
                        string city = reader.GetString(1);
                        string together = String.Format(
                            CultureInfo.InvariantCulture,
                            "{0}, {1}",
                            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.ToLower()),
                            state.ToUpper());

                        cities.Add(together);
                    }
                }
            }

            return cities;
        }
    }
}
