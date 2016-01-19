using System;

namespace Pic4Pic.ObjectModel
{
    public enum UserActivityType
    {
        Unknown = 0,

        Signup,
        Signin,
        Logout,                
        UploadImage,
        DiscardUploadedImage,
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
            new ActionLimit(UserActivityType.Signup, 1000),                 // do not log this
            new ActionLimit(UserActivityType.Signin, 1000),                 // do not log this
            new ActionLimit(UserActivityType.Logout, 1000),                 // do not log this        
            new ActionLimit(UserActivityType.UploadImage, 200),             
            new ActionLimit(UserActivityType.DiscardUploadedImage, 199),
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
