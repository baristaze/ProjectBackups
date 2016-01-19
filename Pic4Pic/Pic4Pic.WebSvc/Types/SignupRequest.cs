using System;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class SignupRequest : UserCredentials
    {
        [DataMember()]
        public long FacebookUserId { get; set; }

        [DataMember()]
        public string FacebookAccessToken { get; set; }

        [DataMember()]
        public string PhotoUploadReference { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (this.FacebookUserId <= 0)
            {
                throw new Pic4PicArgumentException("Facebook User Id is invalid", "FacebookUserId");
            }

            if (String.IsNullOrWhiteSpace(this.FacebookAccessToken))
            {
                throw new Pic4PicArgumentException("Facebook Access Token is invalid", "FacebookAccessToken");
            }

            if (String.IsNullOrWhiteSpace(this.PhotoUploadReference))
            {
                throw new Pic4PicArgumentException("Photo upload reference is invalid", "PhotoUploadReference");
            }
        }
    }
}
