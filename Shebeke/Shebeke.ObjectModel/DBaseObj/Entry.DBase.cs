using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class Entry : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {   
            int index = 0;

            this.Id = reader.GetInt64(index++);
            this.Status = (AssetStatus)reader.GetByte(index++);
            this.TopicId = reader.GetInt64(index++);
            this.Content = reader.GetString(index++);
            this.FormatVersion = reader.GetInt32(index++);
            this.CreatedBy = reader.GetInt64(index++);
            this.CreateTimeUTC = reader.GetDateTime(index++);
            this.LastUpdateTimeUTC = reader.GetDateTime(index++);

            this.VotingSummary = new VotingSummary();
            if (reader.FieldCount > index)
            {   
                this.VotingSummary.InitFromSqlReader(reader, ref index);
            }

            this.ReactionSummary = new ReactionSummary();
            if (reader.FieldCount > index)
            {
                this.ReactionSummary.InitFromSqlReader(reader, ref index);
            }
        }

        public long CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            this.Id = Database.ExecScalar<long>(
                conn,
                trans,
                "[dbo].[CreateNewEntry]",
                Database.SqlParam("@TopicId", this.TopicId),
                Database.SqlParam("@Content", this.Content),
                Database.SqlParam("@FormatVersion", this.FormatVersion),
                Database.SqlParam("@CreatedBy", this.CreatedBy));

            return this.Id;
        }

        public static List<Entry> GetLatestActiveEntries(
            SqlConnection conn,
            SqlTransaction trans,
            long topicId,
            long currentUserId,
            int pageSize,
            int pageIndex, /*0-based*/
            bool checkStatus,
            AssetStatus status,
            long includedEntryId)
        {
            return GetEntries(conn, trans, topicId, currentUserId, pageSize, pageIndex, checkStatus, AssetStatus.New, includedEntryId, false);
        }

        public static List<Entry> GetEntries(
            SqlConnection conn, 
            SqlTransaction trans, 
            long topicId, 
            long currentUserId,
            int pageSize,
            int pageIndex, /*0-based*/
            bool checkStatus,
            AssetStatus status,
            long includedEntryId,
            bool fromOldToNew)
        {
            return Database.ExecSProc<Entry>(
                conn,
                trans,
                "[dbo].[GetEntries]",
                Database.SqlParam("@TopicId", topicId),
                Database.SqlParam("@CurrentUserId", currentUserId),
                Database.SqlParam("@Offset", pageIndex * pageSize),
                Database.SqlParam("@NextFetchCount", pageSize),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status),
                Database.SqlParam("@IncludedEntryId", includedEntryId),
                Database.SqlParam("@FromOldToNew", fromOldToNew));

        }

        public static List<Entry> GetEntriesByNetVoteSum(
            SqlConnection conn,
            SqlTransaction trans,
            long topicId,
            long currentUserId,
            int pageSize,
            int pageIndex, /*0-based*/
            bool checkStatus,
            AssetStatus status,
            long includedEntryId)
        {
            return Database.ExecSProc<Entry>(
                conn,
                trans,
                "[dbo].[GetUpVotedEntries]",
                Database.SqlParam("@TopicId", topicId),
                Database.SqlParam("@CurrentUserId", currentUserId),
                Database.SqlParam("@Offset", pageIndex * pageSize),
                Database.SqlParam("@NextFetchCount", pageSize),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status),
                Database.SqlParam("@IncludedEntryId", includedEntryId));
        }

        public int DeleteFromDBase(SqlConnection conn, SqlTransaction trans, bool checkUser, long userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteEntry]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public static Entry ReadFromDBase(SqlConnection conn, SqlTransaction trans, long id, bool checkStatus, AssetStatus status)
        {
            List<Entry> entries = Database.ExecSProc<Entry>(
                conn,
                trans,
                "[dbo].[GetEntry]",
                Database.SqlParam("@Id", id),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));

            if (entries.Count > 0)
            {
                return entries[0];
            }

            return null;
        }
    }
}
