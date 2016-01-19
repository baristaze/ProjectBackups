using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class TopicListResponse : BaseResponse
    {
        [DataMember]
        public List<Topic> Topics { get { return this.topics; } }
        private List<Topic> topics = new List<Topic>();
    }
}