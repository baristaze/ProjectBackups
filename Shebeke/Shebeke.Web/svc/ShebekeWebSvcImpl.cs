using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Security;

using Shebeke.ObjectModel;

namespace Shebeke.Web.Services
{
    public partial class ShebekeWebSvc : ServiceBase, IShebekeWebSvc
    {
        public const int MaxFaceDescrInMessageSection = 200;

        protected AuthTokenResponse _Signin(SigninRequest request, out UserAuthInfo user)
        {
            Trace.WriteLine("Signin - Method started", LogCategory.Info);

            if (request == null)
            {
                throw new ShebekeArgumentException("Kimlik bilgileri bir garip, null diyeyim sen anla artık!");
            }

            if (request.OAuthProvider != OAuthProvider.Facebook)
            {
                throw new ShebekeArgumentException("Bu OAuthProvider desteklenmiyor. Sorun bizde!");
            }

            Trace.WriteLine("Signin - Facebook User Id: " + request.OAuthUserId, LogCategory.Info);
            
            // check expire time: TODO
            // check your memcache: TODO
            // check your database if memcache is empty: TODO
            // check expiration on cached-value if there is any: TODO
            FacebookUser fbUserNew = FacebookHelpers.GetUserFromFacebook(request.OAuthAccessToken, request.OAuthUserIdAsInt64);
            Trace.WriteLine("Signin - Facebook User FullName: " + fbUserNew.FullName, LogCategory.Info);

            // check to see if user is saved or not
            Config config = new Config();
            config.Init();
                        
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                bool userCreatedNew = this.CreateOrUpdateSignedInUser(connection, null, fbUserNew, request.SplitId);
                UserToken oauthTokenMeta = this.CreateOrUpdateUserTokenFor(connection, null, config, request, fbUserNew, userCreatedNew);

                user = fbUserNew.GetAuthInfo(oauthTokenMeta.OAuthAccessToken);
                string sessionInfo = user.ToString();

                int MaxLifeTimeSeconds = 60 * 24 * 60 * 60;
                TimeSpan lifeTime = oauthTokenMeta.ExpireTimeUTC - DateTime.UtcNow;
                int lifeTimeAsSeconds = (int)lifeTime.TotalSeconds;
                if (lifeTimeAsSeconds > MaxLifeTimeSeconds)
                {
                    lifeTimeAsSeconds = MaxLifeTimeSeconds;
                }

                AuthTokenResponse sessionToken = new AuthTokenResponse();
                sessionToken.Token = Crypto.EncryptAES(sessionInfo, config.AES_Key, config.AES_IV);
                sessionToken.ExpiresInSeconds = lifeTimeAsSeconds;
                sessionToken.OAuthProvider = user.OAuthProvider;
                sessionToken.OAuthUserId = user.OAuthUserId;
                sessionToken.UserFriendlyName = user.FirstName;
                sessionToken.PhotoUrl = user.PhotoUrl;

                return sessionToken;
            }
        }

        private bool CreateOrUpdateSignedInUser(SqlConnection connection, SqlTransaction trans, FacebookUser fbUserNew, int splitId)
        {
            bool userCreatedNew = false;
            FacebookUser existing = FacebookUser.ReadFromDBase(connection, trans, fbUserNew.FacebookId);
            if (existing == null)
            {
                SqlParameter splitParam = Database.SqlParam("@SplitId", (splitId > 0 ? (object)splitId : DBNull.Value));
                long newUserId = Database.ExecScalar<long>(connection, trans, "[dbo].[CreateNewUser]", splitParam);
                if (newUserId <= 0)
                {
                    throw new ShebekeException("Kullanıcı bilgisini kaydedemedik, nasıl oldu anlamadık!");
                }

                fbUserNew.Id = newUserId;
                fbUserNew.CreateNewOnDBase(connection, trans);

                fbUserNew.UserStatus = UserStatus.Active;
                fbUserNew.UserType = UserType.Regular; // we have just created this new; and it is authenticated
                fbUserNew.SplitId = splitId > 0 ? splitId : 0;                
                userCreatedNew = true;

                Trace.WriteLine("Signin - New user created: " + fbUserNew.Id, LogCategory.Info);
            }
            else
            {
                if (existing.UserStatus != UserStatus.Active)
                {
                    throw new ShebekeArgumentException("Böyle bir kullanıcı yok veya kullanıcı geçici olarak bloke edildi");
                }

                // check to see if we need to update the split Id
                if (splitId > 0 && existing.SplitId == 0)
                {
                    // User currently doesn't have a split. however, a new split parameter has been specified in the request.
                    // let's update the user split info at the database
                    // ...
                    // however, the given split may not be valid; we need to 
                    int stableSplitId = Database.ExecScalar<int>(
                        connection, 
                        trans, 
                        "[dbo].[UpdateUsersSplit]",
                        Database.SqlParam("@Id", existing.Id),
                        Database.SqlParam("@SplitId", splitId));

                    existing.SplitId = stableSplitId;
                }

                // why we are doing this?
                // assume that user gave us full-permission on day1; we have all info
                // later, s/he decided to revoke the right to pull 'hometown'... 
                // we still want to keep that existing data although we get 'null' from now on.
                Trace.WriteLine("Signin - Existing User: " + existing.Id, LogCategory.Info);

                fbUserNew.Id = existing.Id;
                fbUserNew.UserType = existing.UserType;
                fbUserNew.UserStatus = existing.UserStatus;
                fbUserNew.SplitId = existing.SplitId;

                bool needsUpdate = fbUserNew.Merge(existing);
                if (needsUpdate)
                {
                    fbUserNew.UpdateOnDBase(connection, trans);
                    Trace.WriteLine("Signin - Existing user updated: " + fbUserNew.Id, LogCategory.Info);
                }
            }

            return userCreatedNew;
        }

