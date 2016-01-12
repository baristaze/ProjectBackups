using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public abstract class YummyZoneEntity
    {
        public YummyZoneEntity() : this(Guid.Empty) { }
        public YummyZoneEntity(Guid groupId) : this(groupId, Guid.NewGuid()) { }
        public YummyZoneEntity(Guid groupId, Guid id)
        {
            this.Id = id;
            this.GroupId = groupId;
           
            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        /// <summary>
        /// This needs to be serialized into JSON; therefore it has 'DataMember'
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }
    }
}
