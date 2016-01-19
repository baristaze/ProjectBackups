using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class ReactionResponse : BaseResponse
    {
        [DataMember]
        public ReactionSummary ReactionSummary { get; set; }
    }
}