using System;
using System.ServiceModel;
using System.ServiceModel.Web;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    [ServiceContract]
    public interface IPic4PicSvc
    {
        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "ping", // This is how you call it: svc/rest/checkusername
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse Ping();

        /**
         * ErrorCode != 0 : Username is already in use
         * ErrorCode == 0 & AuthToken == null => Username is available
         * ErrorCode == 0 & AuthToken != null => User is already signed up
         */
        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "checkusername", // This is how you call it: svc/rest/checkusername
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UserResponse CheckUsername(UserCredentials request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "signin", // This is how you call it: svc/rest/login
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UserResponse Signin(UserCredentials request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "verifybio", // This is how you call it: svc/rest/signin
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UserResponse VerifyBio(VerifyBioRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "signup", // This is how you call it: svc/rest/signup
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        UserResponse Signup(SignupRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "activate", // This is how you call it: svc/rest/activate
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse ActivateUser(BaseRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "user/description", // This is how you call it: svc/rest/user/description
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse SaveUserDescription(SimpleStringRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "friends", // This is how you call it: svc/rest/friends
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse DownloadFacebookFriends(FacebookRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "notifications",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<Interaction> GetNotifications(NotificationRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "mark", // This is how you call it: svc/rest/matches
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse Mark(MarkingRequest request);
        
        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "matches", // This is how you call it: svc/rest/matches
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<MatchedCandidate> GetMatches(SimpleObjectRequest<Location> request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "matches/preview", // This is how you call it: svc/rest/matches
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<MatchedCandidate> GetPreviewMatches(SimpleObjectRequest<Location> request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "matches/buy", // This is how you call it: svc/rest/matches
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<MatchedCandidate> BuyNewMatches(BuyingNewMatchRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "p4p/request", 
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleResponse<MatchedCandidate> RequestPic4Pic(StartingPic4PicRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "p4p/accept",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleResponse<MatchedCandidate> AcceptPic4Pic(AcceptingPic4PicRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "candidate/details",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        CandidateDetailsResponse GetCandidateDetails(CandidateDetailsRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "im/send",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<InstantMessage> SendInstantMessage(InstantMessageRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "im/conversation",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<InstantMessage> GetConversation(ConversationRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "im/conversation/summary",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<ConversationsSummary> GetConversationSummary(BaseRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "offers",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SimpleListResponse<PurchaseOffer> GetOffers(BaseRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "purchase",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse ProcessPurchase(SimpleObjectRequest<PurchaseRecord> request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "credit",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse GetCurrentCredit(BaseRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "device",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse TrackDevice(MobileDeviceRequest request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "location/new",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse AssureSupportAtLocation(SimpleObjectRequest<Location> request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "location/current",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse SetCurrentLocation(SimpleObjectRequest<Location> request);

        [OperationContract]
        [WebInvoke(
            Method = "*", //Firefox sends OPTIONS first instead of POST. Cross-domain issue. See impl.
            UriTemplate = "log",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        BaseResponse LogClientTraces(ClientLogRequest request);
    }
}
