using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

using Facebook;

namespace Pic4Pic.ObjectModel
{
    public class FacebookHelpers
    {
        public static FacebookUser GetUserFromFacebook(string accessToken)
        { 
            WorkHistory workHistory = null;
            EducationHistory eduHistory = null;
            return GetUserFromFacebook(accessToken, out workHistory, out eduHistory);
        }

        public static FacebookUser GetUserFromFacebook(string accessToken, out WorkHistory workHistory, out EducationHistory eduHistory)
        {
            return GetUserFromFacebook(accessToken, null, out workHistory, out eduHistory);
        }

        public static FacebookUser GetUserFromFacebook(string accessToken, long? fbUserIdToCompare, out WorkHistory workHistory, out EducationHistory eduHistory)
        {
            // verify user
            string fields = "id,name,first_name,last_name,link,username,birthday,hometown,location,gender,email,timezone,locale,verified,picture,work,education,religion,political,relationship_status";
            dynamic me = FacebookGetCall("me?fields=" + fields, accessToken);

            if (me != null)
            {
                FacebookUser user = FacebookUser.CreateFacebookUser(me, out workHistory, out eduHistory);

                if (fbUserIdToCompare.HasValue)
                {
                    // check is enforced
                    if (user.FacebookId != fbUserIdToCompare.Value)
                    {
                        throw new Pic4PicException("Facebook userId doesn't match");
                    }
                }

                return user;
            }
            else
            {
                throw new Pic4PicException("We couldn't get the user info from Facebook");
            }
        }

        public static NameLongIdPairList GetFriendsListFromFacebook(string accessToken)
        {
            dynamic friends = FacebookGetCall("me/friends", accessToken);

            if (friends != null && friends.data != null)
            {
                NameLongIdPairList list = new NameLongIdPairList();
                for (int x = 0; x < friends.data.Count; x++)
                {
                    NameLongIdPair friend = ConvertToNameIdPair(friends.data[x]);
                    list.Add(friend);
                }
                
                return list;
            }
            else
            {
                throw new Pic4PicException("We couldn't get the user's friends from Facebook");
            }
        }

        public static NameLongIdPair ConvertToNameIdPair(dynamic obj)
        {
            return ConvertToNameIdPair(obj, false);
        }

        public static NameLongIdPair ConvertToNameIdPair(dynamic obj, bool requireAll)
        {
            if (obj == null)
            {
                return null;
            }

            NameLongIdPair pair = new NameLongIdPair();
            if (obj.name != null && !String.IsNullOrWhiteSpace(obj.name))
            {
                pair.Name = obj.name;
            }
            else if (requireAll)
            {
                return null;
            }

            if (!String.IsNullOrWhiteSpace(obj.id))
            {
                pair.Id = Int64.Parse(obj.id.ToString());
            }
            else if (requireAll)
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(pair.Name) && pair.Id == 0)
            {
                return null;
            }

            return pair;
        }

        public static UserToken ExtendAcessToken(string appId, string appSecret, string shortLivedToken)
        {
            string path = String.Format(
                CultureInfo.InvariantCulture,
                "oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}",
                appId,
                appSecret,
                shortLivedToken);

            dynamic response = FacebookGetCall(path, shortLivedToken);

            if (response == null || response.access_token == null || response.expires == null)
            {
                return null;
            }

            UserToken token = new UserToken();
            token.OAuthAccessToken = response.access_token;

            int expirationSeconds = Int32.Parse(response.expires.ToString());
            token.ExpireTimeUTC = DateTime.UtcNow.AddSeconds(expirationSeconds);

            return token;
        }

        // Posts on Facebook wall
        /*
        public static long PostOnWall(FacebookPostItem post, string accessToken)
        {
            dynamic postItem = post.GetAsDynamicObject();
            dynamic postResult = FacebookPostCall("me/feed", accessToken, postItem);

            long result = -1;
            if (postResult != null)
            {
                if (postResult.id != null)
                {
                    string concatenatedId = postResult.id.ToString();
                    if (!String.IsNullOrWhiteSpace(concatenatedId))
                    {
                        string[] tokens = concatenatedId.Split('_');
                        if (tokens.Length == 2)
                        {
                            long.TryParse(tokens[1], out result);
                        }
                        else
                        {
                            Logger.bag().LogError("PostOnWall->concatenatedId->tokens.Length != 2");
                        }
                    }
                    else
                    {
                        Logger.bag().LogError("PostOnWall->concatenatedId == (null | empty)");
                    }
                }
                else
                {
                    string log = "PostOnWall->postResult.id == (null | empty) where postResult = " + postResult.ToString();
                    Logger.bag().LogError(log);
                }
            }
            else
            {
                Logger.bag().LogError("PostOnWall->postResult == (null | empty)");
            }

            return result;
        }
        */
        protected static dynamic FacebookGetCall(string graphPath, string accessToken)
        {
            // verify user
            dynamic obj = null;

            try
            {
                FacebookClient client = new FacebookClient(accessToken);
                obj = client.Get(graphPath);
            }
            catch (FacebookOAuthException e)
            {
                throw new Pic4PicAuthException("Facebook AuthToken is not valid or expired.", e);
            }
            catch (FacebookApiLimitException e)
            {
                throw new Pic4PicException("Facebook API call limit is reached. Try again later.", e);
            }
            catch (FacebookApiException e)
            {
                throw new Pic4PicException("Unknown Facebook API exception.", e);
            }

            return obj;
        }

        protected static dynamic FacebookPostCall(string graphPath, string accessToken, object postItem)
        {
            // verify user
            dynamic postResult = null;

            try
            {
                FacebookClient client = new FacebookClient(accessToken);
                postResult = client.Post(graphPath, postItem);
            }
            catch (FacebookOAuthException e)
            {
                throw new Pic4PicAuthException("Facebook AuthToken is not valid or expired.", e);
            }
            catch (FacebookApiLimitException e)
            {
                throw new Pic4PicException("Facebook API call limit is reached. Try again later.", e);
            }
            catch (FacebookApiException e)
            {
                throw new Pic4PicException("Unknown Facebook API exception.", e);
            }

            return postResult;
        }
    }
}
