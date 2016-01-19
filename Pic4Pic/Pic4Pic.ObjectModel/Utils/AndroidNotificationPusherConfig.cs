using System;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Pic4Pic.ObjectModel
{
    public class AndroidNotificationPusherConfig : ConfigOnDBase
    {
        public int ReferenceTimeZoneWinIndex { get; set; }

        public string ReferenceTimeZoneName
        {
            get
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                if (this.ReferenceTimeZoneWinIndex >= 0 && this.ReferenceTimeZoneWinIndex < timeZones.Count)
                {
                    TimeZoneInfo tzi = timeZones[this.ReferenceTimeZoneWinIndex];
                    return tzi.DisplayName;
                }

                return String.Empty;
            }
        }

        public DateTime ReferenceTimeLocal
        {
            get
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                if(this.ReferenceTimeZoneWinIndex >= 0 && this.ReferenceTimeZoneWinIndex < timeZones.Count)
                {
                    TimeZoneInfo tzi = timeZones[this.ReferenceTimeZoneWinIndex];
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
                }

                return DateTime.Now;
            }
        }

        public int StartingTimeForPushes{ get; set; }

        public int EndingTimeForPushes { get; set; }

        public string DBaseConnectionString { get; set; }
        
        public string GoogleApiKey { get; set; }
        
        public int SleepSeconds { get; set; }
        
        public int SleepSecondsAfterTransmission { get; set; }
        
        public int HeartbeatLogFreq { get; set; }

        public bool SendStatusEmailOnPushNotifications { get; set; }

        public MailSettingsEx MailSettings { get; set; }

        public AndroidNotificationPusherConfig()
        {            
        }

        public override bool Init()
        {
            if (!base.Init())
            {
                return false;
            }

            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            this.ReferenceTimeZoneWinIndex = this.GetAsInt("ReferenceTimeZoneWinIndex", 5, 0, timeZones.Count - 1);
            this.StartingTimeForPushes = this.GetAsInt("StartingTimeForPushes", 540, 0, 1439);
            this.EndingTimeForPushes = this.GetAsInt("EndingTimeForPushes", 1350, 0, 1439);
            this.DBaseConnectionString = this.Get("DBConnectionString");
            this.GoogleApiKey = this.Get("GoogleApiKey");
            this.SleepSeconds = this.GetAsInt("SleepSeconds", 60, 0, 86400);
            this.SleepSecondsAfterTransmission = this.GetAsInt("SleepSecondsAfterTransmission", 1, 0, 86400);
            this.HeartbeatLogFreq = this.GetAsInt("HeartbeatLogFreq", 1, 0, 500);
            this.SendStatusEmailOnPushNotifications = this.GetAsBool("SendStatusEmailOnPushNotifications", true);

            this.MailSettings = this.GetMailSettingsEx();
            if (this.MailSettings.IsEmailEnabled && !this.MailSettings.IsValid())
            {
                this.MailSettings = null;
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "ReferenceTimeZoneWinIndex", this.ReferenceTimeZoneWinIndex);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "ReferenceTimeZone", this.ReferenceTimeZoneName);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "ReferenceTimeLocal", this.ReferenceTimeLocal);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "StartingTimeForPushes", this.StartingTimeForPushes);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}:{2}", "StartingTime", (this.StartingTimeForPushes / 60).ToString("0#"), (this.StartingTimeForPushes % 60).ToString("0#"));
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "EndingTimeForPushes", this.EndingTimeForPushes);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}:{2}", "EndingTime", (this.EndingTimeForPushes / 60).ToString("0#"), (this.EndingTimeForPushes % 60).ToString("0#"));
            str.AppendLine();
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "DBConnectionString", this.DBaseConnectionString);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "GoogleApiKey", this.GoogleApiKey);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SleepSeconds", this.SleepSeconds);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SleepSecondsAfterTransmission", this.SleepSecondsAfterTransmission);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "HeartbeatLogFreq", this.HeartbeatLogFreq);
            str.AppendLine();
            str.AppendFormat("\t{0}:\t{1}", "SendStatusEmailOnPushNotifications", this.SendStatusEmailOnPushNotifications);
            str.AppendLine();
            str.AppendLine();

            if (this.MailSettings != null)
            {
                str.Append(this.MailSettings.ToString());
            }
            else
            {
                str.AppendFormat("\t{0}:\t{1}", "IsEmailEnabled", false);
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}
