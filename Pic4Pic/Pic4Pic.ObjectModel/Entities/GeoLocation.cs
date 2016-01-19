using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class GeoLocation : IVerifiable
    {
        private const double LAT_MIN = -90.0d;
        private const double LAT_MAX = 90.0d;

        private const double LNG_MIN = -180.0d;
        private const double LNG_MAX = 180.0d;

        [DataMember()]
        public double Latitude { get; set; }

        [DataMember()]
        public double Longitude { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Lat={0}, Long={1}", this.Latitude, this.Longitude);
        }

        public void Validate() 
        {
            if (this.Latitude == 0.0d)
            {
                throw new Pic4PicException("Latitude hasn't been defined");
            }

            if (this.Longitude == 0.0d)
            {
                throw new Pic4PicException("Longitude hasn't been defined");
            }

            if (this.Latitude < LAT_MIN || this.Latitude > LAT_MAX)
            {
                throw new Pic4PicException("Invalid Latitude(" + this.Latitude + "). It must be between -90.0 and +90.0");
            }

            if (this.Longitude < LNG_MIN || this.Longitude > LNG_MAX)
            {
                throw new Pic4PicException("Invalid Longitude(" + this.Longitude + "). It must be between -180.0 and +180.0");
            }
        }
    }
}
