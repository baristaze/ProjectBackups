using System;
using System.Runtime.Serialization;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    [DataContract]
    public class VoteResponse : BaseResponse
    {
        [DataMember()]
        public VotingSummary VotingSummary { get; set; }
    }
}