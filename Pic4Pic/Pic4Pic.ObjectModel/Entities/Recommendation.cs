using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    public partial class Recommendation : Identifiable, IDBEntity
    {
        public Guid Id { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public DateTime RecommendTimeUTC { get; set; }
    }
}
