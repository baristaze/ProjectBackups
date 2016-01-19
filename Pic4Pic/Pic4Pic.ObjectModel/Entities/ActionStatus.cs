using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    public enum ActionStatus
    {
        Created = 0, 
        NotificationOmitted = 1,
        NotificationScheduled = 2,
        NotificationSent = 3,
        NotificationViewed = 4,
    }
}
