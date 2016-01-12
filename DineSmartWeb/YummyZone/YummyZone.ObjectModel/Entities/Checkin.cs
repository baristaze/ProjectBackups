using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public enum AuditFlag
    {
        NotAudited = 0,
        Genuine,
        Suspicious,
        Spam
    }

    public partial class Checkin
    {
        public Guid Id { get; set; }
        public Guid DinerId { get; set; }
        public Guid VenueId { get; set; }
        public DateTime TimeUTC { get; set; }
        public AuditFlag AuditFlag { get; set; }
        public int RatedMenuItemCount { get; private set; }
        public int AnswerCountToSurvey { get; private set; }
        public int CheckinCountInTotal { get; private set; }
        public int CheckinCountAtVenue { get; private set; }
        public int SentCouponCount { get; private set; }
        public int SentMessageCount { get; private set; }
        public int ReadByManager { get; private set; }

        public Checkin()
        {
            this.Id = Guid.NewGuid();
            this.TimeUTC = DateTime.UtcNow;
        }
    }

    public class CheckinList : List<Checkin> 
    {
        public CheckinList FilterBy(Guid venueId)
        {
            CheckinList list = new CheckinList();
            foreach (Checkin item in this)
            {
                if (item.VenueId == venueId)
                {
                    list.Add(item);
                }
            }

            return list;
        }
    }
}
