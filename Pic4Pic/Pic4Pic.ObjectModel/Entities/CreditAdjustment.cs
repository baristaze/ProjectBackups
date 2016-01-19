using System;

namespace Pic4Pic.ObjectModel
{
    public partial class CreditAdjustment : IDBEntity
    {
        public Guid UserId { get; set; }
        public int Credit { get; set; }
        public CreditAdjustmentReason Reason { get; set; }
        public DateTime CreateTimeUTC { get; set; }
    }
}
