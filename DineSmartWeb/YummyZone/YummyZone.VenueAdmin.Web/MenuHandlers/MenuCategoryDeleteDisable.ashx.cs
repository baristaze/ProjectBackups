using System;
using System.Collections.Generic;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuCategoryDeleteDisable httphandler
    /// </summary>
    public class MenuCategoryDeleteDisable: YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            MenuCategory menuCategory = new MenuCategory();
            menuCategory.Id = this.GetMenuCategoryId(context, Source.Url);
            menuCategory.GroupId = identity.GroupId;
            menuCategory.LastUpdateTimeUTC = DateTime.UtcNow;
            
            List<IEditable> entities = new List<IEditable>();
            entities.Add(menuCategory);

            bool isDelete = this.GetDeleteOrDisableAction(context, Source.Url);            
            if (isDelete)
            {
                Database.Delete(entities, Helpers.ConnectionString);
            }
            else
            {
                throw new NotImplementedException();
            }

            return 1; // success
        }
    }
}