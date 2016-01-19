using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class NotificationRequest : BaseRequest
    {
        [DataMember()]
        public Guid LastNotificationId { get; set; }

        public override string ToString()
        {
            return this.LastNotificationId.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            /* Not a requirement since initial call will have this EMPTY
            if (this.LastNotificationId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unexpected error");
            }
            */
        }
    }
}