        private UserToken CreateOrUpdateUserTokenFor(
            SqlConnection connection, 
            SqlTransaction trans, 
            Config config, 
            SigninRequest signinRequest, 
            FacebookUser fbUserNew, 
            bool userCreatedNew)
        {
            UserToken token = new UserToken();
            token.UserId = fbUserNew.Id;
            token.OAuthProvider = OAuthProvider.Facebook;
            token.OAuthUserId = fbUserNew.FacebookId.ToString();
            token.OAuthAccessToken = signinRequest.OAuthAccessToken;
            token.ExpireTimeUTC = signinRequest.OAuthExpireTimeUTC;

            // update the user token            
            bool updateUserToken = false;
            if (userCreatedNew)
            {
                // we don't have token for sure since user is new
                updateUserToken = true;
            }
            else 
            {
                UserToken tempToken = token.Clone();
                if (!Database.Select(tempToken, connection, trans, Database.TimeoutSecs))
                {
                    // we don't have the token... Update it
                    updateUserToken = true;
                }
                else // we could read from database
                {
                    Trace.WriteLine("Signin - We have user token already: " + fbUserNew.Id, LogCategory.Info);

                    if (token.OAuthAccessToken != tempToken.OAuthAccessToken)
                    {
                        Trace.WriteLine("Signin - Existing token is different than the new one. FB User: " + fbUserNew.Id, LogCategory.Info);

                        // Check1: Check to see if the existing token is valid! We are not sure at this moment
                        //          because the token is different and the user might have revoked the access 
                        //          for the existing/old token...
                        try
                        {
                            // make a call to check this
                            FacebookUser tempUser = FacebookHelpers.GetUserFromFacebook(tempToken.OAuthAccessToken);
                            if (tempUser == null || tempUser.FacebookId != fbUserNew.FacebookId)
                            {
                                // weird... we better refresh the token
                                updateUserToken = true;
                                Trace.WriteLine("Signin - Existing token couldn't make a successfull call. FB User: " + fbUserNew.Id, LogCategory.Info);
                            }
                            else
                            {
                                // Check 2: Check to see if the token has enough life. Extend it if it is going to expire in 7 days
                                TimeSpan diff = tempToken.ExpireTimeUTC - signinRequest.OAuthExpireTimeUTC;
                                if (diff.TotalDays < 7)
                                {
                                    updateUserToken = true;
                                    Trace.WriteLine("Signin - Existing token is valid but doesn't have enough life. Extend it for : " + fbUserNew.Id, LogCategory.Info);
                                }
                                else
                                {
                                    // YAY! WE DON'T NEED to UPDATE the TOKEN :)
                                    token = tempToken;
                                    Trace.WriteLine("Signin - YAY! WE DON'T NEED to UPDATE the TOKEN for : " + fbUserNew.Id, LogCategory.Info);
                                }
                            }
                        }
                        catch (ShebekeAuthException)
                        {
                            // user has revoked the access and now s/he is re-connecting
                            // refresh the token
                            updateUserToken = true;
                            Trace.WriteLine("Signin - User has revoked the access for the old token. FB User: " + fbUserNew.Id, LogCategory.Info);
                        }
                    }                    
                    else // it never comes here... anyway
                    {
                        // *** NOTE: This ELSE is not effective since incoming short-lived token is never equal 
                        // to the other short-lived tokens or long-lived tokens... it differs all the time.
                        // keep it just in case...

                        // they are equal already. check to see if we need to extend it
                        TimeSpan diff = tempToken.ExpireTimeUTC - signinRequest.OAuthExpireTimeUTC;
                        if (diff.TotalDays < 7)
                        {
                            updateUserToken = true;
                            Trace.WriteLine("Signin - Existing token is short-lived. Extend it for : " + fbUserNew.Id, LogCategory.Info);
                        }
                        else
                        {
                            // they are equal and the valid token is long-lived
                            // YAY! WE DON'T NEED to UPDATE the TOKEN :)
                            token = tempToken;
                            Trace.WriteLine("Signin - YAY! WE DON'T NEED to UPDATE the TOKEN for : " + fbUserNew.Id, LogCategory.Info);
                        }
                    }
                }
            }
            
            if (updateUserToken)
            {
                UserToken tempToken = null;
                
                Trace.WriteLine("Signin - User token needs to be created/updated: " + fbUserNew.Id, LogCategory.Info);

                try
                {
                    tempToken = FacebookHelpers.ExtendAcessToken(config.FacebookAppId, config.FacebookAppSecret, signinRequest.OAuthAccessToken);
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeWebSvcImpl.cs",
                        "Signin",
                        "ExtendAcessToken",
                        0,
                        0,
                        ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }
                
                if (tempToken != null)
                {
                    token.OAuthAccessToken = tempToken.OAuthAccessToken;
                    token.ExpireTimeUTC = tempToken.ExpireTimeUTC;
                    Trace.WriteLine("Signin - User token extended: " + fbUserNew.Id, LogCategory.Info);
                }
                else
                {
                    // use the new token although it is short lived
                    token.OAuthAccessToken = signinRequest.OAuthAccessToken;
                    token.ExpireTimeUTC = signinRequest.OAuthExpireTimeUTC;
                    Trace.WriteLine("Signin - User token couldn't be extended. Short-lived token will be used: " + fbUserNew.Id, LogCategory.Info);
                }

                Database.InsertOrUpdate(token, connection, trans, Database.TimeoutSecs);
                Trace.WriteLine("Signin - User token inserted to the database: " + fbUserNew.Id, LogCategory.Info);
            }

            return token;
        }

