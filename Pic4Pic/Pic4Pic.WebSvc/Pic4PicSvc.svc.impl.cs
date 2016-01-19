using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;

using Pic4Pic.ObjectModel;
using System.Text;
using System.Globalization;

namespace Pic4Pic.WebSvc
{
    public partial class Pic4PicSvc : ServiceBase, IPic4PicSvc
    {
        #region SERVICE IMPLs : Check-User-Name, Sign-Up, Sign-in, Verify-Bio, Download FB Friends

        private static String[] reservedKeys = new String[]{
                "pic4pic", 
                "picforpic", 
                "appsicle"
        };

        protected bool IsReservedUsername(String username) 
        {
            username = username.Trim().ToLower();
            foreach (String reservedKey in reservedKeys)
            {
                if (username.IndexOf(reservedKey) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        protected BaseResponse _Ping(out UserAuthInfo auth)
        {
            auth = null;

            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            return new BaseResponse();
        }

        /**
         * ErrorCode != 0 : Username is already in use
         * ErrorCode == 0 & AuthToken == null => Username is available
         * ErrorCode == 0 & AuthToken != null => User is already signed up
         */
        protected UserResponse _CheckUsername(UserCredentials request, out UserAuthInfo auth)
        {
            auth = null;

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            if (this.IsReservedUsername(request.Username))
            {
                throw new Pic4PicAuthException("Username is not allowed");
            }

            Config config = new Config();
            config.Init();
            
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                bool usernameExists = false;
                User user = User.ReadFromDBase(conn, null, request.Username, request.Password, out usernameExists);
                if (user == null)
                {
                    if (usernameExists)
                    {
                        // this username is already in use
                        // reply with "no authToken" but with an "error"
                        throw new Pic4PicAuthException("Username is already in use");
                    }
                    else
                    {
                        // username is available
                        // reply with "no error code" & "no authToken"
                        UserResponse response = new UserResponse();
                        int randomSplitId = UserResponse.GetRandomSplitId();
                        response.AttachSettings(randomSplitId);

                        // save temp split id
                        try
                        {
                            User.SaveTemporarySplitId(conn, null, request.ClientId, randomSplitId);
                        }
                        catch (Exception ex)
                        {
                            Logger.bag(true)
                                .Add("action", "SaveTemporarySplitId")
                                .Add(ex)
                                .LogError();
                        }                        
                        
                        // return
                        return response;
                    }
                }
                else
                {
                    // user has been signed up already
                    // reply with "no error code" but with a "valid authToken"
                    try
                    {
                        return this.ReadUserProfile(conn, null, config, user, out auth);
                    }
                    catch (Exception ex)
                    {
                        // we want to throw a different exception as the message needs to worded differently
                        throw new Pic4PicAuthException("You are signed up already but you don't have a profile. Please contact to [contact@pic4pic.net]", ex);
                    }
                }
            }
        }

        protected UserResponse _Signin(UserCredentials request, out UserAuthInfo auth)
        {
            auth = null;

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate(true);

            Config config = new Config();
            config.Init();

            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();
                
                // get user
                User user = User.ReadFromDBase(conn, null, request.Username, request.Password);
                if (user == null)
                {
                    throw new Pic4PicAuthException("Access denied");
                }
                           
                // user is found... read other properties and return
                return this.ReadUserProfile(conn, null, config, user, out auth);                
            }
        }

        #region VERIFY BIO

        protected UserResponse _VerifyBio(VerifyBioRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // read properties from facebook one more time
            // get user from facebook
            WorkHistory workHistory = null;
            EducationHistory eduHistory = null;
            FacebookUser fbUser = FacebookHelpers.GetUserFromFacebook(request.FacebookAccessToken, request.FacebookUserId, out workHistory, out eduHistory);

            // handle database
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // read old facebook record
                FacebookUser oldFaceUser = FacebookUser.ReadFromDBase(conn, null, auth.UserId);

                // check integrity
                if (fbUser.FacebookId != oldFaceUser.FacebookId)
                {
                    throw new Pic4PicAuthException("Integrity Exception: Check that you own this facebook account.");
                }

                // merge facebook properties
                fbUser.UserId = auth.UserId;
                fbUser.CreateTimeUTC = oldFaceUser.CreateTimeUTC;
                bool updateNeeded = fbUser.Merge(oldFaceUser);

                // update facebook user & work history & education history
                if (updateNeeded)
                {
                    // start a transaction
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        // below method performs + (commits | [rolls back + throw exc] )
                        Database.PerformAndCommit_or_RollbackAndThrow(trans, () => {                            
                            // save facebook user
                            this.UpdateFacebook(conn, trans, fbUser, workHistory, eduHistory);

                            // update preferences as well since Gender might have been changed.
                            if (fbUser.Gender != Gender.Unknown)
                            {
                                // prepare
                                UserPreferences pref = new UserPreferences();
                                pref.UserId = fbUser.UserId;
                                pref.InterestedIn = fbUser.Gender == Gender.Male ? Gender.Female : Gender.Male;

                                // update
                                pref.UpdateOnDBase(conn, trans);
                            }
                        });
                    }
                }

                // verify bio fields
                string[] fields = request.UserFieldsAsArray;
                foreach (string field in fields)
                {
                    this.VerifyBioField(fbUser, field);
                }

                // read user
                User user = User.ReadFromDBaseById(conn, null, auth.UserId);
                fbUser.IsActive = user.UserStatus == UserStatus.Active;

                // prepare response                
                return ReadUserProfile(conn, null, config, fbUser, auth);
            }
        }

