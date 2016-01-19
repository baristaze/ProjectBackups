using System;
using System.Text;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

using Shebeke.ObjectModel;
using System.Collections.Generic;

namespace Shebeke.Web
{
    public class ShebekeWebPage : System.Web.UI.Page
    {
        public ShebekeWebPage()
        {
 
        }

        protected void SetApplicationPath()
        {
            HtmlGenericControl ctrl = this.Page.FindControl("applicationPath") as HtmlGenericControl;
            if (ctrl != null)
            {
                ctrl.InnerHtml = this.Request.ApplicationPath.TrimEnd('/');
            }
        }

        protected void SetIsAuthenticatedFlag(UserAuthInfo user)
        {
            string result = "0";
            if (user != null && user.UserId > 0)
            {
                result = "1";
            }

            HtmlGenericControl ctrl = this.Page.FindControl("isAuthenticated") as HtmlGenericControl;
            if (ctrl != null)
            {
                ctrl.InnerText = result;
            }
        }

        protected UserAuthInfo SetCurrentUserInfo(UserAuthInfo user)
        {
            if (user != null)
            {
                HtmlGenericControl nameCtrl = this.Page.FindControl("currentUserName") as HtmlGenericControl;
                if (nameCtrl != null)
                {
                    nameCtrl.InnerText = user.FirstName;
                }

                HtmlGenericControl nameCtrl2 = this.Page.FindControl("currentUserName2") as HtmlGenericControl;
                if (nameCtrl2 != null)
                {
                    nameCtrl2.InnerText = user.FirstName;
                }

                HtmlImage photoCtrl = this.Page.FindControl("currentUserPhoto") as HtmlImage;
                if (photoCtrl != null)
                {
                    photoCtrl.Src = user.PhotoUrl;
                }
            }

            return user;
        }
        
        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV)
        {
            return GetUserInfoFromContent(secretKey, secretIV, true);
        }

        /// <summary>
        /// Gets the user info or throws exception
        /// </summary>
        /// <returns>UserAuthInfo</returns>
        protected UserAuthInfo GetUserInfoFromContent(string secretKey, string secretIV, bool throwOnNoAuth)
        {
            UserAuthInfo theUser = new UserAuthInfo()
            {
                UserId = 0,
                UserType = UserType.Guest,
                FirstName = "Misafir",
                PhotoUrl = HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Images/user.png",
            };

            Exception theException = null;
            HttpCookie authCookie = HttpContext.Current.Request.Cookies["XAuthToken"];
            if (authCookie != null && !String.IsNullOrWhiteSpace(authCookie.Value))
            {
                string token = authCookie.Value;
                string decrypt = Crypto.DecryptAES(token, secretKey, secretIV);
                try
                {
                    theUser = UserAuthInfo.Parse(decrypt);
                }
                catch (ShebekeArgumentException ex)
                {
                    theException = ex;
                }

                if (theUser.UserId > 0)
                {
                    string blockedUserCacheKey = "BlockedUser_" + theUser.UserId.ToString();
                    if (CacheHelper.GetOrDefault<int>(CacheHelper.CacheName_LongLastingObjects, blockedUserCacheKey) > 0)
                    {
                        theException = new ShebekeAuthException(
                            "Hesabınız geçici bir süre bloke edilmiştir. Lütfen sistem yöneticisine (contact@shebeke.net) başvurun.");
                    }
                }
            }
            else
            {
                theException = new ShebekeAuthException("Erişim hakkın yok, veya onun gibi bişey!");
            }

            if (theException == null)
            {
                String metricLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[MetricName=AuthenticatedCall];[Source=Service];[UserId={0}];[FacebookUserId={1}]",
                    theUser.UserId,
                    theUser.OAuthUserId);

                System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);

