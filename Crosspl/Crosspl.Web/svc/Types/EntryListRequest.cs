using System;
using System.Runtime.Serialization;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class EntryListRequest
    {
        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public long IncludeEntryId { get; set; }
    }
}