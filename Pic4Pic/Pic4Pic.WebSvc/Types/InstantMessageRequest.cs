using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    public class InstantMessageRequest : BaseRequest
    {
        [DataMember()]
        public Guid UserIdToInteract { get; set; }

        [DataMember()]
        public String Content { get; set; }

        public override string ToString()
        {
            return this.Content;
        }

        public override void Validate()
        {
            base.Validate();

            if (this.UserIdToInteract == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown User Id to send instant message");
            }

            if (String.IsNullOrWhiteSpace(this.Content))
            {
                throw new Pic4PicArgumentException("Instant message is empty");
            }
        }
    }
}