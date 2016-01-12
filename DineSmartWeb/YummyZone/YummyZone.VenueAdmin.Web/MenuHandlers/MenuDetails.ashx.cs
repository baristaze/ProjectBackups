using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuDetails httphandler
    /// </summary>
    public class MenuDetails : YummyZoneHttpHandlerJson<MenuDetailsResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            Guid menuId = this.GetMenuId(context, Source.Url);
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                MapVenueToMenu theMap = new MapVenueToMenu(identity.GroupId, identity.VenueId, menuId);
                if (!Database.Select(theMap, connection, null, Database.TimeoutSecs))
                {
                    throw new YummyZoneArgumentException("Requested menu could not be found for the selected venue");
                }

                MenuCategoryList filteredCategories = MenuCategory.GetMenuCategories(connection, identity.GroupId, menuId);
                MenuDetailsResponse details = new MenuDetailsResponse();
                details.Categories.AddRange(filteredCategories);
                return details;
            }
        }
    }

    [DataContract]
    public class MenuDetailsResponse : BaseJsonResponse
    {
        [DataMember]
        public MenuCategoryList Categories { get { return this.categories; } }
        private MenuCategoryList categories = new MenuCategoryList();
    }
}