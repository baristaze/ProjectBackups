using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public partial class Topic : IDBEntity, Identifiable
    {
        protected static void CacheTopic(Topic topic, bool cacheSeoLink)
        {
            // cache topic [key = id, value=topic]
            CacheUtils.Cache<Topic>(topic);

            // cache Seo link [key=seo, value=topic-id]
            if (cacheSeoLink)
            {
                string keyForSeo = String.Format(CultureInfo.InvariantCulture, "SeoLink_{0}", topic.SeoLink);
                CacheHelper.Put(CacheHelper.CacheName_RandomAccessObjects, keyForSeo, topic.Id);
            }
        }

        public static List<Topic> ReadPromotedTopics(
            SqlConnection conn,
            SqlTransaction trans,
            UserAuthInfo user,
            bool checkTopicStatus,
            AssetStatus topicStatus,
            bool checkPromotionStatus,
            bool isPromotionActive,
            bool checkPromotionDate)
        {
            CacheUtils.DBaseReader<Topic> reader = () =>
            {
                return Topic.ReadPromotedTopicsFromDBase(
                     conn, trans, checkTopicStatus, topicStatus, checkPromotionStatus, isPromotionActive, checkPromotionDate);
            };

            return CacheUtils.ReadListFromCacheOrDBase<Topic>(reader, CacheHelper.CacheName_PopularTopics, "PromotedTopicIDs");
        }

        public static List<Topic> ReadLatestTopics(SqlConnection conn, SqlTransaction trans, int topX, bool checkStatus, AssetStatus status)
        {
            CacheUtils.DBaseReader<Topic> reader = () =>
            {
                return Topic.ReadLatestTopicsFromDBase(conn, trans, topX, checkStatus, status);
            };

            return CacheUtils.ReadListFromCacheOrDBase<Topic>(reader, CacheHelper.CacheName_PopularTopics, "LatestTopicIDs");
        }

        public static List<Topic> ReadRelatedTopics(SqlConnection conn, SqlTransaction trans, long topicId, int maxTopicCount)
        {
            CacheUtils.DBaseReader<Topic> reader = () =>
            {
                return Topic.ReadRelatedTopicsFromDBase(conn, trans, topicId, maxTopicCount);
            };

            string key = String.Format(CultureInfo.InvariantCulture, "RelatedTopicsOf_{0}", topicId);
            return CacheUtils.ReadListFromCacheOrDBase<Topic>(reader, CacheHelper.CacheName_RandomAccessObjects, key);
        }

        public static Topic ReadById(SqlConnection conn, SqlTransaction trans, long id, bool checkStatus, AssetStatus status)
        {
            Topic item = CacheUtils.Read<Topic>(id);
            if (item == null)
            {
                item = Topic.ReadFromDBase(conn, trans, id, checkStatus, status);
                if (item != null)
                {
                    CacheTopic(item, true);
                }
            }

            return item;
        }

        public static Topic ReadBySeoLink(SqlConnection conn, SqlTransaction trans, string seoLink, bool checkStatus, AssetStatus status)
        {
            // check cache first
            string keyForSeo = String.Format(CultureInfo.InvariantCulture, "SeoLink_{0}", seoLink);

            // check to see if the seolink is mapped to a topic id already
            long itemId = 0;
            Topic item = null;
            bool searchBySeoAndCache = false;
            if (!CacheHelper.Get<long>(CacheHelper.CacheName_RandomAccessObjects, keyForSeo, ref itemId))
            {
                searchBySeoAndCache = true;
            }
            else
            {
                // we have seolink in cache... it maps to a topicId... let's get the topic Id
                // this one handles the cache issues already
                item = Topic.ReadById(conn, trans, itemId, checkStatus, status);
                if (item == null)
                {
                    // re-search by seo
                    searchBySeoAndCache = true;
                }
            }

            if (searchBySeoAndCache)
            {
                // seolink is not in the cache
                item = Topic.ReadFromDBase(conn, trans, seoLink, checkStatus, status);
                if (item != null)
                {
                    CacheTopic(item, true);
                }
            }

            return item;
        }

        public static void UpdateCacheOnTopicDelete(long topicId)
        {
            CacheUtils.DeleteFromList<Topic>(CacheHelper.CacheName_PopularTopics, "PromotedTopicIDs", topicId);
            CacheUtils.DeleteFromList<Topic>(CacheHelper.CacheName_PopularTopics, "LatestTopicIDs", topicId);

            string relatedTopicsKey = String.Format(CultureInfo.InvariantCulture, "RelatedTopicsOf_{0}", topicId);
            if (CacheHelper.Remove(CacheHelper.CacheName_RandomAccessObjects, relatedTopicsKey))
            {
                Trace.WriteLine("Related topic IDs have been deleted from cache for: " + topicId.ToString(), LogCategory.Info);
            }

            Topic topic = CacheUtils.Read<Topic>(topicId);
            if (topic != null)
            {
                // remove seo, too, since we have the topic object in hand
                string keyForSeo = String.Format(CultureInfo.InvariantCulture, "SeoLink_{0}", topic.SeoLink);
                if (CacheHelper.Remove(CacheHelper.CacheName_RandomAccessObjects, keyForSeo))
                {
                    Trace.WriteLine("Topic deleted from SeoLink cache: " + topic.SeoLink, LogCategory.Info);
                }

                // remove topic
                CacheUtils.Delete<Topic>(topicId);
            }
        }

        public static void UpdateCacheOnTopicAdd(Topic topic)
        {
            CacheTopic(topic, true);

            // purge latest topics
            if (CacheHelper.Remove(CacheHelper.CacheName_PopularTopics, "LatestTopicIDs"))
            {
                Trace.WriteLine("Latest topics cache is purged based on topic addition: " + topic.Title, LogCategory.Info);
            }
        }

        public static void UpdateCacheOnEntryAddOrDelete(long topicId, bool isAdd)
        {
            Topic topic = CacheUtils.Read<Topic>(topicId);
            if (topic != null)
            {
                // update the cache
                topic.EntryCount += (isAdd ? 1 : -1);
                CacheTopic(topic, false);
            }
        }
    }
}