        private void VerifyBioField(FacebookUser fbUser, string fieldName)
        {
            NameValuePair<string>[] all = new NameValuePair<string>[] 
            {
                new NameValuePair<string>(FacebookUser.FB_FIELD_EMAIL, fbUser.EmailAddress),
                new NameValuePair<string>(FacebookUser.FB_FIELD_HOMETOWN, fbUser.HometownCity),
                new NameValuePair<string>(FacebookUser.FB_FIELD_BIRTHDAY, fbUser.BirthDay == default(DateTime) ? null : fbUser.BirthDay.ToString()),
                new NameValuePair<string>(FacebookUser.FB_FIELD_WORK, fbUser.Profession),
                new NameValuePair<string>(FacebookUser.FB_FIELD_EDUCATION, fbUser.EducationLevel == EducationLevel.Unknown ? null : fbUser.EducationLevel.ToString()),
                new NameValuePair<string>(FacebookUser.FB_FIELD_RELATIONSHIP, fbUser.MaritalStatus == MaritalStatus.Unknown ? null : fbUser.MaritalStatus.ToString()),
            };

            foreach (NameValuePair<string> pair in all)
            {
                if (String.Compare(fieldName, pair.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    if (String.IsNullOrWhiteSpace(pair.Value))
                    {
                        throw new Pic4PicException("Your " + fieldName + " data couldn't be verified via Facebook");
                    }

                    break;
                }
            }
        }

        #endregion

        #region SIGN-UP

        protected UserResponse _Signup(SignupRequest request, out UserAuthInfo auth)
        {
            auth = null;

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            if (this.IsReservedUsername(request.Username))
            {
                throw new Pic4PicAuthException("Username is not allowed");
            }

            // get config
            // no need to catch
            Config config = new Config();
            config.Init();

            // decrypt photo upload reference
            List<Guid> imageIds = this.DecryptImageReferences(config, request.PhotoUploadReference, 4);
            
            // get user from facebook
            WorkHistory workHistory = null;
            EducationHistory eduHistory = null;
            FacebookUser fbUser = FacebookHelpers.GetUserFromFacebook(request.FacebookAccessToken, request.FacebookUserId, out workHistory, out eduHistory);
            
            // create user in memory
            User user = new User();
            user.Id = Guid.NewGuid();
            user.Username = request.Username;
            user.Password = request.Password;
            user.UserType = UserType.Regular;
            user.UserStatus = UserStatus.Partial;
            user.SplitId = 0;
            
            // update facebookUser object
            fbUser.UserId = user.Id;
            
            // handle database
            List<ImageFile> userPictures = null;
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // get split id
                try
                {
                    user.SplitId = User.ReadTemporarySplitId(conn, null, request.ClientId);
                }
                catch (Exception ex)
                {
                    Logger.bag(true)
                        .Add("action", "ReadTemporarySplitId")
                        .Add("userid", user.Id.ToString())
                        .Add("split", user.SplitId.ToString())
                        .Add(ex)
                        .LogError();
                }                

                // check username
                // this.VerifyUserNotExist(conn, null, request.Username, request.Password);
                bool usernameExists = false;
                User existingUser = User.ReadFromDBase(conn, null, request.Username, request.Password, out usernameExists);
                if (existingUser == null)
                {
                    if (usernameExists)
                    {
                        // this username is already in use
                        // reply with "no authToken" but with an "error"
                        throw new Pic4PicAuthException("Username is already in use");
                    }
                    else
                    {
                        // username is available... continue with signup
                        #region REGULAR SIGNUP

                        // make sure that current facebook hasn't been used for another user account
                        FacebookUser existingFaceUser = FacebookUser.ReadFromDBase(conn, null, fbUser.FacebookId);

                        // Check Facebook integrity
                        if (existingFaceUser != null)
                        {
                            throw new Pic4PicAuthException("Your facebook has been associated to an account already. Multiple accounts are not allowed.");
                        }

                        // create User Preference
                        UserPreferences pref = new UserPreferences();
                        pref.UserId = user.Id;
                        if (fbUser.Gender != Gender.Unknown)
                        {
                            pref.InterestedIn = fbUser.Gender == Gender.Male ? Gender.Female : Gender.Male;
                        }

                        // start a transaction
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // below method performs + (commits | [rolls back + throw exc] )
                            Database.PerformAndCommit_or_RollbackAndThrow(trans, () =>
                            {
                                this.CreateAllEntities(conn, trans, user, fbUser, workHistory, eduHistory, pref, imageIds);
                            });
                        }
                        #endregion
                    }
                }
                else
                {
                    #region SIGNUP is called TWICE! Maybe with different pictures and Facebook profile!

                    // user has signed up already... let's not fail but update the properties...
                    // ...
                    // check to see if the user is an active one
                    user = existingUser;
                    if (user.UserStatus > UserStatus.Active) // i.e. suspended || disabled || deleted
                    {
                        throw new Pic4PicAuthException("Access Denied");
                    }

                    // user has been created already. We need to update other properties.
                    // NOTE that, if there is a user, then there must be a FacebookUser, too.
                    // This is because our previous Signup was executed in a transaction. 
                    // We are assuming that this is a re-post by the user for some reason; e.g. timeout

                    // Read old facebook record. There must be one because of atomic transaction
                    FacebookUser oldFaceUser = FacebookUser.ReadFromDBase(conn, null, user.Id);
                    
                    // Check Facebook integrity
                    if (oldFaceUser == null || (fbUser.FacebookId != oldFaceUser.FacebookId))
                    {
                        throw new Pic4PicAuthException("Integrity Exception: Check that you haven't linked your another Facebook account to this app before.");
                    }

                    // Load profile picture details (getting actual data). They must be there because of atomic transaction.
                    userPictures = ImageFile.ReadAllFromDBaseByUserId(conn, null, user.Id);
                    if (userPictures == null || userPictures.Count <= 0)
                    {
                        throw new Pic4PicAuthException("Integrity Exception: Profile images (1).");
                    }

                    // check profile images -2-
                    UserProfilePics previousProfilePics = UserProfilePics.From(userPictures);
                    if (!previousProfilePics.HasAll())
                    {
                        // user should have had at least profile images because of the atomic transaction
                        throw new Pic4PicAuthException("Integrity Exception: Profile images (2).");
                    }

                    // check profile images -3-
                    bool sameImages = previousProfilePics.HasAll(imageIds);
                    if (!sameImages && previousProfilePics.HasAny(imageIds))
                    {
                        // profile images (4 image in different variations) is a consistent quadruple. 
                        throw new Pic4PicAuthException("Integrity Exception: Profile images (3).");
                    }
                    
                    // merge facebook properties
                    fbUser.UserId = user.Id;
                    fbUser.CreateTimeUTC = oldFaceUser.CreateTimeUTC;
                    fbUser.Description = oldFaceUser.Description;
                    fbUser.IsActive = user.UserStatus == UserStatus.Active;
                    bool facebookUpdateNeeded = fbUser.Merge(oldFaceUser);
                    bool imageReplaceNeeded = !sameImages;
                    
                    // update facebook user & work history & education history
                    if (facebookUpdateNeeded || imageReplaceNeeded)
                    {
                        // start a transaction
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            // below method performs + (commits | [rolls back + throw exc] )
                            Database.PerformAndCommit_or_RollbackAndThrow(trans, () =>
                            {
                                if (facebookUpdateNeeded)
                                {
                                    // save facebook user
                                    this.UpdateFacebook(conn, trans, fbUser, workHistory, eduHistory);

                                    // update preferences as well since Gender might have been changed.
                                    if (fbUser.Gender != Gender.Unknown)
                                    {
                                        // prepare
                                        UserPreferences pref = new UserPreferences();
                                        pref.UserId = fbUser.UserId;
                                        pref.InterestedIn = fbUser.Gender == Gender.Male ? Gender.Female : Gender.Male;

                                        // update
                                        pref.UpdateOnDBase(conn, trans);
                                    }
                                }

                                if (imageReplaceNeeded)
                                {
                                    // reset profile feature of previous images
                                    List<Guid> imageIdsToReset = previousProfilePics.GetImageIds();
                                    string concatenatedImageIdsToReset = String.Join(",", imageIdsToReset);
                                    if (ImageFile.ResetProfileFlags(conn, trans, user.Id, concatenatedImageIdsToReset) != imageIdsToReset.Count)
                                    {
                                        throw new Pic4PicArgumentException("Unexpected failure while marking images at the database", "PhotoUploadReference");
                                    }
                                    
                                    // assign previously downloaded images to the user
                                    string concatenatedImageIds = String.Join(",", imageIds);
                                    if (ImageFile.UpdateAllImageOwnerships(conn, trans, user.Id, concatenatedImageIds) != imageIds.Count)
                                    {
                                        throw new Pic4PicArgumentException("Referenced photo doesn't exist", "PhotoUploadReference");
                                    }
                                }
                            });
                        }
                    }

                    if (imageReplaceNeeded)
                    {
                        // we need to re-read this
                        userPictures = ImageFile.ReadAllFromDBaseByUserId(conn, null, user.Id);
                    }

                    #endregion // #region SIGNUP is called TWICE! Maybe with different pictures and Facebook profile!
                }

