using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public partial class UserPreferences : IDBEntity
    {
        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public Gender InterestedIn { get; set; }

        [DataMember()]
        public DateTime LastUpdateTimeUTC { get; set; }

        public UserPreferences()
        {
            this.LastUpdateTimeUTC = DateTime.UtcNow;
        }
    }
}
