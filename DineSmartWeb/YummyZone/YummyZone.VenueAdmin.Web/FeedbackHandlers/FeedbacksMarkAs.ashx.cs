using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class FeedbacksMarkAs : YummyZoneHttpHandler
    {
        private const int MarkAsRead = 1;
        private const int MarkAsUnRead = 2;
        private const int MarkAsSpam = 3;
        private const int MarkAsHidden = 4;

        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            Guid checkinId = this.GetMandatoryGuid(context, "chid", "Checkin Id", Source.Url);
            int action = this.GetMandatoryInt(context, "mark", "Marker action code", 1, 4, Source.Url);
            
            if (action == MarkAsRead || action == MarkAsUnRead)
            {
                if (identity.UserType != UserType.Customer)
                {
                    throw new YummyZoneException("Support agent is not allowed to do this operation");
                }

                MarkAs markAs = new MarkAs(checkinId, identity.UserId);

                if (action == MarkAsRead)
                {
                    Database.InsertOrUpdate(markAs, Helpers.ConnectionString);
                }
                else // action == MarkAsUnRead
                {
                    Database.Delete(markAs, Helpers.ConnectionString);
                }
            }
            else // (action == MarkAsSpam || action == MarkAsHidden)
            {
                if (action == MarkAsSpam)
                {
                    if (identity.UserType == UserType.SupportAgent)
                    {
                        throw new YummyZoneException("Support agent is not allowed to mark a feedback as spam");
                    }
                }
                else if (action == MarkAsHidden)
                {
                    if (identity.UserType != UserType.Customer)
                    {
                        throw new YummyZoneException("Support agent is not allowed to archive a feedback");
                    }
                }

                // Spam(3), Deleted(4)
                using (SqlConnection conn = new SqlConnection(Helpers.ConnectionString))
                {
                    conn.Open();

                    // this checks the ownership as well...
                    Checkin checkin = Checkin.SelectCoreBy(conn, identity.GroupId, checkinId);
                    if (checkin == null)
                    {
                        throw new YummyZoneException("Access denied");
                    }

                    if (action == MarkAsSpam)
                    {
                        Checkin.MarkAsSpam(conn, checkinId);
                    }
                    else // action == MarkAsHidden
                    {
                        Checkin.MarkAsDeleted(conn, checkinId);
                    }
                }
            }

            return 1; // success
        }
    }
}