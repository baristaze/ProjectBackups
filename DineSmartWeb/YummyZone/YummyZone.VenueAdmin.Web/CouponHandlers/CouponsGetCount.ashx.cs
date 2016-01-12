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
    public class CouponsGetCount : YummyZoneHttpHandler
    {
        private enum CpnType
        {
            Unknown = 0,
            Sent,
            Redeemed
        }

        private enum CpnTime
        { 
            Unknown = 0,
            Today,
            Yesterday,
            Last7days,
            Last30days,
            SinceBeginningOfMonth,
            InLastMonth,
            InThisCalenderYear,
        }

        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            CpnType couponType = (CpnType)this.GetMandatoryInt(context, "ctype", "Coupon Type", 1, 2, Source.Url);
            CpnTime couponTime = (CpnTime)this.GetMandatoryInt(context, "ctime", "Coupon Time", 1, 7, Source.Url);
            int timeOffset = this.GetMandatoryInt(context, "offset", "Time Offset", -500, +500, Source.Url);
            DateTime[] timeWindow = this.GetTimeWindow(couponTime, timeOffset);

            DateTime start = timeWindow[0].AddMinutes(timeOffset);
            DateTime end = timeWindow[1].AddMinutes(timeOffset);
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                return Coupon.GetCouponCount(
                    connection, 
                    identity.ChainId,
                    start,
                    end, 
                    (couponType == CpnType.Redeemed), 
                    Database.TimeoutSecs);
            }
        }

        private DateTime[] GetTimeWindow(CpnTime couponTime, int timeOffset)
        {
            DateTime temp = DateTime.UtcNow.AddMinutes(timeOffset * -1);
            DateTime clientLocalNow = new DateTime(temp.Ticks, DateTimeKind.Unspecified);
            DateTime clientLocalToday = new DateTime(clientLocalNow.Year, clientLocalNow.Month, clientLocalNow.Day);

            if (couponTime == CpnTime.Today)
            {
                return new DateTime[] { clientLocalToday, clientLocalNow };
            }
            else if (couponTime == CpnTime.Yesterday)
            {
                return new DateTime[] { clientLocalToday.AddDays(-1), clientLocalToday };
            }
            else if (couponTime == CpnTime.Last7days)
            {
                return new DateTime[] { clientLocalToday.AddDays(-7), clientLocalNow };
            }
            else if (couponTime == CpnTime.Last30days)
            {
                return new DateTime[] { clientLocalToday.AddDays(-30), clientLocalNow };
            }
            else if (couponTime == CpnTime.SinceBeginningOfMonth)
            {
                DateTime begMonth = new DateTime(clientLocalNow.Year, clientLocalNow.Month, 1);
                return new DateTime[] { begMonth, clientLocalNow };
            }
            else if (couponTime == CpnTime.InLastMonth)
            {
                DateTime begMonth = new DateTime(clientLocalNow.Year, clientLocalNow.Month, 1);                
                return new DateTime[] { begMonth.AddMonths(-1), begMonth };
            }
            else if (couponTime == CpnTime.InThisCalenderYear)
            {
                DateTime begYear = new DateTime(clientLocalNow.Year, 1, 1);
                return new DateTime[] { begYear, clientLocalNow };
            }
            else 
            {
                throw new NotImplementedException();
            }
        }
    }
}