                return theUser;
            }
            else if (!throwOnNoAuth)
            {
                String metricLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[MetricName=AnonymousCall];[Source=Service];[UserId={0}];[FacebookUserId={1}]",
                    0,
                    "");

                System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);

                return theUser; // the guest
            }

            throw theException;
        }

        protected virtual string GetSplitSectionFilters() {
            return "0";
        }

        protected string GetCssForSplitTesting()
        {
            string sections = this.GetSplitSectionFilters();
            string parameters = "&rid=" + Guid.NewGuid().ToString("N");
            string split = this.Request.Params["split"];
            if (!String.IsNullOrWhiteSpace(split))
            {
                parameters = "?sections=" + sections + "&split=" + split.Trim() + parameters;
            }
            else
            {
                parameters = "?sections=" + sections + parameters;
            }

            return "\"" + this.Request.ApplicationPath.TrimEnd('/') + "/StyleHandlers/CssForSplitTesting.ashx" + parameters + "\"";
        }

        protected void AdjustSplitTestData(SqlConnection conn, SqlTransaction trans, UserAuthInfo user)
        {
            // determine the split id
            string splitText = this.Request.Params["split"];
            int theSplitId = Splitter.DetermineSplitId(user, -1, splitText);

            // sanitize the input
            string sections = this.GetSplitSectionFilters();
            sections = Splitter.SanitizeSectionFilters(sections);

            // it checks the cache first. it reads split info from db if the cache is not available.
            TextSplitterList splitterList = TextSplitter.Read(conn, trans, theSplitId, sections);

            string clientSideResources = string.Empty;
            string clientResourceTemplate = "<div class='splitResourceItem'><div class='splitResourceSelector'>{0}</div><div class='splitResourceFeed'>{1}</div></div>";
            foreach (TextSplitter splitter in splitterList)
            {
                if (splitter.SideToApply == Side.Server)
                {
                    Control ctrl = this.FindControl(splitter.JQSelector);
                    if (ctrl != null)
                    {
                        HtmlContainerControl container = ctrl as HtmlContainerControl;
                        if (container != null)
                        {
                            // splitter.ReplacePlaceholders();
                            container.InnerHtml = splitter.Value;
                        }
                    }
                }
                else if (splitter.SideToApply == Side.Client)
                {
                    if (!String.IsNullOrWhiteSpace(splitter.JQSelector))
                    {
                        // splitter.ReplacePlaceholders();
                        string res = String.Format(
                            CultureInfo.InvariantCulture,
                            clientResourceTemplate,
                            HttpUtility.HtmlEncode(splitter.JQSelector),
                            HttpUtility.HtmlEncode(splitter.Value));

                        clientSideResources += res;
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(clientSideResources))
            {
                Control ctrl = this.FindControl("splitResources");
                if (ctrl != null)
                {
                    HtmlContainerControl container = ctrl as HtmlContainerControl;
                    if (container != null)
                    {
                        container.InnerHtml = clientSideResources;
                    }
                }
            }
            // splitResources
        }

        protected virtual bool CheckFacebookRedirect(SqlConnection connection, SqlTransaction trans, UserAuthInfo user, int defaultSplitId)
        {
            string facebookReqIds = this.Request.QueryString["request_ids"];
            if (!String.IsNullOrWhiteSpace(facebookReqIds))
            {
                List<long> reqIds = ShebekeUtils.TokenizeIDs(facebookReqIds);
                if (reqIds.Count > 0)
                {
                    string concat = String.Join(",", reqIds);
                    RedirectInfo redirectInfo = RedirectInfo.ReadFromDBaseByAppRequestIds(connection, trans, concat, true, AssetStatus.New);
                    if (redirectInfo != null)
                    {
                        Topic topic = new Topic();
                        topic.Title = redirectInfo.TopicTitle;

                        System.Diagnostics.Trace.WriteLine("Redirecting for SEO since it is from facebook", LogCategory.Info);

                        // we won't get split-id and experiment-id from an invitation
                        // still, we perform this action for just in case...
                        string delim = "?";
                        string link = this.Request.ApplicationPath.TrimEnd('/') + "/" + topic.SeoLink;
                        
                        link = Splitter.AppendSplitId(
                            user, defaultSplitId, redirectInfo.SenderSplitId, link, this.Request.QueryString, ref delim);

                        // don't specify experiment id here since the experiment might have been stopped. 
                        // The client-script is capable to detect it but server-side is not capable to do so.
                        // we will have one more re-direct and it is OK.
                        // link = Splitter.AppendExperimentId(link, this.Request.QueryString, true, "76168981-0", ref delim);
                        link = Splitter.AppendExperimentId(link, this.Request.QueryString, false, null, ref delim);

                        // this part is more important
                        link += delim + "e=" + redirectInfo.EntryId.ToString();
                        if (redirectInfo.SentBy != user.UserId)
                        {
                            string photoUrl = redirectInfo.SenderPhotoUrl;
                            if (String.IsNullOrWhiteSpace(photoUrl))
                            {
                                photoUrl = String.Format(
                                    CultureInfo.InvariantCulture, 
                                    "https://graph.facebook.com/{0}/picture?type=small",
                                    redirectInfo.SenderFacebookId);
                            }

                            link += "#f=1"
                                + "&u=" + redirectInfo.SenderFacebookId.ToString()
                                + "&n=" + HttpUtility.UrlEncode(redirectInfo.SenderName)
                                + "&p=" + HttpUtility.UrlEncode(photoUrl);
                        }

                        this.Response.Redirect(link);
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual string GetLinkUponFacebookRedirect(SqlConnection connection, SqlTransaction trans, UserAuthInfo user, int defaultSplitId)
        {
            string facebookReqIds = this.Request.QueryString["request_ids"];
            if (!String.IsNullOrWhiteSpace(facebookReqIds))
            {
                List<long> reqIds = ShebekeUtils.TokenizeIDs(facebookReqIds);
                if (reqIds.Count > 0)
                {
                    string concat = String.Join(",", reqIds);
                    RedirectInfo redirectInfo = RedirectInfo.ReadFromDBaseByAppRequestIds(connection, trans, concat, true, AssetStatus.New);
                    if (redirectInfo != null)
                    {
                        Topic topic = new Topic();
                        topic.Title = redirectInfo.TopicTitle;

                        System.Diagnostics.Trace.WriteLine("Redirecting for SEO since it is from facebook", LogCategory.Info);

                        // we won't get split-id and experiment-id from an invitation
                        // still, we perform this action for just in case...
                        string delim = "?";
                        string link = this.Request.ApplicationPath.TrimEnd('/') + "/" + topic.SeoLink;

                        link = Splitter.AppendSplitId(
                            user, defaultSplitId, redirectInfo.SenderSplitId, link, this.Request.QueryString, ref delim);

                        // don't specify experiment id here since the experiment might have been stopped. 
                        // The client-script is capable to detect it but server-side is not capable to do so.
                        // we will have one more re-direct and it is OK.
                        // link = Splitter.AppendExperimentId(link, this.Request.QueryString, true, "76168981-0", ref delim);
                        link = Splitter.AppendExperimentId(link, this.Request.QueryString, false, null, ref delim);

                        // this part is more important
                        link += delim + "e=" + redirectInfo.EntryId.ToString();
                        if (redirectInfo.SentBy != user.UserId)
                        {
                            string photoUrl = redirectInfo.SenderPhotoUrl;
                            if (String.IsNullOrWhiteSpace(photoUrl))
                            {
                                photoUrl = String.Format(
                                    CultureInfo.InvariantCulture,
                                    "https://graph.facebook.com/{0}/picture?type=small",
                                    redirectInfo.SenderFacebookId);
                            }

                            link += "#f=1"
                                + "&u=" + redirectInfo.SenderFacebookId.ToString()
                                + "&n=" + HttpUtility.UrlEncode(redirectInfo.SenderName)
                                + "&p=" + HttpUtility.UrlEncode(photoUrl);
                        }

                        return link;
                    }
                }
            }

            return null;
        }
    }
}