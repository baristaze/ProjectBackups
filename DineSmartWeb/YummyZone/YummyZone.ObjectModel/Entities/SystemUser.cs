using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YummyZone.ObjectModel
{
    public partial class SystemUser : IEditable
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
        public bool IsAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string UserPassword { get; set; }
        public DateTime CreateTimeUTC { get; set; }

        public override string ToString()
        {
            return this.EmailAddress;
        }
    }
}
