using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public class BaseResponse
    {
        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool NeedsRelogin { get; set; }

        public BaseResponse() : this(0, "Success") { }

        public BaseResponse(int code, string message)
        {
            this.ErrorCode = code;
            this.ErrorMessage = message;
        }
    }

    [DataContract]
    public class SimpleResponse<T> : BaseResponse
    {
        [DataMember]
        public T Data { get; set; }
    }

    [DataContract]
    public class SimpleListResponse<T> : BaseResponse
    {
        [DataMember]
        public List<T> Items { get { return this.items; } }
        private List<T> items = new List<T>();
    }
}