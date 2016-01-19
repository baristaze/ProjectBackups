using System;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Shebeke.ObjectModel;

namespace Shebeke.UnitTests
{
    [TestClass]
    public class UserTests
    {        
        [TestMethod]
        public void TestFacebookUserEntity()
        {
            using (SqlConnection conn = new SqlConnection(Constants.ConnectionString))
            {
                conn.Open();

                long testUser1 = Database.ExecScalar<long>(conn, null, "[dbo].[CreateNewUser]", Database.SqlParam("@SplitId", DBNull.Value));
                long testUser2 = Database.ExecScalar<long>(conn, null, "[dbo].[CreateNewUser]", Database.SqlParam("@SplitId", DBNull.Value));
                long testUser3 = Database.ExecScalar<long>(conn, null, "[dbo].[CreateNewUser]", Database.SqlParam("@SplitId", DBNull.Value));

                FacebookUser fb = new FacebookUser();
                fb.Id = testUser1;
                fb.FacebookId = long.MaxValue;
                fb.CreateNewOnDBase(conn, null);

                FacebookUser fb1x = FacebookUser.ReadFromDBase(conn, null, long.MaxValue);
                if (fb1x != null)
                {
                    if (fb.Id != fb1x.Id)
                    {
                        throw new ApplicationException("fb.Id != fb1x.Id after select");
                    }
                }
                else
                {
                    throw new ApplicationException("couldn't be read");
                }

                FacebookUser fb2 = new FacebookUser();
                fb2.Id = testUser2;
                fb2.FacebookId = long.MaxValue - 1;
                fb2.CreateNewOnDBase(conn, null);

                FacebookUser fb3 = new FacebookUser();
                fb3.Id = testUser3;
                fb3.FacebookId = long.MaxValue - 2;
                fb3.CreateNewOnDBase(conn, null);
                
                FacebookFriend ff12 = new FacebookFriend();
                ff12.FacebookId1 = fb.FacebookId;
                ff12.FacebookId2 = fb2.FacebookId;
                Database.InsertOrUpdate(ff12, Constants.ConnectionString);

                FacebookFriend ff12x = new FacebookFriend();
                ff12x.FacebookId1 = fb.FacebookId;
                ff12x.FacebookId2 = fb2.FacebookId;
                if (Database.Select(ff12x, Constants.ConnectionString))
                {
                    if (ff12x.FacebookId1 != fb.FacebookId || ff12x.FacebookId2 != fb2.FacebookId)
                    {
                        throw new ApplicationException("ff12x.FacebookId1 != fb.Id || ff12x.FacebookId2 != fb2.Id");
                    }
                }
                else
                {
                    throw new ApplicationException("couldn't be read");
                }

                FacebookFriend ff13 = new FacebookFriend();
                ff13.FacebookId1 = fb.FacebookId;
                ff13.FacebookId2 = fb3.FacebookId;
                Database.InsertOrUpdate(ff13, Constants.ConnectionString);

                FacebookFriendList ffList = Database.SelectAll<FacebookFriend, FacebookFriendList>(Constants.ConnectionString, fb.FacebookId);
                if (ffList.Count != 2)
                {
                    throw new ApplicationException("ffList.Count != 2");
                }

                Database.Delete(ff12, Constants.ConnectionString);
                Database.Delete(ff13, Constants.ConnectionString);

                if (fb.DeleteFromDBase(conn, null) <= 0)
                {
                    throw new ApplicationException("couldn't delete");
                }

                if (fb2.DeleteFromDBase(conn, null) <= 0)
                {
                    throw new ApplicationException("couldn't delete");
                }

                if (fb3.DeleteFromDBase(conn, null) <= 0)
                {
                    throw new ApplicationException("couldn't delete");
                }

                Database.ExecNonQuery(conn, null, "[dbo].[DeleteUser]", Database.SqlParam("@Id", testUser1));
                Database.ExecNonQuery(conn, null, "[dbo].[DeleteUser]", Database.SqlParam("@Id", testUser2));
                Database.ExecNonQuery(conn, null, "[dbo].[DeleteUser]", Database.SqlParam("@Id", testUser3));
            }
        }
    }
}
