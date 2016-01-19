using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class EntryResponse : BaseResponse
    {
        [DataMember]
        public Entry Entry { get; set; }
    }
}