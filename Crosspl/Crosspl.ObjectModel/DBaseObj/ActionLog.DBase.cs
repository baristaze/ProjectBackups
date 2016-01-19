using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class ActionLog : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.UserId = Database.GetInt64OrDefault(reader, ref index);
            this.UserType = (UserType)Database.GetByteOrDefault(reader, ref index);
            this.UserSplit = Database.GetInt32OrDefault(reader, ref index);
            this.FacebookId = Database.GetInt64OrDefault(reader, ref index);
            this.PhotoUrl = Database.GetStringOrDefault(reader, ref index);
            this.FirstName = Database.GetStringOrDefault(reader, ref index);
            this.LastName = Database.GetStringOrDefault(reader, ref index);
            this.Gender = (Gender)Database.GetByteOrDefault(reader, ref index);
            this.FacebookLink = Database.GetStringOrDefault(reader, ref index);
            this.Hometown = Database.GetStringOrDefault(reader, ref index);
            this.AssetType = (AssetType)Database.GetByteOrDefault(reader, ref index);
            this.Action = (Action)Database.GetByteOrDefault(reader, ref index);
            this.ActionTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.ActionValue = Database.GetStringOrDefault(reader, ref index);
            this.TopicId = Database.GetInt64OrDefault(reader, ref index);
            this.TopicTitle = Database.GetStringOrDefault(reader, ref index);
            this.EntryId = Database.GetInt64OrDefault(reader, ref index);
            this.EntryContent = Database.GetStringOrDefault(reader, ref index);
        }

        public static List<ActionLog> ReadFromDBase(
            SqlConnection conn,
            SqlTransaction trans,
            DateTime startTimeUTC,
            DateTime endTimeUTC,
            int topX,
            bool applyUserFilter,
            long userId)
        {
            return Database.ExecSProc<ActionLog>(
                conn,
                trans,
                "[dbo].[GetActions]",
                Database.SqlParam("@startTimeUtc", startTimeUTC),
                Database.SqlParam("@endTimeUtc", endTimeUTC),
                Database.SqlParam("@TopX", topX),
                Database.SqlParam("@ApplyUserFilter", applyUserFilter),
                Database.SqlParam("@UserId", userId));
        }

        public static int LogSocialShare(
            SqlConnection conn,
            SqlTransaction trans,
            long userId,
            long topicId,
            long entryId,
            SocialChannel channel)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[LogSocialShare]",
                Database.SqlParam("@TopicId", topicId),
                Database.SqlParam("@EntryId", entryId > 0 ? (object)entryId : null),
                Database.SqlParam("@UserId", userId > 0 ? (object)userId : null),
                Database.SqlParam("@SocialChannel", (byte)channel),
                Database.SqlParam("@ShareCount", 1));
        }
    }
}
