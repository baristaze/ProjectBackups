using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// httphandler
    /// </summary>
    public class MessageSend : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            if (identity.UserType == UserType.SupportAgent)
            {
                throw new YummyZoneException("Support agent is not allowed to send message");
            }

            Guid checkinId = this.GetGuid(context, "CheckinId", "Checkin Id", Source.Form);
            if (checkinId == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Checkin Id is must in this version");
            }

            string title = this.GetMandatoryString(context, "MsgSubject", "Message Subject", Message.MaxLengthTitle, Source.Form);
            string details = this.GetMandatoryString(context, "MsgDetails", "Message Details", Message.MaxLengthContent, Source.Form);

            Message message = new Message();
            message.GroupId = identity.GroupId;
            message.ChainId = identity.ChainId;
            message.SenderId = identity.UserId;
            message.Title = title;
            message.Content = details;

            if (checkinId != Guid.Empty)
            {
                message.CheckInId = checkinId;
            }

            using (SqlConnection conn = new SqlConnection(Helpers.ConnectionString))
            {
                conn.Open();

                // this checks the ownership as well...
                Checkin checkin = Checkin.SelectCoreBy(conn, identity.GroupId, checkinId);
                if (checkin == null)
                {
                    throw new YummyZoneException("Access denied");
                }

                message.ReceiverId = checkin.DinerId;

                int msgCountPerCheckin = Message.CountPerCheckin(conn, checkinId);
                if (msgCountPerCheckin >= Message.MaxMessagePerCheckin) /*including this message*/
                {
                    string msg = String.Format(
                        CultureInfo.InvariantCulture, 
                        "No more than {0} messages is allowed per checkin", 
                        Message.MaxMessagePerCheckin);

                    throw new YummyZoneException(msg);
                }

                DateTime aweekAgo = DateTime.UtcNow.AddDays(-7);
                int msgCountPerWeek = Message.CountPerChannel(conn, message.GroupId, message.ChainId, message.ReceiverId, aweekAgo);
                if (msgCountPerWeek >= Message.MaxMessagePerWeek) /*including this message*/
                {
                    string msg = String.Format(
                        CultureInfo.InvariantCulture,
                        "No more than {0} messages per customer is allowed within last 7 days",
                        Message.MaxMessagePerWeek);

                    throw new YummyZoneException(msg);
                }

                Database.InsertOrUpdate(message, conn, null, Database.TimeoutSecs);
            }

            return message.Id.ToString("N");
        }
    }
}