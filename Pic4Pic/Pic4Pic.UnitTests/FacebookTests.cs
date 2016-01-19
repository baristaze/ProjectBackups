using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class FacebookTests
    {
        [TestMethod]
        public void TestMethodCreateSaveGet()
        {
            WorkHistory workHistory = null;
            EducationHistory eduHistory = null;
            FacebookUser fbUser = null;
            try
            {
                fbUser = FacebookHelpers.GetUserFromFacebook(TestConstants.FacebookToken, out workHistory, out eduHistory);
                Console.WriteLine(fbUser.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionString))
            {
                conn.Open();

                try
                {
                    fbUser.UserId = TestConstants.testUserId;
                    int i = fbUser.CreateNewOnDBase(conn, null);
                    Console.WriteLine("Affected Record Count = " + i);

                    FacebookUser fbUser2 = FacebookUser.ReadFromDBase(conn, null, fbUser.FacebookId);
                    fbUser2.CreateTimeUTC = fbUser.CreateTimeUTC;
                    fbUser2.LastUpdateTimeUTC = fbUser.LastUpdateTimeUTC;
                    if (fbUser2.Merge(fbUser))
                    {
                        throw new ApplicationException("Some properties are not same");
                    }

                    fbUser2.FirstName += "x";
                    fbUser2.LastName += "x";
                    fbUser2.Profession += "x";
                    fbUser2.HometownState += "x";
                    fbUser2.PhotoUrl += "x";
                    i = fbUser2.UpdateOnDBase(conn, null);
                    Console.WriteLine("Affected Record Count = " + i);

                    if (workHistory != null && workHistory.Count > 0)
                    {
                        i = workHistory.CreateAllOnDBase(conn, null, fbUser2.FacebookId);
                        Console.WriteLine("Affected Record Count = " + i);

                        WorkHistory workHistory2 = WorkHistory.ReadAllFromDatabase(conn, null, fbUser2.FacebookId);
                        if (workHistory.Count != workHistory2.Count)
                        {
                            throw new ApplicationException("Work History Failed");
                        }

                        i = WorkHistory.DeleteAllFromDBase(conn, null, fbUser2.FacebookId);
                        Console.WriteLine("Affected Record Count = " + i);
                    }

                    if (eduHistory != null && eduHistory.Count > 0)
                    {
                        i = eduHistory.CreateAllOnDBase(conn, null, fbUser2.FacebookId);
                        Console.WriteLine("Affected Record Count = " + i);

                        EducationHistory eduHistory2 = EducationHistory.ReadAllFromDatabase(conn, null, fbUser2.FacebookId);
                        if (eduHistory.Count != eduHistory2.Count)
                        {
                            throw new ApplicationException("Edu History Failed");
                        }

                        i = EducationHistory.DeleteAllFromDBase(conn, null, fbUser2.FacebookId);
                        Console.WriteLine("Affected Record Count = " + i);
                    }

                    i = fbUser.DeleteFromDBase(conn, null);
                    Console.WriteLine("Affected Record Count = " + i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw ex;
                }


            }
        }
    }
}
