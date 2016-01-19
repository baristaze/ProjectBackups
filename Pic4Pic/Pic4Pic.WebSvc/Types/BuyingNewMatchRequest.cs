using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [DataContract()]
    public class BuyingNewMatchRequest : BaseRequest
    {
        [DataMember()]
        public int MaxCount { get; set; }

        [DataMember()]
        public Location Location { get; set; }

        public override string ToString()
        {
            return this.MaxCount.ToString();
        }

        public override void Validate()
        {
            this.Validate(false);
        }

        public virtual void Validate(bool canLocationBeNull)
        {
            base.Validate();

            if (this.MaxCount <= 0)
            {
                throw new Pic4PicArgumentException("Please specify amount of new match that you want to purchase", "MaxCount");
            }

            if (this.Location == null)
            {
                if (!canLocationBeNull)
                {
                    throw new Pic4PicArgumentException("Location may not be null", "Location");
                }
            }
            else 
            {
                this.Location.Validate();
            }
        }
    }
}