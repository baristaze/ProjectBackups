using System;

namespace Shebeke.ObjectModel
{
    public enum UserActivityType
    {
        Unknown = 0,
        
        Signin,
        Logout,
        
        GetInvitedFriends,
        MarkFriendsAsInvited,
        RefreshCachedFriends,

        SaveEntryVote,
        SaveEntryReaction,
        LogSocialShare,
        SaveTopicInvitationRequest,
                
        UploadImage,
        DiscardUploadedImage,

        AddNewEntry,
        UpdateEntry,  // future
        DeleteEntry,        
        GetLatestEntries,
        GetEntriesByNetVoteSum,
                
        AddNewTopic,
        UpdateTopic,  // future
        DeleteTopic,        
        SearchTopic,
        GetRelatedTopics,
    }
    
    public partial class UserLogActivity
    {
        private class ActionLimit
        {
            public UserActivityType Activity { get; private set; }
            public int Limit { get; private set; }

            private ActionLimit() : this(UserActivityType.Unknown, 0) { }

            public ActionLimit(UserActivityType activity, int limit)
            {
                this.Activity = activity;
                this.Limit = limit;
            }
        }

        private static ActionLimit[] defaultLimits = new ActionLimit[] 
        { 
            new ActionLimit(UserActivityType.Unknown, 1000),                // do not log this        
            new ActionLimit(UserActivityType.Signin, 1000),                 // do not log this
            new ActionLimit(UserActivityType.Logout, 1000),                 // do not log this        
            new ActionLimit(UserActivityType.GetInvitedFriends, 1000),      // do not log this
            new ActionLimit(UserActivityType.MarkFriendsAsInvited, 100),
            new ActionLimit(UserActivityType.RefreshCachedFriends, 1000),   // do not log this
            new ActionLimit(UserActivityType.SaveEntryVote, 100),           
            new ActionLimit(UserActivityType.SaveEntryReaction, 100),       
            new ActionLimit(UserActivityType.LogSocialShare, 100),          
            new ActionLimit(UserActivityType.SaveTopicInvitationRequest, 100),                
            new ActionLimit(UserActivityType.UploadImage, 200),             
            new ActionLimit(UserActivityType.DiscardUploadedImage, 199),    
            new ActionLimit(UserActivityType.AddNewEntry, 100),             
            new ActionLimit(UserActivityType.UpdateEntry, 99),              // future
            new ActionLimit(UserActivityType.DeleteEntry, 99),              
            new ActionLimit(UserActivityType.GetLatestEntries, 1000),       // do not log this
            new ActionLimit(UserActivityType.GetEntriesByNetVoteSum, 1000), // do not log this                
            new ActionLimit(UserActivityType.AddNewTopic, 20),              
            new ActionLimit(UserActivityType.UpdateTopic, 19),              // future
            new ActionLimit(UserActivityType.DeleteTopic, 19),              
            new ActionLimit(UserActivityType.SearchTopic, 1000),            // do not log this
            new ActionLimit(UserActivityType.GetRelatedTopics, 1000),       // do not log this
        };

        public UserLogActivity() : this(0, UserActivityType.Unknown) { }

        public UserLogActivity(long userId, UserActivityType activity)
        {
            this.UserId = userId;
            this.ActionId = activity;
        }

        public long UserId { get; set; }
        public UserActivityType ActionId { get; set; }

        public static int GetDefaultLimit(UserActivityType activity)
        {
            foreach (ActionLimit limit in defaultLimits)
            {
                if (limit.Activity == activity)
                {
                    return limit.Limit;
                }
            }

            return 1000;
        }
    }
}
