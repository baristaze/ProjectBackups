using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class ReactionSummary : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.InitFromSqlReader(reader, ref index);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int index)
        {
            this.Top1ReactionId = Database.GetInt64OrDefault(reader, ref index);
            this.Top2ReactionId = Database.GetInt64OrDefault(reader, ref index);
            this.Top3ReactionId = Database.GetInt64OrDefault(reader, ref index);
            this.Top1ReactionCount = Database.GetInt32OrDefault(reader, ref index);
            this.Top2ReactionCount = Database.GetInt32OrDefault(reader, ref index);
            this.Top3ReactionCount = Database.GetInt32OrDefault(reader, ref index);
            this.TotalReactionCount = Database.GetInt32OrDefault(reader, ref index);

            long myReactionsSum = Database.GetInt64OrDefault(reader, ref index);

            if (myReactionsSum > 0 && Reaction.All != null)
            {
                foreach (Reaction r in Reaction.All)
                {
                    if ((r.Id & myReactionsSum) > 0)
                    {
                        this.MyReactions.Add(r.Id);
                    }
                }
            }
        }

        public static ReactionSummary SaveReactionOnDBase(SqlConnection conn, SqlTransaction trans, long entryId, long userId, long reactionTypeId)
        {
            List<ReactionSimpleResult> reactions = Database.ExecSProc<ReactionSimpleResult>(
                conn,
                trans,
                "[dbo].[ReactToEntry]",
                Database.SqlParam("@EntryId", entryId),
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ReactionTypeId", reactionTypeId));

            ReactionSummary summary = null;

            if (reactions.Count > 0)
            {
                summary = new ReactionSummary();
                summary.Top1ReactionId = reactions[0].ReactionTypeId;
                summary.Top1ReactionCount = reactions[0].ReactionCount;

                if (reactions.Count > 1)
                {
                    summary.Top2ReactionId = reactions[1].ReactionTypeId;
                    summary.Top2ReactionCount = reactions[1].ReactionCount;
                }

                if (reactions.Count > 2)
                {
                    summary.Top3ReactionId = reactions[2].ReactionTypeId;
                    summary.Top3ReactionCount = reactions[2].ReactionCount;
                }

                foreach(ReactionSimpleResult item in reactions)
                {
                    summary.TotalReactionCount += item.ReactionCount;
                    if(item.MeReacted)
                    {
                        summary.MyReactions.Add(item.ReactionTypeId);
                    }
                }
            }

            return summary;
        }
    }

    public class ReactionSimpleResult : IDBEntity
    {
        public long ReactionTypeId { get; set; }
        public int ReactionCount { get; set; }
        public bool MeReacted { get; set; }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.ReactionTypeId = Database.GetInt64OrDefault(reader, ref index);
            this.ReactionCount = Database.GetInt32OrDefault(reader, ref index);
            this.MeReacted = Database.GetInt32OrDefault(reader, ref index) > 0;
        }
    }
}
