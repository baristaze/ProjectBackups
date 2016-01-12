using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuImportCategories httphandler
    /// </summary>
    public class MenuImportCategories: YummyZoneHttpHandler
    {
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            Guid menuId = this.GetMenuId(context, Source.Url);
            List<Guid> menuCategoryIds = GetMandatoryGuidList(context, "mcids", "menu category ids");
            
            List<IEditable> entitiesToInsertOrUpdate = new List<IEditable>();
            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                MapListMenuToCategoryEx list = MenuCategory.GetMenuCategoryNames(connection, identity.GroupId, identity.VenueId, menuId);

                foreach (Guid categoryId in menuCategoryIds)
                {
                    MapMenuToCategoryEx helperFictiveMap = list.SearchByMenuCategoryId(categoryId);
                    if(helperFictiveMap != null)
                    {
                        // create a new category if it is not there already
                        // ...
                        // read category Id
                        Guid existingCategoryId = Guid.Empty;
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = MenuCategory.SearchCategoryId(1, identity.GroupId, menuId, Status.Removed);
                            command.CommandTimeout = Database.TimeoutSecs;
                            MenuCategory.AddSqlParametersForCategoryIdSearch(command, helperFictiveMap.MenuCategoryName);
                            object o = command.ExecuteScalar();
                            if(o != null)
                            {
                                existingCategoryId = (Guid)o;
                            }
                        }

                        // check category Id
                        MenuCategory newCategory = null;
                        MapMenuToCategory targetCategoryMap = null;
                        if (existingCategoryId != Guid.Empty)
                        {
                            // update the map
                            targetCategoryMap = new MapMenuToCategory(identity.GroupId, menuId, existingCategoryId, Byte.MaxValue);
                            entitiesToInsertOrUpdate.Add(targetCategoryMap); // insert or update => update
                        }
                        else
                        {
                            // create a new category
                            newCategory = new MenuCategory(identity.GroupId, helperFictiveMap.MenuCategoryName);
                            entitiesToInsertOrUpdate.Add(newCategory);

                            // create a map to menu
                            targetCategoryMap = new MapMenuToCategory(identity.GroupId, menuId, newCategory.Id, Byte.MaxValue);
                            entitiesToInsertOrUpdate.Add(targetCategoryMap);
                        }

                        // get menu items of category
                        bool atLeastOneMenuItem = false;
                        MapCategoryToMenuItem menuItemTemplate = new MapCategoryToMenuItem();
                        string query = menuItemTemplate.SelectAllOfFirst(identity.GroupId, helperFictiveMap.MenuCategoryId);
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = query;
                            command.CommandTimeout = Database.TimeoutSecs;
                            
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    MapCategoryToMenuItem existingMenuItemMap = new MapCategoryToMenuItem();
                                    existingMenuItemMap.InitFromSqlReader(reader);

                                    if (existingMenuItemMap.Status == Status.Active)
                                    {
                                        // create or update maps
                                        MapCategoryToMenuItem menuItemMap = new MapCategoryToMenuItem();
                                        menuItemMap.GroupId = identity.GroupId;
                                        menuItemMap.MenuCategoryId = targetCategoryMap.MenuCategoryId; // existing Id or new Id based on previous actions
                                        menuItemMap.MenuItemId = existingMenuItemMap.MenuItemId;
                                        menuItemMap.OrderIndex = existingMenuItemMap.OrderIndex;
                                        entitiesToInsertOrUpdate.Add(menuItemMap); // insert or update based on previous actions
                                        atLeastOneMenuItem = true;
                                    }
                                }
                            }
                        }

                        if (!atLeastOneMenuItem)
                        {
                            if (newCategory != null)
                            {
                                entitiesToInsertOrUpdate.Remove(newCategory);
                            }

                            entitiesToInsertOrUpdate.Remove(targetCategoryMap);
                        }
                    }
                }
            }

            if (entitiesToInsertOrUpdate.Count > 0)
            {
                Database.InsertOrUpdate(entitiesToInsertOrUpdate, Helpers.ConnectionString);
            }

            return 1; // success
        }
    }
}