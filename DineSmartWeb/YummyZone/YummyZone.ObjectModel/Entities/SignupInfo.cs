using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class SignupInfo : IEditable
    {
        public Guid Id { get; set; }
        public string VenueName { get; set; }
        public string VenueURL { get; set; }
        public byte? VenueTimeZoneWinIndex { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserPassword { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public Address Address { get; set; }

        public SignupInfo() : this(Guid.Empty) { }
        public SignupInfo(Guid id)
        {
            this.Id = id;
            this.CreateTimeUTC = DateTime.Now;
        }

        public override string ToString()
        {
            return this.VenueName + " by " + this.UserEmailAddress;
        }

        public string ToSubjectString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "Subscription Request for {0}",
                this.VenueName);
        }

        public string ToContentString(string delim)
        {
            string format = String.Empty;
            format += "Id = {1}{0}";
            format += "VenueName = {2}{0}";
            format += "VenueURL = {3}{0}";
            format += "UserFirstName = {4}{0}";
            format += "UserLastName = {5}{0}";
            format += "UserPhoneNumber = {6}{0}";
            format += "UserEmailAddress = {7}{0}";
            format += "Address = {8}{0}";

            return String.Format(
                CultureInfo.InvariantCulture,
                format,
                delim,
                this.Id,
                this.VenueName,
                this.VenueURL,
                this.UserFirstName,
                this.UserLastName,
                this.UserPhoneNumber,
                this.UserEmailAddress,
                this.Address.ToString());
        }
    }
}
