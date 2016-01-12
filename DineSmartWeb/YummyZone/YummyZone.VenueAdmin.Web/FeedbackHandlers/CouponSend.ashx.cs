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
    public class CouponSend : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            if (identity.UserType == UserType.SupportAgent)
            {
                throw new YummyZoneException("Support agent is not allowed to send coupons");
            }

            Guid checkinId = this.GetGuid(context, "CheckinId", "Checkin Id", Source.Form);
            if (checkinId == Guid.Empty)
            {
                throw new YummyZoneArgumentException("Checkin Id is must in this version");
            }

            string title = this.GetMandatoryString(context, "CpnTitle", "Coupon Summary", Coupon.MaxLengthTitle, Source.Form);
            string details = this.GetString(context, "CpnDetails", "Coupon Details", Coupon.MaxLengthContent, Source.Form);
            int validity = this.GetMandatoryInt(context, "CpnValidDays", "Validity", 7, 120, Source.Form);
            
            Coupon coupon = new Coupon();
            coupon.GroupId = identity.GroupId;
            coupon.ChainId = identity.ChainId;
            coupon.SenderId = identity.UserId;
            coupon.Title = title;
            coupon.Content = details;
            coupon.ExpiryDateUTC = coupon.QueueTimeUTC.AddDays(validity);

            if (checkinId != Guid.Empty)
            {
                coupon.CheckInId = checkinId;
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

                coupon.ReceiverId = checkin.DinerId;

                int cpnCountPerCheckin = Coupon.CountPerCheckin(conn, checkinId);
                if (cpnCountPerCheckin >= Coupon.MaxCouponPerCheckin) // including this coupon
                {
                    string err = String.Format(
                        CultureInfo.InvariantCulture, 
                        "No more than {0} coupon is allowed per checkin", 
                        Coupon.MaxCouponPerCheckin);

                    throw new YummyZoneException(err);
                }

                DateTime aweekAgo = DateTime.UtcNow.AddDays(-7);
                int cpnCountPerWeek = Coupon.CountPerChannel(conn, coupon.GroupId, coupon.ChainId, coupon.ReceiverId, aweekAgo);
                if (cpnCountPerWeek >= Coupon.MaxCouponPerWeek) // including this coupon
                {
                    string err = String.Format(
                        CultureInfo.InvariantCulture,
                        "No more than {0} coupons per customer is allowed within last 7 days",
                        Coupon.MaxCouponPerWeek);

                    throw new YummyZoneException(err);
                }

                Database.InsertOrUpdate(coupon, conn, null, Database.TimeoutSecs);
            }

            return coupon.Id.ToString("N");
        }
    }
}