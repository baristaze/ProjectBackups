using System;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace YummyZone.WebService
{
    [ServiceContract]    
    public partial interface IYummyZoneWebService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "Signup?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        AuthResult Signup(Identity identity);

        [OperationContract]
        [WebInvoke(UriTemplate = "Signin?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        AuthResult Signin(Identity identity);

        [OperationContract]
        [WebInvoke(UriTemplate = "Signup2?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        AuthResult Signup2(Identity identity);

        [OperationContract]
        [WebInvoke(UriTemplate = "Signin2?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        AuthResult Signin2(Identity identity);

        [OperationContract]
        [WebGet(UriTemplate = "Notify?format=plist&device={deviceToken}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult Notify(string deviceToken);

        /*
        [OperationContract]
        [WebInvoke(UriTemplate = "ChangeCreds?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        AuthResult ChangeUserNameAndPassword(Identity identity);
        */
        
        [OperationContract]
        [WebGet(UriTemplate = "SendPasswordReminder?format=plist&email={email}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult SendPasswordReminder(string email);
        
        [OperationContract]
        [WebGet(UriTemplate = "Venues?format=plist&lat={latitude}&long={longitude}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        VenueList GetNearbyVenues(double latitude, double longitude);

        [OperationContract]
        [WebGet(UriTemplate = "Menu?format=plist&venue={venueId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        Menu GetMenu(Guid venueId);

        [OperationContract]
        [WebGet(UriTemplate = "Survey?format=plist&venue={venueId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        Survey GetSurveyQuestions(Guid venueId);

        [OperationContract]
        [WebGet(UriTemplate = "Messages?format=plist&pageHint={pageHint}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        MessageList GetMessages(string pageHint);
        
        /*
        [OperationContract]
        [WebGet(UriTemplate = "Coupons?format=plist&pageHint={pageHint}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        CouponList GetCoupons(string pageHint);
        */

        [OperationContract]
        [WebGet(UriTemplate = "Coupons?format=plist&pageHint={pageHint}&venue={venueId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        CouponList GetCoupons(string pageHint, Guid venueId);

        [OperationContract]
        [WebGet(UriTemplate = "Favorites?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        Favorites GetFavorites();

        [OperationContract]
        [WebGet(UriTemplate = "History?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        History GetHistory();

        /// <summary>
        /// Content Type of the HTTP POST requests must be text/xml as shown below to let this method work properly
        /// Content-Type: text/xml
        /// </summary>        
        [OperationContract]
        [WebInvoke(UriTemplate = "Feedback?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult SaveFeedback(Feedback feedback);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "MarkMessageAsRead?format=plist&msg={messageId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult MarkMessageAsRead(Guid messageId);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "MarkCouponAsRead?format=plist&coupon={couponId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult MarkCouponAsRead(Guid couponId);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "DeleteMessage?format=plist&msg={messageId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult DeleteMessage(Guid messageId);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "DeleteCoupon?format=plist&coupon={couponId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        OperationResult DeleteCoupon(Guid couponId);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "RedeemCoupon?format=plist&coupon={couponId}&lat={latitude}&long={longitude}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        RedeemResult RedeemCoupon(Guid couponId, double latitude, double longitude);

        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "Settings?format=plist&name={name}&value={value}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        UserSettings SaveSettings(string name, string value);

        /*
        [OperationContract]
        [WebInvoke(UriTemplate = "TestSingleObject?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        SingleObjectTest TestSingleObject(SingleObjectTest data);

        [OperationContract]
        [WebInvoke(UriTemplate = "TestMultiObject?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        MultiObjectTest TestMultiObject(MultiObjectTest data);
        */

        /*
        /// <summary>
        /// All input values can be sent as URL params, therefore this method be an HTTP GET since it is enough
        /// </summary>        
        [OperationContract]
        [WebGet(UriTemplate = "RedeemCoupon2?format=plist&coupon={couponId}&venue={venueId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        RedeemResult RedeemCoupon2(Guid couponId, Guid venueId);
        */

        /*
        [OperationContract]
        [WebGet(UriTemplate = "GetAuthors")]
        [PListBehavior]
        Person GetAuthors();

        [OperationContract]
        [WebInvoke(UriTemplate = "SetAuthors?format=plist")]
        [PListBehavior]
        void SetAuthors(Person person);

        [OperationContract]
        [WebGet(UriTemplate = "Restaurants?format=plist&long={longitude}&lat={latitude}", BodyStyle=WebMessageBodyStyle.Bare)]
        [PListBehavior]
        RestaurantResponse GetRestaurants(long longitude, long latitude);

        [OperationContract]
        [WebGet(UriTemplate = "Menu?format=plist&id={restaurandId}", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        Menu GetRestaurantMenu(Guid restaurandId);

        [OperationContract]
        [WebInvoke(UriTemplate = "SetMenu?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        void SetRestaurantMenu(Menu menu);
        
        [OperationContract]
        [WebInvoke(UriTemplate = "PutString?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        void PutString(string s);

        [OperationContract]
        [WebInvoke(UriTemplate = "PutStringList?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        void PutStringList(System.Collections.Generic.List<string> s);

        [OperationContract]
        [WebInvoke(UriTemplate = "PutInteger?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        void PutInteger(int n);

        [OperationContract]
        [WebInvoke(UriTemplate = "PutIntegerList?format=plist", BodyStyle = WebMessageBodyStyle.Bare)]
        [PListBehavior]
        void PutIntegerList(System.Collections.Generic.List<int> n);

        [OperationContract]
        [WebInvoke(UriTemplate = "SetAuthorList?format=plist")]
        [PListBehavior]
        void SetAuthorList(System.Collections.Generic.List<Person> persons);*/
    }
}
