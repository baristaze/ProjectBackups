using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public partial class City : BaseDistrict, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.Id = Database.GetInt32OrDefault(reader, ref colIndex);
            this.CountryId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.RegionId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.SubRegionId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.Name = Database.GetStringOrDefault(reader, ref colIndex);
            this.OrderIndex = Database.GetInt32OrDefault(reader, ref colIndex);
            this.WeightIndex = Database.GetInt32OrDefault(reader, ref colIndex);
            this.AddToAlternateNames(Database.GetStringOrDefault(reader, ref colIndex));
        }

        public static List<City> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId)
        {
            return Database.ExecSProc<City>(
                conn,
                trans,
                "[dbo].[GetCitiesByCountry]",
                Database.SqlParam("@CountryId", countryId));
        }

        public static List<City> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId, int regionId)
        {
            return Database.ExecSProc<City>(
                conn,
                trans,
                "[dbo].[GetCitiesByRegion]",
                Database.SqlParam("@CountryId", countryId),
                Database.SqlParam("@RegionId", regionId));
        }

        public static List<City> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId, int regionId, int subRegionId)
        {
            return Database.ExecSProc<City>(
                conn,
                trans,
                "[dbo].[GetCitiesBySubRegion]",
                Database.SqlParam("@CountryId", countryId),
                Database.SqlParam("@RegionId", regionId),
                Database.SqlParam("@SubRegionId", subRegionId));
        }
    }
}
