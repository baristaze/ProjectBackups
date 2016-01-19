using System;
using System.Globalization;
using Microsoft.ApplicationServer.Caching;

namespace Crosspl.ObjectModel
{   
    public class CacheHelper
    {
        public const string CacheName_Default = "default";
        public const string CacheName_PopularTopics = "PopularTopics";
        public const string CacheName_FacebookUsers = "FacebookUsers";
        public const string CacheName_StaticResources = "StaticResources";
        public const string CacheName_RandomAccessObjects = "RandomAccessObjects";
        
        static CacheHelper() 
        {
            InitializeCaches();
        }

        private static void InitializeCaches()
        {
            string[] caches = new string[] 
            { 
                CacheName_Default, 
                CacheName_PopularTopics, 
                CacheName_FacebookUsers, 
                CacheName_StaticResources,
                CacheName_RandomAccessObjects
            };

            foreach (string cacheName in caches)
            {
                try
                {
                    DataCache cache = new DataCache(cacheName);
                    cache.Put("InitialSetup", 1);
                }
                catch { }
            }
        }

        public static bool Get<T>(string cacheName, string keyName, ref T value) where T : struct
        {
            DataCache cache = new DataCache(cacheName);
            object obj = cache.Get(keyName);
            if (obj != null)
            {
                if (obj is T)
                {
                    T t = (T)obj;
                    value = t;
                    return true;
                }
            }
            
            return false;
        }

        public static T GetOrDefault<T>(string cacheName, string keyName) where T : struct
        {
            DataCache cache = new DataCache(cacheName);
            object obj = cache.Get(keyName);
            if (obj != null)
            {
                if (obj is T)
                {
                    return (T)obj;
                }
            }

            return default(T);
        }

        public static bool GetObj<T>(string cacheName, string keyName, ref T value) where T : class
        {
            DataCache cache = new DataCache(cacheName);
            object obj = cache.Get(keyName);
            if (obj != null)
            {
                T t = obj as T;
                if (t != null)
                {
                    value = t;
                    return true;
                }
            }

            return false;
        }

        public static T GetObjOrDefault<T>(string cacheName, string keyName) where T : class
        {
            DataCache cache = new DataCache(cacheName);
            object obj = cache.Get(keyName);
            if (obj != null)
            {
                T t = obj as T;
                if (t != null)
                {
                    return t;
                }
            }

            return default(T);
        }

        public static void Put(string cacheName, string keyName, object value)
        {
            DataCache cache = new DataCache(cacheName);
            cache.Put(keyName, value);
        }

        public static bool Remove(string cacheName, string keyName)
        {
            DataCache cache = new DataCache(cacheName);
            return cache.Remove(keyName);
        }
    }
}
