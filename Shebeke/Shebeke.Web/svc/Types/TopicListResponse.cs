using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class TopicListResponse : BaseResponse
    {
        [DataMember]
        public List<Topic> Topics { get { return this.topics; } }
        private List<Topic> topics = new List<Topic>();
    }
}