using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class Locality : IVerifiable
    {
        [DataMember()]
        public String Country { get; set; }

        [DataMember()]
        public String CountryCode { get; set; }

        // a.k.a. State
        [DataMember()]
        public String Region { get; set; }

        // a.k.a. County
        [DataMember()]
        public String SubRegion { get; set; }

        [DataMember()]
        public String City { get; set; }

        [DataMember()]
        public String Neighborhood { get; set; }

        [DataMember()]
        public String ZipCode { get; set; }

        public override string ToString()
        {
            String address = "";

            if (!String.IsNullOrWhiteSpace(this.City))
            {
                if (!String.IsNullOrWhiteSpace(address))
                {
                    address += ", ";
                }

                address += this.City;
            }

            if (!String.IsNullOrWhiteSpace(this.Region))
            {
                if (!String.IsNullOrWhiteSpace(address))
                {
                    address += ", ";
                }

                address += this.Region;
            }

            if (!String.IsNullOrWhiteSpace(this.Country))
            {
                if (!String.IsNullOrWhiteSpace(address))
                {
                    address += ", ";
                }

                address += this.Country;
            }

            if (!String.IsNullOrWhiteSpace(this.ZipCode))
            {
                if (!String.IsNullOrWhiteSpace(address))
                {
                    address += ", ";
                }

                address += this.ZipCode;
            }

            return address;
        }

        public void Validate()
        {
            this.Validate(false);
        }

        public void Validate(bool checkZipCode)
        {
            if (String.IsNullOrWhiteSpace(this.Country))
            {
                throw new Pic4PicException("Country may not be empty");
            }

            if (String.IsNullOrWhiteSpace(this.Region))
            {
                throw new Pic4PicException("Region may not be empty");
            }

            if (String.IsNullOrWhiteSpace(this.City))
            {
                throw new Pic4PicException("City may not be empty");
            }

            if(checkZipCode)
            {
                if (String.IsNullOrWhiteSpace(this.ZipCode))
                {
                    throw new Pic4PicException("ZipCode may not be empty");
                }
            }
        }
    }
}
