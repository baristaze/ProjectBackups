using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class BaseResponse
    {
        [DataMember()]
        public OperationResult OperationResult { get; set; }

        public BaseResponse()
        {
            this.OperationResult = new OperationResult();
        }
    }
}