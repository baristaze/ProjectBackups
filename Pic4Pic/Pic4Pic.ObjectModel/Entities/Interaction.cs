using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public enum ActionType
    {
        [EnumMember]
        Undefined = 0,

        [EnumMember]
        ViewedProfile = 1,

        [EnumMember]
        Poked = 2,

        [EnumMember]
        SentText = 3,

        [EnumMember]
        LikedBio = 4,

        [EnumMember]
        LikedPhoto = 5,

        [EnumMember]
        RequestingP4P = 6,

        [EnumMember]
        AcceptedP4P = 7,
    }

    [DataContract()]
    public enum ActionMatch
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        PokeBack = 1,

        [EnumMember]
        LikeBackBio = 2,

        [EnumMember]
        ViewProfile = 3,

        [EnumMember]
        ViewMessage = 4,

        [EnumMember]
        RequestP4P = 5,

        [EnumMember]
        AcceptP4P = 6,
    }

    [DataContract()]
    public class Interaction
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public string Title { get; set; }

        [DataMember()]
        public MatchedCandidate StartedBy { get; set; }

        [DataMember()]
        public ActionType ActionType { get; set; }

        [DataMember()]
        public ActionMatch RecommendedAction { get; set; }

        [DataMember()]
        public DateTime ActionTimeUTC { get; set; }

        [DataMember()]
        public bool IsViewed { get; set; }

        public override string ToString()
        {
            return this.Title;
        }

        public static ActionMatch CalculateRecommendedAction(ActionType action)
        { 
            if(action == ActionType.ViewedProfile)
            {
                // return ActionMatch.ViewProfile;
                return ActionMatch.RequestP4P;
            }

            if(action == ActionType.Poked)
            {
                // return ActionMatch.PokeBack;
                return ActionMatch.RequestP4P;
            }

            if(action == ActionType.SentText)
            {
                return ActionMatch.ViewMessage;
            }

            if(action == ActionType.LikedBio)
            {
                // return ActionMatch.LikeBackBio;
                return ActionMatch.RequestP4P;
            }

            if(action == ActionType.LikedPhoto)
            {
                // return ActionMatch.None;
                return ActionMatch.RequestP4P;
            }

            if(action == ActionType.RequestingP4P)
            {
                return ActionMatch.AcceptP4P;
            }

            if(action == ActionType.AcceptedP4P)
            {
                return ActionMatch.ViewProfile;
            }

            return ActionMatch.ViewProfile;
        }

        public static string SummarizeAction(ActionType action, string username)
        {
            if (action == ActionType.ViewedProfile)
            {
                return String.Format(
                    CultureInfo.InvariantCulture, 
                    "{0} viewed your profile", 
                    username);
            }

            if (action == ActionType.Poked)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} poked you",
                    username);
            }

            if (action == ActionType.SentText)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} sent you a message",
                    username);
            }

            if (action == ActionType.LikedBio)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} liked your bio",
                    username);
            }

            if (action == ActionType.LikedPhoto)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} liked your photo",
                    username);
            }

            if (action == ActionType.RequestingP4P)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} sent you a pic4pic request",
                    username);
            }

            if (action == ActionType.AcceptedP4P)
            {
                return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} accepted your pic4pic request",
                    username);
            }

            return String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} has something for you",
                    username);
        }

        public static string SummarizeAction(ActionType action)
        {
            if (action == ActionType.ViewedProfile)
            {
                return "viewed your profile";
            }

            if (action == ActionType.Poked)
            {
                return "poked you";
            }

            if (action == ActionType.SentText)
            {
                return "sent you a message";
            }

            if (action == ActionType.LikedBio)
            {
                return "liked your bio";
            }

            if (action == ActionType.LikedPhoto)
            {
                return "liked your photo";
            }

            if (action == ActionType.RequestingP4P)
            {
                return "sent you a pic4pic request";
            }

            if (action == ActionType.AcceptedP4P)
            {
                return "accepted your pic4pic request";
            }

            return "has something for you";
        }

        public static Interaction CreateFrom(Action action, MatchedCandidate startedBy, bool includeNickInTitle) 
        {
            Interaction interaction = new Interaction();

            interaction.Id = action.Id;
            interaction.StartedBy = startedBy;
            interaction.ActionType = action.ActionType;
            interaction.ActionTimeUTC = action.ActionTimeUTC;
            interaction.RecommendedAction = CalculateRecommendedAction(action.ActionType);
            interaction.IsViewed = action.NotifViewTimeUTC > default(DateTime);

            if (includeNickInTitle)
            {
                interaction.Title = SummarizeAction(action.ActionType, startedBy.CandidateProfile.DisplayName);
            }
            else 
            {
                interaction.Title = SummarizeAction(action.ActionType);
            }

            return interaction;
        }
    }
}
