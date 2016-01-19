using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public partial class Region : BaseDistrict, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.Id = Database.GetInt32OrDefault(reader, ref colIndex);
            this.CountryId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.Code = Database.GetStringOrDefault(reader, ref colIndex);
            this.Name = Database.GetStringOrDefault(reader, ref colIndex);
            this.AddToAlternateNames(Database.GetStringOrDefault(reader, ref colIndex));
        }

        public static List<Region> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int countryId)
        {
            return Database.ExecSProc<Region>(
                conn,
                trans,
                "[dbo].[GetRegionsByCountryId]",
                Database.SqlParam("@CountryId", countryId));
        }

        public static List<Region> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, String countryCode)
        {
            return Database.ExecSProc<Region>(
                conn,
                trans,
                "[dbo].[GetRegionsByCountryCode]",
                Database.SqlParam("@CountryCode", countryCode));
        }
    }
}
