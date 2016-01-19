using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class ReactionResponse : BaseResponse
    {
        [DataMember]
        public ReactionSummary ReactionSummary { get; set; }
    }
}