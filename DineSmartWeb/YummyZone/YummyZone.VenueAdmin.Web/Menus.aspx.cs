using System;
using System.Collections.Generic;
using YummyZone.ObjectModel;
using System.Data.SqlClient;

namespace YummyZone.VenueAdmin.Web
{
    public partial class Menus : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TenantIdentity identity = this.UpdateIdentitySection();
            MenuList menus = new MenuList();

            if (identity != null)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();

                    string query = Menu.SelectAllForVenue(identity.GroupId, identity.VenueId);
                    menus = Database.SelectAll<Menu, MenuList>(connection, null, query, Database.TimeoutSecs);
                }
            }

            menuRepeater.DataSource = menus;
            menuRepeater.DataBind();
        }
    }
}