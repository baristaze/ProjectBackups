using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public enum Role
    {
        VenueManager = 0,
    }

    public partial class UserRole : IEditable
    {
        public Guid GroupId { get; set; }
        public Guid VenueId { get; set; }
        public Guid UserId { get; set; }
        public Role Role { get; set; }
        public Status Status { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public UserRole() : this(Guid.Empty) { }

        public UserRole(Guid groupId)
        {
            this.GroupId = groupId;

            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }
    }
}
