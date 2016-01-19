using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

using Crosspl.ObjectModel;
using System.Web.UI.HtmlControls;

namespace Crosspl.Web
{
    public partial class UserActivities : CrossplWebPage
    {
        protected override string GetSplitSectionFilters()
        {
            // 15: 'user activities'
            return "15";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                    "UserActivities.aspx.cs",
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

                this.LogPageVisit(user);
                this.AdjustSplitTestData(connection, null, user);
                this.SetApplicationPath();
                this.SetIsAuthenticatedFlag(user);
                this.SetCurrentUserInfo(user);

                long targetUserId = user.UserId;

                long tempUserId = 0;
                string tempUserIdText = this.Request.QueryString["userId"];
                if (long.TryParse(tempUserIdText, out tempUserId) && tempUserId > 0)
                {
                    targetUserId = tempUserId;
                }

                FacebookUser targetUser = null;
                if (targetUserId > 0)
                {
                    List<FacebookUser> userList = FacebookUser.ReadAllByID(connection, null, targetUserId.ToString());
                    if (userList.Count > 0)
                    {
                        targetUser = userList[0];
                    }
                }

                if (targetUser != null)
                {
                    string name = targetUser.FirstName + " " + targetUser.LastName;
                    this.targetUserName.InnerText = name;
                    this.pageTitle.Text = name;

                    HtmlAnchor anchorCtrl = this.Page.FindControl("targetUserFacebookLink") as HtmlAnchor;
                    if (anchorCtrl != null)
                    {
                        anchorCtrl.HRef = targetUser.FacebookLink;
                    }

                    HtmlImage photoCtrl = this.Page.FindControl("targetUserImage") as HtmlImage;
                    if (photoCtrl != null)
                    {
                        // photoCtrl.Src = targetUser.GetPhotoUrlOrDefault(config.RootWebUrl);
                        string photoUrl = "https://graph.facebook.com/" + targetUser.FacebookId.ToString() + "/picture?type=large";
                        photoCtrl.Src = photoUrl;
                    }

                    this.LoadTable(connection, null, config, defaultSplitId, user, 100, targetUserId);
                }
            }
        }

        protected void LoadTable(SqlConnection connection, SqlTransaction trans, Config config, int defaultSplitId, UserAuthInfo user, int top, long targetUserId)
        {
            DateTime since = DateTime.UtcNow.AddDays(-1 * config.UserActivity_LastXDays);
            List<ActionLog> logs = ActionLog.ReadFromDBase(connection, trans, since, DateTime.UtcNow, top, true, targetUserId);

            string delim = "";
            string urlPostFix = Splitter.AppendSplitId(user, defaultSplitId, -1, "", this.Request.QueryString, ref delim);
            // urlPostFix = Splitter.AppendExperimentId(urlPostFix, this.Request.QueryString, true, "70062152-6", ref delim);
            urlPostFix = Splitter.AppendExperimentId(urlPostFix, this.Request.QueryString, false, null, ref delim);

            List<ActionLogView> logViews = new List<ActionLogView>();
            foreach (ActionLog log in logs)
            {
                logViews.Add(new ActionLogViewLight(user, log, config.RootWebUrl, urlPostFix, config.ReferenceTimeZone, config.LocalTimeFormat, 60));
            }

            this.activityLogTableRepeater.DataSource = logViews;
            this.activityLogTableRepeater.DataBind();
        } 

        protected void LogPageVisit(UserAuthInfo user)
        {
            String metricLog = String.Format(
                CultureInfo.InvariantCulture,
                "[Version=2];[MetricName=PageVisit];[Page=UserActivities];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}];[Split={4}]",
                "Web",
                user.UserId,
                0,
                0,
                user.SplitId);

            System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);
        }
    }
}