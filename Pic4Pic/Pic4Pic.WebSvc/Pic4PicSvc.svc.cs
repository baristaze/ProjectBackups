using System;
using System.Drawing;
using System.ServiceModel.Activation;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    // [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Pic4PicSvc : ServiceBase, IPic4PicSvc
    {
        static Pic4PicSvc()
        {
            Logger.AppType = "WebSvc";
        }

        public BaseResponse Ping()
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse>(this._Ping, out user);
        }

        /**
         * ErrorCode != 0 : Username is already in use
         * ErrorCode == 0 & AuthToken == null => Username is available
         * ErrorCode == 0 & AuthToken != null => User is already signed up
         */
        public UserResponse CheckUsername(UserCredentials request)
        {
            UserAuthInfo user = null;
            return SafeExecute<UserResponse, UserCredentials>(request, this._CheckUsername, out user);
        }

        public UserResponse Signin(UserCredentials request) 
        {
            UserAuthInfo user = null;
            return SafeExecute<UserResponse, UserCredentials>(request, this._Signin, out user);
        }

        public UserResponse VerifyBio(VerifyBioRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<UserResponse, VerifyBioRequest>(request, this._VerifyBio, out user);
        }

        public UserResponse Signup(SignupRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<UserResponse, SignupRequest>(request, this._Signup, out user);
        }

        public BaseResponse ActivateUser(BaseRequest request) {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, BaseRequest>(request, this._ActivateUser, out user);
        }

        public BaseResponse SaveUserDescription(SimpleStringRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, SimpleStringRequest>(request, this._SaveUserDescription, out user);
        }

        public BaseResponse DownloadFacebookFriends(FacebookRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, FacebookRequest>(request, this._DownloadFacebookFriends, out user);
        }

        public SimpleListResponse<Interaction> GetNotifications(NotificationRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<Interaction>, NotificationRequest>(request, this._GetNotifications, out user);
        }

        public BaseResponse Mark(MarkingRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, MarkingRequest>(request, this._Mark, out user);
        }

        public SimpleListResponse<MatchedCandidate> GetMatches(SimpleObjectRequest<Location> request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<MatchedCandidate>, SimpleObjectRequest<Location>>(request, this._GetMatches, out user);
        }

        public SimpleListResponse<MatchedCandidate> GetPreviewMatches(SimpleObjectRequest<Location> request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<MatchedCandidate>, SimpleObjectRequest<Location>>(request, this._GetPreviewMatches, out user);
        }

        public SimpleListResponse<MatchedCandidate> BuyNewMatches(BuyingNewMatchRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<MatchedCandidate>, BuyingNewMatchRequest>(request, this._BuyNewMatches, out user);
        }

        public SimpleResponse<MatchedCandidate> RequestPic4Pic(StartingPic4PicRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleResponse<MatchedCandidate>, StartingPic4PicRequest>(request, this._RequestPic4Pic, out user);
        }

        public SimpleResponse<MatchedCandidate> AcceptPic4Pic(AcceptingPic4PicRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleResponse<MatchedCandidate>, AcceptingPic4PicRequest>(request, this._AcceptPic4Pic, out user);
        }

        public CandidateDetailsResponse GetCandidateDetails(CandidateDetailsRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<CandidateDetailsResponse, CandidateDetailsRequest>(request, this._GetCandidateDetails, out user);
        }

        public SimpleListResponse<InstantMessage> SendInstantMessage(InstantMessageRequest request) 
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<InstantMessage>, InstantMessageRequest>(request, this._SendInstantMessage, out user);
        }

        public SimpleListResponse<InstantMessage> GetConversation(ConversationRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<InstantMessage>, ConversationRequest>(request, this._GetConversation, out user);
        }

        public SimpleListResponse<ConversationsSummary> GetConversationSummary(BaseRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<ConversationsSummary>, BaseRequest>(request, this._GetConversationSummary, out user);
        }

        public SimpleListResponse<PurchaseOffer> GetOffers(BaseRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<SimpleListResponse<PurchaseOffer>, BaseRequest>(request, this._GetOffers, out user);
        }

        public BaseResponse ProcessPurchase(SimpleObjectRequest<PurchaseRecord> request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, SimpleObjectRequest<PurchaseRecord>>(request, this._ProcessPurchase, out user);
        }

        public BaseResponse GetCurrentCredit(BaseRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, BaseRequest>(request, this._GetCurrentCredit, out user);
        }

        public BaseResponse TrackDevice(MobileDeviceRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, MobileDeviceRequest>(request, this._TrackDevice, out user);
        }

        public BaseResponse AssureSupportAtLocation(SimpleObjectRequest<Location> request) 
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, SimpleObjectRequest<Location>>(request, this._AssureSupportAtLocation, out user);
        }

        public BaseResponse SetCurrentLocation(SimpleObjectRequest<Location> request) 
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, SimpleObjectRequest<Location>>(request, this._SetCurrentLocation, out user);
        }

        public BaseResponse LogClientTraces(ClientLogRequest request)
        {
            UserAuthInfo user = null;
            return SafeExecute<BaseResponse, ClientLogRequest>(request, this._LogClientTraces, out user);
        }
    }
}
