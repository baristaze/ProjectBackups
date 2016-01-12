using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class User : YummyZoneEntity, IEditable
    {
        public const string ExcludedCharsInName = "~!@#$%^&*()_=+[]{}\\|;:\"<>/?";

        public Status Status { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return this.EmailAddress;
        }
    }
}
