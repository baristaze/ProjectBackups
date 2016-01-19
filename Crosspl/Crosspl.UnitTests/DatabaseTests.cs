using System;
using System.Data.SqlClient;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Crosspl.ObjectModel;

namespace Crosspl.UnitTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestGetEntries1()
        {
            using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
            {
                conn.Open();

                List<Entry> entries = Entry.GetLatestActiveEntries(conn, null, 49, 1, 5, 0, true, AssetStatus.New, 273);
                Console.WriteLine(entries.Count);
            }
        }

        [TestMethod]
        public void TestGetEntries2()
        {
            using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
            {
                conn.Open();

                List<Entry> entries = Entry.GetEntriesByNetVoteSum(conn, null, 49, 1, 5, 0, true, AssetStatus.New, 268);
                Console.WriteLine(entries.Count);
            }
        }
    }
}
