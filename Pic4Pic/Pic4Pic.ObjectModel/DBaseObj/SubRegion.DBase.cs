using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public partial class SubRegion : BaseDistrict, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.Id = Database.GetInt32OrDefault(reader, ref colIndex);
            this.CountryId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.RegionId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.Name = Database.GetStringOrDefault(reader, ref colIndex);
            this.OrderIndex = Database.GetInt32OrDefault(reader, ref colIndex);
            this.AddToAlternateNames(Database.GetStringOrDefault(reader, ref colIndex));
        }

        public static List<SubRegion> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId)
        {
            return Database.ExecSProc<SubRegion>(
                conn,
                trans,
                "[dbo].[GetSubRegionsByCountryId]",
                Database.SqlParam("@CountryId", countryId));
        }

        public static List<SubRegion> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId, int regionId)
        {
            return Database.ExecSProc<SubRegion>(
                conn,
                trans,
                "[dbo].[GetSubRegionsByIDs]",
                Database.SqlParam("@CountryId", countryId),
                Database.SqlParam("@RegionId", regionId));
        }

        public static List<SubRegion> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, String countryCode)
        {
            return Database.ExecSProc<SubRegion>(
                conn,
                trans,
                "[dbo].[GetSubRegionsByCountryCode]",
                Database.SqlParam("@CountryCode", countryCode));
        }

        public static List<SubRegion> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, String countryCode, String regionCode)
        {
            return Database.ExecSProc<SubRegion>(
                conn,
                trans,
                "[dbo].[GetSubRegionsByCodes]",
                Database.SqlParam("@CountryCode", countryCode),
                Database.SqlParam("@RegionCode", regionCode));
        }
    }
}
