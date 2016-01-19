using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class EntryResponse : BaseResponse
    {
        [DataMember]
        public Entry Entry { get; set; }
    }
}