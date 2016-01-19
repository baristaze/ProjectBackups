using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class MobileDeviceRequest : BaseRequest
    {
        [DataMember()]
        public String OSVersion { get; set; }

        [DataMember()]
        public String AppVersion { get; set; }

        [DataMember()]
        public String SDKVersion { get; set; }

        [DataMember()]
        public String DeviceType { get; set; }

        [DataMember()]
        public String PushNotifRegId { get; set; }

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(this.PushNotifRegId))
            {
                return this.DeviceType;
            }
            else
            {
                return this.DeviceType + " : " + this.PushNotifRegId;
            }
        }

        public override void Validate()
        {
            base.Validate();

            // all of them are optional
            /*
            if (String.IsNullOrWhiteSpace(this.OSVersion))
            {
                throw new Pic4PicArgumentException("OSVersion is null or empty", "OSVersion");
            }

            if (String.IsNullOrWhiteSpace(this.AppVersion))
            {
                throw new Pic4PicArgumentException("AppVersion is null or empty", "AppVersion");
            }

            if (String.IsNullOrWhiteSpace(this.SDKVersion))
            {
                throw new Pic4PicArgumentException("SDKVersion is null or empty", "SDKVersion");
            }

            if (String.IsNullOrWhiteSpace(this.DeviceType))
            {
                throw new Pic4PicArgumentException("DeviceType is null or empty", "DeviceType");
            }

            if (String.IsNullOrWhiteSpace(this.PushNotifRegId))
            {
                throw new Pic4PicArgumentException("PushNotifRegId is null or empty", "PushNotifRegId");
            }
            */
        }

        public MobileDevice CreateMobileDevice(Guid userId, MobileOSType osType)
        {
            DateTime utcNow = DateTime.UtcNow;

            MobileDevice device = new MobileDevice();

            device.ClientId = this.ClientId;
            device.UserId = userId;
            device.OSType = osType;
            device.OSVersion = this.OSVersion;
            device.AppVersion = this.AppVersion;
            device.SDKVersion = this.SDKVersion;
            device.DeviceType = this.DeviceType;
            device.PushNotifRegId = this.PushNotifRegId;
            device.CreateTimeUTC = utcNow;
            device.LastUpdateTimeUTC = utcNow;

            return device;
        }
    }
}