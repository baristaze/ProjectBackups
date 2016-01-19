using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Shebeke.ObjectModel;
using System.Data.SqlClient;

namespace Shebeke.Web
{
    public partial class FBCanvas : ShebekeWebPage
    {
        protected override string GetSplitSectionFilters()
        {
            // means nothing
            return "0";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            string facebookReqIds = this.Request.QueryString["request_ids"];
            if (!String.IsNullOrWhiteSpace(facebookReqIds))
            {
                this.redirectLink.Attributes["href"] = "http://shebeke.net/?request_ids=" + facebookReqIds;
            }
            */

            Config config = new Config();
            config.Init();

            UserAuthInfo user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            try
            {
                this.ProcessRequest(config, user);
            }
            catch (Exception ex)
            {
                // no need to re-throw
                string errorLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                    "FBCanvas.aspx.cs",
                    "Page_Load",
                    "ProcessRequest",
                    user.UserId,
                    user.SplitId,
                    ex.ToString());

                System.Diagnostics.Trace.WriteLine(errorLog, LogCategory.Error);
            }            
        }

        protected void ProcessRequest(Config config, UserAuthInfo user)
        {
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                // it checks the cache first
                int defaultSplitId = Splitter.GetDefaultSplitId(connection, null);
                string redirectUrl = GetLinkUponFacebookRedirect(connection, null, user, defaultSplitId);
                if (String.IsNullOrWhiteSpace(redirectUrl))
                {
                    Topic topic = this.GetReferencedTopic(connection, null);
                    if (topic != null)
                    {
                        // topic has been specified
                        string delim = "?";
                        string link = this.Request.ApplicationPath.TrimEnd('/') + "/" + topic.SeoLink;
                        link = Splitter.AppendSplitId(user, defaultSplitId, -1, link, this.Request.QueryString, ref delim);
                        // don't specify experiment id here since the experiment might have been stopped. 
                        // The client-script is capable to detect it but server-side is not capable to do so.
                        // we will have one more re-direct and it is OK.
                        // link = Splitter.AppendExperimentId(link, this.Request.QueryString, true, "76168981-0", ref delim);
                        link = Splitter.AppendExperimentId(link, this.Request.QueryString, false, null, ref delim);

                        redirectUrl = link;
                    }
                }

                if (String.IsNullOrWhiteSpace(redirectUrl))
                {
                    redirectUrl = "http://shebeke.net";
                }

                redirectUrl = redirectUrl.Trim();
                string httpsPrefix = "https://";
                if (redirectUrl.StartsWith(httpsPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    redirectUrl = "http://" + redirectUrl.Substring(httpsPrefix.Length, redirectUrl.Length - httpsPrefix.Length);
                }

                string domainPrefix = "http://shebeke.net";
                if (!redirectUrl.StartsWith(domainPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (redirectUrl[0] == '/')
                    {
                        redirectUrl = domainPrefix + redirectUrl;
                    }
                    else
                    {
                        redirectUrl = domainPrefix + "/" + redirectUrl;
                    }
                }

                this.redirectLink.Attributes["href"] = redirectUrl;
                this.redirectLink2.Attributes["href"] = redirectUrl;

                this.LogPageVisit(user);                
                this.SetApplicationPath();
                this.SetIsAuthenticatedFlag(user);
            }
        }

        protected Topic GetReferencedTopic(SqlConnection connection, SqlTransaction trans)
        {
            long referencedTopicId = -1;
            string topicIdText = this.Request.QueryString["topicId"];
            if (long.TryParse(topicIdText, out referencedTopicId) && referencedTopicId > 0)
            {
                // it checks the cache first
                return Topic.ReadById(connection, trans, referencedTopicId, true, AssetStatus.New);
            }

            return null;
        }

        protected void LogPageVisit(UserAuthInfo user)
        {
            String metricLog = String.Format(
                CultureInfo.InvariantCulture,
                "[Version=2];[MetricName=PageVisit];[Page=FBCanvas];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}];[Split={4}]",
                "Web",
                user.UserId,
                0,
                0,
                user.SplitId);

            System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);
        }
    }
}