using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public partial class CssSplitter : Splitter, IDBEntity
    {
        public static CssSplitterList Read(SqlConnection conn, SqlTransaction trans, int splitId, string sectionFilters)
        {
            int itemCount = 0;
            CssSplitterList items = new CssSplitterList();
            string sectionConcat = String.Join("_", sectionFilters);
            string keyPrefix = "CssSplitters_Split_" + splitId.ToString() + "_Section_" + sectionConcat;
            string countKey = keyPrefix + "_Count";
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_StaticResources, countKey, ref itemCount))
            {
                // read from dbase
                Trace.WriteLine("Reading CssSplitters from database for " + keyPrefix, LogCategory.Info);
                items = CssSplitter.ReadFromDBase(conn, trans, splitId, sectionFilters);

                for (int x = 0; x < items.Count; x++)
                {
                    string key = keyPrefix + "__" + x.ToString();
                    CacheHelper.Put(CacheHelper.CacheName_StaticResources, key, items[x]);
                }

                CacheHelper.Put(CacheHelper.CacheName_StaticResources, countKey, items.Count);
                Trace.WriteLine("CssSplitters for " + keyPrefix + " have been cached successfully", LogCategory.Info);
            }
            else
            {
                // read from cache
                int successfullReads = 0;
                Trace.WriteLine("Reading CssSplitters from cache for " + keyPrefix, LogCategory.Info);
                for (int x = 0; x < itemCount; x++)
                {
                    CssSplitter temp = null;
                    string key = keyPrefix + "__" + x.ToString();
                    if (CacheHelper.GetObj<CssSplitter>(CacheHelper.CacheName_StaticResources, key, ref temp) && temp != null)
                    {
                        items.Add(temp);
                        successfullReads++;
                    }
                }

                if (successfullReads == itemCount)
                {
                    Trace.WriteLine("CssSplitters for " + keyPrefix + " have been read from cache successfully", LogCategory.Info);
                }
                else
                {
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details=Expected {5}, Retrieved {6}]",
                        "CssSplitter.Cache.cs",
                        "Read",
                        "Reading cached ccs splitters",
                        0,
                        0,
                        itemCount,
                        successfullReads);

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }
            }

            return items;
        }
    }
}
