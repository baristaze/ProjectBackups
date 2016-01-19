using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class VotingSummary : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.InitFromSqlReader(reader, ref index);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int index)
        {
            this.MyVote = Database.GetInt32OrDefault(reader, ref index);
            this.UpVoteCount = Database.GetInt32OrDefault(reader, ref index);
            this.DownVoteCount = Database.GetInt32OrDefault(reader, ref index);
            this.UpVoteSum = Database.GetInt32OrDefault(reader, ref index);
            this.DownVoteSum = Database.GetInt32OrDefault(reader, ref index);
            this.NetVoteSum = Database.GetInt32OrDefault(reader, ref index);
        }

        public static VotingSummary SaveVoteOnDBase(SqlConnection conn, SqlTransaction trans, long entryId, long userId, int vote)
        {
            List<VotingSummary> voting = Database.ExecSProc<VotingSummary>(
                conn,
                trans,
                "[dbo].[VoteForEntry]",
                Database.SqlParam("@EntryId", entryId),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@VoteValue", (short)vote));

            if (voting.Count > 0)
            {
                return voting[0];
            }

            return null;
        }
    }
}
