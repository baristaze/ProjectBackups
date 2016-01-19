using System;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public class AuthorInfo : Identifiable
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string PhotoUrl { get; set; }

        public override string ToString()
        {
            return this.FirstName;
        }
    }
}
