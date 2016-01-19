using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Shebeke.ObjectModel
{
    public partial class TopicInfo : IDBEntity, Identifiable
    {
        public static List<TopicInfo> GetRecentPopularTopics(
            SqlConnection conn,
            SqlTransaction trans,
            DateTime startTimeUTC,
            DateTime endTimeUTC,
            int topX,
            bool checkStatus,
            AssetStatus status,
            int topWritersLimit)
        {
            CacheUtils.DBaseReader<TopicInfo> reader = () =>
            {
                return TopicInfo.GetRecentPopularTopicsFromDBase(
                    conn, trans, startTimeUTC, endTimeUTC, topX, checkStatus, status, topWritersLimit);
            };
            
            return CacheUtils.ReadListFromCacheOrDBase<TopicInfo>(
                reader, CacheHelper.CacheName_PopularTopics, "RecentPopularTopicInfoIDs", CacheHelper.CacheName_PopularTopics);
        }

        public static void UpdateCacheOnTopicDelete(long topicId)
        {
            CacheUtils.DeleteFromList<TopicInfo>(
                CacheHelper.CacheName_PopularTopics, "RecentPopularTopicInfoIDs", topicId);

            CacheUtils.Delete<TopicInfo>(CacheHelper.CacheName_PopularTopics, topicId);
        }

        public static void UpdateCacheOnTopicAdd(Topic topic)
        {
            // purge latest topics
            if (CacheHelper.Remove(CacheHelper.CacheName_PopularTopics, "RecentPopularTopicInfoIDs"))
            {
                Trace.WriteLine("Latest topics cache is purged based on topic addition: " + topic.Title, LogCategory.Info);
            }
        }

        public static void UpdateCacheOnEntryAddOrDelete(long topicId, bool isAdd)
        {
            TopicInfo topic = CacheUtils.Read<TopicInfo>(CacheHelper.CacheName_PopularTopics, topicId);
            if (topic != null)
            {
                // update the cache
                topic.EntryCount += (isAdd ? 1 : -1);
                CacheUtils.Cache<TopicInfo>(CacheHelper.CacheName_PopularTopics, topic);
            }
        }

        public static void UpdateCacheOnSocialActions(long topicId, Action action, int actionData)
        {
            TopicInfo topic = CacheUtils.Read<TopicInfo>(CacheHelper.CacheName_PopularTopics, topicId);
            if (topic != null)
            {
                // update the cache
                // update the cache
                if (action == Action.Vote)
                {
                    topic.VoteCount += actionData;
                }
                else if (action == Action.React)
                {
                    topic.ReactionCount += actionData;
                }
                else if (action == Action.Share || action == Action.Invite)
                {
                    topic.ShareCount += actionData;
                }

                CacheUtils.Cache<TopicInfo>(CacheHelper.CacheName_PopularTopics, topic);
            }
        }
    }
}
