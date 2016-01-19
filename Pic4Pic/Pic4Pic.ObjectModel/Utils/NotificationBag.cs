using System;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class NotificationBag
    {
        private List<String> deviceKeys = new List<string>();
        private List<Action> combinedActions = new List<Action>();

        public Guid UserId { get; set; }

        public List<Action> CombinedActions { get { return this.combinedActions; } }

        public PushNotification SummarizedNotification { get; set; }

        public List<String> DeviceKeys { get { return this.deviceKeys; } }
    }
}
