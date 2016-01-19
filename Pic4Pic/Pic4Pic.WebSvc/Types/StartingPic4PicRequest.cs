using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class StartingPic4PicRequest : BaseRequest
    {
        [DataMember()]
        public Guid UserIdToInteract { get; set; }

        [DataMember()]
        public Guid PictureIdToExchange { get; set; }

        public override string ToString()
        {
            return this.PictureIdToExchange.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            if (this.UserIdToInteract == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown User Id to send pic4pic request");
            }

            if (this.PictureIdToExchange == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown Picture ID to exchange");
            }
        }
    }
}