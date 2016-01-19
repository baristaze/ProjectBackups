using System;
using System.Globalization;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    public class ConversationRequest : BaseRequest
    {
        [DataMember()]
        public Guid UserIdToInteract { get; set; }

        [DataMember()]
        public Guid LastExchangedMessageId { get; set; }

        public override string ToString()
        {
            return this.UserIdToInteract.ToString();
        }

        public override void Validate()
        {
            base.Validate();

            if (this.UserIdToInteract == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown User Id to send instant message");
            }

            /* this is optional
            if (this.LastExchangedMessageId == Guid.Empty)
            {
                throw new Pic4PicArgumentException("Unknown message Id that was exchanged last time");
            }
            */
        }
    }
}