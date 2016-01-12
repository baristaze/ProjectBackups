using System;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using YummyZone.ObjectModel;

namespace YummyZone.UnitTests
{
    [TestClass]
    public class PushNotificationTests
    {
        [TestMethod]
        public void SendDeviceTokenTest()
        {
            using (SqlConnection connection = new SqlConnection(Constants.ConnectionString))
            {
                string user = AuthTests.GetTestUserName(3);
                string pswd = AuthTests.GetTestUserPswd(3);

                connection.Open();
                AuthTests._CleanUp(connection, user);

                try
                {
                    AuthTests._Signup(user, pswd);
                    AuthTests._Signin(user, pswd);
                    _SendDeviceTokenTest(user, pswd);
                    _SendDeviceTokenTest(user, pswd);
                }
                finally
                {
                    AuthTests._CleanUp(connection, user);
                }
            }
        }

        public static void _SendDeviceTokenTest(string user, string pswd)
        {   
            var parameters = Helpers.WrapAsDictionary("device", GetNewDeviceToken());
            // var headers = AuthTests.GetAuthHeader(cookie);

            string url = Helpers.GetMethodUrl(Constants.WebSvcUrl, Constants.NotifyMethod);
            string response = Helpers.GetRequest(url, parameters, null, user, pswd);
            Helpers.VerifyOperationResult(response);
        }
                
        [TestMethod]
        public void TestDeviceTokenDBaseOperations()
        {
            using (SqlConnection connection = new SqlConnection(Constants.ConnectionString))
            {
                connection.Open();

                Guid dinerId = new Guid("946ABAAD-C146-411C-849A-1BAB8C1904E7");
                _TestDeviceTokenDBaseOperations(connection, dinerId);
            }
        }
        
        private void _TestDeviceTokenDBaseOperations(SqlConnection connection, Guid dinerId)
        {
            NotificationClient info = new NotificationClient();
            info.DinerId = dinerId;
            info.DeviceType = MobileDeviceType.IPhone;
            info.DeviceToken = GetNewDeviceToken();

            // initial
            WriteReadVerify(connection, info, dinerId);

            // duplicate
            WriteReadVerify(connection, info, dinerId);

            // re-do later
            System.Threading.Thread.Sleep(500);
            info.LastUpdateTimeUTC = DateTime.UtcNow;
            WriteReadVerify(connection, info, dinerId);

            // update device token
            System.Threading.Thread.Sleep(500);
            info.LastUpdateTimeUTC = DateTime.UtcNow;
            string previousDeviceToken = info.DeviceToken;
            info.DeviceToken = GetNewDeviceToken();
            WriteReadVerify(connection, info, dinerId);

            // delete
            int affectedRows = Database.Delete(info, Constants.ConnectionString);
            if (affectedRows != 1)
            {
                throw new YummyTestException("Couldn't delete test NotificationClient");
            }

            // delete other as well
            info.DeviceToken = previousDeviceToken;            
            affectedRows = Database.Delete(info, Constants.ConnectionString);
            if (affectedRows != 1)
            {
                throw new YummyTestException("Couldn't delete test NotificationClient");
            }
        }

        private static string GetNewDeviceToken()
        {
            return String.Format(
                "{0}{1}{2}",
                Guid.NewGuid().ToString("N"),
                Guid.NewGuid().ToString("N"),
                Guid.NewGuid().ToString("N"));
        }

        private static void WriteReadVerify(SqlConnection connection, NotificationClient info, Guid dinerId)
        {
            Database.InsertOrUpdate(info, Constants.ConnectionString);

            NotificationClientList list = Database.SelectAll<NotificationClient, NotificationClientList>(
                Constants.ConnectionString, dinerId);

            if (FindExact(list, info) == null)
            {
                throw new YummyTestException("Couldn't find the exact same NotificationClient in the DB");
            }
        }

        private static NotificationClient FindExact(NotificationClientList list, NotificationClient target)
        {
            foreach (NotificationClient item in list)
            {
                if (IsEqual(item, target))
                {
                    return item;
                }
            }

            return null;
        }

        private static bool IsEqual(NotificationClient first, NotificationClient second)
        {
            if (first.DinerId != second.DinerId) 
            {
                return false;
            }

            if (first.DeviceType != second.DeviceType)
            {
                return false;
            }

            if (String.Compare(first.DeviceToken, second.DeviceToken, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            if (Helpers.CompareSqlDateTime(first.CreateTimeUTC, second.CreateTimeUTC) != 0) 
            {
                return false;
            }

            if (Helpers.CompareSqlDateTime(first.LastUpdateTimeUTC, second.LastUpdateTimeUTC) != 0)
            {
                return false;
            }

            return true;
        }
    }
}
