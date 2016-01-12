using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuCategoriesReorderhttphandler
    /// </summary>
    public class MenuCategoriesReorder: YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            Guid menuId = this.GetMenuId(context, Source.Url);
            List<Guid> menuCategoryIds = GetMandatoryGuidList(context, "mcids", "menu category ids");

            byte orderIndex = 0;
            MapListMenuToCategory orderedEntities = new MapListMenuToCategory();
            foreach (Guid categoryId in menuCategoryIds)
            {
                MapMenuToCategory map = new MapMenuToCategory();
                map.GroupId = identity.GroupId;
                map.MenuCategoryId = categoryId;
                map.MenuId = menuId;
                map.OrderIndex = orderIndex++;

                orderedEntities.Add(map);
            }

            Database.Reorder(orderedEntities, false, Helpers.ConnectionString);

            return 1; // success
        }
    }
}