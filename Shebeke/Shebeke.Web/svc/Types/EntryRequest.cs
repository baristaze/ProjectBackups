using System;
using System.Runtime.Serialization;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class EntryRequest
    {
        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public long EntryId { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public bool ShareOnFacebook { get; set; }

        [DataMember]
        public bool ShareOnTwitter { get; set; }
    }
}