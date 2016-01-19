using System;
using System.Collections.Generic;
using System.Linq;

namespace Shebeke.ObjectModel
{
    public class FacebookFriendList : List<FacebookFriend>
    {
        public const int MaxFriends = 500;

        public FacebookFriendList() { }
        public FacebookFriendList(List<FacebookFriend> list) : base(list) { }

        public static FacebookFriendList Create(long user, NameIdPairList friends)
        {
            FacebookFriendList list = new FacebookFriendList();
            foreach (NameIdPair pair in friends)
            {
                FacebookFriend friend = new FacebookFriend(user, pair);
                list.Add(friend);
            }

            return list;
        }

        public FacebookFriendList Subtract(FacebookFriendList currentlyExistingList)
        {
            // prepare a dict of this
            Dictionary<long, FacebookFriend> thisDict = new Dictionary<long, FacebookFriend>();
            foreach (FacebookFriend friendNew in this)
            {
                thisDict.Add(friendNew.FacebookId2, friendNew);
            }

            foreach (FacebookFriend friendPreviouslySaved in currentlyExistingList)
            {
                if (thisDict.Keys.Contains(friendPreviouslySaved.FacebookId2))
                {
                    thisDict.Remove(friendPreviouslySaved.FacebookId2);
                }
            }

            return new FacebookFriendList(thisDict.Values.ToList());
        }
    }
}
