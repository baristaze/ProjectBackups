using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public enum MobileDeviceType
    {
        None = 0,
        IPhone
    }

    public partial class NotificationClient : IEditable
    {
        public NotificationClient() : this(Guid.Empty) { }
        public NotificationClient(Guid dinerId)
        {
            this.DinerId = dinerId;
           
            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        public Guid DinerId { get; set; }
        public MobileDeviceType DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }
    }

    public class NotificationClientList : List<NotificationClient>
    {
        public List<string> AllDeviceTokens
        {
            get
            {
                List<string> tokens = new List<string>();
                foreach (NotificationClient item in this)
                {
                    tokens.Add(item.DeviceToken);
                }

                return tokens;
            }
        }
    }
}
