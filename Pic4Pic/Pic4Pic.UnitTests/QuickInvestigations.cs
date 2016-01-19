using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class QuickInvestigations
    {
        static QuickInvestigations()
        {
            Logger.AppType = "UnitTest";
        }

        [TestMethod]
        public void Investigate()
        {
            String s = Logger.bag().ToString();
            Console.WriteLine(s);

            /*
            using (SqlConnection connection = new SqlConnection(TestConstants.DBConnectionStringProduction))
            {
                connection.Open();

                int splitId = User.ReadTemporarySplitId(connection, null, new Guid("FE672591-EFB4-49DA-893C-8393CCA37D11"));
                Console.WriteLine(splitId);
            }
            */
            /*
            CraigsListParser parser = new CraigsListParser();
            List<CraigsListParser.LinkChain> all = parser.ExtractAllSubDomains(CraigsListParser.RootUrl);
            String s = CraigsListParser.LinkChain.ToStringAll(all, "\t", "\n");
            Console.WriteLine(s);
            */ 
        }
    }
}
