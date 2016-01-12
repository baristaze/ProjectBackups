using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using YummyZone.ObjectModel;
using System.Data.SqlClient;

namespace YummyZone.UnitTests
{
    [TestClass]
    public class AuthTests
    {
        #region Creds

        public static string GetTestUserName(int i)
        {
            i %= 10;
            return new Guid("11111111-1111-1111-1111-00000000000" + i.ToString()).ToString();
        }

        public static string GetTestUserPswd(int i)
        {
            i %= 10;
            return new Guid("10101010-1010-1010-1010-00000000000" + i.ToString()).ToString();
        }

        public static string GetTestUserEmail(int i)
        {
            i %= 10;
            return "StxTestUser" + i.ToString() + "@dinesmart365.com";
        }

        #endregion // Creds

        [TestMethod]
        public void SignupTest()
        {
            using (SqlConnection connection = new SqlConnection(Constants.ConnectionString))
            {
                string user = GetTestUserName(1);
                string pswd = GetTestUserPswd(1);

                connection.Open();
                _CleanUp(connection, user);

                try
                {
                    _Signup(user, pswd);
                }
                finally
                {
                    _CleanUp(connection, user);
                }
            }
        }

        [TestMethod]
        public void SigninTest()
        {
            using (SqlConnection connection = new SqlConnection(Constants.ConnectionString))
            {
                string user = GetTestUserName(2);
                string pswd = GetTestUserPswd(2);

                connection.Open();
                _CleanUp(connection, user);

                try
                {
                    _Signup(user, pswd);
                    _Signin(user, pswd);
                }
                finally
                {
                    _CleanUp(connection, user);
                }
            }
        }

        public static string _Signup(string userName, string pswd)
        {
            return _SignupOrSignin(userName, pswd, true);
        }

        public static string _Signin(string userName, string pswd)
        {
            return _SignupOrSignin(userName, pswd, false);
        }

        public static string _SignupOrSignin(string userName, string pswd, bool signup)
        {
            string content = signup ? TestResources.SignupPList : TestResources.SigninPList;
            content = String.Format(CultureInfo.InvariantCulture, content, userName, pswd);
            string method = signup ? Constants.SignupMethod : Constants.SigninMethod;
            string url = Helpers.GetMethodUrl(Constants.WebSvcUrl, method);
            string response = Helpers.PostRequest(url, content);
            Helpers.VerifyOperationResult(response);

            // get cookie
            string cookie = Helpers.GetPListStringKeyVal(response, "Cookie");
            string decodedText = AsciiBase64Codex.Decode(cookie);
            string expectedDecodedtext = userName + ":" + pswd;
            if (String.Compare(decodedText, expectedDecodedtext, StringComparison.Ordinal) != 0)
            {
                string msg = "Unexpected cookie... Expected = '{0}'. Retrieved = '{1}'";
                msg = String.Format(CultureInfo.InvariantCulture, msg, expectedDecodedtext, decodedText);
                throw new YummyTestException(msg);
            }

            return cookie;
        }

        public static void _CleanUp(SqlConnection connection, string userName)
        {
            string query = @"   declare @dinerId uniqueidentifier;
                                set @dinerId = (select Id from Diner where username = @username);
                                delete from Coupon where ReceiverId = @dinerId;
                                delete from Message where ReceiverId = @dinerId;
                                delete from DinerPassword where username = @username;
                                delete from NotificationClient where DinerId = @dinerId;
                                delete from Diner where Id = @dinerId;";

            using(SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 30;
                command.Parameters.Add(new SqlParameter("@username", userName));

                command.ExecuteNonQuery();
            }
        }

        public static Dictionary<string, string> GetAuthHeader(string cookie)
        {
            return Helpers.WrapAsDictionary("Authorization", "Basic " + cookie);
        }
    }
}
