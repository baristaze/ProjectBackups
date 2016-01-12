using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class FeedbacksGetNewCount : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            Guid latestCheckinId = this.GetGuid(context, "chid", "latest checkin is", Source.Url);
            if (latestCheckinId != Guid.Empty)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {   
                    connection.Open();

                    FeedbackList fbkList = Feedback.GetRecentFeedbacks(
                        connection, identity.VenueId, null, identity.UserId, Database.TimeoutSecs);

                    int newCount = 0;   
                    foreach (Feedback fbk in fbkList)
                    {
                        if (fbk.CheckInId == latestCheckinId)
                        {
                            break;
                        }

                        newCount++;
                    }

                    return newCount;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}