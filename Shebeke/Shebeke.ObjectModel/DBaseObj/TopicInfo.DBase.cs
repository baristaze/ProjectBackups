using System;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class TopicInfo : IDBEntity, Identifiable
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.Id = Database.GetInt64OrDefault(reader, ref index);
            this.Status = (AssetStatus)Database.GetByteOrDefault(reader, ref index);
            this.Title = Database.GetStringOrDefault(reader, ref index);
            this.CreatedBy = Database.GetInt64OrDefault(reader, ref index);
            this.EntryCount = Database.GetInt32OrDefault(reader, ref index);
            this.VoteCount = Database.GetInt32OrDefault(reader, ref index);
            this.ReactionCount = Database.GetInt32OrDefault(reader, ref index);
            this.ShareCount = Database.GetInt32OrDefault(reader, ref index);
            this.InvitationCount = Database.GetInt32OrDefault(reader, ref index);
            this.SocialScore = (double)Database.GetDecimalOrDefault(reader, ref index);

            string topWritersBulk = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(topWritersBulk))
            {
                // <CreatedBy>333</CreatedBy><CreatedBy>444</CreatedBy>...
                string[] tokens = topWritersBulk.Split(new string[] { "<CreatedBy>", "</CreatedBy>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string token in tokens)
                {
                    long parsed = 0;
                    if (Int64.TryParse(token, out parsed) && parsed > 0)
                    {
                        this.TopWriterIDs.Add(parsed);
                    }
                }
            }
        }

        protected static List<TopicInfo> GetRecentPopularTopicsFromDBase(
            SqlConnection conn,
            SqlTransaction trans,
            DateTime startTimeUTC,
            DateTime endTimeUTC,
            int topX,
            bool checkStatus,
            AssetStatus status,
            int topWritersLimit)
        {
            List<TopicInfo> topics = Database.ExecSProc<TopicInfo>(
                conn,
                trans,
                "[dbo].[GetRecentPopularTopics]",
                Database.SqlParam("@startTimeUtc", startTimeUTC),
                Database.SqlParam("@endTimeUtc", endTimeUTC),
                Database.SqlParam("@TopX", topX),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status),
                Database.SqlParam("@TopWritersLimit", topWritersLimit));

            if (topics.Count > 0)
            {
                List<long> writerIds = MergeWriterIds(topics);
                string writerIdsConcatenated = String.Join<long>(",", writerIds);

                // it checks the cache first
                List<FacebookUser> facebookUsers = FacebookUser.ReadAllByID(conn, trans, writerIdsConcatenated);

                foreach (TopicInfo topic in topics)
                {
                    FacebookUser createdBy = ShebekeUtils.Search<FacebookUser>(facebookUsers, topic.CreatedBy);
                    topic.Creator = createdBy;

                    foreach (long writerId in topic.TopWriterIDs)
                    {
                        FacebookUser contributedBy = ShebekeUtils.Search<FacebookUser>(facebookUsers, writerId);
                        if (contributedBy != null)
                        {
                            topic.TopWriters.Add(contributedBy);
                        }
                    }
                }
            }            

            return topics;
        }

        protected static List<long> MergeWriterIds(List<TopicInfo> topics)
        {
            List<long> ids = new List<long>();
            foreach (TopicInfo topic in topics)
            {
                // writers
                ids = ids.Union(topic.TopWriterIDs).ToList();

                // creators
                if (!ids.Contains(topic.CreatedBy))
                {
                    ids.Add(topic.CreatedBy);
                }
            }

            return ids;
        }
    }
}
