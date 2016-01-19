using System;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class LogRequest
    {
        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public long EntryId { get; set; }

        [DataMember]
        public SocialChannel Channel { get; set; }
    }
}