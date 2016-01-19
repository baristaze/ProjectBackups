using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IShebekeWebSvc" in both code and config file together.
    [ServiceContract]
    public interface IShebekeWebSvc
    {
        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "signin", // This is how you call it: svc/rest/signin
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        AuthTokenResponse Signin(SigninRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "logout", // This is how you call it: svc/rest/logout
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse Logout();

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "friends/invited", /* This is how you call it: svc/rest/friends/invited */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        FacebookFriendsResponse GetInvitedFriends();

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "mark/friends/invited", /* This is how you call it: svc/rest/mark/friends/invited */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse MarkFriendsAsInvited(FacebookFriendList friends);

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "friends/refresh", /* This is how you call it: svc/rest/friends/refresh */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse RefreshCachedFriends();

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "vote/{topicId}/{entryId}/{vote}", /* This is how you call it: svc/rest/vote/3/5/1 */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        VoteResponse SaveEntryVote(string topicId, string entryId, string vote);

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "react/{topicId}/{entryId}/{reactionId}", /* This is how you call it: svc/rest/react/3/5/1 */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ReactionResponse SaveEntryReaction(string topicId, string entryId, string reactionId);

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "image/discard/{fileId}", /* This is how you call it: svc/rest/image/discard/entryId/704c94ab-e8fd-4a63-93a1-022ad58e1726*/
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse DiscardUploadedImage(string fileId);
        
        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "topic/entry/new", /* This is how you call it: svc/rest/topic/entry/new*/
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        EntryResponse AddNewEntry(EntryRequest request);
        
        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/entries/latest/{topicId}/{pageSize}/{pageIndex}?e={includeEntryId}", // This is how you call it: svc/rest/topic/entries/latest/3/50/0?e=12
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        EntryListResponse GetLatestEntries(string topicId, string pageSize, string pageIndex, string includeEntryId);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/entries/byvote/{topicId}/{pageSize}/{pageIndex}?e={includeEntryId}", // This is how you call it: svc/rest/topic/entries/byvote/3/50/0?e=12
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        EntryListResponse GetEntriesByNetVoteSum(string topicId, string pageSize, string pageIndex, string includeEntryId);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/entry/mark/{topicId}/{entryId}/delete", // This is how you call it: svc/rest/topic/entry/mark/3/5/delete
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse DeleteEntry(string topicId, string entryId);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/search/{query}", // This is how you call it: svc/rest/topic/search/The+Oscar+Awards
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        TopicListResponse SearchTopic(string query);

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "topic/new", /* This is how you call it: svc/rest/topic/new */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleResponse<Topic> AddNewTopic(TopicRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", /*Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.*/
            UriTemplate = "topic/{topicId}/related", /* This is how you call it: svc/rest/topic/3/related */
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<Topic> GetRelatedTopics(string topicId);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/mark/{topicId}/delete", // This is how you call it: svc/rest/topic/mark/3/delete
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse DeleteTopic(string topicId);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/invitation/{topicId}/{entryId}/{appRequestId}/{inviteeCount}", // This is how you call it: svc/rest/topic/3/invitation/1234567890/5
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse SaveTopicInvitationRequest(string topicId, string entryId, string appRequestId, string inviteeCount);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "topic/entry/log/share/{topicId}/{entryId}/{channel}", // This is how you call it: svc/rest/topic/entry/log/share/3/5/Facebook
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse LogSocialShare(string topicId, string entryId, string channel);
    }
}