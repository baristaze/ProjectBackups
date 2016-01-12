using System;

namespace YummyZone.ObjectModel
{
    public partial class MarkAs : IEditable
    {
        public Guid CheckInId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ReadTimeUTC { get; set; }

        public MarkAs() : this(Guid.Empty, Guid.Empty) { }

        public MarkAs(Guid checkInId, Guid userId)
        {
            this.CheckInId = checkInId;
            this.UserId = userId;
            this.ReadTimeUTC = DateTime.UtcNow;
        }
    }
}
