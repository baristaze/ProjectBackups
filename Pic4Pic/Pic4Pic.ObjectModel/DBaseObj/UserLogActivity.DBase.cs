using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class UserLogActivity
    {
        public int CheckUserActivityOnDBase(SqlConnection conn, SqlTransaction trans, int timeWindowInSeconds, int actionCountLimit)
        {
            return Database.ExecScalar<int>(
                conn,
                trans,
                "[dbo].[CheckUserActivity]",
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@ActionId", (byte)this.ActionId),
                Database.SqlParam("@TimeWindowInSeconds", timeWindowInSeconds),
                Database.SqlParam("@ActionCountLimit", actionCountLimit));
        }

        public int LogUserActivityOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[LogUserActivity]",
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@ActionId", (byte)this.ActionId));
        }
    }
}
