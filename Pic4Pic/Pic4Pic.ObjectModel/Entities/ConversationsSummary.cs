using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public partial class ConversationsSummary : IDBEntity
    {
        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public int UnreadMessageCount { get; set; }

        [DataMember()]
        public DateTime LastUpdateUTC { get; set; }
    }
}
