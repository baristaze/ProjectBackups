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
    public class FeedbacksGetRecentBulk : YummyZoneHttpHandlerJson<FeedbackListResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                                
                DateTime previousCheckinTime = this.GetMandatoryDateTime(context, "time", "previous checkin time", Source.Url);

                FeedbackList fbkList = Feedback.GetRecentFeedbacks(
                    connection, identity.VenueId, previousCheckinTime, identity.UserId, Database.TimeoutSecs);
                                
                FeedbackListResponse response = new FeedbackListResponse();
                response.Items.AddRange(fbkList);
                return response;
            }
        }
    }

    [DataContract]
    public class FeedbackListResponse : BaseJsonResponse
    {
        [DataMember]
        public FeedbackList Items { get { return this.items; } }
        private FeedbackList items = new FeedbackList();
    }
}