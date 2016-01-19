﻿using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class FacebookFriend : IEditable
    {
        public long FacebookId1 { get; set; }

        [DataMember(Name = "FacebookId")]
        public long FacebookId2 { get; set; }

        [DataMember(Name = "FullName")]
        public string Friend2Name { get; set; }

        public bool IsInvited { get; set; }
        public DateTime InvitationTimeUTC { get; set; }
        public DateTime CreateTimeUTC { get; set; }

        public FacebookFriend()
        {
            this.CreateTimeUTC = DateTime.UtcNow;
        }

        public FacebookFriend(long user, NameIdPair friend)
        {
            this.FacebookId1 = user;
            this.FacebookId2 = friend.Id;
            this.Friend2Name = friend.Name;
            this.CreateTimeUTC = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return this.FacebookId1.ToString() + ", " + this.FacebookId2.ToString();
        }
    }
}
