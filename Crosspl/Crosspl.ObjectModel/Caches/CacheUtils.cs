using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace Crosspl.ObjectModel
{
    public class CacheUtils
    {
        public delegate List<T> DBaseReader<T>();

        public static void Cache<T>(T item) where T : class, Identifiable
        {
            Cache<T>(CacheHelper.CacheName_RandomAccessObjects, item);
        }

        public static void Cache<T>(string cacheName, T item) where T : class, Identifiable
        {
            string key = String.Format(CultureInfo.InvariantCulture, "{0}_{1}", typeof(T).Name, item.Id);
            CacheHelper.Put(cacheName, key, item);
        }

        public static void CacheList<T>(string listCacheName, string listKey, IList<T> items) where T : class, Identifiable
        {
            CacheList<T>(listCacheName, listKey, CacheHelper.CacheName_RandomAccessObjects, items);
        }

        public static void CacheList<T>(string listCacheName, string listKey, string itemCacheName, IList<T> items) where T : class, Identifiable
        {
            foreach (T item in items)
            {
                Cache<T>(itemCacheName, item);
            }

            string concatenatedIDs = CrossplUtils.ConcatenateIDs<T>(items);
            CacheHelper.Put(listCacheName, listKey, concatenatedIDs);
        }

        public static T Read<T>(long id) where T : class, Identifiable
        {
            return Read<T>(CacheHelper.CacheName_RandomAccessObjects, id);
        }

        public static T Read<T>(string cacheName, long id) where T : class, Identifiable
        {
            string key = String.Format(CultureInfo.InvariantCulture, "{0}_{1}", typeof(T).Name, id);

            T item = default(T);
            CacheHelper.GetObj<T>(cacheName, key, ref item);
            return item;
        }

        public static List<T> ReadList<T>(string listCacheName, string listKey) where T : class, Identifiable
        {
            return ReadList<T>(listCacheName, listKey, CacheHelper.CacheName_RandomAccessObjects);
        }

        public static List<T> ReadList<T>(string listCacheName, string listKey, string itemCacheName) where T : class, Identifiable
        {
            List<T> items = new List<T>();
            string concatenatedIDs = null;
            if (CacheHelper.GetObj<string>(listCacheName, listKey, ref concatenatedIDs))
            {
                List<long> ids = CrossplUtils.TokenizeIDs(concatenatedIDs);
                foreach (long id in ids)
                {
                    T item = Read<T>(itemCacheName, id);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public static List<T> ReadListByIDs<T>(string concatenatedIDs) where T : class, Identifiable
        {
            return ReadListByIDs<T>(concatenatedIDs, CacheHelper.CacheName_RandomAccessObjects);
        }

        public static List<T> ReadListByIDs<T>(string concatenatedIDs, string itemCacheName) where T : class, Identifiable
        {
            List<T> items = new List<T>();
            List<long> ids = CrossplUtils.TokenizeIDs(concatenatedIDs);
            foreach (long id in ids)
            {
                T item = Read<T>(itemCacheName, id);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public static List<T> ReadListFromCacheOrDBase<T>(DBaseReader<T> reader, string listCacheName, string listKey) where T : class, Identifiable
        {
            return ReadListFromCacheOrDBase(reader, listCacheName, listKey, CacheHelper.CacheName_RandomAccessObjects);
        }

        public static List<T> ReadListFromCacheOrDBase<T>(DBaseReader<T> reader, string listCacheName, string listKey, string itemCacheName) where T : class, Identifiable
        {
            string concatenatedIDs = null;
            List<T> items = new List<T>();
            if (!CacheHelper.GetObj<string>(listCacheName, listKey, ref concatenatedIDs))
            {
                // read from dbase                
                items = reader();

                // cache them all
                CacheList<T>(listCacheName, listKey, itemCacheName, items);
            }
            else
            {
                items = ReadListByIDs<T>(concatenatedIDs, itemCacheName);
            }

            return items;
        }

        public static bool Delete<T>(long id) where T : class, Identifiable
        {
            return Delete<T>(CacheHelper.CacheName_RandomAccessObjects, id);
        }

        public static bool Delete<T>(string cacheName, long id) where T : class, Identifiable
        {
            string typeName = typeof(T).Name;
            string key = String.Format(CultureInfo.InvariantCulture, "{0}_{1}", typeName, id);
            if (CacheHelper.Remove(cacheName, key))
            {
                string msg = String.Format("{0} {1} was deleted from cache {2}", typeName, id, cacheName);
                Trace.WriteLine(msg, LogCategory.Info);
                return true;
            }

            return false;
        }

        public static void DeleteFromList<T>(string listCacheName, string listKey, long id) where T : class, Identifiable
        {
            string concatenatedIDs = null;
            if (CacheHelper.GetObj<string>(listCacheName, listKey, ref concatenatedIDs))
            {
                List<long> ids = CrossplUtils.TokenizeIDs(concatenatedIDs);
                if (ids.Contains(id))
                {
                    ids.Remove(id);

                    // re-cache
                    concatenatedIDs = String.Join(",", ids);
                    CacheHelper.Put(listCacheName, listKey, concatenatedIDs);
                }
            }
        }
    }
}
