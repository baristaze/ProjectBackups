using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuCategoryNames httphandler
    /// </summary>
    public class MenuCategoryNames : YummyZoneHttpHandlerJson<CategoryNamesResponse>
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            Guid menuId = this.GetMenuId(context, Source.Url);

            NameIdPairList pairs = new NameIdPairList();
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                MapListMenuToCategoryEx list = MenuCategory.GetMenuCategoryNames(connection, identity.GroupId, identity.VenueId, menuId);
                foreach (MapMenuToCategoryEx map in list)
                {
                    NameIdPair pair = new NameIdPair();
                    pair.Id = map.MenuCategoryId;
                    pair.Name = map.MenuName + " - " + map.MenuCategoryName;
                    pairs.Add(pair);
                }
            }

            CategoryNamesResponse response = new CategoryNamesResponse();
            response.Categories.AddRange(pairs);
            return response;            
        }
    }

    [DataContract]
    public class NameIdPair
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    public class NameIdPairList : List<NameIdPair>
    {
        public NameIdPair this[Guid id]
        {
            get
            {
                foreach (NameIdPair item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }

    [DataContract]
    public class CategoryNamesResponse : BaseJsonResponse
    {
        [DataMember]
        public NameIdPairList Categories { get { return this.categories; } }
        private NameIdPairList categories = new NameIdPairList();
    }
}