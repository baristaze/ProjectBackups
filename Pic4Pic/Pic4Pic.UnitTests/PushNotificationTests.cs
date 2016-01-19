using System;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.UnitTests
{
    [TestClass]
    public class PushNotificationTests
    {
        private String googleApiKey = "AIzaSyAYVidqUWTItez8neiLhLblheyAN8S_RWk";
        private String userDeviceKey = "APA91bGo4mrhTlRQkiZnLm38KU9O3-s9A5PCeJnQDgQ6MVJpvWBo8kh1ZYyWMszu0-jo_gysaocIXxZczZ2uoZdew9MuW8DYoBNQoCvqznM6pj2VJoofOXEQvqycjplx0aj8UVjGHS_4UTbO48V7V3gt9EY75iJu5g";

        [TestMethod]
        public void Test_SendPushNotificationToMe()
        {
            PushNotification notification = new PushNotification();
            notification.Title = "New alert from pic4pic";
            notification.Message = "Ashley sent you a new pic4pic request";
            notification.NotificationType = 1;
            notification.ActionType = 1;
            notification.ActionData = null;
            notification.SmallIcon = 1;

            bool isSent = notification.send(this.googleApiKey, this.userDeviceKey);

            if (!isSent) 
            {
                throw new ApplicationException("Push notification couldn't be sent to the user");
            }
        }

        [TestMethod]
        public void Test_NotificationCalculator()
        {
            using (SqlConnection conn = new SqlConnection(TestConstants.DBConnectionString))
            {
                conn.Open();

                NotificationCalculator calculator = new NotificationCalculator();
                List<NotificationBag> pushNotifications = calculator.GetNotificationsToPush(conn, null, false);
                int x=1;
                foreach (NotificationBag bag in pushNotifications)
                {
                    Console.WriteLine("{0} Title  : {1}", x, bag.SummarizedNotification.Title);
                    Console.WriteLine("{0} Message: {1}", x, bag.SummarizedNotification.Message);
                    Console.WriteLine("{0} User ID: {1}", x, bag.UserId);
                    Console.WriteLine("{0} Devices: {1}, [{2}{3}]", x, bag.DeviceKeys.Count, bag.DeviceKeys[0], (bag.DeviceKeys.Count > 1 ? ", ..." : ""));
                    Console.WriteLine("{0} Combine: {1}", x, bag.CombinedActions.Count);
                    Console.WriteLine("");
                    x++;
                }
            }
        }
    }
}
