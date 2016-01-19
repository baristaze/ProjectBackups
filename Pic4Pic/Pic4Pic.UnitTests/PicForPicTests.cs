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
    public class PicForPicTests
    {
        [TestMethod]
        public void Test_Pic4PicRequests()
        {
            Guid userId = new Guid("32141998-80F2-4659-83E9-B112C3F649EF");

            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionString))
            {
                conn.Open();

                Familiarity currentFamiliarity = Familiarity.Stranger;
                Guid matchingUserId = new Guid("4922D083-E9D1-4C7B-9CE4-027AC9AD58F7");
                PicForPic pi4pic = PicTradeEngine.RequestPic4Pic(conn, null, userId, matchingUserId, Guid.Empty, 1 * 24 * 60, ref currentFamiliarity);

                PicForPic pi4picX = PicTradeEngine.AcceptPic4PicRequest(conn, null, matchingUserId, pi4pic.Id, Guid.Empty);
                pi4picX = PicTradeEngine.AcceptPic4PicRequest(conn, null, matchingUserId, pi4pic.Id, Guid.Empty);
            }
        }    
    }
}
