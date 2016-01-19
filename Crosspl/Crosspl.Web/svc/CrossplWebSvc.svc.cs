using System;
using System.Collections.Generic;
using System.ServiceModel.Web;

using Crosspl.ObjectModel;
using System.ServiceModel.Activation;

namespace Crosspl.Web.Services
{
    // AspNetCompatibility lets the requests go through ASP.NET pipeline. You need to
    // enable this to use the Form-based auth for WCF service which is hosted within web site.
    // See the corresponding flag in the web.config file.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CrossplWebSvc : ServiceBase, ICrossplWebSvc
    {
        public AuthTokenResponse Signin(SigninRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<AuthTokenResponse, SigninRequest>("Signin", request, this._Signin, out user);
        }

        public BaseResponse Logout()
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse>("Logout", this._Logout, out user);
        }

        public FacebookFriendsResponse GetInvitedFriends()
        {
            UserAuthInfo user = null;
            return SafeExecute<FacebookFriendsResponse>("GetInvitedFriends", this._GetInvitedFriends, out user);
        }

        public BaseResponse MarkFriendsAsInvited(FacebookFriendList friends)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, FacebookFriendList>("MarkFriendsAsInvited", friends, this._MarkFriendsAsInvited, out user);
        }

        public BaseResponse RefreshCachedFriends()
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse>("RefreshCachedFriends", this._RefreshCachedFriends, out user);
        }

        public VoteResponse SaveEntryVote(string topicId, string entryId, string vote)
        {
            long topic = 0;
            long entry = 0;
            int theVote = 0;

            bool parsed = true;
            parsed &= long.TryParse(topicId, out topic);
            parsed &= long.TryParse(entryId, out entry);
            parsed &= int.TryParse(vote, out theVote);

            VoteRequest request = null;
            if (parsed)
            {
                request = new VoteRequest();
                request.TopicId = topic;
                request.EntryId = entry;
                request.Vote = theVote;
            }

            UserAuthInfo user = null;
            return SafeExecute<VoteResponse, VoteRequest>("SaveEntryVote", request, this._SaveEntryVote, out user);
        }

        public ReactionResponse SaveEntryReaction(string topicId, string entryId, string reactionId)
        {
            long topic = 0;
            long entry = 0;
            long reaction = 0;

            bool parsed = true;
            parsed &= long.TryParse(topicId, out topic);
            parsed &= long.TryParse(entryId, out entry);
            parsed &= long.TryParse(reactionId, out reaction);

            ReactionRequest request = null;
            if (parsed)
            {
                request = new ReactionRequest();
                request.TopicId = topic;
                request.EntryId = entry;
                request.ReactionId = reaction;
            }

            UserAuthInfo user = null;
            return SafeExecute<ReactionResponse, ReactionRequest>("SaveEntryReaction", request, this._SaveEntryReaction, out user);
        }

        public BaseResponse DiscardUploadedImage(string fileId)
        {
            // let the child method fail if we cannot parse the long.
            // no need to take extra action here
            long id = -1;
            long.TryParse(fileId, out id);

            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, long>("DiscardUploadedImage", id, this._DiscardUploadedImage, out user);
        }

        public EntryResponse AddNewEntry(EntryRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<EntryResponse, EntryRequest>("AddNewEntry", request, this._AddNewEntry, out user);
        }

        public EntryListResponse GetLatestEntries(string topicIdText, string pageSizeText, string pageIndexText, string includeEntryIdText)
        {
            UserAuthInfo user = null;
            EntryListRequest request = this.GetEntryListRequest(topicIdText, pageSizeText, pageIndexText, includeEntryIdText);
            return SafeExecute<EntryListResponse, EntryListRequest>("GetLatestEntries", request, this._GetLatestEntries, out user);
        }

        public EntryListResponse GetEntriesByNetVoteSum(string topicIdText, string pageSizeText, string pageIndexText, string includeEntryIdText)
        {
            UserAuthInfo user = null;
            EntryListRequest request = this.GetEntryListRequest(topicIdText, pageSizeText, pageIndexText, includeEntryIdText);
            return SafeExecute<EntryListResponse, EntryListRequest>("GetEntriesByNetVoteSum", request, this._GetEntriesByNetVoteSum, out user);
        }

        private EntryListRequest GetEntryListRequest(string topicIdText, string pageSizeText, string pageIndexText, string includeEntryIdText)
        {
            long topicId = 0;
            int pageSize = 0;
            int pageIndex = 0;
            long includeEntryId = -1;

            bool parsed = true;
            parsed &= long.TryParse(topicIdText, out topicId);
            parsed &= int.TryParse(pageSizeText, out pageSize);
            parsed &= int.TryParse(pageIndexText, out pageIndex);
            parsed &= long.TryParse(includeEntryIdText, out includeEntryId);

            EntryListRequest request = null;
            if (parsed)
            {
                request = new EntryListRequest();
                request.TopicId = topicId;
                request.PageSize = pageSize;
                request.PageIndex = pageIndex;
                request.IncludeEntryId = includeEntryId;
            }

            return request;
        }

        public BaseResponse DeleteEntry(string topicId, string entryId)
        {
            long topic = 0;
            long entry = 0;

            bool parsed = true;
            parsed &= long.TryParse(topicId, out topic);
            parsed &= long.TryParse(entryId, out entry);

            EntryRequest request = null;
            if (parsed)
            {
                request = new EntryRequest();
                request.TopicId = topic;
                request.EntryId = entry;
            }

            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, EntryRequest>("DeleteEntry", request, this._DeleteEntry, out user);
        }

        public TopicListResponse SearchTopic(string query)
        {
            UserAuthInfo user = null;
            return SafeExecute<TopicListResponse, string>("SearchTopic", query, this._SearchTopic, out user);
        }

        public SimpleResponse<Topic> AddNewTopic(TopicRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleResponse<Topic>, TopicRequest>("AddNewTopic", request, this._AddNewTopic, out user);
        }

        public SimpleListResponse<Topic> GetRelatedTopics(string topicId)
        {
            long topic = 0;
            TopicRequest request = null;
            if (long.TryParse(topicId, out topic))
            {
                request = new TopicRequest();
                request.Id = topic;
            }

            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<Topic>, TopicRequest>("GetRelatedTopics", request, this._GetRelatedTopics, out user);
        }

        public BaseResponse DeleteTopic(string topicId)
        {
            long topic = 0;
            TopicRequest request = null;
            if (long.TryParse(topicId, out topic))
            {
                request = new TopicRequest();
                request.Id = topic;
            }

            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, TopicRequest>("DeleteTopic", request, this._DeleteTopic, out user);
        }

        public BaseResponse SaveTopicInvitationRequest(string topicId, string entryId, string appRequestId, string inviteeCount)
        {
            long topic = 0;
            long entry = 0;
            long appRequest = 0;
            short invitees = 0;

            bool parsed = true;
            parsed &= long.TryParse(topicId, out topic);
            parsed &= long.TryParse(entryId, out entry);
            parsed &= long.TryParse(appRequestId, out appRequest);
            parsed &= short.TryParse(inviteeCount, out invitees);

            TopicInvitationRequest request = null;
            if (parsed)
            {
                request = new TopicInvitationRequest();
                request.TopicId = topic;
                request.AppRequestId = appRequest;
                request.InviteeCount = invitees;
                request.EntryId = entry;
            }

            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, TopicInvitationRequest>("SaveTopicInvitationRequest", request, this._SaveTopicInvitationRequest, out user);
        }

        public BaseResponse LogSocialShare(string topicId, string entryId, string channel)
        {
            long topic = 0;
            long entry = 0;
            SocialChannel ch = SocialChannel.None;

            bool parsed = true;
            parsed &= long.TryParse(topicId, out topic);
            parsed &= long.TryParse(entryId, out entry);
            parsed &= Enum.TryParse<SocialChannel>(channel, out ch);

            LogRequest request = null;
            if (parsed)
            {
                request = new LogRequest();
                request.TopicId = topic;
                request.EntryId = entry;
                request.Channel = ch;
            }

            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, LogRequest>("LogSocialShare", request, this._LogSocialShare, out user);
        }
    }
}
