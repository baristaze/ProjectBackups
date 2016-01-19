using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class RedirectInfo : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.TopicId = reader.GetInt64(index++);
            this.TopicTitle = reader.GetString(index++);
            this.EntryId = Database.GetInt64OrDefault(reader, ref index);
            this.SentBy = reader.GetInt64(index++);
            this.SenderFacebookId = reader.GetInt64(index++);
            this.SenderName = reader.GetString(index++);
            this.SenderSplitId = Database.GetInt32OrDefault(reader, ref index);
            this.SenderPhotoUrl = Database.GetStringOrDefault(reader, ref index);
        }

        public static RedirectInfo ReadFromDBaseByAppRequestIds(SqlConnection conn, SqlTransaction trans, string concatenatedAppRequestIds, bool checkStatus, AssetStatus status)
        {
            List<RedirectInfo> items = Database.ExecSProc<RedirectInfo>(
                conn,
                trans,
                "[dbo].[GetAppRequest]",
                Database.SqlParam("@ConcatenatedAppRequestIds", concatenatedAppRequestIds),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));

            if (items.Count > 0)
            {
                return items[0];
            }

            return null;
        }
    }
}