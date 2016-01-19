using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class FriendProfile : UserProfile
    {
        [DataMember()]
        public Familiarity Familiarity { get; set; }
        
        public FriendProfile() : base() { }

        public FriendProfile(FacebookUser fbUser, string userName, string description, Familiarity familiarity, bool isTestUser)
            : base(fbUser, userName, description, true, isTestUser)
        {
            this.Familiarity = familiarity;
        }

        public override string ToString()
        {
            return this.DisplayName + " : " + this.ShortBio;
        }
    }
}
