using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Coupons : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = this.UpdateIdentitySection();
            SimpleCouponList simpleCpnList = new SimpleCouponList();

            if (identity != null)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();
                    CouponList cpnList = Coupon.GetRecentCouponNews(connection, identity.ChainId, DateTime.Now.AddDays(3), Database.TimeoutSecs);

                    foreach (Coupon cpn in cpnList)
                    {
                        simpleCpnList.Add(new SimpleCoupon(cpn));
                    }
                }
            }

            this.couponRepeater.DataSource = simpleCpnList;
            this.couponRepeater.DataBind();
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
    }
}