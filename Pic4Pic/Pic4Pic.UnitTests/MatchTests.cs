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
    public class MatchTests
    {
        [TestMethod]
        public void Test_TodaysMatches() 
        {
            // using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionString))
            {
                conn.Open();

                // get today's matches
                Guid userId = new Guid("32141998-80F2-4659-83E9-B112C3F649EF");
                
                MatchConfig matchConfig = MatchConfig.CreateDefault();
                List<MatchedCandidate> todaysMatches = MatchEngine.PrepareTodaysMatches(conn, null, userId, matchConfig, null, null);
                List<MatchedCandidate> moreMatches = MatchEngine.BuyNewMatches(conn, null, userId, 20, MatchConfig.RematchLimitAsDays * 24 * 60, null, null);

                matchConfig.MaxMatchToShow = 300;
                List<MatchedCandidate> cumulativeMatches = MatchEngine.PrepareTodaysMatches(conn, null, userId, matchConfig, null, null);
                Console.WriteLine(cumulativeMatches.Count);

                /*
                foreach (MatchedCandidate match in cumulativeMatches)
                {
                    Console.WriteLine(match.CandidateProfile.Username);
                }*/
            }
        }

        private string GetDBConnectionString()
        {
            return "SERVER=cl2yszx8kw.database.windows.net;UID=ginger-bizspark-db-admin;PWD=sUphangile-yedi-K;database=ginger-dbase";
        }
    }
}
