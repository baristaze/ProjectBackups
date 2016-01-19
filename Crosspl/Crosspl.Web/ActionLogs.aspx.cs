using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using Crosspl.ObjectModel;

namespace Crosspl.Web
{
    public partial class ActionLogs : CrossplWebPage
    {   
        protected void Page_Load(object sender, EventArgs e)
        {
            Config config = new Config();
            config.Init();

            UserAuthInfo user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);
            this.SetCurrentUserInfo(user);
            this.SetApplicationPath();

            if (user.UserId == 0)
            {
                this.interactionBlock.Style.Add("display", "none");
                this.dataBlock.Style.Add("display", "none");
                this.errorMessage.InnerText = "You haven't logged in yet. Use another browser tab (not a separate window) to connect " + 
                    "http://vizibuzz.com as a login procedure. Then, come back and refresh this page!";
            }
            else if (user.UserType <= UserType.Regular)
            {
                this.interactionBlock.Style.Add("display", "none");
                this.dataBlock.Style.Add("display", "none");
                this.errorMessage.InnerText = user.FirstName + ", you must be an admin to view this page! Please contact system administrator!";
            }
            else
            {
                int top = 20;
                string topText = this.Request.QueryString["top"];
                if (!String.IsNullOrWhiteSpace(topText))
                {
                    int temp = 0;
                    if (Int32.TryParse(topText, out temp) && temp > 0)
                    {
                        top = temp;
                    }
                }

                if (top > 500)
                {
                    top = 500;
                }

                string ridText = this.Request.QueryString["rid"];
                if (!String.IsNullOrWhiteSpace(ridText))
                {
                    string url = this.Request.ApplicationPath.TrimEnd('/') + "/admin/logs/" + top.ToString();
                    this.Response.Redirect(url);
                    return;
                }
                else
                {
                    this.errorMessage.Style.Add("display", "none");
                    this.LoadTable(config, user, top);
                }
            }
        }

        protected void LoadTable(Config config, UserAuthInfo user, int top)
        {
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                // it checks the cache first
                int defaultSplitId = Splitter.GetDefaultSplitId(connection, null);

                List<ActionLog> logs = ActionLog.ReadFromDBase(
                    connection, null, DateTime.UtcNow.AddDays(-3), DateTime.UtcNow, top, false, 0);

                string delim = "";
                string urlPostFix = Splitter.AppendSplitId(user, defaultSplitId, -1, "", this.Request.QueryString, ref delim);
                // urlPostFix = Splitter.AppendExperimentId(urlPostFix, this.Request.QueryString, true, "70062152-6", ref delim);
                urlPostFix = Splitter.AppendExperimentId(urlPostFix, this.Request.QueryString, false, null, ref delim);

                List<ActionLogView>  logViews = new List<ActionLogView>();
                foreach(ActionLog log in logs)
                {
                    logViews.Add(new ActionLogView(user, log, config.RootWebUrl, urlPostFix, config.ReferenceTimeZone, config.LocalTimeFormat, 40));
                }

                this.activityLogTableRepeater.DataSource = logViews;
                this.activityLogTableRepeater.DataBind();
            }
        }
    }
}