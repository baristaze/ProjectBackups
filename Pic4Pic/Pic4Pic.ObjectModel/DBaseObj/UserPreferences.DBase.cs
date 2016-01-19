using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class UserPreferences : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.UserId = Database.GetGuidOrDefault(reader, ref colIndex);
            this.InterestedIn = (Gender)Database.GetByteOrDefault(reader, ref colIndex);
            this.LastUpdateTimeUTC = Database.GetDateTimeOrDefault(reader, ref colIndex);
        }

        protected List<SqlParameter> GetSqlParameters()
        {
            return new List<SqlParameter>(new SqlParameter[]{
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@InterestedIn", (byte)this.InterestedIn)
            });
        }

        public int CreateNewOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[CreateNewPreference]",
                this.GetSqlParameters().ToArray());
        }

        public int UpdateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[UpdatePreference]",
                this.GetSqlParameters().ToArray());
        }
    }
}
