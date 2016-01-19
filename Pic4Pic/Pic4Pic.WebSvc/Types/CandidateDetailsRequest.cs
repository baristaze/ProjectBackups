using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class CandidateDetailsRequest : BaseRequest
    {
        [DataMember()]
        public Guid UserId { get; set; }

        public override string ToString()
        {
            return this.UserId.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            if (this.UserId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown User Id to retrieve details");
            }
        }
    }
}