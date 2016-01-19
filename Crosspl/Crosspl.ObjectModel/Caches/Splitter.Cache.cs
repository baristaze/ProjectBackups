using System;
using System.Data.SqlClient;

namespace Crosspl.ObjectModel
{
    public partial class Splitter
    {
        public static int GetDefaultSplitId(SqlConnection conn, SqlTransaction trans)
        {
            int defaultSplitId = 0;
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_StaticResources, "DefaultSplitId", ref defaultSplitId))
            {
                defaultSplitId = Splitter.GetDefaultSplitIdFromDBase(conn, trans);
                CacheHelper.Put(CacheHelper.CacheName_StaticResources, "DefaultSplitId", defaultSplitId);
            }

            return defaultSplitId;
        }
    }
}
