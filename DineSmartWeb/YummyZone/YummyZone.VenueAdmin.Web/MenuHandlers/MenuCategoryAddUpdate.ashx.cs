using System;
using System.Collections.Generic;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuCategoryAddUpdate httphandler to receive files and save them to the server.
    /// </summary>
    public class MenuCategoryAddUpdate : YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            MenuCategory menuCategory = new MenuCategory();
            menuCategory.GroupId = identity.GroupId;

            List<IEditable> entities = new List<IEditable>();
            entities.Add(menuCategory);

            // get the category id
            string menuCategoryId = context.Request.Params["mcid"];
            if (!String.IsNullOrWhiteSpace(menuCategoryId))
            {
                // if we have a valid ID, then this is an update case
                menuCategory.Id = this.GetMenuCategoryId(context, Source.Url);

                // get the category from database
                if (!Database.Select(menuCategory, Helpers.ConnectionString, Database.TimeoutSecs))
                {
                    string msg = "There is not a category with the given Id: " + menuCategory.Id.ToString();
                    throw new YummyZoneArgumentException(msg, "mcid");
                }

                // update the time
                menuCategory.LastUpdateTimeUTC = DateTime.UtcNow;
            }
            else
            {
                // this is add-new case, therefore we need add the map as well   
                MapMenuToCategory map = new MapMenuToCategory();
                map.GroupId = identity.GroupId;
                map.MenuId = this.GetMenuId(context, Source.Url);
                map.MenuCategoryId = menuCategory.Id;

                entities.Add(map);
            }
            
            // get name
            menuCategory.Name = this.GetMandatoryString(context, "mcn", "Category name", MenuCategory.MaxNameLength, Source.Url);
            menuCategory.Name = StringHelpers.ToTitleCase(menuCategory.Name);

            // no need to catch... 
            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            return menuCategory.Id.ToString("N");
        }
    }
}