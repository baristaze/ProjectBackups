using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public partial class InstantMessage : Identifiable, IDBEntity
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public Guid UserId1 { get; set; }

        [DataMember()]
        public Guid UserId2 { get; set; }

        [DataMember()]
        public string Content { get; set; }

        [DataMember()]
        public DateTime SentTimeUTC { get; set; }

        [DataMember()]
        public DateTime ReadTimeUTC { get; set; }
    }
}
