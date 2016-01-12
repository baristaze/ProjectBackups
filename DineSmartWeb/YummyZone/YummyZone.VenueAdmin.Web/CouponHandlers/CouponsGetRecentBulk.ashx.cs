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
    public class CouponsGetRecentBulk : YummyZoneHttpHandlerJson<SimpleCouponListResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();
                                
                DateTime previousTime = this.GetMandatoryDateTime(context, "time", "previous coupon time", Source.Url);
                CouponList cpnList = Coupon.GetRecentCouponNews(connection, identity.ChainId, previousTime, Database.TimeoutSecs);

                SimpleCouponListResponse response = new SimpleCouponListResponse();
                foreach (Coupon cpn in cpnList)
                {
                    response.Items.Add(new SimpleCoupon(cpn));
                }
                
                return response;
            }
        }
    }

    [DataContract]
    public class SimpleCouponListResponse : BaseJsonResponse
    {
        [DataMember]
        public SimpleCouponList Items { get { return this.items; } }
        private SimpleCouponList items = new SimpleCouponList();
    }
}