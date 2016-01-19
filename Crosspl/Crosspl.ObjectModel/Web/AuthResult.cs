using System;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract()]
    public class AuthResult : BaseResponse
    {
        [DataMember()]
        public UserAuthInfo AuthInfo { get; set; }
    }
}
