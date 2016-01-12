using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class Group : IEditable
    {
        public Guid Id { get; set; }
        public VenueStatus Status { get; set; }
        public string Name { get; set; }
        public string WebURL { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public Group() : this(Guid.Empty) { }
        public Group(Guid id)
        {
            this.Id = id;
           
            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}

