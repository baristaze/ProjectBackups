using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Pic4Pic.ObjectModel
{
    public class NotificationCalculator
    {
        public List<NotificationBag> GetNotificationsToPush(SqlConnection conn, SqlTransaction trans)
        {
            return this.GetNotificationsToPush(conn, trans, false);
        }

        public List<NotificationBag> GetNotificationsToPush(SqlConnection conn, SqlTransaction trans, bool markAsOmittedIfNoDevice)
        {
            // prepare a structure to return
            List<NotificationBag> resultBag = new List<NotificationBag>();

            // get actions
            List<Action> actions = Action.GetActionsToBeNotifiedFromDBase(conn, trans, 500, 48 * 60);
            
            // get involved users
            List<Guid> involvedUserIDs = this.GetInvolvedUserIDs(actions, 3);
            Dictionary<Guid, User> involvedUsers = this.GetInvolvedUsers(conn, trans, involvedUserIDs);

            // get receiver mobile devices
            List<Guid> receiverUserIDs = this.GetInvolvedUserIDs(actions, 2);
            Dictionary<Guid, Dictionary<String, MobileDevice>> mobileDevicesPerUser = MobileDevice.ReadMobileDevicesFromDBase(conn, trans, String.Join(",", receiverUserIDs));

            // classify actions
            Dictionary<Guid, List<Action>> actionsPerReceiver = this.ClassifyActionsPerReceiver(actions);
            foreach(Guid receiverUserId in actionsPerReceiver.Keys)
            {
                List<Action> actionsPerCurrentReceiver = actionsPerReceiver[receiverUserId];
                if(!mobileDevicesPerUser.ContainsKey(receiverUserId))
                {
                    // mark as omitted
                    if(markAsOmittedIfNoDevice)
                    {
                        string concatenatedActionIDs = Pic4PicUtils.ConcatenateIDs<Action>(actionsPerCurrentReceiver, (a) => {return a.Id;});
                        int affectedRows = Action.MarkNotificationsAsOmittedOnDBase(conn, trans, concatenatedActionIDs);
                        System.Diagnostics.Trace.TraceWarning("Some notications are marked as 'omitted' since there were no associated mobile device(s). Count=" + affectedRows);
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("Some notications are ignored since there were no associated mobile device(s). Count=" + actionsPerCurrentReceiver.Count);
                    }
                }
                else
                {
                    // prepare push notifications
                    /*
                    if (actionsPerCurrentReceiver.Count <= 3)
                    {
                        // process one by one                        
                        foreach (Action action in actionsPerCurrentReceiver)
                        {
                            NotificationBag bagItem = new NotificationBag();
                            bagItem.UserId = receiverUserId;
                            bagItem.CombinedActions.Add(action);
                            bagItem.SummarizedNotification = this.SummarizeActions(new List<Action>() { action }, action.ActionType, involvedUsers);
                            bagItem.DeviceKeys.AddRange(mobileDevicesPerUser[receiverUserId].Keys);

                            resultBag.Add(bagItem);
                        }
                    }
                    else
                    {  
                        // moved to outside of if/else
                    }
                    */ 

                    // group by action type
                    Dictionary<ActionType, List<Action>> groupedActions = this.ClassifyActionsPerActionType(actionsPerCurrentReceiver);
                    foreach (ActionType actionType in groupedActions.Keys)
                    {
                        if (actionType == ActionType.AcceptedP4P)
                        {
                            // process acceptedP4Ps one by one
                            List<Action> tempList = groupedActions[actionType];
                            foreach (Action action in tempList)
                            {
                                NotificationBag bagItem = new NotificationBag();
                                bagItem.UserId = receiverUserId;
                                bagItem.CombinedActions.Add(action);
                                bagItem.SummarizedNotification = this.SummarizeActions(new List<Action>() { action }, action.ActionType, involvedUsers);
                                bagItem.DeviceKeys.AddRange(mobileDevicesPerUser[receiverUserId].Keys);

                                resultBag.Add(bagItem);
                            }
                        }
                        else
                        {
                            // group them
                            NotificationBag bagItem = new NotificationBag();
                            bagItem.UserId = receiverUserId;
                            bagItem.CombinedActions.AddRange(groupedActions[actionType]);
                            bagItem.SummarizedNotification = this.SummarizeActions(groupedActions[actionType], actionType, involvedUsers);
                            bagItem.DeviceKeys.AddRange(mobileDevicesPerUser[receiverUserId].Keys);

                            resultBag.Add(bagItem);
                        }
                    }
                }
            }

            return resultBag;
        }

        protected Dictionary<Guid, User> GetInvolvedUsers(SqlConnection conn, SqlTransaction trans, List<Guid> involvedUserIDs)
        {
            string concatenatedUserIDs = String.Join<Guid>(",", involvedUserIDs);
            List<User> involvedUsers = User.ReadAllFromDBase(conn, trans, concatenatedUserIDs);
            Dictionary<Guid, User> map = new Dictionary<Guid, User>();
            foreach (User user in involvedUsers)
            {
                map.Add(user.Id, user);
            }

            return map;
        }

        protected List<Guid> GetInvolvedUserIDs(List<Action> actions, int firstOrSecondOrBoth)
        {
            Dictionary<Guid, Guid> userIDs = new Dictionary<Guid, Guid>();
            foreach (Action action in actions)
            {
                if (firstOrSecondOrBoth == 1 || firstOrSecondOrBoth == 3)
                {
                    if (!userIDs.ContainsKey(action.UserId1))
                    {
                        userIDs.Add(action.UserId1, action.UserId1);
                    }
                }

                if (firstOrSecondOrBoth == 2 || firstOrSecondOrBoth == 3)
                {
                    if (!userIDs.ContainsKey(action.UserId2))
                    {
                        userIDs.Add(action.UserId2, action.UserId2);
                    }
                }
            }

            return userIDs.Keys.ToList();
        }

        protected Dictionary<Guid, List<Action>> ClassifyActionsPerReceiver(List<Action> actions)
        {
            Dictionary<Guid, List<Action>> dictionary = new Dictionary<Guid, List<Action>>();
            foreach (Action action in actions)
            {
                if (!dictionary.ContainsKey(action.UserId2))
                {
                    dictionary.Add(action.UserId2, new List<Action>());
                }

                dictionary[action.UserId2].Add(action);
            }

            return dictionary;
        }

        protected Dictionary<ActionType, List<Action>> ClassifyActionsPerActionType(List<Action> actions)
        {
            Dictionary<ActionType, List<Action>> dictionary = new Dictionary<ActionType, List<Action>>();
            foreach (Action action in actions)
            {
                if (!dictionary.ContainsKey(action.ActionType))
                {
                    dictionary.Add(action.ActionType, new List<Action>());
                }

                dictionary[action.ActionType].Add(action);
            }

            return dictionary;
        }

        protected PushNotification SummarizeActions(List<Action> actions, ActionType commonType, Dictionary<Guid, User> involvedUsers)
        {
            if (actions == null || actions.Count == 0)
            {
                throw new Pic4PicException("Action List is empty");
            }

            if (commonType == ActionType.AcceptedP4P)
            {
                if (actions.Count == 1)
                {
                    return this.ConvertAcceptingP4PAction(actions[0], involvedUsers);
                }
                else
                {
                    return this.ConvertAcceptingP4PAction(actions, involvedUsers);
                }
            }
            else if (commonType == ActionType.RequestingP4P)
            {
                if (actions.Count == 1)
                {
                    return this.ConvertRequestingP4PAction(actions[0], involvedUsers);
                }
                else
                {
                    return this.ConvertRequestingP4PAction(actions, involvedUsers);
                }
            }
            else if (commonType == ActionType.ViewedProfile)
            {
                if (actions.Count == 1)
                {
                    return this.ConvertViewingProfileAction(actions[0], involvedUsers);
                }
                else
                {
                    return this.ConvertViewingProfileAction(actions, involvedUsers);
                }
            }
            else if (commonType == ActionType.LikedBio)
            {
                if (actions.Count == 1)
                {
                    return this.ConvertLikingBioAction(actions[0], involvedUsers);
                }
                else
                {
                    return this.ConvertLikingBioAction(actions, involvedUsers);
                }
            }
            else if (commonType == ActionType.SentText)
            {
                if (actions.Count == 1)
                {
                    return this.ConvertSentMessageAction(actions[0], involvedUsers);
                }
                else
                {
                    return this.ConvertSentMessageAction(actions, involvedUsers);
                }
            }
            else
            {
                throw new Pic4PicException("Not implemented yet");
            }
        }

        protected bool IsAllFromSameUser(List<Action> actions)
        {
            for (int x = 1; x < actions.Count; x++)
            {
                if (actions[x].UserId1 != actions[x - 1].UserId1)
                {
                    return false;
                }
            }

            return true;
        }

        protected PushNotification CreateDefaultNotification() 
        {
            PushNotification notification = new PushNotification();
            notification.Title = "pic4pic notification";
            notification.Message = "You have a new notification via pic4pic";
            notification.NotificationType = 1;  // not in use
            notification.ActionType = 1;        // not in use unless you set it to 88 (opens 1st tab instead of 2nd, which is default)
            notification.ActionData = null;     // not in use
            notification.SmallIcon = 1;         // not in use
            return notification;
        }

        protected void CheckBeforeConvert(Action action, Dictionary<Guid, User> involvedUsers, ActionType targetActionType)
        {
            if (action.ActionType != targetActionType)
            {
                throw new Pic4PicException("Wrong action has been passed. ActionType was expected to be " + targetActionType.ToString());
            }

            if (!involvedUsers.ContainsKey(action.UserId1))
            {
                throw new Pic4PicException("User '" + action.UserId1.ToString() + "' is not in the retrieved dictionary");
            }
        }

        protected void CheckBeforeConvert(List<Action> actions, Dictionary<Guid, User> involvedUsers, ActionType targetActionType)
        {
            if (actions == null || actions.Count <= 0)
            {
                throw new Pic4PicException("Action List is empty");
            }

            foreach (Action action in actions)
            {
                if (action.ActionType != targetActionType)
                {
                    throw new Pic4PicException("Wrong action has been passed. ActionType was expected to be " + targetActionType.ToString());
                }

                if (!involvedUsers.ContainsKey(action.UserId1))
                {
                    throw new Pic4PicException("User '" + action.UserId1.ToString() + "' is not in the retrieved dictionary");
                }
            }
        }

        protected PushNotification ConvertAcceptingP4PAction(Action action, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(action, involvedUsers, ActionType.AcceptedP4P);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "You're classy!";
            notification.Message = String.Format(
                CultureInfo.InvariantCulture, 
                "{0} accepted your pic4pic request",
                involvedUsers[action.UserId1].DisplayName);

            return notification;
        }

        protected PushNotification ConvertAcceptingP4PAction(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(actions, involvedUsers, ActionType.AcceptedP4P);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "You're classy!";
            if (this.IsAllFromSameUser(actions))
            {   
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} accepted your pic4pic requests",
                    involvedUsers[actions[0].UserId1].DisplayName);
            }
            else 
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} newly accepted pic4pic requests are awaiting you",
                    actions.Count);
            }
            
            return notification;
        }

        protected PushNotification ConvertRequestingP4PAction(Action action, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(action, involvedUsers, ActionType.RequestingP4P);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "It's pic4pic time!";
            notification.Message = String.Format(
                CultureInfo.InvariantCulture,
                "{0} sent you a pic4pic request",
                involvedUsers[action.UserId1].DisplayName);

            return notification;
        }

        protected PushNotification ConvertRequestingP4PAction(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(actions, involvedUsers, ActionType.RequestingP4P);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "It's pic4pic time!";
            if (this.IsAllFromSameUser(actions))
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} sent you {1} pic4pic requests",
                    involvedUsers[actions[0].UserId1].DisplayName,
                    actions.Count);
            }
            else
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} new pic4pic requests are awaiting you",
                    actions.Count);
            }

            return notification;
        }

        protected PushNotification ConvertViewingProfileAction(Action action, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(action, involvedUsers, ActionType.ViewedProfile);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "Time to break the ice!";
            notification.Message = String.Format(
                CultureInfo.InvariantCulture,
                "{0} viewed your profile",
                involvedUsers[action.UserId1].DisplayName);

            return notification;
        }

        protected PushNotification ConvertViewingProfileAction(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(actions, involvedUsers, ActionType.ViewedProfile);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "Time to break the ice!";
            if (this.IsAllFromSameUser(actions))
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                     "{0} viewed your profile",
                    involvedUsers[actions[0].UserId1].DisplayName);
            }
            else
            {
                string userNicknames = this.GetAFewUsernames(actions, involvedUsers);
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                     "{0} viewed your profile",
                    userNicknames);
            }

            return notification;
        }

        protected PushNotification ConvertLikingBioAction(Action action, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(action, involvedUsers, ActionType.LikedBio);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "You are awesome!";
            notification.Message = String.Format(
                CultureInfo.InvariantCulture,
                "{0} liked your profile.",
                involvedUsers[action.UserId1].DisplayName);

            return notification;
        }

        protected PushNotification ConvertLikingBioAction(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(actions, involvedUsers, ActionType.LikedBio);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "You are awesome!";
            if (this.IsAllFromSameUser(actions))
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                     "{0} liked your profile",
                    involvedUsers[actions[0].UserId1].DisplayName);
            }
            else
            {
                string userNicknames = this.GetAFewUsernames(actions, involvedUsers);
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                     "{0} liked your profile",
                    userNicknames);
            }

            return notification;
        }

        protected String GetAFewUsernames(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            string userNicknames = "";
            List<Guid> userIDs = this.GetInvolvedUserIDs(actions, 1);
            if (userIDs.Count <= 2)
            {
                userNicknames = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} and {1}",
                    involvedUsers[userIDs[0]].DisplayName,
                    involvedUsers[userIDs[1]].DisplayName);
            }
            else // >= 3
            {
                userNicknames = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}, {1} and {2} more",
                    involvedUsers[userIDs[0]].DisplayName,
                    involvedUsers[userIDs[1]].DisplayName,
                    userIDs.Count - 2);
            }

            return userNicknames;
        }

        protected PushNotification ConvertSentMessageAction(Action action, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(action, involvedUsers, ActionType.SentText);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "pic4pic notification";
            notification.Message = String.Format(
                CultureInfo.InvariantCulture,
                "{0} sent you a new message",
                involvedUsers[action.UserId1].DisplayName);

            return notification;
        }

        protected PushNotification ConvertSentMessageAction(List<Action> actions, Dictionary<Guid, User> involvedUsers)
        {
            this.CheckBeforeConvert(actions, involvedUsers, ActionType.SentText);

            PushNotification notification = this.CreateDefaultNotification();
            notification.Title = "pic4pic notification";
            if (this.IsAllFromSameUser(actions))
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} sent you {1} new messages",
                    involvedUsers[actions[0].UserId1].DisplayName,
                    actions.Count);
            }
            else
            {
                notification.Message = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} new unread messages are awaiting you",
                    actions.Count);
            }

            return notification;
        }
    }
}
