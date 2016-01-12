using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Feedbacks : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = this.UpdateIdentitySection();
            FeedbackList fbkList = new FeedbackList();

            if (identity != null)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();

                    fbkList = Feedback.GetRecentFeedbacks(
                        connection, identity.VenueId, null, identity.UserId, Database.TimeoutSecs);
                }
            }

            this.feedBackRepeater.DataSource = fbkList;
            this.feedBackRepeater.DataBind();
        }

        public string GetShowHideCssClass(object show, bool reverse)
        {
            if (!reverse)
            {
                return ((bool)show) ? "" : "hidden";
            }
            else
            {
                return ((bool)show) ? "hidden" : "";
            }
        }

        public string GetReadCssClass(object isRead)
        {
            return ((bool)isRead) ? "" : "fbkUnread";
        }
    }
}