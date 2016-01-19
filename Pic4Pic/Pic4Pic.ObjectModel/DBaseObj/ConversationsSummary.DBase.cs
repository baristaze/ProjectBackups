using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class ConversationsSummary : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.UserId = Database.GetGuidOrDefault(reader, ref index);
            this.UnreadMessageCount = Database.GetInt32OrDefault(reader, ref index);            
            this.LastUpdateUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public static List<ConversationsSummary> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffMinutes)
        {
            return Database.ExecSProc<ConversationsSummary>(
                conn,
                trans,
                "[dbo].[GetConversationsSummary]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@CutOffMinutes", cutOffMinutes));
        }
    }
}
