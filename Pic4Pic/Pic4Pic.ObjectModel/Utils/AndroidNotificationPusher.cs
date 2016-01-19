using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Pic4Pic.ObjectModel
{
    public class AndroidNotificationPusher : BaseTask<AndroidNotificationPusherConfig>
    {
        private int heartbeatLog = 1;
        private DateTime stalePushTimeUTC = new DateTime(2000, 1, 1, 0, 0, 0);

        public AndroidNotificationPusher(AndroidNotificationPusherConfig config) 
        {
            this.Config = config;
        }

        public void Run()
        {
            Logger.bag().LogInfo("AndroidNotificationPusher is starting");
            Logger.bag().LogInfo(this.Config.ToString());
            
            while (true)
            {
                if (!this.Init(this.Config.TaskConfigMeta))
                {
                    Logger.bag().LogError("AndroidNotificationPusher couldn't be initialized");
                }
                
                // run either new or old config
                this.PerformTask(this.Config);
            }
        }

        public override void PerformTask(AndroidNotificationPusherConfig config)
        {
            DateTime localTimeAtRefZone = config.ReferenceTimeLocal;
            int localTimeAsMinutes = localTimeAtRefZone.Hour * 60 + localTimeAtRefZone.Minute;
            if (localTimeAsMinutes > config.EndingTimeForPushes && localTimeAsMinutes < config.StartingTimeForPushes) // do not send between 2am - 9am
            {
                if ((++heartbeatLog) >= config.HeartbeatLogFreq)
                {
                    // Logger.bag().LogInfo("It is not a good time to push a notification.");
                    // Logger.bag().LogInfo("Sleeping " + config.SleepSeconds + " seconds");
                    heartbeatLog = 0;
                }

                Thread.Sleep(config.SleepSeconds * 1000);
                return;
            }


            int successfulPushes = 0;

            try
            {
                successfulPushes = this.Work(config);
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError();
            }

            if (successfulPushes == 0)
            {
                // nothing to push or error
                // Logger.bag().LogInfo("Sleeping " + config.SleepSeconds + " seconds");
                Thread.Sleep(config.SleepSeconds * 1000);
            }

            // Logger.bag().LogInfo("Working");
        }

        private int Work(AndroidNotificationPusherConfig config)
        {
            int successfulPushes = 0;
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                // getting notifications
                NotificationCalculator calc = new NotificationCalculator();
                List<NotificationBag> bags = calc.GetNotificationsToPush(connection, null, true);

                // push notifications
                if (bags.Count > 0)
                {
                    Logger.bag().LogInfo("Number of notifications that need to be pushed: " + bags.Count);

                    foreach (NotificationBag bag in bags)
                    {
                        int sentDeviceCount = this.SafePush(connection, null, bag.UserId, bag.CombinedActions, bag.SummarizedNotification, config.GoogleApiKey, bag.DeviceKeys);
                        if (sentDeviceCount > 0)
                        {
                            successfulPushes++;
                        }
                    }

                    string msg = "{0} out of {1} notifications have been queued";
                    msg = String.Format(CultureInfo.InvariantCulture, msg, successfulPushes, bags.Count);
                    Logger.bag().LogInfo(msg);

                    if (successfulPushes > 0)
                    {
                        // Logger.bag().LogInfo("Sleeping " + config.SleepSecondsAfterTransmission + " seconds");
                        Thread.Sleep(config.SleepSecondsAfterTransmission * 1000);
                    }
                }

                // sending email
                if (bags.Count > 0 && config.SendStatusEmailOnPushNotifications)
                {
                    this.SendEmail(config.MailSettings, successfulPushes, bags.Count);
                }
            }

            return successfulPushes;
        }

        private void SendEmail(MailSettingsEx mailSettings, int successfulPush, int totalPush)
        {
            if (mailSettings != null)
            {
                string msg = "{0} out of {1} notifications have been sent";
                msg = String.Format(CultureInfo.InvariantCulture, msg, successfulPush, totalPush);

                string body = msg;
                
                Logger.bag().LogInfo("Sending email about the notification pushes...");

                MailSender mailSender = new MailSender(mailSettings);
                if (mailSender.TrySend(msg, body, mailSettings.EmailToList, mailSettings.EmailCCList, mailSettings.EmailBCCList))
                {
                    Logger.bag().LogInfo("Email has been sent about the notification pushes successfully");
                }
                else
                {
                    Logger.bag().LogError("Email couldn't be sent about the notification pushes.");
                }
            }
            else
            {
                // Logger.bag().LogError("Omiting email part...");
            }
        }

        private int SafePush(SqlConnection connection, SqlTransaction trans, Guid userId, List<Action> actionsForSingleUser, PushNotification summaryNotification, String googleApiKey, List<string> deviceKeys)
        {
            int sentDeviceCount = 0;
            foreach (string deviceKey in deviceKeys)
            {
                try
                {
                    if (summaryNotification.send(googleApiKey, deviceKey))
                    {
                        sentDeviceCount++;
                    }
                    else
                    {
                        Logger.bag()
                            .Add("userid", userId.ToString())
                            .Add("device", deviceKey)
                            .Add("title", summaryNotification.Title)
                            .Add("msg", summaryNotification.Message)
                            .LogError("Couldn't send the notification");
                    }
                }
                catch (Exception ex)
                {
                    Logger.bag().Add(ex).LogError();
                }
            }

            if (sentDeviceCount > 0)
            {
                string concatenatedActionIDs = Pic4PicUtils.ConcatenateIDs<Action>(actionsForSingleUser);
                this.SafeMarkAsPushed(connection, trans, concatenatedActionIDs, DateTime.UtcNow);
            }
            else
            {
                // 
            }

            return sentDeviceCount;
        }

        private int SafeMarkAsPushed(SqlConnection connection, SqlTransaction trans, string concatenatedActionIDs, DateTime pushTimeUTC)
        {
            try
            {
                return this.MarkAsPushed(connection, trans, concatenatedActionIDs, pushTimeUTC);
            }
            catch (Exception ex)
            {
                Logger.bag().Add(ex).LogError();
                return 0;
            }
        }

        private int MarkAsPushed(SqlConnection conn, SqlTransaction trans, string concatenatedActionIDs, DateTime pushTimeUTC)
        {
            //string concatenatedActionIDs = Pic4PicUtils.ConcatenateIDs<Action>(actionItems);
            return Action.MarkNotificationsAsSentOnDBase(conn, trans, concatenatedActionIDs);
        }
    }
}
