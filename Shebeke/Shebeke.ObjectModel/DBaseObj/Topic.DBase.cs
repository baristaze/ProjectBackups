using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class Topic : IDBEntity, Identifiable
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;

            this.Id = reader.GetInt64(index++);
            this.Status = (AssetStatus)reader.GetByte(index++);
            this.Title = reader.GetString(index++);
            this.CreatedBy = reader.GetInt64(index++);
            this.CreateTimeUTC = reader.GetDateTime(index++);
            this.LastUpdateTimeUTC = reader.GetDateTime(index++);
            this.EntryCount = reader.GetInt32(index++);

            string catIds = Database.GetStringOrDefault(reader, ref index);
            if (!String.IsNullOrWhiteSpace(catIds))
            {
                List<long> cats = ShebekeUtils.TokenizeIDs(catIds);
                foreach (long id in cats)
                {
                    Category cat = Category.GetById(id);
                    if (cat != null)
                    {
                        this.Categories.Add(cat);
                    }
                }
            }
        }

        public long CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            this.Id = Database.ExecScalar<long>(
                conn,
                trans,
                "[dbo].[CreateNewTopic]",
                Database.SqlParam("@Title", this.Title),
                Database.SqlParam("@SeoLink", this.SeoLink),
                Database.SqlParam("@CreatedBy", this.CreatedBy));

            return this.Id;
        }

        protected static Topic ReadFromDBase(SqlConnection conn, SqlTransaction trans, long id, bool checkStatus, AssetStatus status)
        {
            List<Topic> topics = Database.ExecSProc<Topic>(
                conn,
                trans,
                "[dbo].[GetTopic]",
                Database.SqlParam("@Id", id),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));

            if (topics.Count > 0)
            {
                return topics[0];
            }

            return null;
        }

        protected static Topic ReadFromDBase(SqlConnection conn, SqlTransaction trans, string seoLink, bool checkStatus, AssetStatus status)
        {
            List<Topic> topics = Database.ExecSProc<Topic>(
                conn,
                trans,
                "[dbo].[GetTopicByLink]",
                Database.SqlParam("@SeoLink", seoLink),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));

            if (topics.Count > 0)
            {
                return topics[0];
            }

            return null;
        }

        public static Topic ReadFromDBaseByAppRequestIds(SqlConnection conn, SqlTransaction trans, string concatenatedAppRequestIds, bool checkStatus, AssetStatus status)
        {
            List<Topic> topics = Database.ExecSProc<Topic>(
                conn,
                trans,
                "[dbo].[GetTopicByAppRequestId]",
                Database.SqlParam("@ConcatenatedAppRequestIds", concatenatedAppRequestIds),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));

            if (topics.Count > 0)
            {
                return topics[0];
            }

            return null;
        }

        protected static List<Topic> ReadRelatedTopicsFromDBase(SqlConnection conn, SqlTransaction trans, long topicId, int maxTopicCount)
        {
            // it checks the cache first
            List<Topic> relatedTopics = new List<Topic>();
            Topic topic = Topic.ReadById(conn, trans, topicId, true, AssetStatus.New);
            if (topic != null)
            {
                List<string> subsentences = StringHelpers.SubSentences(topic.Title);
                foreach (string subsentence in subsentences)
                {
                    // finetune it for the dbase search
                    // string query = SpecialCharUtils.ReplaceSpecials(subsentence, '_');
                    // wrap query with sql like
                    // query = "%" + query + "%";

                    string query = "%" + subsentence + "%";
                    List<Topic> similars = Topic.SearchOnDBase(conn, trans, query, maxTopicCount, true, AssetStatus.New);
                    foreach (Topic similar in similars)
                    {
                        if (similar.Id != topicId)
                        {
                            if (ShebekeUtils.Search(relatedTopics, similar.Id) == null)
                            {
                                relatedTopics.Add(similar);
                                if (relatedTopics.Count >= maxTopicCount)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (relatedTopics.Count >= maxTopicCount)
                    {
                        break;
                    }
                }
            }

            if (relatedTopics.Count < maxTopicCount)
            {
                // it checks the cache first
                List<Topic> latestTopics = Topic.ReadLatestTopics(conn, trans, 100, true, AssetStatus.New);
                if (latestTopics.Count > 0)
                {
                    Random rand = new Random((int)DateTime.Now.Ticks);
                    while (relatedTopics.Count < maxTopicCount && latestTopics.Count > 0)
                    {
                        int randomIndex = rand.Next(0, latestTopics.Count); /*upper bound is exclusive*/
                        Topic randomTopic = latestTopics[randomIndex];
                        if (randomTopic.Id != topicId)
                        {
                            if (ShebekeUtils.Search(relatedTopics, randomTopic.Id) == null)
                            {
                                relatedTopics.Add(randomTopic);
                            }
                        }

                        latestTopics.RemoveAt(randomIndex);
                    }
                }
            }

            return relatedTopics;
        }

        public static List<Topic> SearchOnDBase(SqlConnection conn, SqlTransaction trans, string query, int topX, bool checkStatus, AssetStatus status)
        {
            return Database.ExecSProc<Topic>(
                conn,
                trans,
                "[dbo].[SearchTopics]",
                Database.SqlParam("@Query", query),
                Database.SqlParam("@TopX", topX),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));
        }

        protected static List<Topic> ReadLatestTopicsFromDBase(SqlConnection conn, SqlTransaction trans, int topX, bool checkStatus, AssetStatus status)
        {
            return Database.ExecSProc<Topic>(
                conn,
                trans,
                "[dbo].[GetLatestTopics]",
                Database.SqlParam("@TopX", topX),
                Database.SqlParam("@CheckStatus", checkStatus),
                Database.SqlParam("@Status", (byte)status));
        }

        public bool DeleteFromDBaseIfNoEntry(SqlConnection conn, SqlTransaction trans, bool checkUser, long userId)
        {
            return Database.ExecScalar<bool>(
                conn,
                trans,
                "[dbo].[DeleteTopicIfNoEntry]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public static int CreateAppRequestForTopicOnDBase(SqlConnection conn, SqlTransaction trans, long appRequestId, long topicId, long entryId, long sentBy, short inviteeCount)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[InsertAppRequestForTopic]",
                Database.SqlParam("@AppRequestId", appRequestId),
                Database.SqlParam("@TopicId", topicId),
                Database.SqlParam("@EntryId", (entryId > 0 ? (object)entryId : DBNull.Value)),
                Database.SqlParam("@SentBy", sentBy),
                Database.SqlParam("@InviteeCount", inviteeCount));
        }

        protected static List<Topic> ReadPromotedTopicsFromDBase(
            SqlConnection conn,
            SqlTransaction trans,
            bool checkTopicStatus,
            AssetStatus topicStatus,
            bool checkPromotionStatus,
            bool isPromotionActive,
            bool checkPromotionDate)
        {
            return Database.ExecSProc<Topic>(
               conn,
               trans,
               "[dbo].[GetPromotedTopics]",
               Database.SqlParam("@CheckTopicStatus", checkTopicStatus),
               Database.SqlParam("@TopicStatus", (byte)topicStatus),
               Database.SqlParam("@CheckPromotionStatus", checkPromotionStatus),
               Database.SqlParam("@IsPromotionActive", isPromotionActive),
               Database.SqlParam("@CheckPromotionDate", checkPromotionDate));
        }
    }
}
