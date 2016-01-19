using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class CreditAdjustment : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.UserId = Database.GetGuidOrDefault(reader, ref index);
            this.Credit = Database.GetInt32OrDefault(reader, ref index);
            this.Reason = (CreditAdjustmentReason)Database.GetByteOrDefault(reader, ref index);
            this.CreateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MakeCreditAdjustment]",
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@Credit", this.Credit),
                Database.SqlParam("@Reason", (byte)this.Reason));
        }

        public static List<CreditAdjustment> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount)
        {
            return Database.ExecSProc<CreditAdjustment>(
                conn,
                trans,
                "[dbo].[GetCreditAdjustments]",
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@MaxCount", maxCount));
        }

        public static int ReadCurrentCreditFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            return Database.ExecScalar<int>(
                conn,
                trans,
                "[dbo].[GetCurrentCredit]",
                Database.SqlParam("@UserId", userId));
        }
    }
}
