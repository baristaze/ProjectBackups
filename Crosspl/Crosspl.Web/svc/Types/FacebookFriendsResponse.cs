using System;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class FacebookFriendsResponse : BaseResponse
    {
        [DataMember()]
        public FacebookFriendList Friends { get { return this.friends; } }
        private FacebookFriendList friends = new FacebookFriendList();
    }
}