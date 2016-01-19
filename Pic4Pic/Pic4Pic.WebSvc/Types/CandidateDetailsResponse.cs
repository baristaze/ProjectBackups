using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class CandidateDetailsResponse : BaseResponse
    {
        [DataMember()]
        public MatchedCandidate Candidate { get; set; }
    }
}