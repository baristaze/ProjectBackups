using System;
using System.Collections.Generic;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuDeleteDisable httphandler
    /// </summary>
    public class MenuDeleteDisable: YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            Menu menu = new Menu();
            menu.GroupId = identity.GroupId;
            menu.Id = this.GetMenuId(context, Source.Url);
            menu.LastUpdateTimeUTC = DateTime.UtcNow;
            
            List<IEditable> entities = new List<IEditable>();
            entities.Add(menu);

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