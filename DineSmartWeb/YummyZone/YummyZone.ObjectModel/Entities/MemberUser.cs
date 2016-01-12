using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public partial class MemberUser
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public bool IsReadOnly { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public Role Role { get; set; }

        [DataMember]
        public int MembershipCount { get; set; }

        [DataMember]
        public int SentMessageCount { get; set; }

        [DataMember]
        public int SentCouponCount { get; set; }

        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return (this.Status == Status.Active);
            }
            set 
            {
            }
        }

        [DataMember]
        public bool IsDisabled
        {
            get
            {
                return !this.IsEnabled;
            }
            set
            {
            }
        }

        [DataMember]
        public bool CanDelete
        {
            get
            {
                return this.SentMessageCount == 0 &&
                    this.SentCouponCount == 0 &&
                    !this.IsReadOnly;
            }
            set
            {
            }
        }

        [DataMember]
        public bool CanDisable
        {
            get
            {
                return this.IsEnabled && !this.IsReadOnly;
            }
            set
            {
            }
        }

        [DataMember]
        public bool CanEnable
        {
            get
            {
                return !this.IsEnabled && !this.IsReadOnly;
            }
            set
            {
            }
        }
    }

    public class MemberUserList : List<MemberUser> 
    {
        public MemberUser this[Guid userId]
        {
            get
            {
                foreach (MemberUser item in this)
                {
                    if (item.UserId == userId)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }
}
