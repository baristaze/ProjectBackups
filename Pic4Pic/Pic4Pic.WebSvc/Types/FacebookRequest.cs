using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class FacebookRequest : BaseRequest
    {
        [DataMember()]
        public long FacebookUserId { get; set; }

        [DataMember()]
        public string FacebookAccessToken { get; set; }

        public override string ToString()
        {
            return this.FacebookUserId.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            if (this.FacebookUserId <= 0)
            {
                throw new Pic4PicArgumentException("Facebook UserId is invalid", "FacebookUserId");
            }

            if (String.IsNullOrWhiteSpace(this.FacebookAccessToken))
            {
                throw new Pic4PicArgumentException("Facebook Access Token is invalid", "FacebookAccessToken");
            }
        }
    }
}