        protected BaseResponse _Logout(out UserAuthInfo user)
        {
            // expire the token and delete if you are persisting it somewhere
            Config config = new Config();
            config.Init();
            user = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);
            return new BaseResponse();
        }

        protected FacebookFriendsResponse _GetInvitedFriends(out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            FacebookFriend selectFilter = new FacebookFriend();
            selectFilter.FacebookId1 = Int64.Parse(user.OAuthUserId);
            selectFilter.IsInvited = true;

            FacebookFriendList invitedFriends = Database.SelectAll<FacebookFriend, FacebookFriendList>(
                config.DBaseConnectionString, selectFilter, Database.TimeoutSecs);

            FacebookFriendsResponse response = new FacebookFriendsResponse();
            response.Friends.AddRange(invitedFriends);

            return response;
        }

        protected BaseResponse _MarkFriendsAsInvited(FacebookFriendList friends, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (friends == null)
            {
                throw new ShebekeArgumentException("Davetliler listesi bi garip, null diyeyim, sen anla artık!");
            }

            if (friends.Count > FacebookFriendList.MaxFriends)
            {
                throw new ShebekeArgumentException("Davetliler listesi çok büyük!");
            }

            DateTime inviteTimeUtc = DateTime.UtcNow;
            if (friends != null && friends.Count > 0)
            {
                StringBuilder queryBuilder = new StringBuilder();
                foreach (FacebookFriend friend in friends)
                {
                    friend.FacebookId1 = Int64.Parse(user.OAuthUserId);
                    friend.IsInvited = true;
                    friend.InvitationTimeUTC = inviteTimeUtc;
                    queryBuilder.AppendLine(friend.MarkAsInvitedQuery());
                }

                using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
                {
                    connection.Open();

                    if (user.UserId > 0)
                    {
                        this.CheckDenialOfService(connection, null, user.UserId, UserActivityType.MarkFriendsAsInvited);
                    }

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = queryBuilder.ToString();
                        cmd.ExecuteNonQuery();
                    }

                    if (user.UserId > 0)
                    {
                        this.LogUserActivity(connection, null, user.UserId, UserActivityType.MarkFriendsAsInvited);
                    }
                }
            }

            return new BaseResponse();
        }

        protected BaseResponse _RefreshCachedFriends(out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            UserToken token = new UserToken();
            token.UserId = user.UserId; 
            token.OAuthProvider = OAuthProvider.Facebook;

            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                if (!Database.Select(token, connection, null, Database.TimeoutSecs))
                {
                    throw new ShebekeException("Facebook erişim bileti kayıplarda!");
                }

                if (String.IsNullOrWhiteSpace(token.OAuthAccessToken))
                {
                    throw new ShebekeException("Facebook erişim bileti bi garip, null diyeyim sen anla!");
                }

                if (token.ExpireTimeUTC <= DateTime.UtcNow)
                {
                    throw new ShebekeException("Facebook erişim biletinin süresi dolmuş!");
                }

                long facebookId = long.Parse(token.OAuthUserId);
                NameIdPairList friendsRow = FacebookHelpers.GetFriendsListFromFacebook(token.OAuthAccessToken);
                FacebookFriendList friends = FacebookFriendList.Create(facebookId, friendsRow);
                
                FacebookFriendList currentFriends = Database.SelectAll<FacebookFriend, FacebookFriendList>(
                    connection, null, facebookId, Database.TimeoutSecs);

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
        } // method

        protected VoteResponse _SaveEntryVote(VoteRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (request == null)
            {
                throw new ShebekeArgumentException("Oylama isteği bir garip, null diyeyim, sen anla artık!");
            }

            // check input
            if (request.Vote > 1 || request.Vote < -1)
            {
                throw new ShebekeArgumentException("Geçersiz oy: " + request.Vote);
            }
            
            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.SaveEntryVote);
                }

                VotingSummary result = VotingSummary.SaveVoteOnDBase(conn, null, request.EntryId, user.UserId, request.Vote);
                if (result == null) 
                {
                    // last vote was reverted... no vote anymore
                    result = new VotingSummary();
                }

                bool facebookTokenIsInvalid = false;
                if (request.Vote != 0)
                {
                    try
                    {
                        ShareEntryVoteOnFacebook(conn, null, config, user, request.TopicId, request.EntryId, request.Vote);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_SaveEntryVote",
                            "ShareEntryVoteOnFacebook",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);

                        if (ex is ShebekeAuthException)
                        {
                            facebookTokenIsInvalid = true;
                        }
                    }
                }

                // update caches
                try
                {
                    TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.Vote, 1); // always 1

                    if (request.Vote != 0)
                    {
                        TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.Share, 1); // always 1
                    }
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_SaveEntryVote",
                        "UpdateCacheOnSocialActions",
                        user.UserId,
                        user.SplitId,
                        "Vote has been saved to the database but the cache couldn't be updated for topic: " + request.TopicId.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {                
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.SaveEntryVote);
                }

                VoteResponse response = new VoteResponse();
                response.VotingSummary = result;
                response.NeedsRelogin = facebookTokenIsInvalid;
                return response;
            }
        }

        protected ReactionResponse _SaveEntryReaction(ReactionRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (request == null)
            {
                throw new ShebekeArgumentException("Kısa yorum isteği bi garip, null diyeyim, sen anla artık");
            }

            // check input
            long rid = Math.Abs(request.ReactionId);
            if ((rid & (rid-1)) != 0)
            {
                throw new ShebekeArgumentException("Invalid ReactionId: " + request.ReactionId);
            }

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.SaveEntryReaction);
                }

                ReactionSummary result = ReactionSummary.SaveReactionOnDBase(
                    conn, null, request.EntryId, user.UserId, request.ReactionId);

                if (result == null)
                {
                    // last reaction was reverted... no reaction anymore
                    result = new ReactionSummary();
                }

                bool facebookTokenIsInvalid = false;
                if (request.ReactionId > 0)
                {
                    try
                    {
                        ShareEntryReactionOnFacebook(conn, null, config, user, request.TopicId, request.EntryId, request.ReactionId);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_SaveEntryReaction",
                            "ShareEntryReactionOnFacebook",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);

                        if (ex is ShebekeAuthException)
                        {
                            facebookTokenIsInvalid = true;
                        }
                    }
                }

                // update caches
                try
                {
                    TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.React, 1); // always 1

                    if (request.ReactionId > 0)
                    {
                        TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.Share, 1); // always 1
                    }
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_SaveEntryReaction",
                        "UpdateCacheOnSocialActions",
                        user.UserId,
                        user.SplitId,
                        "Reaction has been saved to the database but the cache couldn't be updated for topic: " + request.TopicId.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.SaveEntryReaction);
                }

                ReactionResponse response = new ReactionResponse();
                response.ReactionSummary = result;
                response.NeedsRelogin = facebookTokenIsInvalid;
                return response;
            }
        }

        protected BaseResponse _DiscardUploadedImage(long fileId, out UserAuthInfo user) 
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);
                        
            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.DiscardUploadedImage);
                }

                ImageFile image = ImageFile.ReadFromDBase(conn, null, fileId, user.UserId);
                if (null == image)
                {
                    throw new ShebekeArgumentException("Bu resim ya yok, ya da senin erişim hakkın yok!");
                }

                // delete from database first
                image.DeleteFromDBase(conn, null, true, user.UserId);

                // this shouldn't throw since we have deleted from database already
                try
                {
                    ImageFile.DeleteFromCloud(config.BlobStorage, image.CloudUrl);
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_DiscardUploadedImage",
                        "DeleteFromCloud",
                        user.UserId,
                        user.SplitId,
                        ex.ToString());
                    
                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {                
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.DiscardUploadedImage);
                }
            }
            
            return new BaseResponse();
        }

        protected EntryResponse _AddNewEntry(EntryRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (request == null)
            {
                throw new ShebekeArgumentException("Eklenmek istenen içerik bi garip, null diyeyim sen anla artık!");
            }

            Entry entry = new Entry();
            entry.Content = request.Content;
            entry.TopicId = request.TopicId;
            entry.CreatedBy = user.UserId;
            entry.FormatVersion = EntryFormatterAbstractFactory.CurrentFormatVersion;

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.AddNewEntry);
                }
                                            
                List<ImageFile> deletedFiles = new List<ImageFile>();
                List<ImageFile> remainingImages = new List<ImageFile>();
                IEntryFormatter formatter = EntryFormatterAbstractFactory.CreateForCurrent(HttpContext.Current.Request.ApplicationPath.TrimEnd('/'));
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Exception exception = null;
                    try
                    {
                        // add the entry to the DB
                        entry.CreateOnDBase(conn, trans);

                        // delete un-referenced but previously allocated images
                        List<long> referencedImageIds = formatter.DetectImageIds(entry.Content);
                        
                        remainingImages = DeleteUnreferencedImagesFromDBase(
                            conn, trans, user.UserId, referencedImageIds, deletedFiles);

                        // attach referenced images to the entry
                        foreach (ImageFile img in remainingImages)
                        {
                            img.AssetType = AssetType.Entry;
                            img.AssetId = entry.Id;
                            img.AssociateImageToAsset(conn, trans, user.UserId);
                        }

                        // commit
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        if (exception != null)
                        {
                            trans.Rollback();
                            throw exception;
                        }
                    }
                }

                // delete images from cloud, too
                // this should be safe
                foreach (ImageFile deletedImage in deletedFiles)
                {
                    try
                    {
                        ImageFile.DeleteFromCloud(config.BlobStorage, deletedImage.CloudUrl);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_AddNewEntry",
                            "DeleteFromCloud",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);
                    }
                }
                
                // share on social channels
                bool facebookTokenIsInvalid = false;
                if (request.ShareOnFacebook) 
                {
                    try
                    {
                        ShareEntryOnFacebook(conn, null, entry, user, config, remainingImages);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_AddNewEntry",
                            "ShareEntryOnFacebook",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);

                        if (ex is ShebekeAuthException)
                        {
                            facebookTokenIsInvalid = true;
                        }
                    }
                }

                // update caches
                try
                {
                    Topic.UpdateCacheOnEntryAddOrDelete(entry.TopicId, true);
                    TopicInfo.UpdateCacheOnEntryAddOrDelete(entry.TopicId, true);

                    if (request.ShareOnFacebook)
                    {
                        TopicInfo.UpdateCacheOnSocialActions(entry.TopicId, Shebeke.ObjectModel.Action.Share, 1); // always 1
                    }
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_AddNewEntry",
                        "UpdateCacheOnEntryAddOrDelete",
                        user.UserId,
                        user.SplitId,
                        "Entry added to the database but some caches couldn't be updated for topic: " + entry.TopicId.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.AddNewEntry);
                }

                // return the success result
                EntryResponse response = new EntryResponse();
                response.Entry = entry;
                response.Entry.ContentAsEncodedHtml = formatter.GetEncodedHtml(entry.Content, remainingImages);
                response.Entry.CanDelete = ((int)user.UserType > (int)UserType.Regular) || (user.UserType == UserType.Regular && user.UserId == entry.CreatedBy);
                response.Entry.CanEdit = ((int)user.UserType > (int)UserType.Regular) || (user.UserType == UserType.Regular && user.UserId == entry.CreatedBy);

                response.Entry.AuthorInfo = new AuthorInfo();
                response.Entry.AuthorInfo.Id = response.Entry.CreatedBy;
                response.Entry.AuthorInfo.FirstName = user.FirstName;
                response.Entry.AuthorInfo.LastName = user.LastName;
                response.Entry.AuthorInfo.PhotoUrl = user.PhotoUrl;

                response.NeedsRelogin = facebookTokenIsInvalid;

                return response;
            }
        }

        private void ShareTopicOnFacebook(SqlConnection conn, SqlTransaction trans, Topic topic, UserAuthInfo user, Config config)
        {
            FacebookPostItem postItem = new FacebookPostItem();

            // link to entry (to topic indeed)
            // we don't include user.SplitId... let GE handle it...
            postItem.ItemLink = config.RootWebUrl + "/" + topic.SeoLink + "?e=0#f=1";
            
            // title of the topic as caption
            postItem.ItemCaption = "shebeke.net";   // we reversed these two lines.
            postItem.ActorName = topic.Title;

            postItem.ItemPicture = config.RootWebUrl + "/Images/logo.png?version=0.81";
            postItem.PersonalMessage = "Az önce shebeke'de bu başlığı açtım. Başlık altına içerik eklemek ve konu hakkında yorumlarınızı paylaşmak için tıklayın.";
            
            string oauthToken = GetFacebookTokenOfCurrentUser(conn, trans, user);
            if (!String.IsNullOrWhiteSpace(oauthToken))
            {
                if (FacebookHelpers.PostOnWall(postItem, oauthToken) > 0)
                {
                    String metricLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[MetricName=FacebookShare];[Asset=Topic];[ShareType=New];[ShareCount=1];[UserId={0}];[TopicId={1}]",
                            "[Version=2];[MetricName=FacebookShare];[Asset=Topic];[ShareType=New];[ShareCount=1];[UserId={0}];[TopicId={1}];[Split={2}]",
                            user.UserId,
                            topic.Id,
                            user.SplitId);

                    Trace.WriteLine(metricLog, LogCategory.Metric);

                    try
                    {
                        ActionLog.LogSocialShare(conn, trans, user.UserId, topic.Id, 0, SocialChannel.Facebook);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "ShareTopicOnFacebook",
                            "LogSocialShare",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);
                    }
                }
            }
        }

        private long ShareEntryOnFacebook(SqlConnection conn, SqlTransaction trans, Entry entry, UserAuthInfo user, Config config, List<ImageFile> entryImages)
        {
            //this.PostEntryActionOnFacebook(conn, trans, config, user, entry, entryImages, "Az önce Shebeke'de bir başlık açtım.", false, 0, false);
            //this.PostEntryActionOnFacebook(conn, trans, config, user, entry, entryImages, String.Empty, true, 500, true);
            long result = this.PostEntryActionOnFacebook(conn, trans, config, user, entry, entryImages, String.Empty, false, 0, false);
            if (result > 0)
            {
                String metricLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[MetricName=FacebookShare];[Asset=Entry];[ShareType=New];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}]",
                            "[Version=2];[MetricName=FacebookShare];[Asset=Entry];[ShareType=New];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}];[Split={3}]",
                            user.UserId,
                            entry.TopicId,
                            entry.Id,
                            user.SplitId);

                Trace.WriteLine(metricLog, LogCategory.Metric);
            }

            return result;
        }

        private long ShareEntryVoteOnFacebook(SqlConnection conn, SqlTransaction trans, Config config, UserAuthInfo user, long topicId, long entryId, int vote)
        {
            string voteText = "+1";
            if (vote < 0)
            {
                voteText = "-1";
            }

            long result = this.PostEntryActionOnFacebook(conn, trans, config, user, entryId, voteText, false, 0, false);
            if (result > 0)
            {
                String metricLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[MetricName=FacebookShare];[Asset=Entry];[ShareType=Vote];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}];[Vote={3}]",
                            "[Version=2];[MetricName=FacebookShare];[Asset=Entry];[ShareType=Vote];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}];[Vote={3}];[Split={4}]",
                            user.UserId,
                            topicId,
                            entryId,
                            vote,
                            user.SplitId);

                Trace.WriteLine(metricLog, LogCategory.Metric);
            }

            return result;
        }

        private long ShareEntryReactionOnFacebook(SqlConnection conn, SqlTransaction trans, Config config, UserAuthInfo user, long topicId, long entryId, long reactionId)
        {
            long result = 0;
            Reaction reaction = Reaction.GetById(reactionId);
            if (reaction != null)
            {
                result = this.PostEntryActionOnFacebook(conn, trans, config, user, entryId, reaction.Name, false, 0, false);
                if (result > 0)
                {
                    String metricLog = String.Format(
                               CultureInfo.InvariantCulture,
                        // "[Version=1];[MetricName=FacebookShare];[Asset=Entry];[ShareType=Reaction];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}];[ReactionId={3}]",
                               "[Version=2];[MetricName=FacebookShare];[Asset=Entry];[ShareType=Reaction];[ShareCount=1];[UserId={0}];[TopicId={1}];[EntryId={2}];[ReactionId={3}];[Split={4}]",
                               user.UserId,
                               topicId,
                               entryId,
                               reactionId,
                               user.SplitId);

                    Trace.WriteLine(metricLog, LogCategory.Metric);
                }
            }

            return result;
        }

        private long PostEntryActionOnFacebook(
            SqlConnection conn, 
            SqlTransaction trans, 
            Config config, 
            UserAuthInfo user, 
            long entryId, 
            string personalMessage,
            bool appendEntryDescToMessage,
            int maxMessageLength,
            bool addReadMoreLink)
        {
            Entry entry = Entry.ReadFromDBase(conn, trans, entryId, true, AssetStatus.New);
            if (entry == null)
            {
                return 0;
            }

            List<ImageFile> entryImages = ImageFile.ReadFromDBaseByAssetIDs(conn, trans, AssetType.Entry, entry.Id.ToString());

            return PostEntryActionOnFacebook(conn, trans, config, user, entry, entryImages, personalMessage, appendEntryDescToMessage, maxMessageLength, addReadMoreLink);
        }

        private long PostEntryActionOnFacebook(
            SqlConnection conn,
            SqlTransaction trans,
            Config config,
            UserAuthInfo user,
            Entry entry,
            List<ImageFile> entryImages,
            string personalMessage,
            bool appendEntryDescToMessage,
            int maxMessageLength,
            bool addReadMoreLink)
        {
            // it checks the cache first
            Topic topic = Topic.ReadById(conn, trans, entry.TopicId, true, AssetStatus.New);
            if (topic == null)
            {
                return 0;
            }

            // prepare item
            FacebookPostItem postItem = new FacebookPostItem();

            // link to entry (to topic indeed)
            // we don't include user.SplitId. Let GE handle it if it can. Otherwise, let it be random
            postItem.ItemLink = config.RootWebUrl + "/" + topic.SeoLink + "?e=" + entry.Id + "#f=1";

            // first image in the entry
            if (entryImages.Count > 0)
            {
                postItem.ItemPicture = entryImages[0].CloudUrl;
            }
            else
            {
                postItem.ItemPicture = config.RootWebUrl + "/Images/logo.png?version=0.81";
            }

            // title of the topic as caption
            postItem.ItemCaption = "shebeke.net"; // we reversed these two lines
            // postItem.ActorName = topic.Title + " #" + entry.Id.ToString();
            postItem.ActorName = topic.Title;

            // entry content as plain text
            EntryPlainTextFormatter plainTextFormatter = new EntryPlainTextFormatter();
            postItem.ItemDescription = plainTextFormatter.GetEncodedHtml(entry.Content, entryImages, config.RootWebUrl);
            if (postItem.ItemDescription.Length > 5000)
            {
                // this is for absolute max which is 10K in reality
                postItem.ItemDescription = postItem.ItemDescription.Substring(0, 5000);
            }

            postItem.PersonalMessage = personalMessage;

            if (appendEntryDescToMessage)
            {
                string desc = postItem.ItemDescription;
                if (desc.Length > maxMessageLength)
                {
                    desc = desc.Substring(0, maxMessageLength);
                    desc += "...";

                    if (addReadMoreLink)
                    {
                        desc += "\nread more on:\n" + postItem.ItemLink;
                    }
                }

                desc = personalMessage + "\n" + desc;
                postItem.PersonalMessage = desc.Trim();
            }

            string oauthToken = GetFacebookTokenOfCurrentUser(conn, trans, user);
            if (!String.IsNullOrWhiteSpace(oauthToken))
            {
                long result = FacebookHelpers.PostOnWall(postItem, oauthToken);
                try
                {
                    ActionLog.LogSocialShare(conn, trans, user.UserId, entry.TopicId, entry.Id, SocialChannel.Facebook);
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "PostEntryActionOnFacebook",
                        "LogSocialShare",
                        user.UserId,
                        user.SplitId,
                        ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                return result;
            }

            return 0;
        }

        private string GetFacebookTokenOfCurrentUser(SqlConnection conn, SqlTransaction trans, UserAuthInfo user)
        {
            string oauthToken = null;
            if (user.OAuthProvider == OAuthProvider.Facebook)
            {
                oauthToken = user.OAuthAccessToken;
            }
            else
            {
                UserToken ut = new UserToken();
                ut.UserId = user.UserId;
                ut.OAuthProvider = OAuthProvider.Facebook;
                if (Database.Select(ut, conn, trans, Database.TimeoutSecs))
                {
                    oauthToken = ut.OAuthAccessToken;
                }
            }

            return oauthToken;
        }

        private List<ImageFile> DeleteUnreferencedImagesFromDBase(
            SqlConnection conn, 
            SqlTransaction trans, 
            long userId, 
            List<long> referencedImageIds,
            List<ImageFile> deletedImages)
        {
            List<ImageFile> allocatedImages = ImageFile.ReadUnassignedImagesFromDBase(
                    conn, trans, userId, AssetType.Entry);

            for (int x = 0; x < allocatedImages.Count; x++)
            {
                ImageFile imageFile = allocatedImages[x];
                if (!referencedImageIds.Contains(imageFile.Id))
                {
                    // delete un-used image
                    imageFile.DeleteFromDBase(conn, trans, true, userId);
                    deletedImages.Add(imageFile);
                    allocatedImages.RemoveAt(x);
                    x--;
                }
            }

            return allocatedImages;
        }

        protected EntryListResponse _GetLatestEntries(EntryListRequest request, out UserAuthInfo user)
        {
            if (request == null)
            {
                throw new ShebekeArgumentException("Garip bir şeyler olmuş, talep içeriği null, biz de anlamadık!");
            }

            if (request.PageSize <= 0)
            {
                request.PageSize = 50;
            }

            if (request.PageIndex < 0)
            {
                request.PageIndex = 0;
            }

            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                List<Entry> entries = Entry.GetLatestActiveEntries(
                    conn, null, request.TopicId, user.UserId, request.PageSize, request.PageIndex, true, AssetStatus.New, request.IncludeEntryId);

                if (entries.Count > 0)
                {
                    string appPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
                    EntryFormatHelper.FormatEntriesAndAttachImages(conn, null, user, entries, appPath, true);
                }

                // UI'da reaction'lari re-usable yap. code-behind'dan repeater ile donersin.
                // sonra bu methodu test edersin. UI tarafinda yapilacaklari halledersin...

                EntryListResponse response = new EntryListResponse();
                response.Entries.AddRange(entries);
                return response;
            }
        }

        protected EntryListResponse _GetEntriesByNetVoteSum(EntryListRequest request, out UserAuthInfo user)
        {
            if (request == null)
            {
                throw new ShebekeArgumentException("Garip bir şeyler olmuş, talep içeriği null, biz de anlamadık!");
            }

            if (request.PageSize <= 0)
            {
                request.PageSize = 50;
            }

            if (request.PageIndex < 0)
            {
                request.PageIndex = 0;
            }

            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                List<Entry> entries = Entry.GetEntriesByNetVoteSum(
                    conn, null, request.TopicId, user.UserId, request.PageSize, request.PageIndex, true, AssetStatus.New, request.IncludeEntryId);

                if (entries.Count > 0)
                {
                    string appPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
                    EntryFormatHelper.FormatEntriesAndAttachImages(conn, null, user, entries, appPath, true);
                }

                // UI'da reaction'lari re-usable yap. code-behind'dan repeater ile donersin.
                // sonra bu methodu test edersin. UI tarafinda yapilacaklari halledersin...

                EntryListResponse response = new EntryListResponse();
                response.Entries.AddRange(entries);
                return response;
            }
        }

        protected BaseResponse _DeleteEntry(EntryRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (request == null)
            {
                throw new ShebekeArgumentException("Silme isteği bi garip, null diyeyim sen anla artık!");
            }

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.DeleteEntry);
                }

                List<ImageFile> images = ImageFile.ReadFromDBaseByAssetIDs(conn, null, AssetType.Entry, request.EntryId.ToString());

                bool checkUserOnDelete = true;
                if(user.UserType > UserType.Regular)
                {
                    // moderators, sys admins, etc don't need to be the owner to delete an entry
                    checkUserOnDelete = false;
                }

                Entry entry = new Entry();
                entry.Id = request.EntryId;
                entry.TopicId = request.TopicId;

                // delete from database first
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Exception exception = null;
                    try
                    {
                        // delete images
                        foreach (ImageFile image in images)
                        {
                            image.DeleteFromDBase(conn, trans, checkUserOnDelete, user.UserId);
                        }

                        // delete entry
                        entry.DeleteFromDBase(conn, trans, checkUserOnDelete, user.UserId);

                        // commit
                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        exception = ex;
                    }
                    finally
                    {
                        if (exception != null)
                        {
                            trans.Rollback();
                            throw exception;
                        }
                    }
                }

                // if we are here, that means database operations were fine
                // now delete images from cloud in a safe way
                foreach (ImageFile image in images)
                {
                    try
                    {
                        ImageFile.DeleteFromCloud(config.BlobStorage, image.CloudUrl);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_DeleteEntry",
                            "DeleteFromCloud",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());

                        Trace.WriteLine(errorLog, LogCategory.Error);
                    }
                }

                // update caches
                try
                {
                    Topic.UpdateCacheOnEntryAddOrDelete(entry.TopicId, false);
                    TopicInfo.UpdateCacheOnEntryAddOrDelete(entry.TopicId, false);
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_DeleteEntry",
                        "UpdateCacheOnEntryAddOrDelete",
                        user.UserId,
                        user.SplitId,
                        "Entry deleted from database but some caches couldn't be updated: " + entry.Id.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.DeleteEntry);
                }

                return new BaseResponse();
            }
        }

        protected TopicListResponse _SearchTopic(string query, out UserAuthInfo user)
        {
            user = null;
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ShebekeArgumentException("Sorgulama sözcüğü bi garip, null diyeyim sen anla artık!");
            }

            query = query.Trim();
            if (query.Length < 3)
            {
                throw new ShebekeArgumentException("Sorgulama sözcüğü çok kısa! '" + query + "' deyince olmuyor öyle!");
            }
            
            // finetune it for the dbase search
            // query = SpecialCharUtils.ReplaceSpecials(query, '_');

            // wrap query with sql like
            query = "%" + query + "%";

            // get config
            Config config = new Config();
            config.Init();

            // search
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                List<Topic> topics = Topic.SearchOnDBase(connection, null, query, 20, true, AssetStatus.New);

                //tweak some fields
                foreach (Topic topic in topics)
                {
                    topic.Title = StringHelpers.ToTitleCase2(topic.Title);
                }

                TopicListResponse response = new TopicListResponse();
                response.Topics.AddRange(topics);
                return response;
            }
        }

        protected SimpleResponse<Topic> _AddNewTopic(TopicRequest request, out UserAuthInfo user)
        {
            if (request == null)
            {
                throw new ShebekeArgumentException("Geçersiz başlık, veya onun gibi bişey!");
            }

            if (String.IsNullOrWhiteSpace(request.Title))
            {
                throw new ShebekeArgumentException("Başlığın uzunluğu 0! Oysaki bunu girememen lazımdı!");
            }

            if (request.Title.Length > Topic.MaxTopicTitle)
            {
                throw new ShebekeArgumentException("Başlık çok uzun yahu, insan okuyacak bunu!");
            }

            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.AddNewTopic);
                }

                // prepare topic
                Topic topic = new Topic();
                topic.Title = request.Title;
                topic.CreatedBy = user.UserId;

                // check to see if there is such a topic already
                // it checks the cache first
                Topic existing = Topic.ReadBySeoLink(conn, null, topic.SeoLink, false, AssetStatus.New);
                if (existing != null) {
                    throw new ShebekeException("Böyle bir başlık zaten var! Olmasa, lafı olmaz yani!");
                }

                // create
                topic.CreateOnDBase(conn, null);

                // tweak some fields
                topic.CanDelete = true;
                topic.Title = StringHelpers.ToTitleCase2(topic.Title);

                // share on social channels
                bool facebookTokenIsInvalid = false;
                if (request.ShareOnFacebook)
                {
                    try
                    {
                        ShareTopicOnFacebook(conn, null, topic, user, config);
                    }
                    catch (Exception ex)
                    {
                        // no need to re-throw
                        string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "ShebekeIWebSvcImpl.cs",
                            "_AddNewTopic",
                            "ShareTopicOnFacebook",
                            user.UserId,
                            user.SplitId,
                            ex.ToString());
                        
                        Trace.WriteLine(errorLog, LogCategory.Error);

                        if (ex is ShebekeAuthException)
                        {
                            facebookTokenIsInvalid = true;
                        }
                    }
                }

                // update caches
                try
                {
                    Topic.UpdateCacheOnTopicAdd(topic);
                    TopicInfo.UpdateCacheOnTopicAdd(topic);

                    if (request.ShareOnFacebook)
                    {
                        TopicInfo.UpdateCacheOnSocialActions(topic.Id, Shebeke.ObjectModel.Action.Share, 1); // always 1
                    }
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_AddNewTopic",
                        "UpdateCacheOnTopicAdd",
                        user.UserId,
                        user.SplitId,
                        "Topic added to the database but some caches couldn't be updated: " + topic.Id.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.AddNewTopic);
                }

                // return
                SimpleResponse<Topic> response = new SimpleResponse<Topic>();
                response.Data = topic;
                response.NeedsRelogin = facebookTokenIsInvalid;
                return response;
            }
        }

        protected SimpleListResponse<Topic> _GetRelatedTopics(TopicRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            if (request == null)
            {
                throw new ShebekeArgumentException("İlgili başlıklar talebi null geldi, nasıl olabildi ki? Neyse boşver!");
            }

            if (request.Id <= 0)
            {
                throw new ShebekeArgumentException("Geçersiz başlık numarası... Yaa böyle işte!");
            }

            // search
            int MAX_RELATED_TOPIC = 10;
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                List<Topic> relatedTopics = Topic.ReadRelatedTopics(connection, null, request.Id, MAX_RELATED_TOPIC);

                //tweak some fields
                foreach (Topic relatedTopic in relatedTopics)
                {
                    relatedTopic.Title = StringHelpers.ToTitleCase2(relatedTopic.Title);
                    //relatedTopic.Title = relatedTopic.Title.ToLower();
                }

                SimpleListResponse<Topic> response = new SimpleListResponse<Topic>();
                response.Items.AddRange(relatedTopics);
                return response;
            }
        }

        protected BaseResponse _DeleteTopic(TopicRequest request, out UserAuthInfo user)
        {
            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            if (request == null)
            {
                throw new ShebekeArgumentException("Başlık silme talebi bi garip, null diyeyim sen anla artık");
            }

            if (request.Id <= 0)
            {
                throw new ShebekeArgumentException("Silinmek istenen başlığın numarası geçersiz... Yaa böyle işte!");
            }

            // get file meta data
            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.DeleteTopic);
                }

                Topic topic = new Topic();
                topic.Id = request.Id;

                bool checkUserOnDelete = true;
                if (user.UserType > UserType.Regular)
                {
                    // moderators, sys admins, etc don't need to be the owner to delete an entry
                    checkUserOnDelete = false;
                }

                if (!topic.DeleteFromDBaseIfNoEntry(conn, null, checkUserOnDelete, user.UserId))
                {
                    throw new ShebekeException("İnsanlar bu başlığın altına bir şeyler eklemişler. Silsek ayıp olur!");
                }

                try
                {
                    Topic.UpdateCacheOnTopicDelete(topic.Id);
                    TopicInfo.UpdateCacheOnTopicDelete(topic.Id);
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_DeleteTopic",
                        "UpdateCacheOnTopicDelete",
                        user.UserId,
                        user.SplitId,
                        "Topic deleted from database but cache couldn't be updated: " + topic.Id.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.DeleteTopic);
                }

                return new BaseResponse();
            }
        }

        protected BaseResponse _SaveTopicInvitationRequest(TopicInvitationRequest request, out UserAuthInfo user)
        {
            if (request == null)
            {
                throw new ShebekeArgumentException("Davet kaydı talebi bi garip! Null yani, daha ne olsun!");
            }

            if (request.TopicId <= 0)
            {
                throw new ShebekeArgumentException("Geçersiz başlık numarası");
            }

            if (request.AppRequestId <= 0)
            {
                throw new ShebekeArgumentException("Geçersiz Facebook davet numarası");
            }

            if (request.InviteeCount <= 0)
            {
                throw new ShebekeArgumentException("Negatif sayıda kişiyi davet etmeyi nasıl başardın!");
            }

            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.SaveTopicInvitationRequest);
                }

                if (Topic.CreateAppRequestForTopicOnDBase(conn, null, request.AppRequestId, request.TopicId, request.EntryId, user.UserId, request.InviteeCount) > 0)
                {
                    String metricLog = String.Format(
                           CultureInfo.InvariantCulture,
                           // "[Version=1];[MetricName=FacebookShare];[Asset={0}];[ShareType=AppReqInvite];[ShareCount={1}];[UserId={2}];[TopicId={3}];[EntryId={4}]",
                           "[Version=2];[MetricName=FacebookShare];[Asset={0}];[ShareType=AppReqInvite];[ShareCount={1}];[UserId={2}];[TopicId={3}];[EntryId={4}];[Split={5}]",
                           (request.EntryId > 0 ? "Entry" : "Topic"),
                           request.InviteeCount,
                           user.UserId,
                           request.TopicId,
                           request.EntryId,
                           user.SplitId);

                    Trace.WriteLine(metricLog, LogCategory.Metric);
                }

                // update caches
                try
                {
                    TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.Invite, request.InviteeCount); // always 1
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_SaveTopicInvitationRequest",
                        "UpdateCacheOnSocialActions",
                        user.UserId,
                        user.SplitId,
                        "Invitation has been saved to the database but the cache couldn't be updated for topic: " + request.TopicId.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.SaveTopicInvitationRequest);
                }

                return new BaseResponse();
            }
        }

        protected BaseResponse _LogSocialShare(LogRequest request, out UserAuthInfo user)
        {
            if (request == null)
            {
                throw new ShebekeArgumentException("Sosyal paylaşım kütük kayıt talebi bi garip! Null diyeyim, sen anla!");
            }

            if (request.TopicId <= 0)
            {
                throw new ShebekeArgumentException("Geçersiz başlık numarası");
            }

            /*
            if (request.EntryId <= 0)
            {
                throw new ShebekeArgumentException("Invalid Entry Id");
            }
            */

            if (request.Channel == SocialChannel.None)
            {
                throw new ShebekeArgumentException("Geçersiz sosyal kanal! Bu ne be?");
            }

            Config config = new Config();
            config.Init();

            user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            String metricLog = String.Format(
                CultureInfo.InvariantCulture,
                // "[Version=1];[MetricName={0}Share];[Asset=Entry];[ShareType=ClientSideShare];[ShareCount=1];[UserId={1}];[TopicId={2}];[EntryId={3}]",
                "[Version=2];[MetricName={0}Share];[Asset=Entry];[ShareType=ClientSideShare];[ShareCount=1];[UserId={1}];[TopicId={2}];[EntryId={3}];[Split={4}]",
                request.Channel,
                user.UserId,
                request.TopicId,
                request.EntryId,
                user.SplitId);

            Trace.WriteLine(metricLog, LogCategory.Metric);

            // don't write to the database
            if (user.UserId <= 0)
            {
                return new BaseResponse();
            }

            using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
            {
                conn.Open();

                if (user.UserId > 0)
                {
                    this.CheckDenialOfService(conn, null, user.UserId, UserActivityType.LogSocialShare);
                }

                ActionLog.LogSocialShare(conn, null, user.UserId, request.TopicId, request.EntryId, request.Channel);

                // update caches
                try
                {
                    TopicInfo.UpdateCacheOnSocialActions(request.TopicId, Shebeke.ObjectModel.Action.Share, 1); // always 1
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "ShebekeIWebSvcImpl.cs",
                        "_LogSocialShare",
                        "UpdateCacheOnSocialActions",
                        user.UserId,
                        user.SplitId,
                        "Social share has been saved to the database but the cache couldn't be updated for topic: " + request.TopicId.ToString() + ". More Details: " + ex.ToString());

                    Trace.WriteLine(errorLog, LogCategory.Error);
                }

                if (user.UserId > 0)
                {
                    this.LogUserActivity(conn, null, user.UserId, UserActivityType.LogSocialShare);
                }

                return new BaseResponse();
            }
        }
    }
}