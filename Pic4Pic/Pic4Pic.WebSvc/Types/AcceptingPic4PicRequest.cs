using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class AcceptingPic4PicRequest : BaseRequest
    {
        [DataMember()]
        public Guid Pic4PicRequestId { get; set; }

        [DataMember()]
        public Guid PictureIdToExchange { get; set; }

        public override string ToString()
        {
            return this.Pic4PicRequestId.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            if (this.Pic4PicRequestId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid pic4pic request ID");
            }

            if (this.PictureIdToExchange == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Invalid Picture ID to Exchange");
            }
        }
    }
}