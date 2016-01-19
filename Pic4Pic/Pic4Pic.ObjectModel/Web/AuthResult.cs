using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class AuthResult : BaseResponse
    {
        [DataMember()]
        public UserAuthInfo AuthInfo { get; set; }
    }
}
