using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    public partial class Action : Identifiable, IDBEntity
    {
        public Guid Id { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionTimeUTC { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime NotifScheduleTimeUTC { get; set; }
        public DateTime NotifSentTimeUTC { get; set; }
        public DateTime NotifViewTimeUTC { get; set; }
        
        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture, 
                "{0} by {1}", 
                this.ActionType, 
                this.UserId1);
        }
    }
}
