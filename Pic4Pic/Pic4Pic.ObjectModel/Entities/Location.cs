using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class Location : IVerifiable
    {
        [DataMember()]
        public GeoLocation GeoLocation { get; set; }

        [DataMember()]
        public Locality Locality { get; set; }

        public void Validate()
        {
            if (this.GeoLocation == null)
            {
                throw new Pic4PicException("GeoLocation may not be null");
            }

            if (this.Locality == null)
            {
                throw new Pic4PicException("Locality may not be null");
            }

            this.GeoLocation.Validate();
            this.Locality.Validate();
        }
    }
}
