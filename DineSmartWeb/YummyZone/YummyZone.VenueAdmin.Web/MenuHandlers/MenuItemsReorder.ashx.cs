using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuItemsReorder httphandler
    /// </summary>
    public class MenuItemsReorder : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            Guid menuCategoryId = this.GetMenuCategoryId(context, Source.Form);
            List<Guid> menuItemIds = this.GetMandatoryGuidList(context, "miids", "MenuItemIdList", Source.Form);

            byte orderIndex = 0;
            MapListCategoryToMenuItem orderedEntities = new MapListCategoryToMenuItem();
            foreach (Guid menuItemId in menuItemIds)
            {
                MapCategoryToMenuItem map = new MapCategoryToMenuItem();
                map.GroupId = identity.GroupId;
                map.MenuCategoryId = menuCategoryId;
                map.MenuItemId = menuItemId;
                map.OrderIndex = orderIndex++;

                orderedEntities.Add(map);
            }

            Database.Reorder(orderedEntities, false, Helpers.ConnectionString);

            return 1; //success
        }
    }
}