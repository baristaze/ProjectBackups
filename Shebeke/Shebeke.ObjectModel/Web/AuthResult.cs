using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract()]
    public class AuthResult : BaseResponse
    {
        [DataMember()]
        public UserAuthInfo AuthInfo { get; set; }
    }
}
