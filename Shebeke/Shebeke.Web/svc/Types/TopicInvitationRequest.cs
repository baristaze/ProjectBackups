using System;
using System.Runtime.Serialization;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class TopicInvitationRequest
    {
        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public long EntryId { get; set; }

        [DataMember]
        public long AppRequestId { get; set; }

        [DataMember]
        public short InviteeCount { get; set; }
    }
}