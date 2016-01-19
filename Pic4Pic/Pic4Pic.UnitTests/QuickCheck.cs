using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class QuickCheck
    {
        [TestMethod]
        public void Test_Foo()
        {
            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionStringProduction))
            {
                conn.Open();
                /*
                // determine home-town city and state....                
                string concatenatedCitiesInRange = "Seattle,Redmond,Kirkland,Bellevue";
                string hometownState = "Washington";

                // get today's matches
                List<MatchedCandidate> matches = MatchEngine.PrepareTodaysMatches(
                    conn, null, new Guid("20983B34-8996-4FAC-9120-3BF6F96791FE"), MatchConfig.CreateDefault(), hometownState, concatenatedCitiesInRange);

                Console.WriteLine("Match count: " + matches.Count);
                */
            }
        }
    }
}
