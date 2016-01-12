using System;
using System.Web.UI;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Downloads : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                TenantIdentity identity = this.SinglePublicPageInit();
            }
        }
    }
}