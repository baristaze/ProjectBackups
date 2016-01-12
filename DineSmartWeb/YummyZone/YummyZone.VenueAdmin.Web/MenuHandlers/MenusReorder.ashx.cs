using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuReorder httphandler
    /// </summary>
    public class MenusReorder : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            List<Guid> menuIds = GetMandatoryGuidList(context, "mids", "menu ids");

            byte orderIndex = 0;
            MapListVenueToMenu orderedEntities = new MapListVenueToMenu();
            foreach (Guid menuId in menuIds)
            {
                MapVenueToMenu map = new MapVenueToMenu();
                map.GroupId = identity.GroupId;
                map.VenueId = identity.VenueId;
                map.MenuId = menuId;
                map.OrderIndex = orderIndex++;

                orderedEntities.Add(map);
            }

            Database.Reorder(orderedEntities, false, Helpers.ConnectionString);

            return 1; // success
        }
    }
}