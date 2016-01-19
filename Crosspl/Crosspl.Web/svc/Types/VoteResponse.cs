using System;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class VoteResponse : BaseResponse
    {
        [DataMember()]
        public VotingSummary VotingSummary { get; set; }
    }
}