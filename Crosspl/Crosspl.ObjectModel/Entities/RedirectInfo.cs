using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class RedirectInfo : IDBEntity
    {
        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public string TopicTitle { get; set; }

        [DataMember]
        public long EntryId { get; set; }

        [DataMember]
        public long SentBy { get; set; }

        [DataMember]
        public long SenderFacebookId { get; set; }

        [DataMember]
        public string SenderName { get; set; }

        [DataMember]
        public int SenderSplitId { get; set; }

        [DataMember]
        public string SenderPhotoUrl { get; set; }
    }
}
