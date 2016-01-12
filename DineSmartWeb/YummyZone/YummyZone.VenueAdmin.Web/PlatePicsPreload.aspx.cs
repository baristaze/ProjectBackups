using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using OM = YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    public partial class PlatePicsPreload : YummyZonePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = String.Empty;

            try
            {
                s = this.GetImageLinks();
            }
            catch
            { 
            }

            if (!String.IsNullOrWhiteSpace(s))
            {
                this.pictureContainer.InnerHtml = s;
            }
        }

        protected string GetImageLinks()
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(this.Request, false);

            List<Guid> menuItemIds = new List<Guid>();

            if (identity != null)
            {
                using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
                {
                    connection.Open();
                    menuItemIds.AddRange(this.GetMenuItemIds(connection, identity));
                }
            }

            string s = string.Empty;
            foreach (Guid imageId in menuItemIds)
            {
                s += "<img alt=\"\" src=\"MenuHandlers/FileDownload.ashx?fid=" + imageId.ToString() + "\" />\r\n";
            }

            return s;
        }

        protected IEnumerable<Guid> GetMenuItemIds(SqlConnection connection, TenantIdentity identity)
        {
            List<Guid> menuItemIds = new List<Guid>();

            OM.MenuList filteredMenus = this.GetMenus(connection, identity);
            foreach (OM.Menu menu in filteredMenus)
            {
                OM.MapVenueToMenu theMap = new OM.MapVenueToMenu(identity.GroupId, identity.VenueId, menu.Id);
                if (OM.Database.Select(theMap, connection, null, OM.Database.TimeoutSecs))
                {
                    OM.MenuCategoryList filteredCategories = OM.MenuCategory.GetMenuCategories(connection, identity.GroupId, menu.Id);
                    foreach (OM.MenuCategory cat in filteredCategories)
                    {
                        foreach (OM.MenuItem menuItem in cat.Items)
                        {
                            if (menuItem.ImageId.HasValue)
                            {
                                menuItemIds.Add(menuItem.ImageId.Value);
                            }
                        }
                    }
                }
            }

            return menuItemIds.Distinct();
        }

        protected OM.MenuList GetMenus(SqlConnection connection, TenantIdentity identity)
        {
            OM.MenuList filteredMenus = new OM.MenuList();
            List<Guid> filteredMenuIds = new List<Guid>();

            OM.MapListVenueToMenu maps = OM.Database.SelectAll<OM.MapVenueToMenu, OM.MapListVenueToMenu>(
                connection, null, identity.GroupId, OM.Database.TimeoutSecs);

            foreach (OM.MapVenueToMenu map in maps)
            {
                if (map.VenueId == identity.VenueId && map.Status != OM.Status.Removed)
                {
                    filteredMenuIds.Add(map.MenuId);
                }
            }

            if (filteredMenuIds.Count > 0)
            {
                OM.MenuList menus = OM.Database.SelectAll<OM.Menu, OM.MenuList>(
                    connection, null, identity.GroupId, OM.Database.TimeoutSecs);

                // do not change the order of for loops
                foreach (Guid menuId in filteredMenuIds)
                {
                    OM.Menu menu = menus[menuId];
                    if (menu != null)
                    {
                        filteredMenus.Add(menu);
                    }
                }
            }

            return filteredMenus;
        }
    }
}