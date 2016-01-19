using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

using Facebook;

namespace Crosspl.ObjectModel
{
    public class FacebookHelpers
    {           
        public static FacebookUser GetUserFromFacebook(string accessToken)
        {
            return GetUserFromFacebook(accessToken, null);
        }

        public static FacebookUser GetUserFromFacebook(string accessToken, long? fbUserIdToCompare)
        {
            // verify user
            string fields = "id,name,first_name,last_name,link,birthday,hometown,location,gender,email,timezone,locale,verified,picture";
            dynamic me = FacebookGetCall("me?fields=" + fields, accessToken);

            if (me != null)
            {   
                FacebookUser user = FacebookUser.CreateFacebookUser(me);

                if (fbUserIdToCompare.HasValue)
                {
                    // check is enforced
                    if (user.FacebookId != fbUserIdToCompare.Value)
                    {
                        throw new CrossplException("Facebook userId doesn't match");
                    }
                }

                return user;
            }
            else
            {
                throw new CrossplException("We couldn't get the user info from Facebook");
            }
        }

        public static NameIdPairList GetFriendsListFromFacebook(string accessToken)
        {
            dynamic friends = FacebookGetCall("me/friends", accessToken);

            if (friends != null && friends.data != null)
            {
                NameIdPairList list = new NameIdPairList();
                for (int x = 0; x < friends.data.Count; x++)
                {
                    NameIdPair friend = ConvertToNameIdPair(friends.data[x]);
                    list.Add(friend);
                }
                
                return list;
            }
            else
            {
                throw new CrossplException("We couldn't get the user's friends from Facebook");
            }
        }

        public static NameIdPair ConvertToNameIdPair(dynamic obj)
        {
            NameIdPair pair = new NameIdPair();
            pair.Name = obj.name;
            if (!String.IsNullOrWhiteSpace(obj.id))
            {
                pair.Id = Int64.Parse(obj.id.ToString());
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
                            Trace.WriteLine("PostOnWall->concatenatedId->tokens.Length != 2", LogCategory.Error);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("PostOnWall->concatenatedId == (null | empty)", LogCategory.Error);
                    }
                }
                else
                {
                    string log = "PostOnWall->postResult.id == (null | empty) where postResult = " + postResult.ToString();
                    Trace.WriteLine(log, LogCategory.Error);
                }
            }
            else
            {
                Trace.WriteLine("PostOnWall->postResult == (null | empty)", LogCategory.Error);
            }

            return result;
        }

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
                throw new CrossplAuthException("Facebook AuthToken is not valid or expired.", e);
            }
            catch (FacebookApiLimitException e)
            {
                throw new CrossplException("Facebook API call limit is reached. Try again later.", e);
            }
            catch (FacebookApiException e)
            {
                throw new CrossplException("Unknown Facebook API exception.", e);
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
                throw new CrossplAuthException("Facebook AuthToken is not valid or expired.", e);
            }
            catch (FacebookApiLimitException e)
            {
                throw new CrossplException("Facebook API call limit is reached. Try again later.", e);
            }
            catch (FacebookApiException e)
            {
                throw new CrossplException("Unknown Facebook API exception.", e);
            }

            return postResult;
        }
    }
}