                // if this is a clean signup, then userPictures shoud be null. Otherwise, they will have the proper value
                if (userPictures == null)
                {
                    // 
                    userPictures = ImageFile.ReadAllFromDBaseByUserId(conn, null, user.Id);
                }

            } // using connection

            // if we are here, we can say that signup is done; update the auth object
            auth = new UserAuthInfo(user);
            
            // prepare response and return
            UserResponse response = new UserResponse();
            response.AuthToken = Crypto.EncryptAES(auth.ToString(), config.AES_Key, config.AES_IV);
            response.ProfilePictures = UserProfilePics.From(userPictures);
            response.UserProfile = new UserProfile(fbUser, auth.Username, user.Description, (user.UserStatus == UserStatus.Active), (user.UserType == UserType.Guest)); // downgrade
            // response.CurrentCredit = this.ReadCurrentCreditFromCacheOrDBase(); // no need this here
            response.AttachSettings(user.SplitId);

            // get other images as well
            List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(userPictures); // familiar // me

            // add other images
            response.OtherPictures.AddRange(otherImages);

            return response;
        }

        protected BaseResponse _ActivateUser(BaseRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // check input
            request.Validate();

            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                if (Pic4Pic.ObjectModel.User.ActivateUserOnDBase(connection, null, auth.UserId, true) <= 0) 
                {
                    throw new Pic4PicException("Access Denied");
                }

                // send initial message
                InstantMessage im = new InstantMessage();
                im.Id = Guid.NewGuid();
                im.SentTimeUTC = DateTime.UtcNow;
                im.UserId1 = User.WellKnownSystemUserId;
                im.UserId2 = auth.UserId;
                im.Content = "Thank you for downloading pic4pic!\n\nShould you have any feedback or question, please send an email to appsicle@gmail.com\n\nEnjoy!\n";

                try
                {
                    InstantMessage.CreateOnDBase(connection, null, im);
                }
                catch
                { 
                    // ignore
                }
                
                
            } // connection

            return new BaseResponse();
        }

        private List<Guid> DecryptImageReferences(Config config, string encryptedPhotoReferences, int count)
        {
            // decrypt photo upload reference
            string decryptedReference = null;
            try
            {
                decryptedReference = Crypto.DecryptAES(encryptedPhotoReferences, config.AES_Key, config.AES_IV);
            }
            catch (Pic4PicException)
            {
                throw new Pic4PicArgumentException("Photo upload reference is invalid", "PhotoUploadReference");
            }

            // split reference text
            string[] uploadedIds = decryptedReference.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (uploadedIds == null || uploadedIds.Length < count) // DO NOT use '==' as we have paddings...
            {
                throw new Pic4PicArgumentException("Photo upload reference is invalid", "PhotoUploadReference");
            }

            // get uploaded photo Ids
            List<Guid> imageIds = new List<Guid>();
            for (int x = 0; x < count; x++) // DO NOT use foreach as we have paddings; i.e. uploadedIds includes more than 4 string tokens.
            {
                Guid temp = Guid.Empty;
                if (Guid.TryParse(uploadedIds[x], out temp) && temp != Guid.Empty)
                {
                    imageIds.Add(temp);
                }
                else
                {
                    throw new Pic4PicArgumentException("Photo upload reference is invalid", "PhotoUploadReference");
                }
            }

            return imageIds;
        }

        private void CreateAllEntities(SqlConnection conn, SqlTransaction trans, User user, FacebookUser fbUser, WorkHistory workHistory, EducationHistory eduHistory, UserPreferences pref, List<Guid> imageIds)
        {
            // save user
            user.CreateNewOnDBase(conn, trans);

            // save facebook user
            fbUser.CreateNewOnDBase(conn, trans);

            // save work history
            if (workHistory != null && workHistory.Count > 0)
            {
                workHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }

            // save education history
            if (eduHistory != null && eduHistory.Count > 0)
            {
                eduHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }

            // save User Preference
            pref.CreateNewOnDBase(conn, trans);

            // assign previously downloaded images to the user
            string concatenatedImageIds = String.Join(",", imageIds);
            if (ImageFile.UpdateAllImageOwnerships(conn, trans, user.Id, concatenatedImageIds) != imageIds.Count)
            {
                throw new Pic4PicArgumentException("Referenced photo doesn't exist", "PhotoUploadReference");
            }
        }

        #endregion

        protected BaseResponse _SaveUserDescription(SimpleStringRequest request, out UserAuthInfo auth)
        { 
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // check input
            request.Validate(true);

            // save
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                // update on DB
                User.AddOrUpdateUserDetailsOnDBase(connection, null, auth.UserId, request.Data);
            }

            // return
            return new BaseResponse();
        }

        protected BaseResponse _DownloadFacebookFriends(FacebookRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // check input
            request.Validate();

            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                NameLongIdPairList friendsRow = FacebookHelpers.GetFriendsListFromFacebook(request.FacebookAccessToken);
                FacebookFriendList friends = FacebookFriendList.Create(request.FacebookUserId, friendsRow);

                FacebookFriendList currentFriends = Database.SelectAll<FacebookFriend, FacebookFriendList>(
                    connection, null, request.FacebookUserId, Database.TimeoutSecs);

                FacebookFriendList delta = friends.Subtract(currentFriends);

                if (delta.Count > 0)
                {
                    // one way to do it
                    // foreach (FacebookFriend newFriend in delta)
                    // {
                    //     Database.InsertOrUpdate(newFriend, connection, null, Database.TimeoutSecs);
                    // }

                    // another way to do it
                    StringBuilder bigQuery = new StringBuilder();
                    foreach (FacebookFriend newFriend in delta)
                    {
                        string query = newFriend.InsertQueryInline();
                        bigQuery.AppendLine(query);
                    }

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = bigQuery.ToString();
                        cmd.ExecuteNonQuery();
                    }

                } // delta > 0

                return new BaseResponse();
            } // connection
        }

        #endregion

        #region SERVICE IMPLs : Matches, Notifications, Pic4Pic

        protected SimpleListResponse<Interaction> _GetNotifications(NotificationRequest request, out UserAuthInfo auth) 
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // get notifications
                int cutOffMinutes = 30 * 24 * 60; // 30 days
                List<Interaction> interactions = InteractionEngine.GetRecentInteractions(conn, null, auth.UserId, 50, cutOffMinutes, MatchConfig.CreateDefault());

                // return
                SimpleListResponse<Interaction> response = new SimpleListResponse<Interaction>();
                response.Items.AddRange(interactions);
                return response;
            } 
        }

        protected BaseResponse _Mark(MarkingRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (request.ObjectType == ObjectType.Notification && request.MarkingType == MarkingType.Viewed)
                {
                    // mark notification as viewed
                    ObjectModel.Action.MarkAsViewedOnDBase(conn, null, auth.UserId, request.ObjectId);
                }
                else if (request.ObjectType == ObjectType.Profile && request.MarkingType == MarkingType.Viewed)
                {
                    // mark profile as viewed...

                    // create object
                    ObjectModel.Action action = new ObjectModel.Action();
                    action.Id = Guid.NewGuid();
                    action.ActionTimeUTC = DateTime.UtcNow;
                    action.ActionType = ActionType.ViewedProfile;
                    action.Status = ActionStatus.Created;
                    action.UserId1 = auth.UserId;
                    action.UserId2 = request.ObjectId;

                    // visiting your own profile?
                    if (!action.UserId1.Equals(action.UserId2))
                    {
                        // save to database
                        action.CreateOnDBase(conn, null);
                    }
                }
                else if (request.ObjectType == ObjectType.Profile && request.MarkingType == MarkingType.Liked)
                {
                    // mark profile as liked

                    // create object
                    ObjectModel.Action action = new ObjectModel.Action();
                    action.Id = Guid.NewGuid();
                    action.ActionTimeUTC = DateTime.UtcNow;
                    action.ActionType = ActionType.LikedBio;
                    action.Status = ActionStatus.Created;
                    action.UserId1 = auth.UserId;
                    action.UserId2 = request.ObjectId;

                    // liking your own profile?
                    if (action.UserId1.Equals(action.UserId2))
                    {
                        throw new Pic4PicException("Ooops! Something is wrong: Liking your own profile?");
                    }

                    // save to database
                    action.CreateOnDBase(conn, null);
                }
                else
                {
                    throw new Pic4PicException("Unknown operation");
                }   
            }

            // return success
            return new BaseResponse();
        }

        protected SimpleListResponse<MatchedCandidate> _GetMatches(SimpleObjectRequest<Location> request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate(true);

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // determine home-town city and state....                
                string concatenatedCitiesInRange = null;
                Locality loc = request.Data == null ? null : request.Data.Locality;
                string hometownState = MatchEngine.GetCachedStateAndCitiesInRangeOrRead(
                    conn, null, auth.UserId, loc, ref concatenatedCitiesInRange);

                // get today's matches
                List<MatchedCandidate> matches = MatchEngine.PrepareTodaysMatches(
                    conn, null, auth.UserId, MatchConfig.CreateDefault(), hometownState, concatenatedCitiesInRange);

                // get current credit
                int currentCredit = this.ReadCurrentCreditFromCacheOrDBase(conn, null, auth.UserId);

                // return
                SimpleListResponse<MatchedCandidate> response = new SimpleListResponse<MatchedCandidate>();
                response.Items.AddRange(matches);
                response.CurrentCredit = currentCredit;
                return response;
            }
        }

        protected SimpleListResponse<MatchedCandidate> _GetPreviewMatches(SimpleObjectRequest<Location> request, out UserAuthInfo auth)
        { 
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate(true);

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // determine home-town city and state....  
                // below data can be null...
                string concatenatedCitiesInRange = null;
                Locality loc = request.Data == null ? null : request.Data.Locality;
                string hometownState = MatchEngine.GetCachedStateAndCitiesInRangeOrRead(
                    conn, null, request.ClientId, loc, ref concatenatedCitiesInRange);

                if (String.IsNullOrWhiteSpace(hometownState) || String.IsNullOrWhiteSpace(concatenatedCitiesInRange)) 
                {
                    hometownState = "Washington";
                    concatenatedCitiesInRange = "Seattle,Bellevue,Kirkland,Redmond";
                }

                // get preview matches
                List<MatchedCandidate> matches = MatchEngine.GetPreviewMatches(conn, null, 10, hometownState, concatenatedCitiesInRange);

                // return
                SimpleListResponse<MatchedCandidate> response = new SimpleListResponse<MatchedCandidate>();
                response.Items.AddRange(matches);
                return response;
            }
        }

        protected SimpleListResponse<MatchedCandidate> _BuyNewMatches(BuyingNewMatchRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate(true);

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // get current credit
                int currentCredit = this.ReadCurrentCreditFromCacheOrDBase(conn, null, auth.UserId);
                if (currentCredit <= 0)
                {
                    throw new Pic4PicException("You don't have any credit to buy new matches");
                }

                // calculcate purchase count
                int price = 10;
                int affordableCount = currentCredit / price;
                if (affordableCount <= 0)
                {
                    throw new Pic4PicException("You don't have enough credit to buy new matches");
                }

                // 
                int buyAttempCount = Math.Min(affordableCount, request.MaxCount);

                // determine home-town city and state....                
                string concatenatedCitiesInRange = null;
                Locality loc = request.Location == null ? null : request.Location.Locality;
                string hometownState = MatchEngine.GetCachedStateAndCitiesInRangeOrRead(
                    conn, null, auth.UserId, loc, ref concatenatedCitiesInRange);

                // get new matches
                List<MatchedCandidate> newMatches = MatchEngine.BuyNewMatches(
                    conn, null, auth.UserId, buyAttempCount, MatchConfig.CreateDefault().RematchLimitAsMinutes, hometownState, concatenatedCitiesInRange);

                int boughtCount = newMatches.Count;
                if (boughtCount <= 0)
                {
                    throw new Pic4PicException("There is not any new match in your region yet. Please try again later.");
                }

                // reverse
                newMatches.Reverse();

                // adjust credit
                int creditDrop = -1 * boughtCount * price;
                CreditAdjustment creditAdjusment = new CreditAdjustment();
                creditAdjusment.UserId = auth.UserId;
                creditAdjusment.Credit = creditDrop;
                creditAdjusment.Reason = CreditAdjustmentReason.PaidMatch;
                creditAdjusment.CreateTimeUTC = DateTime.UtcNow;
                creditAdjusment.CreateOnDBase(conn, null);

                // adjust credit on the cache as well
                int newCredit = this.AdjustCurrentCredit(conn, null, auth.UserId, creditDrop);
                
                // return
                SimpleListResponse<MatchedCandidate> response = new SimpleListResponse<MatchedCandidate>();
                response.Items.AddRange(newMatches);
                response.CurrentCredit = newCredit;
                return response;
            }
        }

        protected SimpleResponse<MatchedCandidate> _RequestPic4Pic(StartingPic4PicRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // request to himself?
            if (auth.UserId.Equals(request.UserIdToInteract)) 
            {
                throw new Pic4PicException("Ooops! Something is wrong: Sending pic4pic request to yourself?");
            }
                        
            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // create pic4pic request
                Familiarity currentFamiliarity = Familiarity.Stranger;
                PicForPic pic4pic = PicTradeEngine.RequestPic4Pic(
                    conn, null, auth.UserId, request.UserIdToInteract, request.PictureIdToExchange, 1 * 24 * 60, ref currentFamiliarity);

                // get info for the candidate
                MatchedCandidate match = MatchEngine.GetMatchByUserId(
                    conn, null, auth.UserId, pic4pic.UserId2, currentFamiliarity, false, MatchConfig.CreateDefault());

                // set last view time
                match.LastViewTimeUTC = DateTime.UtcNow;

                // return 
                SimpleResponse<MatchedCandidate> response = new SimpleResponse<MatchedCandidate>();
                response.Data = match;
                return response;
            }
        }

        protected SimpleResponse<MatchedCandidate> _AcceptPic4Pic(AcceptingPic4PicRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);
            
            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // accept pic4pic request
                PicForPic pic4pic = PicTradeEngine.AcceptPic4PicRequest(
                    conn, null, auth.UserId, request.Pic4PicRequestId, request.PictureIdToExchange);

                // get info for the candidate
                MatchedCandidate match = MatchEngine.GetMatchByUserId(
                    conn, null, auth.UserId, pic4pic.UserId1, Familiarity.Familiar, false, MatchConfig.CreateDefault());

                // set last view time
                match.LastViewTimeUTC = DateTime.UtcNow;

                // return 
                SimpleResponse<MatchedCandidate> response = new SimpleResponse<MatchedCandidate>();
                response.Data = match;
                return response;
            }
        }

        protected CandidateDetailsResponse _GetCandidateDetails(CandidateDetailsRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // get today's matches...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // accept pic4pic request
                List<PicForPic> pic4picList = PicForPic.ReadAllFromDBase(conn, null, auth.UserId, request.UserId, 100);
                Pic4PicHistory history = Pic4PicHistory.From(pic4picList, auth.UserId, request.UserId);

                // get info for the candidate
                MatchedCandidate match = MatchEngine.GetMatchByUserId(
                    conn, null, auth.UserId, request.UserId, history.GetFamiliarity(), false, MatchConfig.CreateDefault());

                // set last view time
                match.LastViewTimeUTC = DateTime.UtcNow;

                // return 
                CandidateDetailsResponse response = new CandidateDetailsResponse();
                response.Candidate = match;
                return response;
            }
        }

        #endregion

        #region SERVICE IMPLs : Instant Messaging

        protected SimpleListResponse<InstantMessage> _SendInstantMessage(InstantMessageRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // prepare cache key...
            Guid uid1 = auth.UserId;
            Guid uid2 = request.UserIdToInteract;

            // sending message to oneself?
            if (uid1.Equals(uid2)) 
            {
                throw new Pic4PicException("Ooops! Something is wrong: Sending instant message to yourself?");
            }

            if (uid1.CompareTo(uid2) > 0)
            {
                // swap
                uid1 = request.UserIdToInteract;
                uid2 = auth.UserId;
            }

            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "LastExchangedMessageId_{0}_{1}",
                uid1,
                uid2);

            // save IM
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                // is user familiar with this person?
                Familiarity familiarity = Familiarity.Stranger;
                User candidate = User.ReadFromDBaseWithFamiliarity(conn, null, request.UserIdToInteract, auth.UserId, ref familiarity);
                if (null == candidate) 
                {
                    throw new Pic4PicException("The user that you are trying to send a message couldn't be found");
                }

                if (familiarity != Familiarity.Familiar)
                {
                    throw new Pic4PicException("You cannot send a message to this user yet");
                }
                
                InstantMessage im = new InstantMessage();
                im.Id = Guid.NewGuid();
                im.SentTimeUTC = DateTime.UtcNow;
                im.UserId1 = auth.UserId;
                im.UserId2 = request.UserIdToInteract;
                im.Content = request.Content;

                InstantMessage.CreateOnDBase(conn, null, im);

                Guid lastExchangedMessageId = im.Id;
                List<InstantMessage> ims = InstantMessage.ReadConversationFromDBase(
                    conn, null, auth.UserId, request.UserIdToInteract, 50, 60 * 24 * 365);

                if (ims.Count > 0)
                {
                    lastExchangedMessageId = ims[0].Id;
                }

                // update the cache
                // IMs are sorted descendanly in the database. Therefore the first item is the last IM. Guid.Empty will be cached if no conversation.
                CacheHelper.Put(CacheHelper.CacheName_Conversations, cacheKey, lastExchangedMessageId);

                // return 
                SimpleListResponse<InstantMessage> response = new SimpleListResponse<InstantMessage>();
                response.Items.AddRange(ims);
                return response;
            }
        }

        protected SimpleListResponse<InstantMessage> _GetConversation(ConversationRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // prepare cache key...
            Guid uid1 = auth.UserId;
            Guid uid2 = request.UserIdToInteract;

            // want to see conversation with yourself?
            if (uid1.Equals(uid2))
            {
                throw new Pic4PicException("Ooops! Something is wrong: Want to see conversaion with yourseld?");
            }

            if (uid1.CompareTo(uid2) > 0) 
            {
                // swap
                uid1 = request.UserIdToInteract;
                uid2 = auth.UserId;
            }

            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "LastExchangedMessageId_{0}_{1}",
                uid1,
                uid2);

            // read last exchanged IM ID
            List<InstantMessage> ims = new List<InstantMessage>(); // empty
            Guid lastExchangedMessageId = CacheHelper.GetOrDefault<Guid>(CacheHelper.CacheName_Conversations, cacheKey);
            if (lastExchangedMessageId == request.LastExchangedMessageId && lastExchangedMessageId != Guid.Empty) 
            {
                // do nothing; i.e. return empty list, which means, there was no interaction since last time.
            }
            else
            {
                // get conversation...
                using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                {
                    conn.Open();
                    ims = InstantMessage.ReadConversationFromDBase(conn, null, auth.UserId, request.UserIdToInteract, 50, 60 * 24 * 365);
                    if (ims.Count > 0) 
                    {
                        lastExchangedMessageId = ims[0].Id;
                    }
                }

                // update the cache
                // IMs are sorted descendanly in the database. Therefore the first item is the last IM. Guid.Empty will be cached if no conversation.
                CacheHelper.Put(CacheHelper.CacheName_Conversations, cacheKey, lastExchangedMessageId);
            }

            // trim
            if (request.LastExchangedMessageId != Guid.Empty) 
            {
                bool found = false;
                for(int x=0; x<ims.Count; x++)
                {
                    if(!found && ims[x].Id == request.LastExchangedMessageId)
                    {
                        found = true;
                    }

                    if(found)
                    {
                        ims.RemoveAt(x--);
                    }
                }
            }

            // return 
            SimpleListResponse<InstantMessage> response = new SimpleListResponse<InstantMessage>();
            response.Items.AddRange(ims);
            return response;
        }

        protected SimpleListResponse<ConversationsSummary> _GetConversationSummary(BaseRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // get conversation...
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();
                List<ConversationsSummary> summaries = ConversationsSummary.ReadAllFromDBase(conn, null, auth.UserId, 100, 60 * 24 * 365);

                // return 
                SimpleListResponse<ConversationsSummary> response = new SimpleListResponse<ConversationsSummary>();
                response.Items.AddRange(summaries);
                return response;
            }
        }

        #endregion

        #region SERVICE IMPLs : Location
        protected BaseResponse _AssureSupportAtLocation(SimpleObjectRequest<Location> request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            //
            /*
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();
            }
            */

            return new BaseResponse();
        }

        protected BaseResponse _SetCurrentLocation(SimpleObjectRequest<Location> request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            //
            /*
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();
            }
            */
            return new BaseResponse();
        }

        #endregion

        #region SERVICE IMPLs : Client Log

        protected BaseResponse _LogClientTraces(ClientLogRequest request, out UserAuthInfo auth) 
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // add user info
            if(auth.UserId != Guid.Empty){
                foreach (LogBag bag in request.Logs)
                {
                    bag.Pairs.Add(new LogProperty("UserId", auth.UserId.ToString()));
                    bag.Pairs.Add(new LogProperty("Username", auth.Username));
                }
            }

            // log
            foreach (LogBag bag in request.Logs)
            {
                String log = bag.ToString();
                if (!String.IsNullOrWhiteSpace(log))
                {
                    Trace.WriteLine(log, "P4P_" + bag.GetErrorLevel());
                }                
            }            

            return new BaseResponse(); 
        }

        #endregion

        #region SERVICE IMPLs : Purchasing

        protected SimpleListResponse<PurchaseOffer> _GetOffers(BaseRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication since we need to know splitId
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "Credit_{0}",
                auth.UserId);

            //read current credit
            int currentCredit = 0;
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_Default, cacheKey, ref currentCredit))
            {
                // get current credit
                using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                {
                    conn.Open();

                    // get current credit
                    currentCredit = this.ReadCurrentCreditFromCacheOrDBase(conn, null, auth.UserId);
                }
            }

            SimpleListResponse<PurchaseOffer> response = new SimpleListResponse<PurchaseOffer>();
            response.Items.AddRange(PurchaseOffer.GetGooglePlayOffers(auth.SplitId));
            response.CurrentCredit = currentCredit;
            return response;
        }

        protected BaseResponse _ProcessPurchase(SimpleObjectRequest<PurchaseRecord> request, out UserAuthInfo auth) 
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();
            request.Data.UserId = auth.UserId;

            // make sure that the original data is not tempered.
            GooglePlayInappPurchaseRecord purchase = Pic4PicUtils.DeserializeObjectFromJSON<GooglePlayInappPurchaseRecord>(request.Data.OriginalData);
            if (!purchase.Compare(request.Data)) {
                throw new Pic4PicException("Original data mismatches with the derived data!");
            }

            // verify signature of purchase
            byte[] purchaseInfoBytes = Encoding.UTF8.GetBytes(request.Data.OriginalData);
            byte[] signatureBytes = Convert.FromBase64String(request.Data.DataSignature);
            byte[] publicKeyBytes = Convert.FromBase64String(config.GooglePlayBillingServicePublicKey);

            if (!Crypto.VerifySignature_2048_Bit_PKCS1_v1_5(purchaseInfoBytes, signatureBytes, publicKeyBytes)) 
            {
                throw new Pic4PicException("Receipt signature of the purchase couldn't be verified. Purchase doesn't seem to be genuine!");
            }

            // save the purchase
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                int creditAdjustment = 0;
                PurchaseRecord existing = PurchaseRecord.ReadFromDBase(conn, null, auth.UserId, request.Data.PurchaseReferenceToken);
                if (existing == null)
                {
                    // save the purchase
                    List<int> showedList = PurchaseOffer.GetGooglePlayOfferIDs(auth.SplitId);
                    string concatenatedIDs = String.Join(",", showedList);
                    request.Data.CreateOnDBase(conn, null, request.Data.GetPurchaseOffer(), concatenatedIDs);
                    creditAdjustment = request.Data.GetPurchaseOffer().CreditValue;
                }
                else 
                {
                    // we have processed this already.
                }

                // adjust current credit on cache
                int currentCredit = this.AdjustCurrentCredit(conn, null, auth.UserId, creditAdjustment);

                // return response
                BaseResponse response = new BaseResponse();
                response.CurrentCredit = currentCredit;
                return response;
            }
        }

        protected BaseResponse _TrackDevice(MobileDeviceRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // doesn't require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();

            // derive mobile device
            MobileDevice mobileDevice = request.CreateMobileDevice(auth.UserId, MobileOSType.Android);

            // save
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                mobileDevice.InsertOrUpdateOnDBase(conn, null);
            }

            // return response
            BaseResponse response = new BaseResponse();
            return response;
        }

        protected BaseResponse _GetCurrentCredit(BaseRequest request, out UserAuthInfo auth)
        {
            // read config. no need to catch
            Config config = new Config();
            config.Init();

            // require authentication
            auth = GetUserInfoFromContent(config.AES_Key, config.AES_IV, true);

            // note: request is not null for sure. see SafeExecute
            // below method throws exception if a field is invalid.
            request.Validate();
            
            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "Credit_{0}",
                auth.UserId);

            //read current credit
            int currentCredit = 0;
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_Default, cacheKey, ref currentCredit))
            {
                // get current credit
                using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                {
                    conn.Open();

                    // get current credit
                    currentCredit = this.ReadCurrentCreditFromCacheOrDBase(conn, null, auth.UserId);
                }
            }

            // return response
            BaseResponse response = new BaseResponse();
            response.CurrentCredit = currentCredit;
            return response;
        }

        #endregion // SERVICE IMPLs : Purchasing

        #region COMMON PRIVATE HELPERs

        private int AdjustCurrentCredit(SqlConnection conn, SqlTransaction trans, Guid userId, int adjustment)
        {
            // check input
            if (adjustment == 0) 
            {
                return ReadCurrentCreditFromCacheOrDBase(conn, trans, userId);
            }

            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "Credit_{0}",
                userId);

            //read current credit
            int currentCredit = 0;
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_Default, cacheKey, ref currentCredit))
            {
                currentCredit = CreditAdjustment.ReadCurrentCreditFromDBase(conn, trans, userId);
            }

            // adjust
            currentCredit += adjustment;
            CacheHelper.Put(CacheHelper.CacheName_Default, cacheKey, currentCredit);

            return currentCredit;
        }

        private int ReadCurrentCreditFromCacheOrDBase(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            // prepare cache key           
            string cacheKey = String.Format(
                CultureInfo.InvariantCulture,
                "Credit_{0}",
                userId);

            //read current credit
            int currentCredit = 0;
            if (!CacheHelper.Get<int>(CacheHelper.CacheName_Default, cacheKey, ref currentCredit))
            {
                currentCredit = CreditAdjustment.ReadCurrentCreditFromDBase(conn, trans, userId);
                CacheHelper.Put(CacheHelper.CacheName_Default, cacheKey, currentCredit);
            }

            return currentCredit;
        }

        private UserResponse ReadUserProfile(SqlConnection conn, SqlTransaction trans, Config config, User user, out UserAuthInfo auth)
        {
            // set the security context as the user already exists
            auth = new UserAuthInfo(user);

            // get user profiles
            FacebookUser fbUser = FacebookUser.ReadFromDBase(conn, trans, user.Id);
            if (fbUser == null)
            {
                throw new Pic4PicAuthException("User profile couldn't be found");
            }

            // handle the rest
            fbUser.IsActive = user.UserStatus == UserStatus.Active;
            return ReadUserProfile(conn, trans, config, fbUser, auth);
        }

        private UserResponse ReadUserProfile(SqlConnection conn, SqlTransaction trans, Config config, FacebookUser fbUser, UserAuthInfo auth)
        {
            // load profile picture details (getting actual data)
            List<ImageFile> userPictures = ImageFile.ReadAllFromDBaseByUserId(conn, trans, auth.UserId);
            if (userPictures.Count < 4)
            {
                throw new Pic4PicAuthException("Profile pictures couldn't be found");
            }
            
            // prepare response and return
            UserResponse response = new UserResponse();
            response.AuthToken = Crypto.EncryptAES(auth.ToString(), config.AES_Key, config.AES_IV);
            response.ProfilePictures = UserProfilePics.From(userPictures);
            response.UserProfile = new UserProfile(fbUser, auth.Username, fbUser.Description, fbUser.IsActive, (auth.UserType == UserType.Guest)); // downgrade
            response.AttachSettings(auth.SplitId);

            // get other images as well
            List<PicturePair> otherImages = PicturePair.GetNonProfilePicturePairs(userPictures); // Familiar // me

            // add other images
            response.OtherPictures.AddRange(otherImages);

            // read current credit
            response.CurrentCredit = this.ReadCurrentCreditFromCacheOrDBase(conn, trans, auth.UserId);

            return response;
        }

        private void VerifyUserNotExist(SqlConnection conn, SqlTransaction trans, string username, string password)
        {
            // check username
            bool usernameExists = false;
            User user = User.ReadFromDBase(conn, trans, username, password, out usernameExists);
            if (user == null)
            {
                if (usernameExists)
                {
                    // this username is already in use
                    // reply with "no authToken" but with an "error"
                    throw new Pic4PicAuthException("Username is already in use");
                }
            }
            else
            {
                // user has been signed up already
                throw new Pic4PicAuthException("User has already signed up");
            }
        }

        private void UpdateFacebook(SqlConnection conn, SqlTransaction trans, FacebookUser fbUser, WorkHistory workHistory, EducationHistory eduHistory)
        {
            // save facebook user
            fbUser.UpdateOnDBase(conn, trans);

            // save work history
            if (workHistory != null && workHistory.Count > 0)
            {
                // delete previous history
                WorkHistory.DeleteAllFromDBase(conn, trans, fbUser.FacebookId);

                // re-create all from scratch
                workHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }

            // save education history
            if (eduHistory != null && eduHistory.Count > 0)
            {
                // delete previous history
                EducationHistory.DeleteAllFromDBase(conn, trans, fbUser.FacebookId);

                // re-create all from scratch
                eduHistory.CreateAllOnDBase(conn, trans, fbUser.FacebookId);
            }
        }

        #endregion
    }
}