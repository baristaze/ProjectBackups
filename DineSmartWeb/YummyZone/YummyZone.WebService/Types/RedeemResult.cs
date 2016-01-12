using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class RedeemResult : BaseResponse
    {
        [DataMember()]
        public Guid CheckedInVenueId { get; set; }
    }
}