using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public partial class User : Identifiable, IDBEntity
    {
        public static readonly Guid WellKnownSystemUserId = new Guid("6C3CF22D-4726-466A-AAA5-F2DDB2D0E0B6");

        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public UserType UserType { get; set; }

        [DataMember()]
        public UserStatus UserStatus { get; set; }

        [DataMember()]
        public int SplitId { get; set; }

        [DataMember()]
        public string Username { get; set; }

        [DataMember()]
        public String DisplayName
        {
            get
            {
                if (this.UserType == UserType.Guest)
                {
                    int trimIndex = this.Username.LastIndexOf("__");
                    if (trimIndex > 0)
                    {
                        return this.Username.Substring(0, trimIndex);
                    }
                }

                return this.Username;
            }
            set
            {
                // void
            }
        }

        [DataMember()]
        public string Password { get; set; }

        [DataMember()]
        public string Description { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

        [DataMember]
        public DateTime LastUpdateTimeUTC { get; set; }

        public User() 
        {
            this.CreateTimeUTC = DateTime.UtcNow;
            this.LastUpdateTimeUTC = this.CreateTimeUTC;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
