using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class OperationResult
    {
        [DataMember()]
        public int ErrorCode { get; set; }

        [DataMember()]
        public string ErrorMessage { get; set; }

        public OperationResult() : this(0, "Success") { }

        public OperationResult(int code, string message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
    }
}