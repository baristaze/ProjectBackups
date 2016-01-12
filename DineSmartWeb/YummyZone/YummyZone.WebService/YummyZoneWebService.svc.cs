using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{   
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class YummyZoneWebService : IYummyZoneWebService
    {
        #region Constants

        private const double OneYardAsMile = 0.000568181818;

        private const int Success = 0;

        private const int UnknownError = 1;
        private const int AccessDenied = 2;
        private const int UsernameInUse = 3;
        private const int InvalidInput = 4;

        private const int EmptyObjectId = 10;
        private const int InvalidObjectId = 11;
        private const int ObjectNotFound = 12;
        private const int InputIsOutOfRange = 13;
        private const int InputIsTooLong = 14;
        private const int InvalidNullInput = 15;
        private const int InvalidEmptyInput = 16;

        private const int NotCheckedInYet = 20;
        private const int MaxCheckinCountPerDayReached = 21;
        private const int MaxCheckinCountPerVenuePerDayReached = 22;

        private const int TooManyRatedItems = 30;

        private const int TooFarAwayFromVenueToRedeem = 40;
        private const int CouponHasAlreadyRedeemed = 41;
        private const int ExpiredCoupon = 42;

        private const int TooFarAwayFromVenueForFeedback = 50;

        #endregion // Constants

        /// <summary>
        /// Sign up user...
        /// Initially both user name and password are expected to be Guid.
        /// Then, user might change both of them via ChangePassword.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public AuthResult Signup(Identity identity)
        {
            try
            {
                return _Signup(identity);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<AuthResult>(ex, "Signup");
            }
        }

        public AuthResult Signup2(Identity identity)
        {
            try
            {
                return _Signup2(identity);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<AuthResult>(ex, "Signup2");
            }
        }

        private AuthResult _Signup(Identity identity)
        {
            AuthResult result = new AuthResult();
            
            // check input
            OperationResult tempResult = new OperationResult();
            if (!VerifyIdentityText(identity, Helpers.UserNameFormat, Helpers.MinUserNameLength, Helpers.MinPasswordLength, ref tempResult))
            {
                result.OperationResult = tempResult;
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                if (DinerUserHelper.UsernameOrUserIdExists(connection, identity.UserName))
                {
                    OM.Diner diner = DinerUserHelper.GetUser(connection, identity.UserName, identity.Password);

                    if (diner != null && diner.Status == OM.Status.Active)
                    {
                        // success
                        result.OperationResult.ErrorMessage = "Already signed up";
                        result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                        OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, diner.Id);
                        result.Settings = new UserProfile(profile);

                        return result;
                    }
                    else
                    {
                        // user is in use
                        result.OperationResult.ErrorCode = UsernameInUse;
                        result.OperationResult.ErrorMessage = "The username is already in use";
                        return result;
                    }
                }

                // else
                // insert
                Guid userId = Guid.NewGuid();
                if (Helpers.UseClientGuid)
                {
                    userId = identity.TryParseUserNameAsGuidOrDefault(userId);
                }

                if (DinerUserHelper.InsertUser(connection, userId, identity.UserName, identity.Password))
                {
                    try
                    {
                        this.CreateInitialCoupons(connection, null, userId);
                    }
                    catch { }

                    try
                    {
                        this.CreateInitialMessage(connection, null, userId);
                    }
                    catch { }

                    result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);
                    
                    // no need to read from database but we read it again for uniformity
                    OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, userId);
                    result.Settings = new UserProfile(profile);
                }
                else
                {
                    result.OperationResult.ErrorCode = UnknownError;
                    result.OperationResult.ErrorMessage = "Signup failed unexpectedly. Please try again later.";
                }

            }

            return result;
        }

        private void CreateInitialCoupons(SqlConnection connection, SqlTransaction transaction, Guid dinerId)
        {
            OM.CampaignList campaigns = OM.Campaign.CampaignsForNewSignupDiners(connection, transaction, OM.Database.TimeoutSecs);
            if (campaigns.Count > 0)
            {
                OM.CouponList coupons = OM.Campaign.CouponsForNewSignupDiners(campaigns, dinerId);
                if (coupons.Count > 0)
                {
                    foreach (OM.Coupon coupon in coupons)
                    {
                        OM.Database.InsertOrUpdate(coupon, connection, transaction, OM.Database.TimeoutSecs);
                    }
                }
            }
        }

        private void CreateInitialMessage(SqlConnection connection, SqlTransaction transaction, Guid dinerId)
        {   
            OM.Message msg = new OM.Message();
            msg.GroupId = Helpers.InitialMessageGroupId;
            msg.ChainId = Helpers.InitialMessageChainId;
            msg.SenderId = Helpers.InitialMessageSenderId;
            msg.Title = Helpers.InitialMessageTitle;
            msg.Content = Helpers.InitialMessageContent;
            msg.ReceiverId = dinerId;

            // we don't want to send push notifications for this message
            // user will see it since he/she is downloading the app first time
            // We will set the pushTime to mimic that the notification is already pushed
            msg.PushTimeUTC = msg.QueueTimeUTC;

            OM.Database.InsertOrUpdate(msg, connection, transaction, OM.Database.TimeoutSecs);
        }

        private AuthResult _Signup2(Identity identity)
        {
            AuthResult result = new AuthResult();

            // current username must be a guid
            Guid tempGuid;
            OM.Diner currentUser = this.GetDinerFromContent();
            if (!Guid.TryParse(currentUser.UserName, out tempGuid))
            {
                result.OperationResult.ErrorCode = InvalidInput;
                result.OperationResult.ErrorMessage = "Current user is not an auto-signed-up user.";
                return result;
            }

            // check input: new username must be an email
            OperationResult tempResult = new OperationResult();
            if (!VerifyIdentityText(identity, UsernameType.Email, Helpers.MinUserNameLength, Helpers.MinPasswordLength, ref tempResult))
            {
                result.OperationResult = tempResult;
                return result;
            }
                        
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                if (DinerUserHelper.UsernameOrUserIdExists(connection, identity.UserName))
                {
                    // email exists
                    OM.Diner diner = DinerUserHelper.GetUser(connection, identity.UserName, identity.Password);

                    // passwords matches
                    if (diner != null && diner.Status == OM.Status.Active)
                    {
                        // merge accounts and return 
                        if (DinerUserHelper.MergeAccounts(connection, diner.Id, currentUser.Id, "Signup2"))
                        {
                            result.OperationResult.ErrorMessage = "Accounts merged";
                            result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                            OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, diner.Id);
                            result.Settings = new UserProfile(profile);

                            return result;
                        }
                        else
                        {
                            result.OperationResult.ErrorCode = UnknownError;
                            result.OperationResult.ErrorMessage = "Signup failed unexpectedly. Please contact to support.";
                        }
                    }
                    else
                    {
                        // user is in use
                        result.OperationResult.ErrorCode = UsernameInUse;
                        result.OperationResult.ErrorMessage = "The username is already in use";
                        return result;
                    }
                }
                else
                {
                    // update...
                    if (DinerUserHelper.UpdateUser(connection, currentUser.Id, currentUser.UserName, identity.UserName, identity.Password))
                    { 
                        result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                        OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, currentUser.Id);
                        result.Settings = new UserProfile(profile);
                    }
                    else
                    {
                        result.OperationResult.ErrorCode = UnknownError;
                        result.OperationResult.ErrorMessage = "Signup failed unexpectedly. Please try again later.";
                    }
                }

            }

            return result;
        }

        internal static bool VerifyIdentityText(Identity identity, UsernameType userNameFormat, int minUserNameLength, int minPswdLength, ref OperationResult operationResult)
        {   
            if (identity == null)
            {
                operationResult.ErrorCode = InvalidNullInput;
                operationResult.ErrorMessage = "Please specify a username and a password";
                return false;
            }

            if (!VerifyUsernameText(identity.UserName, userNameFormat, minUserNameLength, ref operationResult))
            {
                return false;
            }

            if (!VerifyPasswordText(identity.Password, minPswdLength, ref operationResult))
            {
                return false;
            }

            if (String.Compare(identity.UserName, identity.Password, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                operationResult.ErrorCode = InvalidInput;
                operationResult.ErrorMessage = "Password must be different than user name";
                return false;
            }

            return true;
        }

        internal static bool VerifyPasswordText(string password, int minPswdLength, ref OperationResult operationResult)
        {
            if (String.IsNullOrWhiteSpace(password))
            {
                operationResult.ErrorCode = InvalidEmptyInput;
                operationResult.ErrorMessage = "Please specify a password";
                return false;
            }

            if (password.Length < minPswdLength)
            {
                operationResult.ErrorCode = InvalidInput;
                operationResult.ErrorMessage = "Password length is too short: " + password.Length.ToString();
                return false;
            }

            return true;
        }

        internal static bool VerifyUsernameText(string userName, UsernameType userNameFormat, int minLength, ref OperationResult operationResult)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                operationResult.ErrorCode = InvalidEmptyInput;
                operationResult.ErrorMessage = "Please specify a username";
                return false;
            }

            userName = userName.Trim();

            if (userNameFormat == UsernameType.FreeText)
            {
                if (userName.Length < minLength)
                {
                    operationResult.ErrorCode = InvalidInput;
                    operationResult.ErrorMessage = "Username is too short: " + userName.Length.ToString();
                    return false;
                }
            }
            else if (userNameFormat == UsernameType.Email)
            {
                if (!Helpers.IsValidEmail(userName))
                {
                    operationResult.ErrorCode = InvalidInput;
                    operationResult.ErrorMessage = "Email format is wrong";
                    return false;
                }
            }
            else if (userNameFormat == UsernameType.Guid)
            {
                Guid userId;
                if (!Guid.TryParse(userName, out userId))
                {
                    operationResult.ErrorCode = InvalidInput;
                    operationResult.ErrorMessage = "Username format is wrong";
                    return false;
                }
            }
            else if (userNameFormat == UsernameType.EmailOrGuid)
            {
                Guid userId;
                if (!Guid.TryParse(userName, out userId) && !Helpers.IsValidEmail(userName))
                {
                    operationResult.ErrorCode = InvalidInput;
                    operationResult.ErrorMessage = "Username format is wrong";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Signs in the user
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public AuthResult Signin(Identity identity)
        {
            try
            {
                return _Signin(identity);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<AuthResult>(ex, "Signin");
            }
        }

        /// <summary>
        /// Signs in the user
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public AuthResult Signin2(Identity identity)
        {
            try
            {
                return _Signin2(identity);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<AuthResult>(ex, "Signin2");
            }
        }

        public AuthResult _Signin(Identity identity)
        {
            AuthResult result = new AuthResult();

            if (identity == null)
            {
                result.OperationResult.ErrorCode = InvalidNullInput;
                result.OperationResult.ErrorMessage = "Please enter an email address and a password";
                return result;
            }

            if (String.IsNullOrWhiteSpace(identity.UserName))
            {
                result.OperationResult.ErrorCode = InvalidEmptyInput;
                result.OperationResult.ErrorMessage = "Email address is empty";
                return result;
            }

            if (String.IsNullOrWhiteSpace(identity.Password))
            {
                result.OperationResult.ErrorCode = InvalidEmptyInput;
                result.OperationResult.ErrorMessage = "Password is empty";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                OM.Diner diner = DinerUserHelper.GetUser(connection, identity.UserName, identity.Password);
                if (diner != null && diner.Status == OM.Status.Active)
                {
                    // success
                    result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                    OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, diner.Id);
                    result.Settings = new UserProfile(profile);

                    return result;
                }
                else
                {
                    result.OperationResult.ErrorCode = AccessDenied;
                    result.OperationResult.ErrorMessage = "Access is denied";
                    return result;
                }
            }
        }

        public AuthResult _Signin2(Identity identity)
        {
            AuthResult result = new AuthResult();

            // current username must be a guid
            Guid tempGuid;
            OM.Diner currentUser = this.GetDinerFromContent();
            if (!Guid.TryParse(currentUser.UserName, out tempGuid))
            {
                result.OperationResult.ErrorCode = InvalidInput;
                result.OperationResult.ErrorMessage = "Current user is not an auto-signed-up user.";
                return result;
            }

            // check input: new username must be an email
            OperationResult tempResult = new OperationResult();
            if (!VerifyIdentityText(identity, UsernameType.Email, 0, 0, ref tempResult))
            {
                result.OperationResult = tempResult;
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                OM.Diner diner = DinerUserHelper.GetUser(connection, identity.UserName, identity.Password);
                if (diner != null && diner.Status == OM.Status.Active)
                {
                    // merge accounts and return 
                    if (DinerUserHelper.MergeAccounts(connection, diner.Id, currentUser.Id, "Signin2"))
                    {
                        result.OperationResult.ErrorMessage = "Accounts merged";
                        result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                        OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, diner.Id);
                        result.Settings = new UserProfile(profile);

                        return result;
                    }
                    else
                    {
                        result.OperationResult.ErrorCode = UnknownError;
                        result.OperationResult.ErrorMessage = "Signin failed unexpectedly. Please contact to support.";
                        return result;
                    }
                }
                else
                {
                    result.OperationResult.ErrorCode = AccessDenied;
                    result.OperationResult.ErrorMessage = "Access is denied";
                    return result;
                }
            }
        }

        public OperationResult Notify(string deviceToken)
        {
            try
            {
                return _Notify(deviceToken);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "Notify");
            }
        }

        private OperationResult _Notify(string deviceToken)
        {
            if (String.IsNullOrWhiteSpace(deviceToken))
            {
                return new OperationResult(InvalidEmptyInput, "Device Token is empty");
            }

            deviceToken = deviceToken.Trim();

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                return new OperationResult(AccessDenied, "Unknown user");
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                OM.NotificationClient info = new OM.NotificationClient();
                info.DinerId = dinerId;
                info.DeviceType = OM.MobileDeviceType.IPhone;
                info.DeviceToken = deviceToken;

                OM.Database.InsertOrUpdate(info, connection, null, OM.Database.TimeoutSecs);
            }

            return new OperationResult(Success, "Push Notification Subscription is done");
        }

        /// <summary>
        /// This is to start using email instead of auto-generated credentials.
        /// Authentication is required for this method. Following scenarios are enabled:
        /// ...
        /// Start to use email address instead of machine guid
        /// Change User Name
        /// Change Password
        /// Change User Name & Password
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public AuthResult ChangeUserNameAndPassword(Identity identity)
        {
            try
            {
                return _ChangeUserNameAndPassword(identity);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<AuthResult>(ex, "ChangeUserNameAndPassword");
            }
        }

        private AuthResult _ChangeUserNameAndPassword(Identity identity)
        {
            AuthResult result = new AuthResult();

            // check input
            OperationResult tempResult = new OperationResult();
            if (!VerifyIdentityText(identity, UsernameType.Email, Helpers.MinUserNameLength, Helpers.MinPasswordLength, ref tempResult))
            {
                result.OperationResult = tempResult;
                return result;
            }

            OM.Diner diner = GetDinerFromContent();
            if (diner == null)
            {   
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Access Denied";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                if (DinerUserHelper.UsernameOrUserIdExists(connection, identity.UserName))
                {
                    if (String.Compare(identity.UserName, diner.UserName, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                             
                    }
                    else
                    {
                        // same user... probably calling the update again, or, changing the password.
                    }
                }

                if (DinerUserHelper.UpdateUser(connection, diner.Id, diner.UserName, identity.UserName, identity.Password))
                {
                    result.Cookie = GenerateBase64EncodedString(identity.UserName, identity.Password);

                    OM.DinerSettings profile = OM.DinerSettings.ReadFromDBase(connection, null, diner.Id);
                    result.Settings = new UserProfile(profile);
                }
                else
                {
                    result.OperationResult.ErrorCode = UnknownError;
                    result.OperationResult.ErrorMessage = "User registration has failed unexpectedly. Please try again later.";
                }
            }

            return result;
        }
        
        /// <summary>
        /// Sends password reminder email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public OperationResult SendPasswordReminder(string email)
        {
            try
            {
                return _SendPasswordReminder(email);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "SendPasswordReminder");
            }
        }

        private OperationResult _SendPasswordReminder(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return new OperationResult(InvalidEmptyInput, "Email address is empty");
            }

            email = email.Trim();
            if (!Helpers.IsValidEmail(email))
            {
                return new OperationResult(InvalidInput, "Email format is wrong");
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                if (DinerUserHelper.PasswordDefined(connection, email))
                {
                    // send email
                }
            }

            return new OperationResult(Success, "Password reminder has been sent to the specified email address");
        }
        
        /// <summary>
        /// Searches nearby restaurants and returns them along with their menus
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>List of restaurants and their menus based on local time</returns>
        public VenueList GetNearbyVenues(double latitude, double longitude)
        {
            try
            {
                return _GetNearbyVenues(latitude, longitude);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<VenueList>(ex, "GetNearbyVenues");
            }
        }

        private VenueList _GetNearbyVenues(double latitude, double longitude)
        {
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                VenueList nearByVenues = this.SearchVenues(connection, latitude, longitude, false);

                int maxNearbyVenues = Helpers.MaxNearbyVenues;
                if (nearByVenues.Venues.Count > maxNearbyVenues)
                {
                    nearByVenues.Venues.RemoveRange(
                        maxNearbyVenues, 
                        nearByVenues.Venues.Count - maxNearbyVenues);
                }

                return nearByVenues;
            }
        }
    
        /// <summary>
        /// Returns reduced menu of a certain restaurant
        /// </summary>
        /// <param name="venueId"></param>
        /// <returns></returns>
        public Menu GetMenu(Guid venueId)
        {
            try
            {
                return _GetMenu(venueId);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<Menu>(ex, "GetMenu");
            }
        }

        private Menu _GetMenu(Guid venueId)
        {
            // output
            Menu menu = new Menu();
            menu.VenueId = venueId;

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                menu.OperationResult.ErrorCode = AccessDenied;
                menu.OperationResult.ErrorMessage = "Unknown user";
                return menu;
            }

            // check input
            if (venueId == Guid.Empty)
            {
                menu.OperationResult.ErrorCode = EmptyObjectId;
                menu.OperationResult.ErrorMessage = "Venu Id may not be empty.";
                return menu;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // check the venue
                OM.Venue venue = this.GetVenue(connection, venueId);
                if (venue == null)
                {
                    menu.OperationResult.ErrorCode = ObjectNotFound;
                    menu.OperationResult.ErrorMessage = "Venue couldn't be found.";
                    return menu;
                }

                // calculate local time at venue
                TimeZoneInfo tzi = this.GetTimeZoneInfo(venue);
                DateTime venueLocalTimeAtCheckin = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
                int timeSliceOfCheckin = venueLocalTimeAtCheckin.Hour * 60 + venueLocalTimeAtCheckin.Minute;
                if (venueLocalTimeAtCheckin.Hour >= 0 && venueLocalTimeAtCheckin.Hour <= 2)
                {
                    timeSliceOfCheckin += 24 * 60;
                }

                // get currently served menus
                IdAndTextList menuInfosAtCheckinTime = this.GetServedMenus(connection, venueId, timeSliceOfCheckin);
                IdAndTextList menuInfos = new IdAndTextList();
                menuInfos.AddRange(menuInfosAtCheckinTime);

                bool showPrices = Helpers.ShowPrices;

                // populate menus
                OM.MenuCategoryList categories = new OM.MenuCategoryList();
                for (int x = 0; x < menuInfos.Count; x++)
                {
                    OM.MenuCategoryList newCats = OM.MenuCategory.GetMenuCategories(connection, venue.GroupId, menuInfos[x].Id);
                    for (int y=0; y<newCats.Count; y++)
                    {
                        if (newCats[y].Items.Count > 0)
                        {
                            if (categories[newCats[y].Id] == null)
                            {
                                if (String.Compare(menuInfos[x].Text, newCats[y].Name, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    newCats[y].Name = menuInfos[x].Text;
                                }
                                else
                                {
                                    newCats[y].Name = menuInfos[x].Text + " - " + newCats[y].Name;
                                }

                                if (!showPrices)
                                {
                                    for (int z = 0; z < newCats[y].Items.Count; z++)
                                    {
                                        if (newCats[y].Items[z].Price.HasValue)
                                        {
                                            newCats[y].Items[z].Price = (decimal)0.0;
                                        }
                                    }
                                }

                                categories.Add(newCats[y]);
                            }
                        }
                    }
                }

                // populate user's own ratings
                ItemRateList<byte> ratesByCurrentDiner = this.GetLatestRatesOfDinerAt(connection, venueId, dinerId, Guid.Empty);

                // populate average ratings
                ItemRateList<decimal> orderedAvgRates = this.GetAverageMenuItemRatesAt(connection, venueId, Guid.Empty);

                // attach rates to the menu
                menu = AttachRatesToMenu(categories, ratesByCurrentDiner, orderedAvgRates);
                menu.VenueId = venueId;

                // return menu
                return menu;
            }
        }

        /// <summary>
        /// Returns survey questions of a certain restaurant
        /// </summary>
        /// <param name="venueId"></param>
        /// <returns></returns>
        public Survey GetSurveyQuestions(Guid venueId)
        {
            try
            {
                return _GetSurveyQuestions(venueId);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<Survey>(ex, "GetSurveyQuestions");
            }
        }
        
        private Survey _GetSurveyQuestions(Guid venueId)
        {
            // output
            Survey survey = new Survey();
            survey.VenueId = venueId;
            survey.MaxLengthOfAnswerForAnOpenEndedQuestion = Helpers.MaxLengthOfAnswerForOpenEndedQuestion;

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                survey.OperationResult.ErrorCode = AccessDenied;
                survey.OperationResult.ErrorMessage = "Unknown user";
                return survey;
            }

            // check input
            if (venueId == Guid.Empty)
            {
                survey.OperationResult.ErrorCode = EmptyObjectId;
                survey.OperationResult.ErrorMessage = "Venu Id may not be empty.";
                return survey;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // check the venu
                OM.Venue venue = this.GetVenue(connection, venueId);
                if (venue == null)
                {
                    survey.OperationResult.ErrorCode = ObjectNotFound;
                    survey.OperationResult.ErrorMessage = "Venue couldn't be found.";
                    return survey;
                }

                // check the current checkin session
                OM.AnswerList answerList = new OM.AnswerList();
                OM.Checkin checkin = this.GetCurrentCheckinSession(connection, dinerId, venueId);
                if (checkin != null)
                {
                    // weird, yes, but pass checkinId instead of GroupId
                    answerList = OM.Database.SelectAll<OM.Answer, OM.AnswerList>(
                        connection, null, checkin.Id, OM.Database.TimeoutSecs);
                }

                // retrieve survey questions
                List<OM.QuestionList> allQuestions = OM.Question.SelectSurveyQuestionsOfChain(
                    connection, venue.GroupId, venue.ChainId);

                // prepare the output
                survey.Init(allQuestions[0], allQuestions[1], allQuestions[2], allQuestions[3], answerList);

                // return 
                return survey;
            }
        }

        /// <summary>
        /// Returns messages that are sent to the diner
        /// </summary>
        /// <param name="pageHint"></param>
        /// <returns></returns>
        public MessageList GetMessages(string pageHint)
        {
            try
            {
                return _GetMessages(pageHint);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<MessageList>(ex, "GetMessages");
            }
        }

        private MessageList _GetMessages(string pageHint)
        { 
            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                MessageList result = new MessageList();
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Unknown user";
                return result;
            }

            // calculate paging cutoff time
            DateTime pageCutoffTimeUTC = this.ConvertPageHintToDateTime(pageHint);

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                return this.GetMessagesForDiner(connection, dinerId, pageCutoffTimeUTC);
            }
        }
        
        /*
        /// <summary>
        /// Returns coupons that are sent to the diner
        /// </summary>
        /// <returns></returns>
        public CouponList GetCoupons(string pageHint)
        {
            try
            {
                return _GetCoupons(pageHint, Guid.Empty);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<CouponList>(ex, "GetCoupons");
            }
        }
        */

        public CouponList GetCoupons(string pageHint, Guid venueId)
        {
            try
            {
                return _GetCoupons(pageHint, venueId);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<CouponList>(ex, "GetCoupons");
            }
        }

        private CouponList _GetCoupons(string pageHint, Guid venueId)
        {
            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                CouponList result = new CouponList();
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Unknown user";
                return result;
            }

            // calculate paging cutoff time
            DateTime pageCutoffTimeUTC = this.ConvertPageHintToDateTime(pageHint);

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                CouponList coupons = this.GetCouponsForDiner(connection, dinerId, pageCutoffTimeUTC);

                Guid chainId = Guid.Empty;
                if (venueId != Guid.Empty && coupons.Coupons.Count > 0)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = OM.Venue.SelectQuery(venueId);
                        command.CommandTimeout = OM.Database.TimeoutSecs;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OM.Venue venue = new OM.Venue();
                                venue.InitFromSqlReader(reader);
                                chainId = venue.ChainId;
                            }
                        }
                    }
                }

                if (chainId != Guid.Empty)
                {
                    for (int x = 0; x < coupons.Coupons.Count; x++)
                    {
                        if (coupons.Coupons[x].SenderChainId != chainId)
                        {
                            coupons.Coupons.RemoveAt(x);
                            x--;
                        }
                    }
                }

                return coupons;
            }
        }

        /// <summary>
        /// Returns favorite plates and restaurants for a diner
        /// </summary>
        /// <returns></returns>
        public Favorites GetFavorites()
        {
            try
            {
                return _GetFavorites();
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<Favorites>(ex, "GetFavorites");
            }
        }

        private Favorites _GetFavorites()
        {
            // prepare result
            Favorites result = new Favorites();

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Access Denied";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                List<FavoriteMenuItem> items = this.GetFavoriteMenuItemsForDiner(
                    connection, dinerId, Helpers.FavMinMenuItemRate, Helpers.FavMaxMenuItem);

                result.FavoriteMenuItems.AddRange(items);

                List<FavoriteVenue> venues = this.GetFavoriteVenuesForDiner(
                    connection, dinerId, Helpers.FavMinVenueRate, Helpers.FavMaxVenue);

                result.FavoriteVenues.AddRange(venues);

                return result;
            }
        }

        /// <summary>
        /// Returns history for a diner
        /// </summary>
        /// <returns></returns>
        public History GetHistory()
        {
            try
            {
                return _GetHistory();
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<History>(ex, "GetHistory");
            }
        }

        private History _GetHistory()
        {
            // prepare result
            History result = new History();

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Access Denied";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                List<HistoryItem> historyItems = this.GetHistoryForDiner(connection, dinerId, Helpers.HistoryMaxElements);
                result.Checkins.AddRange(historyItems);
                return result;
            }
        }

        /// <summary>
        /// Saves the feedback to the database
        /// </summary>
        /// <param name="venueId"></param>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public OperationResult SaveFeedback(Feedback feedback)
        {
            try
            {
                return _SaveFeedback(feedback);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "SaveFeedback");
            }
        }

        private OperationResult _SaveFeedback(Feedback feedback)
        {
            // prepare the output
            OperationResult operationResult = new OperationResult();

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                operationResult.ErrorCode = AccessDenied;
                operationResult.ErrorMessage = "Unknown user";
                return operationResult;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                
                // check the venu and checkin
                OM.Venue venue = this.GetVenue(connection, feedback.VenueId);
                if (venue == null)
                {
                    operationResult.ErrorCode = ObjectNotFound;
                    operationResult.ErrorMessage = "Venue couldn't be found.";
                    return operationResult;
                }

                // check the distance if this is version 1.1
                if (feedback.CurrentLocation != null)
                {
                    // we have coordinate on feedback... this is version 1.1+
                    // check distance
                    double rangeLimit = venue.RangeLimitInMilesForFeedback.HasValue ? 
                        (double)venue.RangeLimitInMilesForFeedback.Value : 
                        Helpers.Default_SendFeedback_RangeLimitInMiles;

                    double distanceInMiles = OM.Venue.CalculateDistanceInMiles(
                        (double)venue.Latitude.Value, 
                        feedback.CurrentLocation.Latitude,
                        (double)venue.Longitude.Value,
                        feedback.CurrentLocation.Longitude);

                    if (distanceInMiles > rangeLimit)
                    {
                        // pose logic might go here... 
                        string err = "You are too far away from {0} to leave feedback.";
                        operationResult.ErrorMessage = String.Format(CultureInfo.InvariantCulture, err, venue.Name);
                        operationResult.ErrorCode = TooFarAwayFromVenueForFeedback;
                        return operationResult;
                    }
                }
                else
                {
                    // we do NOT have coordinate on feedback... this is version 1.0
                    if (!Helpers.AllowNullCoordinateOnFeedback)
                    {
                        operationResult.ErrorCode = InvalidInput;
                        operationResult.ErrorMessage = "No location info provided! Download latest version of the app!";
                        return operationResult;
                    }
                }

                // check check-in
                OM.Checkin checkin = null;
                bool isNewCheckin = false;
                operationResult = this.Checkin(connection, dinerId, feedback.VenueId, out checkin, ref venue, out isNewCheckin);
                if (operationResult.ErrorCode != Success)
                {
                    return operationResult;
                }

                // get menu item rates
                OM.MenuItemRateList newMenuItemRateList = feedback.ToMenuItemRateList(checkin.Id);
                if (newMenuItemRateList.Count > 0)
                {
                    // validate menu item Ids
                    List<Guid> menuItemIds = this.GetAllMenuItemIdsOfAVenue(connection, venue.GroupId, feedback.VenueId);
                    foreach (OM.MenuItemRate mir in newMenuItemRateList)
                    {
                        if (!menuItemIds.Contains(mir.MenuItemId))
                        {
                            operationResult.ErrorCode = InvalidObjectId;
                            operationResult.ErrorMessage = "One of the menu item that you have rated doesn't seem to belong to the venue or it has been removed.";
                            return operationResult;
                        }
                    }

                    // make sure that user doesn't over-rate too many items
                    OM.MenuItemRateList oldRateList = new OM.MenuItemRateList();
                    if (!isNewCheckin)
                    {
                        oldRateList = this.GetLatestMenuItemRates(connection, checkin.Id);
                        if (oldRateList.Count > 0)
                        {
                            foreach (OM.MenuItemRate mir in newMenuItemRateList)
                            {
                                int index = oldRateList.IndexOf(mir.MenuItemId);
                                if (index >= 0)
                                {
                                    oldRateList.RemoveAt(index);
                                }

                                if (oldRateList.Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    // calculate total rated item count
                    int maxRateCountLimit = Helpers.MaxMenuItemRateCount;
                    int totalCount = oldRateList.Count + newMenuItemRateList.Count;
                    if (totalCount > maxRateCountLimit)
                    {
                        string errMsg = "You may not rate or check-out more than {0} menu items in a certain check-in session";
                        errMsg = String.Format(CultureInfo.InvariantCulture, errMsg, maxRateCountLimit);
                        operationResult.ErrorCode = TooManyRatedItems;
                        operationResult.ErrorMessage = errMsg;
                        return operationResult;
                    }
                }

                // get user answers
                OM.AnswerList userAnswers = feedback.ToAnswerList(checkin.Id);
                if (userAnswers.Count > 0)
                {
                    // retrieve survey questions
                    List<OM.QuestionList> allQuestions = OM.Question.SelectSurveyQuestionsOfChain(
                        connection, venue.GroupId, venue.ChainId);

                    // flat the questions
                    OM.QuestionList joinedQuestions = new OM.QuestionList();
                    foreach (OM.QuestionList qlist in allQuestions)
                    {
                        joinedQuestions.AddRange(qlist);
                    }

                    // verify questions
                    foreach (OM.Answer answer in userAnswers)
                    {
                        // verify question Id of the anser
                        OM.Question question = joinedQuestions[answer.QuestionId];
                        if (question == null)
                        {
                            operationResult.ErrorCode = InvalidObjectId;
                            operationResult.ErrorMessage = "One of the survey question that you have answered doesn't seem to belong to the venue or it has been removed.";
                            return operationResult;
                        }

                        // verify rate answers
                        if (answer.AnswerRate.HasValue)
                        {
                            if (answer.AnswerRate.Value <= 0 && answer.AnswerRate.Value > 5)
                            {
                                operationResult.ErrorCode = InputIsOutOfRange;
                                operationResult.ErrorMessage = "Rates can be between 1 and 5 inclusive for the questions";
                                return operationResult;
                            }
                        }

                        // check the choice id
                        if (answer.AnswerChoiceId.HasValue)
                        {
                            if (question.Choices[answer.AnswerChoiceId.Value] == null)
                            {
                                operationResult.ErrorCode = InvalidObjectId;
                                operationResult.ErrorMessage = "One of the multiple choice options that you have chosen as an answer doesn't seem to belong to the venue or it has been removed.";
                                return operationResult;
                            }
                        }

                        // check length of answer of open ended questions
                        if (!String.IsNullOrWhiteSpace(answer.AnswerFreeText))
                        {
                            answer.AnswerFreeText = answer.AnswerFreeText.Trim();
                            int maxLength = Helpers.MaxLengthOfAnswerForOpenEndedQuestion;
                            if (answer.AnswerFreeText.Length > maxLength)
                            {
                                operationResult.ErrorCode = InputIsTooLong;
                                operationResult.ErrorMessage = "One of your answer to an open ended question is too long. Max length is " + maxLength.ToString();
                                return operationResult;
                            }
                        }
                    }
                }

                // save to the database
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    bool allSucceeded = false;

                    try
                    {
                        // save the menu item rates   
                        foreach (OM.MenuItemRate mir in newMenuItemRateList)
                        {
                            OM.Database.InsertOrUpdate(mir, connection, trans, OM.Database.TimeoutSecs);
                        }

                        // save the answers
                        foreach (OM.Answer answer in userAnswers)
                        {
                            OM.Database.InsertOrUpdate(answer, connection, trans, OM.Database.TimeoutSecs);
                        }

                        trans.Commit();
                        allSucceeded = true;
                    }
                    finally
                    {
                        if (!allSucceeded)
                        {
                            trans.Rollback();
                        }
                    }
                }

                // return
                return operationResult;
            }
        }

        /// <summary>
        /// Marks a message as read
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public OperationResult MarkMessageAsRead(Guid messageId)
        {
            try
            {
                return _MarkMessageAsRead(messageId);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "MarkMessageAsRead");
            }
        }

        private OperationResult _MarkMessageAsRead(Guid messageId)
        {
            OperationResult result = new OperationResult();

            if (messageId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Message Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.ErrorCode = AccessDenied;
                result.ErrorMessage = "Unknown user";
                return result;
            }
                        
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // read the message
                OM.Message msg = new OM.Message();
                msg.Id = messageId;
                if (!OM.Database.Select(msg, connection, null, OM.Database.TimeoutSecs))
                {
                    result.ErrorCode = ObjectNotFound;
                    result.ErrorMessage = "Message could not be found.";
                    return result;
                }

                // check ownership
                if (dinerId != msg.ReceiverId)
                {
                    result.ErrorCode = AccessDenied;
                    result.ErrorMessage = "You don't have access to perform that operation.";
                    return result;
                }

                // check to se if it is marked already
                if (msg.ReadTimeUTC.HasValue)
                {
                    result.ErrorCode = Success;
                    result.ErrorMessage = "Message has been already marked as read in the past";
                    return result;
                }

                // update the read time to mark it as read
                using (SqlCommand markCommand = connection.CreateCommand())
                {
                    string readTime = Helpers.ToDBaseString(DateTime.UtcNow);
                    string markQuery = "UPDATE [dbo].[Message] SET [ReadTimeUTC] = '{0}' WHERE [ReceiverId] = '{1}' AND [Id] = '{2}' AND [ReadTimeUTC] is NULL;";
                    markQuery = String.Format(CultureInfo.InvariantCulture, markQuery, readTime, dinerId, messageId);

                    markCommand.CommandText = markQuery;
                    markCommand.CommandTimeout = OM.Database.TimeoutSecs;

                    int affectedRows = markCommand.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        result.ErrorCode = Success;
                        result.ErrorMessage = "Message has been already marked as read in the past";
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Marks a coupon as read
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public OperationResult MarkCouponAsRead(Guid couponId)
        {
            try
            {
                return _MarkCouponAsRead(couponId);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "MarkCouponAsRead");
            }
        }

        private OperationResult _MarkCouponAsRead(Guid couponId)
        {
            OperationResult result = new OperationResult();

            if (couponId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Coupon Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.ErrorCode = AccessDenied;
                result.ErrorMessage = "Unknown user";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // read the coupon
                OM.Coupon coupon = new OM.Coupon();
                coupon.Id = couponId;
                if (!OM.Database.Select(coupon, connection, null, OM.Database.TimeoutSecs))
                {
                    result.ErrorCode = ObjectNotFound;
                    result.ErrorMessage = "Coupon could not be found.";
                    return result;
                }

                // check ownership
                if (dinerId != coupon.ReceiverId)
                {
                    result.ErrorCode = AccessDenied;
                    result.ErrorMessage = "You don't have access to perform that operation.";
                    return result;
                }

                // check to se if it is marked already
                if (coupon.ReadTimeUTC.HasValue)
                {
                    result.ErrorCode = Success;
                    result.ErrorMessage = "Coupon has been already marked as read in the past";
                    return result;
                }

                // update the read time to mark it as read
                using (SqlCommand markCommand = connection.CreateCommand())
                {
                    string readTime = Helpers.ToDBaseString(DateTime.UtcNow);
                    string markQuery = "UPDATE [dbo].[Coupon] SET [ReadTimeUTC] = '{0}' WHERE [ReceiverId] = '{1}' AND [Id] = '{2}' AND [ReadTimeUTC] is NULL;";
                    markQuery = String.Format(CultureInfo.InvariantCulture, markQuery, readTime, dinerId, couponId);

                    markCommand.CommandText = markQuery;
                    markCommand.CommandTimeout = OM.Database.TimeoutSecs;

                    int affectedRows = markCommand.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        result.ErrorCode = Success;
                        result.ErrorMessage = "Coupon has been already marked as read in the past";
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes a message (marks as deleted)
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public OperationResult DeleteMessage(Guid messageId)
        {
            try
            {
                return _DeleteMessage(messageId);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "DeleteMessage");
            }
        }

        private OperationResult _DeleteMessage(Guid messageId)
        {
            OperationResult result = new OperationResult();

            if (messageId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Message Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.ErrorCode = AccessDenied;
                result.ErrorMessage = "Unknown user";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // read the message
                OM.Message msg = new OM.Message();
                msg.Id = messageId;
                if (!OM.Database.Select(msg, connection, null, OM.Database.TimeoutSecs))
                {
                    result.ErrorCode = ObjectNotFound;
                    result.ErrorMessage = "Message could not be found.";
                    return result;
                }

                // check ownership
                if (dinerId != msg.ReceiverId)
                {
                    result.ErrorCode = AccessDenied;
                    result.ErrorMessage = "You don't have access to perform that operation.";
                    return result;
                }

                // check to se if it is marked already
                if (msg.DeleteTimeUTC.HasValue)
                {
                    result.ErrorCode = Success;
                    result.ErrorMessage = "Message has been already deleted";
                    return result;
                }

                // update the delete time to mark it as deleted
                using (SqlCommand markCommand = connection.CreateCommand())
                {
                    string deleteTime = Helpers.ToDBaseString(DateTime.UtcNow);
                    string markQuery = "UPDATE [dbo].[Message] SET [DeleteTimeUTC] = '{0}' WHERE [ReceiverId] = '{1}' AND [Id] = '{2}' AND [DeleteTimeUTC] is NULL;";
                    markQuery = String.Format(CultureInfo.InvariantCulture, markQuery, deleteTime, dinerId, messageId);

                    markCommand.CommandText = markQuery;
                    markCommand.CommandTimeout = OM.Database.TimeoutSecs;

                    int affectedRows = markCommand.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        result.ErrorCode = Success;
                        result.ErrorMessage = "Message has been already deleted";
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes a coupon (marks as deleted)
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public OperationResult DeleteCoupon(Guid couponId)
        {
            try
            {
                return _DeleteCoupon(couponId);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult(ex, "DeleteCoupon");
            }
        }

        private OperationResult _DeleteCoupon(Guid couponId)
        {
            OperationResult result = new OperationResult();

            if (couponId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Coupon Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.ErrorCode = AccessDenied;
                result.ErrorMessage = "Unknown user";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // read the coupon
                OM.Coupon coupon = new OM.Coupon();
                coupon.Id = couponId;
                if (!OM.Database.Select(coupon, connection, null, OM.Database.TimeoutSecs))
                {
                    result.ErrorCode = ObjectNotFound;
                    result.ErrorMessage = "Coupon could not be found.";
                    return result;
                }

                // check ownership
                if (dinerId != coupon.ReceiverId)
                {
                    result.ErrorCode = AccessDenied;
                    result.ErrorMessage = "You don't have access to perform that operation.";
                    return result;
                }

                // check to se if it is marked already
                if (coupon.DeleteTimeUTC.HasValue)
                {
                    result.ErrorCode = Success;
                    result.ErrorMessage = "Coupon has been already deleted";
                    return result;
                }

                // update the delete time to mark it as deleted
                using (SqlCommand markCommand = connection.CreateCommand())
                {
                    string deleteTime = Helpers.ToDBaseString(DateTime.UtcNow);
                    string markQuery = "UPDATE [dbo].[Coupon] SET [DeleteTimeUTC] = '{0}' WHERE [ReceiverId] = '{1}' AND [Id] = '{2}' AND [DeleteTimeUTC] is NULL;";
                    markQuery = String.Format(CultureInfo.InvariantCulture, markQuery, deleteTime, dinerId, couponId);

                    markCommand.CommandText = markQuery;
                    markCommand.CommandTimeout = OM.Database.TimeoutSecs;

                    int affectedRows = markCommand.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        result.ErrorCode = Success;
                        result.ErrorMessage = "Coupon has been already deleted";
                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Redeems a coupon
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public RedeemResult RedeemCoupon(Guid couponId, double latitude, double longitude)
        {
            try
            {
                return _RedeemCoupon(couponId, latitude, longitude);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<RedeemResult>(ex, "RedeemCoupon");
            }
        }

        private RedeemResult _RedeemCoupon(Guid couponId, double latitude, double longitude)
        {
            RedeemResult result = new RedeemResult();
            
            // check input
            if (couponId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = EmptyObjectId;
                result.OperationResult.ErrorMessage = "Coupon Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Unknown user";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // check coupon
                OM.Coupon coupon;
                result.OperationResult = this.CheckCouponToRedeem(connection, couponId, dinerId, out coupon);
                if (result.OperationResult.ErrorCode != Success)
                {
                    return result;
                }
                
                // where to redeem?
                VenueList nearByVenues = this.SearchVenues(connection, latitude, longitude, true);
                nearByVenues = nearByVenues.FilterByChainId(coupon.ChainId);
                
                if (nearByVenues.Venues.Count <= 0)
                {   
                    // read chain name
                    OM.Chain chain = new OM.Chain();
                    chain.Id =coupon.ChainId;
                    chain.GroupId = coupon.GroupId;
                    string chainName = "the venue";
                    if (OM.Database.Select(chain, connection, null, OM.Database.TimeoutSecs))
                    {
                        chainName = chain.Name;
                    }

                    string err = "You are too far away from {0} to redeem this coupon.";
                    result.OperationResult.ErrorMessage = String.Format(CultureInfo.InvariantCulture, err, chainName);
                    result.OperationResult.ErrorCode = TooFarAwayFromVenueToRedeem;
                    return result;
                }
                
                // which menu to redeem?
                OM.Checkin checkin = null;

                // assume that there is one or more active check-in sessions.
                int maxMinutesPerCheckinSession = Helpers.MaxMinutesPerCheckinSession;
                OM.CheckinList lastCheckins = this.GetLatestCheckins(connection, dinerId);
                foreach (Venue v in nearByVenues.Venues)
                {
                    OM.CheckinList filtered = lastCheckins.FilterBy(v.Id);
                    if (filtered.Count > 0)
                    {
                        OM.Checkin last = filtered[0];
                        TimeSpan diff = DateTime.UtcNow - last.TimeUTC;
                        if (diff.TotalMinutes <= maxMinutesPerCheckinSession)
                        {
                            checkin = last;
                            break;
                        }
                    }
                }

                // user is attempting to redeem without checking in... let's make a check-in
                if (checkin == null)
                {
                    OM.Venue venue = null;
                    bool isNewCheckin = false;
                    Venue venueToRedeem = nearByVenues.Venues[0];
                    result.OperationResult = this.Checkin(connection, dinerId, venueToRedeem.Id, out checkin, ref venue, out isNewCheckin);
                    if (result.OperationResult.ErrorCode != Success)
                    {
                        return result;
                    }
                }

                result.OperationResult = this.RedeemCoupon(connection, coupon, checkin);
                result.CheckedInVenueId = checkin.VenueId;
                return result;
            }
        }

        /// <summary>
        /// Redeems a coupon
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public RedeemResult RedeemCoupon2(Guid couponId, Guid venueId)
        {
            try
            {
                return _RedeemCoupon2(couponId, venueId);
            }
            catch(Exception ex)
            {
                return LogExceptionAndGetErrorResult<RedeemResult>(ex, "RedeemCoupon2");
            }
        }

        private RedeemResult _RedeemCoupon2(Guid couponId, Guid venueId)
        {
            RedeemResult result = new RedeemResult();

            if (couponId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = EmptyObjectId;
                result.OperationResult.ErrorMessage = "Coupon Id may not be empty.";
                return result;
            }

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Unknown user";
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // check coupon
                OM.Coupon coupon;
                result.OperationResult = this.CheckCouponToRedeem(connection, couponId, dinerId, out coupon);
                if (result.OperationResult.ErrorCode != Success)
                {
                    return result;
                }

                OM.Checkin checkin = this.GetCurrentCheckinSession(connection, dinerId, venueId);
                if (checkin == null)
                {
                    result.OperationResult.ErrorCode = NotCheckedInYet;
                    result.OperationResult.ErrorMessage = "You haven't checked in at the venue to redeem this coupon.";
                    return result;
                }

                result.OperationResult = this.RedeemCoupon(connection, coupon, checkin);
                result.CheckedInVenueId = checkin.VenueId;
                return result;
            }
        }

        /// <summary>
        /// Saves the current settings and return all
        /// </summary>
        /// <param name="name">Settings item to be updated</param>
        /// <param name="value">Value for the settings item to be updated</param>
        /// <returns>All settings</returns>
        public UserSettings SaveSettings(string name, string value)
        {
            try
            {
                return _SaveSettings(name, value);
            }
            catch (Exception ex)
            {
                return LogExceptionAndGetErrorResult<UserSettings>(ex, "SaveSettings");
            }
        }

        private UserSettings _SaveSettings(string name, string value)
        {
            UserSettings result = new UserSettings();

            // get diner Id from content
            Guid dinerId = this.GetDinerIdFromContent();
            if (dinerId == Guid.Empty)
            {
                result.OperationResult.ErrorCode = AccessDenied;
                result.OperationResult.ErrorMessage = "Unknown user";
                return result;
            }

            if (!UserProfile.IsValidName(name))
            {
                result.OperationResult.ErrorCode = InvalidInput;
                result.OperationResult.ErrorMessage = "Invalid Settings Name: " + name;
                return result;
            }

            if (!UserProfile.IsValidValue(name, value))
            {
                result.OperationResult.ErrorCode = InvalidInput;
                result.OperationResult.ErrorMessage = "Invalid Settings Value for: " + name;
                return result;
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                OM.DinerSettings settings = null;
                if (name.Trim() == "0")
                {
                    settings = OM.DinerSettings.ReadFromDBase(connection, null, dinerId);
                }
                else
                {
                    OM.DinerSettingsItem item = new OM.DinerSettingsItem(dinerId, name, value);
                    settings = OM.DinerSettings.Save(connection, null, item);
                }

                result.Settings = new UserProfile(settings);
            }
                        
            return result;
        }

        private OperationResult CheckCouponToRedeem(SqlConnection connection, Guid couponId, Guid dinerId, out OM.Coupon coupon)
        {
            coupon = null;
            OperationResult result = new OperationResult();

            // check input
            if (couponId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Coupon Id may not be empty.";
                return result;
            }

            // read the coupon
            OM.Coupon tempCoupon = new OM.Coupon();
            tempCoupon.Id = couponId;
            if (!OM.Database.Select(tempCoupon, connection, null, OM.Database.TimeoutSecs))
            {
                result.ErrorCode = ObjectNotFound;
                result.ErrorMessage = "Coupon could not be found.";
                return result;
            }

            // check ownership
            if (dinerId != tempCoupon.ReceiverId)
            {
                result.ErrorCode = AccessDenied;
                result.ErrorMessage = "You don't have access to perform that operation.";
                return result;
            }

            // check to see if it is redeemed already
            if (tempCoupon.RedeemTimeUTC.HasValue)
            {
                result.ErrorCode = CouponHasAlreadyRedeemed;
                result.ErrorMessage = "Coupon has been already redeemed.";
                return result;
            }

            // check to see if it is expired
            if (tempCoupon.ExpiryDateUTC < DateTime.UtcNow)
            {
                result.ErrorCode = ExpiredCoupon;
                result.ErrorMessage = "Coupon may not be redeemed since it is expired.";
                return result;
            }

            coupon = tempCoupon;
            return result;
        }

        private OperationResult RedeemCoupon(SqlConnection connection, OM.Coupon coupon, OM.Checkin redeemCheckin)
        {
            OperationResult result = new OperationResult();

            string query = @"UPDATE [dbo].[Coupon]
                               SET [ReadTimeUTC] = '{0}',
                                   [RedeemCheckInId] = '{1}',
                                   [RedeemTimeUTC] = '{2}'
                             WHERE [Id] = '{3}'";

            query = OM.Database.ShortenQuery(query);

            DateTime utcNow = DateTime.UtcNow;
            DateTime readTimeUtc = coupon.ReadTimeUTC.HasValue ? coupon.ReadTimeUTC.Value : utcNow;
            string readTimeUtcAsText = Helpers.ToDBaseString(readTimeUtc);
            string redeemTimeAsText = Helpers.ToDBaseString(utcNow);

            query = String.Format(
                CultureInfo.InvariantCulture, 
                query,
                readTimeUtcAsText, 
                redeemCheckin.Id, 
                redeemTimeAsText,
                coupon.Id);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows == 0)
                {
                    result.ErrorCode = UnknownError;
                    result.ErrorMessage = "Unexpected error.";                    
                }
            }

            return result;
        }

        private string GenerateBase64EncodedString(string username, string password)
        {
            string plainText = "{0}:{1}";
            plainText = String.Format(CultureInfo.InvariantCulture, plainText, username, password);
            byte[] encodedBytes = new ASCIIEncoding().GetBytes(plainText);
            string encodedText = Convert.ToBase64String(encodedBytes);
            return encodedText;
        }

        private Guid GetDinerIdFromContent()
        {
            OM.Diner diner = this.GetDinerFromContent();
            if (diner != null)
            {
                return diner.Id;
            }

            return Guid.Empty;
        }

        private OM.Diner GetDinerFromContent()
        {  
            if (HttpContext.Current != null &&
                HttpContext.Current.User != null && 
                HttpContext.Current.User.Identity != null && 
                !String.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
            {
                OM.Diner diner = new OM.Diner(HttpContext.Current.User.Identity.Name);
                if (diner.Id != Guid.Empty && !String.IsNullOrWhiteSpace(diner.UserName))
                {
                    return diner;
                }
            }

            return null;
        }

        private VenueList SearchVenues(
            SqlConnection connection, 
            double latitude, 
            double longitude,
            bool isForRedeem)
        {
            OM.VenueList venues = new OM.VenueList();
            using (SqlCommand command = connection.CreateCommand())
            {
                double latitudeThreshold = isForRedeem ? Helpers.Default_RedeemCoupon_LatitudeThreshold : Helpers.Default_SearchVenue_LatitudeThreshold;
                double longitudeThreshold = isForRedeem ? Helpers.Default_RedeemCoupon_LongitudeThreshold : Helpers.Default_SearchVenue_LongitudeThreshold;

                command.CommandText = OM.Venue.SelectByGeoLocationQuery(
                    latitude, 
                    longitude,
                    latitudeThreshold,
                    longitudeThreshold,
                    isForRedeem);

                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            OM.Venue item = new OM.Venue();
                            item.InitFromSqlReader(reader);
                            venues.Add(item);
                        }
                    }
                }
            }

            VenueList nearByVenues = new VenueList();
            double defaultMaxMiles = isForRedeem ? Helpers.Default_RedeemCoupon_RangeLimitInMiles : Helpers.Default_SearchVenue_RangeLimitInMiles;
            
            foreach (OM.Venue venue in venues)
            {
                if (venue.Latitude.HasValue && venue.Longitude.HasValue)
                {
                    decimal? rangeLimit = isForRedeem ? venue.RangeLimitInMilesForRedeem : venue.RangeLimitInMilesForSearch;
                    double maxDistanceInMiles = rangeLimit.HasValue ? (double)rangeLimit.Value : defaultMaxMiles;

                    double distanceInMiles = OM.Venue.CalculateDistanceInMiles(
                        latitude, (double)venue.Latitude.Value, longitude, (double)venue.Longitude.Value);

                    if (distanceInMiles <= maxDistanceInMiles)
                    {
                        double distanceInYards = distanceInMiles * OM.Venue.MilesToYard;
                        Venue vx = new Venue(venue);
                        vx.DistanceInYards = distanceInYards;
                        if (distanceInYards > Helpers.ShowMilesInsteadOfYardsLimit)
                        {
                            double miles = OneYardAsMile * distanceInYards;
                            string milesText = miles.ToString("0.0");
                            vx.Distance = String.Format(CultureInfo.InvariantCulture, "{0} mi", milesText);
                            
                        }
                        else
                        {
                            vx.Distance = String.Format(CultureInfo.InvariantCulture, "{0} yd", (int)distanceInYards);
                        }

                        nearByVenues.Venues.Add(vx);
                    }
                }
            }

            nearByVenues.Venues.Sort(Venue.CompareByDistance);

            return nearByVenues;
        }

        private OM.Venue GetVenue(SqlConnection connection, Guid venueId)
        {
            OM.Venue venue = new OM.Venue();
            venue.Id = venueId;

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = venue.SelectQueryWithIdOnly();
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            venue.InitFromSqlReader(reader);
                            return venue;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the current check-in or it creates a new one if certain criteria is met:
        /// - Checks inputs
        /// - Checks venue
        /// - Checks if the user is already checked in
        /// - Checks if the user exceeds the maximum check-in count per day (last 24 hours)
        /// - Checks if the user exceeds the maximum check-in count per venue per day (last 24 hours)
        /// - Creates a new check-in if everything is fine
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="venuId"></param>
        /// <param name="checkin"></param>
        /// <returns></returns>
        private OperationResult Checkin(SqlConnection connection, Guid dinerId, Guid venueId, out OM.Checkin checkin, ref OM.Venue venue, out bool isNew)
        {           
            // output
            isNew = false;
            checkin = null;
            OperationResult result = new OperationResult();

            // check input
            if (venueId == Guid.Empty)
            {
                result.ErrorCode = EmptyObjectId;
                result.ErrorMessage = "Venu Id may not be empty.";
                return result;
            }

            // check the venue
            if (venue == null)
            {
                venue = this.GetVenue(connection, venueId);
                if (venue == null)
                {
                    result.ErrorCode = ObjectNotFound;
                    result.ErrorMessage = "Venue couldn't be found.";
                    return result;
                }
            }

            // get the latest check-ins in 24 hours
            OM.CheckinList latestCheckins = this.GetLatestCheckins(connection, dinerId);
            OM.CheckinList checkinsInCurrentVenue = latestCheckins.FilterBy(venueId);
            
            // is this a same check-in session?            
            if (checkinsInCurrentVenue.Count > 0)
            {
                TimeSpan diff = DateTime.UtcNow - checkinsInCurrentVenue[0].TimeUTC;
                if (diff.TotalMinutes <= Helpers.MaxMinutesPerCheckinSession)
                {
                    checkin = checkinsInCurrentVenue[0];
                    return result;
                }
            }

            // if we are here, that means we will create a new check-in object...
            // therefore we need to check the limits...

            // Check if the user exceeds the maximum check-in count per venue per day (last 24 hours)          
            if (checkinsInCurrentVenue.Count >= Helpers.MaxCheckinPerVenueInLast24Hours)
            {
                string errMsg = "You may not make more than {0} check-ins in a certain venue within last 24 hours.";
                errMsg = String.Format(CultureInfo.InvariantCulture, errMsg, Helpers.MaxCheckinPerVenueInLast24Hours);
                result.ErrorCode = MaxCheckinCountPerVenuePerDayReached;
                result.ErrorMessage = errMsg;
                return result;
            }

            // Check if the user exceeds the maximum check-in count per day (last 24 hours)
            if(latestCheckins.Count >= Helpers.MaxCheckinInLast24Hours)
            {
                string errMsg = "You may not make more than {0} check-ins in last 24 hours.";
                errMsg = String.Format(CultureInfo.InvariantCulture, errMsg, Helpers.MaxCheckinInLast24Hours);
                result.ErrorCode = MaxCheckinCountPerDayReached;
                result.ErrorMessage = errMsg;
                return result;
            }

            
            // this is a new check-in session... create it...
            checkin = new OM.Checkin();
            checkin.VenueId = venueId;
            checkin.DinerId = dinerId;

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = checkin.InsertQuery();
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;
                checkin.InsertParameters(command);

                command.ExecuteNonQuery();
            }

            // return
            isNew = true;
            return result;
        }

        private TimeZoneInfo GetTimeZoneInfo(OM.Venue venue)
        {
            if (venue.TimeZoneWinIndex.HasValue && venue.TimeZoneWinIndex.Value >= 0)
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                if (venue.TimeZoneWinIndex.Value < timeZones.Count)
                {
                    return timeZones[venue.TimeZoneWinIndex.Value];
                }
            }

            return Helpers.DefaultTimeZone;
        }

        private IdAndTextList GetServedMenus(SqlConnection connection, Guid venueId, int timeSlice)
        {
            string query = @"SELECT [VMP].[MenuId], [M].[Name]
                                  FROM [dbo].[VenueAndMenuMap] [VMP]
                                  JOIN [dbo].[Menu] [M] ON [VMP].[MenuId] = [M].[Id]
                                  WHERE [VMP].[Status] = {0} AND [VMP].[VenueId] = '{1}'
                                        AND [M].[ServiceStartTime] is NOT NULL AND [M].[ServiceStartTime] <= {2} 
                                        AND [M].[ServiceEndTime] is NOT NULL AND [M].[ServiceEndTime] >= {2} 
                                  ORDER BY [VMP].[OrderIndex] ASC;";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, (int)OM.Status.Active, venueId, timeSlice);
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                IdAndTextList menus = new IdAndTextList();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Guid menuId = reader.GetGuid(0);
                            string menuName = reader.GetString(1);
                            menus.Add(new IdAndText(menuId, menuName));
                        }
                    }
                }

                return menus;
            }
        }

        private ItemRateList<byte> GetLatestRatesOfDinerAt(SqlConnection connection, Guid venueId, Guid dinerId, Guid excludedCheckInId)
        {
            // might include multiple rates from a single diner for a certain plate
            string query = @"SELECT [MenuItemId], [Rate] FROM [dbo].[MenuItemRate]
	                            WHERE [Rate] <> 0 AND [CheckInId] IN
	                            (
		                            SELECT [Id] FROM [dbo].[CheckIn]
		                            WHERE [VenueId] = '{0}' AND [DinerId] = '{1}' AND [Id] <> '{2}'
	                            )
	                            ORDER BY [LastUpdateTimeUTC] DESC;";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, venueId, dinerId, excludedCheckInId);

            ItemRateList<byte> menuItemRates = new ItemRateList<byte>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Guid menuItemId = reader.GetGuid(0);
                            byte rate = reader.GetByte(1);
                            if (menuItemRates[menuItemId] == null)
                            {
                                menuItemRates.Add(new ItemRate<byte>(menuItemId, rate));
                            }
                        }
                    }
                }
            }

            return menuItemRates;
        }

        private ItemRateList<decimal> GetAverageMenuItemRatesAt(SqlConnection connection, Guid venueId, Guid excludedCheckInId)
        {
            string query = @"SELECT [MIR].[MenuItemId], AVG([MIR].[Rate] / 1.0) AS [AvgMenuItemRate]
                                FROM [dbo].[MenuItemRate] [MIR]
                                JOIN [dbo].[CheckIn] [CI] ON [CI].[VenueId] = '{0}' AND [CI].[Id] <> '{1}' AND [MIR].[CheckInId] = [CI].[Id]
                                WHERE [MIR].[Rate] <> 0 AND [MIR].[CheckInId] <> '{1}'
                                GROUP BY [MIR].[MenuItemId] 
                                ORDER BY [AvgMenuItemRate] DESC;";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, venueId, excludedCheckInId);

            ItemRateList<decimal> menuItemRates = new ItemRateList<decimal>();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Guid menuItemId = reader.GetGuid(0);
                            decimal rate = reader.GetDecimal(1);
                            menuItemRates.Add(new ItemRate<decimal>(menuItemId, rate));
                        }
                    }
                }
            }
            
            return menuItemRates;
        }

        private Menu AttachRatesToMenu(OM.MenuCategoryList categories, ItemRateList<byte> ratesByCurrentDiner, ItemRateList<decimal> orderedAvgRates)
        {
            // prepare output
            Menu menu = new Menu();
            string imageFileUrlTemplate = Helpers.ImageFileUrlTemplate;
            decimal minimumRateToRecommend = Helpers.MinimumRateToRecommend;
            int minItemCountForTopRate2 = Helpers.MinItemCountForTopRate2;
            int minItemCountForTopRate3 = Helpers.MinItemCountForTopRate3;
            foreach (OM.MenuCategory category in categories)
            {
                MenuCategory cat = new MenuCategory();
                cat.Id = category.Id;
                cat.Name = category.Name;
                menu.MenuCategories.Add(cat);
                foreach (OM.MenuItem menuItem in category.Items)
                {
                    MenuItem item = new MenuItem(menuItem, imageFileUrlTemplate);
                    cat.MenuItems.Add(item);

                    ItemRate<byte> selfRate = ratesByCurrentDiner[menuItem.Id];
                    if (selfRate != null)
                    {
                        item.SelfRating = selfRate.Rate;
                    }

                    ItemRate<decimal> avgRate = orderedAvgRates[menuItem.Id];
                    if (avgRate != null)
                    {
                        item.AverageRating = avgRate.Rate;
                    }
                }

                // mark top rated ones...
                int topRatedCount = 1;
                if (cat.MenuItems.Count >= minItemCountForTopRate2)
                {
                    topRatedCount++;
                }
                if (cat.MenuItems.Count >= minItemCountForTopRate3)
                {
                    topRatedCount++;
                }

                int markCount = 0;
                List<MenuItem> topDishes = new List<MenuItem>();
                for (int x = 0; x < orderedAvgRates.Count && markCount < topRatedCount; x++)
                {
                    if (orderedAvgRates[x].Rate < minimumRateToRecommend)
                    {
                        break;
                    }

                    MenuItem mi = cat[orderedAvgRates[x].ItemId];
                    if (mi != null)
                    {
                        mi.IsMostLikedItem = true;
                        topDishes.Add(mi);

                        if (++markCount == topRatedCount)
                        {
                            break;
                        }
                    }
                }

                topDishes.Sort(MenuItem.CompareByAverageRating);
                while (topDishes.Count > 0)
                {
                    MenuItem item = topDishes[0];
                    topDishes.RemoveAt(0);
                    cat.MenuItems.Remove(item);
                    cat.MenuItems.Insert(0, item);
                }
            }

            return menu;
        }

        private OM.CheckinList GetLatestCheckins(SqlConnection connection, Guid dinerId)
        {
            OM.CheckinList checkins = new OM.CheckinList();
            string query = OM.Checkin.LastCheckinsQuery(dinerId, Helpers.MaxCheckinInLast24Hours, DateTime.UtcNow.AddDays(-1));

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            OM.Checkin checkin = new OM.Checkin();
                            checkin.InitFromSqlReader(reader);
                            checkins.Add(checkin);
                        }
                    }
                }
            }

            return checkins;
        }

        private OM.Checkin GetCurrentCheckinSession(SqlConnection connection, Guid dinerId, Guid venueId)
        {
            OM.CheckinList latestCheckins = this.GetLatestCheckins(connection, dinerId);
            OM.CheckinList checkinsInCurrentVenue = latestCheckins.FilterBy(venueId);
  
            if (checkinsInCurrentVenue.Count > 0)
            {
                OM.Checkin last = checkinsInCurrentVenue[0];
                TimeSpan diff = DateTime.UtcNow - last.TimeUTC;
                if (diff.TotalMinutes <= Helpers.MaxMinutesPerCheckinSession)
                {  
                    return last;
                }
            }
    
            return null;
        }

        private string GetMessageByDinerIdQuery(int topX, Guid dinerId, DateTime pageCutoffTimeUTC)
        {
            string query = @"SELECT TOP {0}
	                               [M].[Id]
                                  ,[C].[Name]
                                  ,[M].[Title]
                                  ,[M].[Content]
                                  ,[M].[QueueTimeUTC]
                                  ,[M].[ReadTimeUTC]
                              FROM [dbo].[Message] [M]
                              JOIN [dbo].[Chain] [C] ON [C].[Id] = [M].[ChainId]
                              WHERE [M].[ReceiverId] = '{1}' AND
                                    [M].[QueueTimeUTC] < '{2}' AND
                                    [M].[DeleteTimeUTC] is NULL
                              ORDER BY [M].[QueueTimeUTC] DESC;";

            string cutoffTime = Helpers.ToDBaseString(pageCutoffTimeUTC);

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, topX, dinerId, cutoffTime);
            return query;
        }

        private MessageList GetMessagesForDiner(SqlConnection connection, Guid dinerId, DateTime pageCutoffTimeUTC)
        {
            int maxMessageForDiner = Helpers.MaxMessageForDiner;
            string query = this.GetMessageByDinerIdQuery(maxMessageForDiner, dinerId, pageCutoffTimeUTC);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                MessageList messages = new MessageList();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int colIndex = 0;

                            MessageToDiner message = new MessageToDiner();
                            message.Id = reader.GetGuid(colIndex++);
                            message.Sender = reader.GetString(colIndex++);
                            message.Title = reader.GetString(colIndex++);
                            message.Content = reader.GetString(colIndex++);
                            message.SentTimeUTC = reader.GetDateTime(colIndex++);
                            message.IsRead = !reader.IsDBNull(colIndex++);
                            
                            /*
                            if (!String.IsNullOrWhiteSpace(message.Content))
                            {
                                message.Content = message.Content.Replace("\n", "<br/>");
                            }
                            */

                            messages.Messages.Add(message);
                        }
                    }
                }

                int count = messages.Messages.Count;
                if (count == maxMessageForDiner)
                {
                    messages.HasMoreMessageOnServer = true;
                    messages.HintForNextPage = messages.Messages[count - 1].SentTimeUTC.Ticks.ToString();
                }

                return messages;
            }
        }

        private string GetCouponsByDinerIdQuery(int topX, Guid dinerId, DateTime pageCutoffTimeUTC)
        {
            string query = @" SELECT TOP {0}
                                     [CP].[Id]      
                                    ,[CP].[ChainId]
                                    ,[CH].[Name]      
                                    ,[CP].[Title]
                                    ,[CP].[Content]
                                    ,[CP].[QueueTimeUTC]
                                    ,[CP].[ReadTimeUTC]
                                FROM [dbo].[Coupon] [CP]
                                JOIN [dbo].[Chain] [CH] ON [CH].[Id] = [CP].[ChainId]
                                WHERE [CP].[RedeemTimeUTC] is NULL AND 
		                            [CP].[ExpiryDateUTC] > GETUTCDATE() AND 
		                            [CP].[DeleteTimeUTC] is NULL AND
		                            [CP].[ReceiverId] = '{1}' AND
                                    [CP].[QueueTimeUTC] < '{2}'
                                ORDER BY [CP].[QueueTimeUTC] DESC;";

            string cutoffTime = Helpers.ToDBaseString(pageCutoffTimeUTC);

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, topX, dinerId, cutoffTime);

            return query;
        }

        private CouponList GetCouponsForDiner(SqlConnection connection, Guid dinerId, DateTime pageCutoffTimeUTC)
        {
            int maxCouponForDiner = Helpers.MaxCouponForDiner;
            string query = this.GetCouponsByDinerIdQuery(maxCouponForDiner, dinerId, pageCutoffTimeUTC);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                CouponList coupons = new CouponList();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int colIndex = 0;

                            Coupon coupon = new Coupon();
                            coupon.Id = reader.GetGuid(colIndex++);
                            coupon.SenderChainId = reader.GetGuid(colIndex++);
                            coupon.Sender = reader.GetString(colIndex++);
                            coupon.Title = reader.GetString(colIndex++);

                            if (!reader.IsDBNull(colIndex))
                            {
                                coupon.Content = reader.GetString(colIndex);
                                
                                /*
                                if (!String.IsNullOrWhiteSpace(coupon.Content))
                                {
                                    coupon.Content = coupon.Content.Replace("\n", "<br/>");
                                }
                                */
                            }
                            colIndex++;

                            coupon.SentTimeUTC = reader.GetDateTime(colIndex++);
                            coupon.IsRead = !reader.IsDBNull(colIndex++);

                            coupons.Coupons.Add(coupon);
                        }
                    }
                }

                int count = coupons.Coupons.Count;
                if (count == maxCouponForDiner)
                {
                    coupons.HasMoreCouponOnServer = true;
                    coupons.HintForNextPage = coupons.Coupons[count - 1].SentTimeUTC.Ticks.ToString();
                }

                return coupons;
            }
        }

        private DateTime ConvertPageHintToDateTime(string pageHintAsText)
        {
            // adjust the page hint
            if (String.IsNullOrWhiteSpace(pageHintAsText))
            {
                pageHintAsText = "0";
            }

            pageHintAsText = pageHintAsText.Trim();

            long pageHint = 0;
            long.TryParse(pageHintAsText, out pageHint);
                        
            long max = DateTime.UtcNow.AddDays(3).Ticks;
            if (pageHint <= 1 || pageHint > max)
            {
                pageHint = max;
            }

            return new DateTime(pageHint, DateTimeKind.Utc);
        }

        private string GetQueryForAllMenuItemIdsOfAVenue(Guid groupId, Guid venueId)
        {
            string query = @"SELECT DISTINCT [Map1].[MenuItemId]
                                FROM [dbo].[MenuCategoryAndMenuItemMap] [Map1]
                                JOIN [dbo].[MenuAndMenuCategoryMap] [Map2] 
		                            ON [Map1].[MenuCategoryId] = [Map2].[MenuCategoryId] 
		                            AND [Map2].[GroupId] = '{0}'
                                JOIN [dbo].[VenueAndMenuMap] [Map3] 
		                            ON [Map2].[MenuId] = [Map3].[MenuId] 
		                            AND [Map3].[GroupId] = '{0}' AND [VenueId] = '{1}'
                                WHERE [Map1].[GroupId] = '{0}'
                                ORDER BY [Map1].[MenuItemId];";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, groupId, venueId);
            return query;
        }

        private List<Guid> GetAllMenuItemIdsOfAVenue(SqlConnection connection, Guid groupId, Guid venueId)
        {
            string query = this.GetQueryForAllMenuItemIdsOfAVenue(groupId, venueId);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                List<Guid> menuIds = new List<Guid>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Guid menuId = reader.GetGuid(0);
                            menuIds.Add(menuId);
                        }
                    }
                }

                return menuIds;
            }
        }

        private List<FavoriteMenuItem> GetFavoriteMenuItemsForDiner(SqlConnection connection, Guid dinerId, decimal minAvgRate, int maxElements)
        {
            string query = @"SELECT	TOP({0}) 
		                            [VN2].[Name] AS [VenueName], 
		                            [MI2].[Name] AS [MenuItemName], 
		                            [Result].[MenuItemAvgRate],
		                            [MII].[MenuItemId] AS [MenuItemImageId] 
	                            FROM (
		                            SELECT [VN].[Id] AS [VenueId]
				                            ,[MI].[Id] AS [MenuItemId]
				                            ,(ROUND(AVG([PR].[Rate] / 0.5), 0) / 2.0) AS [MenuItemAvgRate]
			                            FROM [dbo].[CheckIn] [CH]
			                            JOIN [dbo].[Venue] [VN] ON [CH].[VenueId] = [VN].[Id]
			                            JOIN [dbo].[MenuItemRate] [PR] ON [CH].[Id] = [PR].[CheckInId] AND [PR].[Rate] > 0
			                            JOIN [dbo].[MenuItem] [MI] ON [MI].[Id] = [PR].[MenuItemId]
			                            WHERE [CH].[DinerId] = '{1}'  
			                            GROUP BY [VN].[Id], [MI].[Id]) AS [Result]
	                            JOIN [dbo].[Venue] [VN2] ON [Result].[VenueId] = [VN2].[Id]
	                            JOIN [dbo].[MenuItem] [MI2] ON [Result].[MenuItemId] = [MI2].[Id]
	                            LEFT JOIN [dbo].[MenuItemImage] [MII] ON [MII].[MenuItemId] = [MI2].[Id]
	                            WHERE [MenuItemAvgRate] >= {2}
	                            ORDER BY [MenuItemAvgRate] DESC;";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, maxElements, dinerId, minAvgRate);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                List<FavoriteMenuItem> favorites = new List<FavoriteMenuItem>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int colIndex = 0;

                            FavoriteMenuItem favItem = new FavoriteMenuItem();
                            favItem.VenueName = reader.GetString(colIndex++);
                            favItem.MenuItemName = reader.GetString(colIndex++);
                            favItem.MenuItemAverageRate = reader.GetDecimal(colIndex++);

                            if (!reader.IsDBNull(colIndex))
                            {
                                favItem.itemImageId = reader.GetGuid(colIndex);
                            }
                            colIndex++;

                            favorites.Add(favItem);
                        }
                    }
                }

                return favorites;
            }
        }

        private List<FavoriteVenue> GetFavoriteVenuesForDiner(SqlConnection connection, Guid dinerId, decimal minAvgRate, int maxElements)
        {
            string query = @"SELECT [V].[Name], [AvgRate], [LI].[ChainId] AS [LogoImageId] FROM (
	                             SELECT TOP({0})
		                            [VenueId], 
		                            ROUND((((SUM([RateSum]) / 1.0) / (SUM([RateCount]) / 1.0)) * 2.0), 0) / 2.0 AS [AvgRate]
		                            FROM (
			                            SELECT [VN].[Id] AS [VenueId]
					                              ,SUM([PR].[Rate]) AS [RateSum]
					                              ,COUNT([PR].[Rate]) AS [RateCount]		  
				                              FROM [dbo].[CheckIn] [CH]
				                              JOIN [dbo].[Venue] [VN] ON [CH].[VenueId] = [VN].[Id]
				                              JOIN [dbo].[MenuItemRate] [PR] ON [CH].[Id] = [PR].[CheckInId] AND [PR].[Rate] > 0
				                              WHERE [CH].[DinerId] = '{1}'
				                              GROUP BY [VN].[Id]
			                            UNION
			                            SELECT [VN].[Id] AS [VenueId]
					                              ,SUM([AR].[AnswerRate]) AS [RateSum]
					                              ,COUNT([AR].[AnswerRate]) AS [RateCount]		  
				                              FROM [dbo].[CheckIn] [CH]
				                              JOIN [dbo].[Venue] [VN] ON [CH].[VenueId] = [VN].[Id]
				                              JOIN [dbo].[Answer] [AR] ON [CH].[Id] = [AR].[CheckInId] AND [AR].[AnswerRate] > 0	  
				                              WHERE [CH].[DinerId] = '{1}'
				                              GROUP BY [VN].[Id]
			                            ) AS [UnionResult]	
		                            GROUP BY [VenueId]		
		                            ORDER BY [AvgRate] DESC
	                            ) AS [Result]
	                              JOIN [dbo].[Venue] [V] ON [V].[Id] = [Result].[VenueId]
	                              LEFT JOIN [dbo].[LogoImage] [LI] ON [LI].[ChainId] = [V].[ChainId]
                            WHERE [AvgRate] >= {2};";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, maxElements, dinerId, minAvgRate);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                List<FavoriteVenue> favorites = new List<FavoriteVenue>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int colIndex = 0;

                            FavoriteVenue favItem = new FavoriteVenue();
                            favItem.VenueName = reader.GetString(colIndex++);
                            favItem.VenueAverageRate = reader.GetDecimal(colIndex++);

                            if (!reader.IsDBNull(colIndex))
                            {
                                favItem.logoImageId = reader.GetGuid(colIndex);
                            }
                            colIndex++;

                            favorites.Add(favItem);
                        }
                    }
                }

                return favorites;
            }
        }

        private List<HistoryItem> GetHistoryForDiner(SqlConnection connection, Guid dinerId, int maxElements)
        {
            string query = @"SELECT TOP({0})
	                               [CH].[Id] AS [CheckinId] 
	                              ,[VN].[Name] AS [VenueName]
	                              ,[CH].[TimeUTC] AS [CheckinTimeUTC]
                                  ,[LI].[ChainId] AS [LogoImageId]
                                  ,[MI].[Name] AS [MenuItemName]
                                  ,[PR].[Rate] AS [MenuItemRate]
                                  ,[MII].[MenuItemId] AS [MenuItemImageId]
                              FROM [dbo].[CheckIn] [CH]
                              JOIN [dbo].[Venue] [VN] ON [CH].[VenueId] = [VN].[Id]
                              LEFT JOIN [dbo].[MenuItemRate] [PR] ON [CH].[Id] = [PR].[CheckInId] AND [PR].[Rate] > 0
                              LEFT JOIN [dbo].[MenuItem] [MI] ON [MI].[Id] = [PR].[MenuItemId]
                              LEFT JOIN [dbo].[LogoImage] [LI] ON [LI].[ChainId] = [VN].[ChainId]
                              LEFT JOIN [dbo].[MenuItemImage] [MII] ON [MII].[MenuItemId] = [MI].[Id]
                              WHERE [CH].[DinerId] = '{1}'
                              ORDER BY [CH].[TimeUTC] DESC;";

            query = OM.Database.ShortenQuery(query);
            query = String.Format(CultureInfo.InvariantCulture, query, maxElements, dinerId);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                Guid lastCheckin = Guid.Empty;
                List<HistoryItem> checkins = new List<HistoryItem>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Guid checkinId = reader.GetGuid(0);
                            if (checkinId != lastCheckin)
                            {
                                HistoryItem historyItem = new HistoryItem();
                                historyItem.VenueName = reader.GetString(1);
                                historyItem.CheckinTimeUTC = reader.GetDateTime(2);

                                if (!reader.IsDBNull(3))
                                {
                                    historyItem.logoImageId = reader.GetGuid(3);
                                }

                                checkins.Add(historyItem);
                                lastCheckin = checkinId;
                            }

                            if (!reader.IsDBNull(4) && !reader.IsDBNull(5))
                            {
                                RatedItem ratedItem = new RatedItem();
                                ratedItem.MenuItemName = reader.GetString(4);
                                ratedItem.MenuItemRate = reader.GetByte(5);

                                if (!reader.IsDBNull(6))
                                {
                                    ratedItem.itemImageId = reader.GetGuid(6);
                                }

                                checkins[checkins.Count - 1].RatedItems.Add(ratedItem);
                            }
                        }
                    }
                }

                return checkins;
            }
        }

        private OM.MenuItemRateList GetLatestMenuItemRates(SqlConnection connection, Guid checkInId)
        {
            string query = OM.MenuItemRate.SelectAllWithoutNameQuery(checkInId);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Transaction = null;
                command.CommandTimeout = OM.Database.TimeoutSecs;

                OM.MenuItemRateList list = new OM.MenuItemRateList();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            OM.MenuItemRate menuItemRate = new OM.MenuItemRate();
                            menuItemRate.InitFromSqlReader(reader);
                            list.Add(menuItemRate);
                        }
                    }
                }

                return list;
            }
        }

        private T LogExceptionAndGetErrorResult<T>(Exception ex, string methodName) where T : BaseResponse, new()
        {
            try
            {
                OM.Diner diner = this.GetDinerFromContent();
                Guid dinerId = (diner == null) ? Guid.Empty : diner.Id;
                Logger.LogException(ex, methodName, dinerId);
            }
            catch { }

            T result = new T();
            result.OperationResult.ErrorCode = UnknownError;
            result.OperationResult.ErrorMessage = "Unexpected Exception";
            return result;
        }

        private OperationResult LogExceptionAndGetErrorResult(Exception ex, string methodName)
        {
            try
            {
                OM.Diner diner = this.GetDinerFromContent();
                Guid dinerId = (diner == null) ? Guid.Empty : diner.Id;
                Logger.LogException(ex, methodName, dinerId);
            }
            catch { }

            OperationResult result = new OperationResult();
            result.ErrorCode = UnknownError;
            result.ErrorMessage = "Unexpected Exception";
            return result;
        }        

        private class IdAndTextList : List<IdAndText> 
        {
            public IdAndText this[Guid id]
            {
                get
                {
                    foreach (IdAndText item in this)
                    {
                        if (item.Id == id)
                        {
                            return item;
                        }
                    }

                    return null;
                }
            }
        }

        private class ItemRate<T>
        {
            public Guid ItemId { get; set; }
            public T Rate { get; set; }

            public ItemRate(Guid id, T rate)
            {
                this.ItemId = id;
                this.Rate = rate;
            }

            public override string ToString()
            {
                return this.Rate.ToString();
            }
        }

        private class ItemRateList<T> : List<ItemRate<T>>
        {
            public ItemRate<T> this[Guid id]
            {
                get
                {
                    foreach (ItemRate<T> item in this)
                    {
                        if (item.ItemId == id)
                        {
                            return item;
                        }
                    }

                    return null;
                }
            }
        }

        /*
        public SingleObjectTest TestSingleObject(SingleObjectTest data)
        {
            return data;
        }

        public MultiObjectTest TestMultiObject(MultiObjectTest data)
        {
            return data;
        }
        */
    }

    /*
    [System.Runtime.Serialization.DataContract()]
    public class SingleObjectTest
    {
        [System.Runtime.Serialization.DataMember()]
        public string Name { get; set; }

        [System.Runtime.Serialization.DataMember()]
        public bool IsMarried { get; set; }

        [System.Runtime.Serialization.DataMember()]
        public int NumberOfChildren { get; set; }

        [System.Runtime.Serialization.DataMember()]
        public DateTime BirthDateUTC { get; set; }
    }

    [System.Runtime.Serialization.DataContract()]
    public class MultiObjectTest
    {
        [System.Runtime.Serialization.DataMember()]
        public List<SingleObjectTest> Items { get { return this.items; } }
        private List<SingleObjectTest> items = new List<SingleObjectTest>();
    }
    */
}
