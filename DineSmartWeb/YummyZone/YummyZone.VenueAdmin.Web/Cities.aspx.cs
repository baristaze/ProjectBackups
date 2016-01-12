using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Cities : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                TenantIdentity identity = this.SinglePublicPageInit();

                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();
                    List<string> cities = Address.Cities(connection, null, Database.TimeoutSecs);

                    this.cityRepeater.DataSource = cities;
                    this.cityRepeater.DataBind();
                }
            }
        }
    }
}