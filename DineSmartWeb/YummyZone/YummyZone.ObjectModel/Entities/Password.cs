using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class Password : IEditable
    {
        public Guid UserId { get; set; }
        public string PasswordText { get; set; }
    }
}
