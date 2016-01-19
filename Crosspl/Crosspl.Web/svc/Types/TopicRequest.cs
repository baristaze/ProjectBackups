using System;
using System.Runtime.Serialization;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class TopicRequest
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public bool ShareOnFacebook { get; set; }

        [DataMember]
        public bool ShareOnTwitter { get; set; }
    }
}