using System;
using System.Collections.Generic;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuAddUpdate httphandler to receive files and save them to the server.
    /// </summary>
    public class MenuAddUpdate : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            Menu menu = new Menu();
            menu.GroupId = identity.GroupId;

            List<IEditable> entities = new List<IEditable>();
            entities.Add(menu);

            // get the menu id
            string menuId = context.Request.Params["mid"];
            if (!String.IsNullOrWhiteSpace(menuId))
            {
                // if we have a valid ID, then this is an update case
                menu.Id = this.GetMenuId(context, Source.Url);

                // get the menu from database
                if (!Database.Select(menu, Helpers.ConnectionString, Database.TimeoutSecs))
                {
                    string msg = "There is not a menu with the given Id: " + menu.Id.ToString();
                    throw new YummyZoneArgumentException(msg, "mid");
                }

                // update the time
                menu.LastUpdateTimeUTC = DateTime.UtcNow;
            }
            else
            {
                // add new case... we need to add the map as well
                MapVenueToMenu map = new MapVenueToMenu();
                map.GroupId = identity.GroupId;
                map.VenueId = identity.VenueId;
                map.MenuId = menu.Id;

                entities.Add(map);
            }
            
            // get name
            menu.Name = this.GetMandatoryString(context, "mn", "Menu name", Menu.MaxNameLength, Source.Url);
            menu.Name = StringHelpers.ToTitleCase(menu.Name);

            menu.ServiceStartTime = (short)this.GetMandatoryInt(context, "sst", "Service Start Time", 0, 1560, Source.Url);
            menu.ServiceEndTime = (short)this.GetMandatoryInt(context, "set", "Service End Time", 0, 1560, Source.Url);

            // no need to catch... 
            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            return menu.Id.ToString("N");
        }
    }
}