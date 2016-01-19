using System;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class InteractionEngine
    {
        public static List<Interaction> GetRecentInteractions(SqlConnection conn, SqlTransaction trans, Guid userId, int maxCount, int cutOffAsMinutes, MatchConfig config)
        {
            // get actions where [UserId2] = @userId
            List<Action> actions = Action.ReadAllFromDBase(conn, trans, userId, maxCount * 10, cutOffAsMinutes);

            // compact them
            List<Action> temp = new List<Action>();
            Dictionary<string, Action> compactActionsPerUser = new Dictionary<string, Action>();
            foreach (Action action in actions) 
            {
                // is it a non-compactible action?
                if (action.ActionType == ActionType.LikedPhoto || action.ActionType == ActionType.RequestingP4P || action.ActionType == ActionType.AcceptedP4P)
                {
                    // non-compactible action...
                    temp.Add(action);
                }
                else 
                {
                    // compactible action... {ViewedProfile, Poked, SentText, LikedBio}
                    // do we have a compacting action for this user already?
                    string key = action.UserId1.ToString() + "::" + ((int)action.ActionType).ToString();
                    if (!compactActionsPerUser.ContainsKey(key))
                    {
                        temp.Add(action);
                        compactActionsPerUser.Add(key, action);
                    }
                    else 
                    {
                        // ignore this action
                    }
                }
            }

            // use compacted ones...
            actions = temp;
            if (actions.Count > maxCount) 
            {
                actions.RemoveRange(maxCount, actions.Count - maxCount);
            }

            // check 
            if (actions.Count <= 0)
            {
                return new List<Interaction>();
            }

            // bind details
            return BindDetailsToActions(conn, trans, userId, actions, false, config);
        }

        private static List<Interaction> BindDetailsToActions(SqlConnection conn, SqlTransaction trans, Guid userId, List<Action> actions, bool includeNickInTitle, MatchConfig config)
        {
            // get involved User IDs... use [UserId1] 
            Dictionary<Guid, Guid> involvedUserIDs = new Dictionary<Guid, Guid>();
            foreach (Action action in actions) 
            {
                if (!involvedUserIDs.ContainsKey(action.UserId1))
                {
                    involvedUserIDs.Add(action.UserId1, action.UserId1);
                }
            }
            string concatenatedUserIDs = Pic4PicUtils.ConcatenateIDs<Guid>(involvedUserIDs.Keys.ToList(), (m) => { return m; });

            // get users with familiarity... NOTE: Below action retrieves Usernames as well
            Dictionary<Guid, UserType> userTypes = new Dictionary<Guid, UserType>();
            Dictionary<Guid, Familiarity> familarities = new Dictionary<Guid, Familiarity>();
            Dictionary<Guid, FacebookUser> involvedUsers = FacebookUser.ReadAllFromDBaseWithFamiliarity(
                conn, trans, concatenatedUserIDs, userId, ref familarities, ref userTypes);

            // get profile pictures of involved users
            Dictionary<Guid, List<ImageFile>> imageMaps = ImageFile.ReadAllFromDBaseByUserIDsAndHash(conn, trans, concatenatedUserIDs);

            // retrive pic4pic's for the users
            Dictionary<Guid, List<PicForPic>> pic4picMaps = PicForPic.ReadAllFromDBaseForMultipleUsers(
                conn, trans, userId, concatenatedUserIDs, (involvedUserIDs.Count * 100));
            
            // prepare interactions
            List<Interaction> interactions = new List<Interaction>();
            foreach (Action action in actions)
            {
                // set the involved user since it might be confusing
                Guid involvedUserId = action.UserId1;

                // prepare profile info of involved user
                FacebookUser involvedUser = involvedUsers[involvedUserId];
                Familiarity familiarity = familarities[involvedUserId];
                if (involvedUserId == User.WellKnownSystemUserId) 
                {
                    familiarity = Familiarity.Familiar;
                }
                UserType userType = userTypes[involvedUserId];
                FriendProfile profile = new FriendProfile(involvedUser, involvedUser.Username, involvedUser.Description, familiarity, (userType == UserType.Guest));
                
                // get profile pictures...
                if (imageMaps.ContainsKey(involvedUserId))
                {
                    List<ImageFile> imagesOfInvolvedUser = imageMaps[involvedUserId];
                    if (imagesOfInvolvedUser.Count >= 4)
                    {
                        // get profile pictures
                        PicturePair profilePics = PicturePair.GetProfilePicturePair(imageMaps[involvedUserId], familiarity);
                        if (profilePics.HasAll())
                        {
                            // create MatchedCandidate which contains all info about an involved user
                            MatchedCandidate friend = new MatchedCandidate();
                            friend.CandidateProfile = profile;
                            friend.ProfilePics = profilePics;

                            // get other images as well
                            if (pic4picMaps.ContainsKey(involvedUserId))
                            {
                                // add sent and received p4p requests...
                                Pic4PicHistory history = Pic4PicHistory.From(pic4picMaps[involvedUserId], userId, involvedUserId);
                                friend.SentPic4PicsByCandidate.AddRange(history.SentByCandidate);
                                friend.SentPic4PicsToCandidate.AddRange(history.SentToCandidate);

                                // add other pictures
                                List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(imageMaps[involvedUserId], pic4picMaps[involvedUserId]);
                                friend.OtherPictures.AddRange(otherImages);
                            }
                            else 
                            {
                                // add other pictures
                                List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(imageMaps[involvedUserId], new List<PicForPic>());
                                friend.OtherPictures.AddRange(otherImages);
                            }

                            // create interaction
                            Interaction interaction = Interaction.CreateFrom(action, friend, includeNickInTitle);

                            // add it to the list
                            interactions.Add(interaction);
                        }
                    }
                }
            }

            // bind likes-by-me info 
            Dictionary<Guid, DateTime> lastLikes = new Dictionary<Guid, DateTime>();
            List<Action> likes = Action.ReadAllByMeFromDBase(conn, trans, userId, concatenatedUserIDs, 500, config.HistoryLimitAsMinutes, ActionType.LikedBio);
            foreach (Action like in likes)
            {
                if (!lastLikes.ContainsKey(like.UserId2))
                {
                    lastLikes.Add(like.UserId2, like.ActionTimeUTC);
                }
                else
                {
                    if(lastLikes[like.UserId2] < like.ActionTimeUTC)
                    {
                        lastLikes[like.UserId2] = like.ActionTimeUTC;
                    }
                }
            }
            
            foreach (Interaction interaction in interactions)
            {
                if (lastLikes.ContainsKey(interaction.StartedBy.CandidateProfile.UserId))
                {
                    interaction.StartedBy.LastLikeTimeUTC = lastLikes[interaction.StartedBy.CandidateProfile.UserId];
                }
            }

            // return
            return interactions;
        }
    }
}
