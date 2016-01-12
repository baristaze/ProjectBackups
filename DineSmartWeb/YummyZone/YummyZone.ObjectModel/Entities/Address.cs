using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public enum ObjectType
    {
        Unspecified = 0,
        Venue,
        SignupVenue,
        Group,
        Chain,
    }

    public enum AddressType
    {
        Unspecified = 0,
        BusinessAddress,
        BillingAddress,
    }

    public partial class Address : IEditable
    {
        public const string ExcludedCharsInCity = "~!@#$%^&*()_=+[]{}\\|;:\"<>/?";

        public ObjectType ObjectType { get; set; }
        public Guid ObjectId { get; set; }
        public AddressType AddressType { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public Address()
        {
            this.Country = "USA";
        }

        public bool IsEmpty()
        {
            if (!String.IsNullOrWhiteSpace(this.AddressLine1))
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(this.AddressLine2))
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(this.City))
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(this.State))
            {
                return false;
            }

            if (!String.IsNullOrWhiteSpace(this.ZipCode))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Join(true, true);
        }

        public string ToShortString()
        {
            return Join(false, false);
        }

        private string Join(bool includeState, bool includeZipCode)
        {
            return Join(includeState, includeZipCode, false);
        }

        private string Join(bool includeState, bool includeZipCode, bool includeCountry)
        {
            string s = String.Empty;
            if (!String.IsNullOrWhiteSpace(this.AddressLine1))
            {
                s += this.AddressLine1.Trim() + " ";
            }

            if (!String.IsNullOrWhiteSpace(this.AddressLine2))
            {
                s += this.AddressLine2.Trim() + " ";
            }

            if (!String.IsNullOrWhiteSpace(this.City))
            {
                s += this.City.Trim() + ", ";
            }

            if (!String.IsNullOrWhiteSpace(this.State) && includeState)
            {
                s += this.State.Trim() + " ";
            }

            if (!String.IsNullOrWhiteSpace(this.ZipCode) && includeZipCode)
            {
                s += this.ZipCode.Trim() + " ";
            }

            if (!String.IsNullOrWhiteSpace(this.Country) && includeCountry)
            {
                s += this.Country.Trim();
            }

            return s.Trim(' ', ',');
        }
    }
}
