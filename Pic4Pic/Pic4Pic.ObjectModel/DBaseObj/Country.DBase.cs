using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace Pic4Pic.ObjectModel
{
    public partial class Country : BaseDistrict, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.Id = Database.GetInt32OrDefault(reader, ref colIndex);
            this.Code = Database.GetStringOrDefault(reader, ref colIndex);
            this.Name = Database.GetStringOrDefault(reader, ref colIndex);
            this.AddToAlternateNames(Database.GetStringOrDefault(reader, ref colIndex));
        }

        public static List<Country> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecSProc<Country>(
                conn,
                trans,
                "[dbo].[GetCountries]");
        }
    }
}
