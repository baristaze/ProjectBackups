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
    public class ActionTests
    {
        [TestMethod]
        public void Test_GettingRecentActions()
        {
            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionString))
            {
                conn.Open();

                // get today's matches
                Guid userId = new Guid("32141998-80F2-4659-83E9-B112C3F649EF");

                int cutOffMinutes = 30 * 24 * 60; // 30 days
                List<Interaction> interactions = InteractionEngine.GetRecentInteractions(conn, null, userId, 50, cutOffMinutes, MatchConfig.CreateDefault());
            }
        }
    }
}
