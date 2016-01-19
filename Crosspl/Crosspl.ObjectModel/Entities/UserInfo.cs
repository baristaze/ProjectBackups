using System;

namespace Crosspl.ObjectModel
{
    public enum UserStatus
    {
        Active = 0,
        Suspended,
        Disabled,
        Deleted,
    }

    public class UserInfo : Identifiable
    {
        public long Id { get; set; }
        public UserType UserType { get; set; }
        public UserStatus UserStatus { get; set; }
        public int SplitId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime BirthDay { get; set; }
        public Gender Gender { get; set; }
        public string Hometown { get; set; }
        public int TimeZoneOffset { get; set; }

        public override string ToString()
        {
            return this.FirstName;
        }
    }
}
