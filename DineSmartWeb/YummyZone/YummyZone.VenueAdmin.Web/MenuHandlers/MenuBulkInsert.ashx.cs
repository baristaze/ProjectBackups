using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.SqlClient;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuBulkInsert httphandler to receive files and save them to the server.
    /// </summary>
    public class MenuBulkInsert : YummyZoneHttpHandler
    {   
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

            Guid menuId = this.GetMenuId(context, Source.Form);
            Guid catId = this.GetGuid(context, "mcid", "Menu Category Id", Source.Form);
            string bulkData = this.GetMandatoryString(context, "bulk", "Bulk Menu Data", 10000, Source.Form, false);

            if (catId == Guid.Empty && !bulkData.StartsWith("[C]"))
            {
                throw new YummyZoneArgumentException("There must be a category tagged with [C] at the beginning of bulk data.");
            }

            using (SqlConnection connection = new SqlConnection(Helpers.ConnectionString))
            {
                connection.Open();

                // check menu
                MapVenueToMenu theMap = new MapVenueToMenu(identity.GroupId, identity.VenueId, menuId);
                if (!Database.Select(theMap, connection, null, Database.TimeoutSecs))
                {
                    throw new YummyZoneArgumentException("Menu doesn't belong to this restaurant!");
                }

                // get current categories
                MenuCategoryList allCategories = MenuCategory.GetMenuCategories(connection, identity.GroupId, menuId);

                // check initial category
                MenuCategory initialExistingCat = null;
                if (catId != Guid.Empty)
                {
                    initialExistingCat = allCategories[catId];
                    initialExistingCat.Items.Clear();
                }

                // parse data
                MenuCategoryList allItems = this.Parse(identity.GroupId, bulkData, initialExistingCat);

                // get all menu items
                MenuItemList allPlates = MenuItem.LoadAllForVenue(
                    connection, null, identity.GroupId, identity.VenueId, Database.TimeoutSecs);

                // get all plate maps
                MapListCategoryToMenuItem plateMaps = Database.SelectAll<MapCategoryToMenuItem, MapListCategoryToMenuItem>(
                    connection, null, identity.GroupId, Database.TimeoutSecs);

                // prepare items to insert or update
                List<IEditable> itemsToInsert = new List<IEditable>();
                foreach (MenuCategory cat in allItems)
                {
                    // check to see if category exists already
                    MenuCategory categoryInAction = allCategories[cat.Name];
                    if (categoryInAction == null)
                    {
                        #region AddNewCategory

                        // add new category
                        itemsToInsert.Add(cat);

                        // add map to menu
                        MapMenuToCategory mapCat = new MapMenuToCategory();
                        mapCat.GroupId = identity.GroupId;
                        mapCat.MenuId = menuId;
                        mapCat.MenuCategoryId = cat.Id;
                        itemsToInsert.Add(mapCat);

                        categoryInAction = cat;
                        
                        #endregion // AddNewCategory
                    }
                                        
                    foreach (MenuItem plate in cat.Items) 
                    {
                        MenuItem plateInAction = allPlates[plate.Name];
                        if (plateInAction == null || !plate.Price.Equals(plateInAction.Price))
                        {
                            #region AddNewPlate

                            // add new category
                            itemsToInsert.Add(plate);

                            // add map to menu
                            MapCategoryToMenuItem mapPlate = new MapCategoryToMenuItem();
                            mapPlate.GroupId = identity.GroupId;
                            mapPlate.MenuCategoryId = categoryInAction.Id;
                            mapPlate.MenuItemId = plate.Id;
                            itemsToInsert.Add(mapPlate);

                            plateInAction = plate;

                            #endregion // AddNewPlate
                        }
                        else
                        {
                            // already exists... update description
                            #region UpdateDescription

                            int descrNew = String.IsNullOrWhiteSpace(plate.Description) ? 0 : plate.Description.Length;
                            int descrOld = String.IsNullOrWhiteSpace(plateInAction.Description) ? 0 : plateInAction.Description.Length;
                            if (descrNew > descrOld)
                            {
                                plateInAction.Description = plate.Description;
                                plateInAction.LastUpdateTimeUTC = plate.LastUpdateTimeUTC;
                                itemsToInsert.Add(plateInAction);
                            }

                            #endregion // UpdateDescription

                            // if plate belongs to above cat already
                            if (plateMaps.Search(categoryInAction.Id, plateInAction.Id) == null)
                            {
                                // add map to menu
                                MapCategoryToMenuItem mapPlate = new MapCategoryToMenuItem();
                                mapPlate.GroupId = identity.GroupId;
                                mapPlate.MenuCategoryId = categoryInAction.Id;
                                mapPlate.MenuItemId = plateInAction.Id;
                                itemsToInsert.Add(mapPlate);
                            }
                        }
                    }
                }

                // insert or update all
                if (itemsToInsert.Count > 0)
                {
                    int affectedRows = 0;
                    bool allSucceeded = false;
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (IEditable entity in itemsToInsert)
                            {
                                affectedRows += Database.InsertOrUpdate(entity, connection, trans, Database.TimeoutSecs);
                            }

                            trans.Commit();
                            allSucceeded = true;
                        }
                        finally
                        {
                            if (!allSucceeded)
                            {
                                trans.Rollback();
                            }
                        }
                    }

                    if (!allSucceeded)
                    {
                        throw new YummyZoneException("Couldn't commit to the database");
                    }
                }
            }

            // do not use "N" here
            return menuId.ToString();
        }

        private MenuCategory GetCategory(Guid groupId, Guid catId, SqlConnection conn, SqlTransaction trans)
        {
            MenuCategory menuCategory = new MenuCategory();
            menuCategory.GroupId = groupId;
            menuCategory.Id = catId;

            // get the category from database
            if (!Database.Select(menuCategory, conn, trans, Database.TimeoutSecs))
            {
                string msg = "There is not a category with the given Id: " + menuCategory.Id.ToString();
                throw new YummyZoneArgumentException(msg);
            }

            return menuCategory;
        }

        private MenuCategoryList Parse(Guid groupId, string data, MenuCategory existingCat)
        {
            data = this.Normalize(data);

            MenuCategoryList cats = new MenuCategoryList();
            if (existingCat != null)
            {
                cats.Add(existingCat);
            }

            int index = 0;
            TokenType tokenType = TokenType.Category;

            while (tokenType != TokenType.None && index < data.Length)
            {
                string token = this.GetNextToken(data, ref index, out tokenType);
                if (String.IsNullOrWhiteSpace(token))
                {
                    if (index == data.Length)
                    {
                        break;
                    }

                    string pointer = this.TryRead(data, index - 3, 20);
                    throw new YummyZoneException("Syntax error near " + pointer + "... (Empty token)");
                }

                if (tokenType == TokenType.Category)
                {
                    if (token.Length > MenuCategory.MaxNameLength)
                    {
                        throw new YummyZoneException("Category name may not be bigger than " + MenuCategory.MaxNameLength.ToString());
                    }

                    MenuCategory newCat = new MenuCategory();
                    newCat.GroupId = groupId;
                    newCat.Name = StringHelpers.ToTitleCase(token);
                    cats.Add(newCat);
                }
                else if (tokenType == TokenType.PlateName)
                {
                    if (cats.Count == 0) 
                    {
                        throw new YummyZoneException("Bulk data must start with a category");
                    }

                    if (token.Length > MenuItem.MaxNameLength)
                    {
                        throw new YummyZoneException(
                            "Plate name (" + token.Substring(0, 10) + "...) may not be bigger than " + 
                            MenuItem.MaxNameLength.ToString());
                    }

                    MenuItem plate = new MenuItem();
                    plate.GroupId = groupId;
                    plate.Name = StringHelpers.ToTitleCase(token);

                    cats[cats.Count - 1].Items.Add(plate);
                }
                else if (tokenType == TokenType.PlatePrice)
                {
                    if (cats.Count == 0)
                    {
                        throw new YummyZoneException("Bulk data must start with a category");
                    }

                    MenuCategory lastCat = cats[cats.Count - 1];
                    if (lastCat.Items.Count == 0)
                    {
                        throw new YummyZoneException(
                            "Price info (" + token + ") must be preceeded by a plate name. Check after " + lastCat.Name);
                    }

                    MenuItem lastPlate = lastCat.Items[lastCat.Items.Count-1];
                    if (lastPlate.Price.HasValue)
                    {
                        throw new YummyZoneException("Consecutive price (" + token + ") info for " + lastPlate.Name);
                    }

                    decimal money = (decimal)0.0;
                    if (!Decimal.TryParse(token, out money))
                    {
                        throw new YummyZoneException("Invalid price (" + token + ") for " + lastPlate.Name);
                    }

                    if (money < (decimal)0.0)
                    {
                        throw new YummyZoneException(
                            "Price may not be negative (" + token + ") for " + lastPlate.Name);
                    }

                    if (money > MenuItem.MaxPrice)
                    {
                        throw new YummyZoneException(
                            "Price (" + token + ") may not be bigger than " + 
                            MenuItem.MaxPrice.ToString() + " for " + lastPlate.Name);
                    }

                    lastPlate.Price = money;
                }
                else if (tokenType == TokenType.PlateDescription)
                {
                    if (cats.Count == 0)
                    {
                        throw new YummyZoneException("Bulk data must start with a category");
                    }

                    MenuCategory lastCat = cats[cats.Count - 1];
                    if (lastCat.Items.Count == 0)
                    {
                        throw new YummyZoneException(
                            "Description info must be preceeded by a plate name. Check after " + lastCat.Name);
                    }

                    MenuItem lastPlate = lastCat.Items[lastCat.Items.Count-1];
                    string desc = lastPlate.Description;
                    if (desc == null)
                    {
                        desc = String.Empty;
                    }

                    if (desc.Length > 0)
                    {
                        desc += "\n";
                    }

                    desc += token;

                    if (desc.Length > MenuItem.MaxDescriptionLength)
                    {
                        throw new YummyZoneException(
                            "Description may not be longer than " + 
                            MenuItem.MaxDescriptionLength.ToString() + 
                            " for " + lastPlate.Name);
                    }

                    lastPlate.Description = desc.Trim();
                }
            }

            return cats;
        }

        private string TryRead(string data, int index, int maxLength)
        {
            string pointer = String.Empty;
            while (index < data.Length && pointer.Length < maxLength)
            {
                if (data[index] == ' ')
                {
                    break;
                }

                pointer += data[index++];
            }

            return pointer.Trim();
        }

        private static string[] _tags = new string[] { "[C]", "[N]", "[D]", "$" };

        private static TokenType[] _tokenTypes = new TokenType[] { 
            TokenType.Category, 
            TokenType.PlateName, 
            TokenType.PlateDescription, 
            TokenType.PlatePrice };

        private string GetNextToken(string data, ref int index, out TokenType tokenType)
        {
            tokenType = TokenType.None;

            int tempIndex = index;
            string nextToken = null;
            TokenType tempTokenType = TokenType.None;

            for (int x = 0; x < _tags.Length; x++)
            {
                int tagLength = _tags[x].Length;
                if (tempIndex + tagLength < data.Length)
                {
                    if (String.Compare(data.Substring(tempIndex, tagLength), _tags[x], StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        tempTokenType = _tokenTypes[x];
                        tempIndex += tagLength;
                        break;
                    }
                }
            }

            if (tempTokenType != TokenType.None)
            {   
                while (tempIndex < data.Length && data[tempIndex] == ' ')
                {
                    tempIndex++;
                }

                if (tempIndex < data.Length)
                {
                    int nextIndex = data.IndexOfAny(new char[] { '[', '$' }, tempIndex);
                    if (nextIndex < tempIndex)
                    {
                        nextIndex = data.Length;
                    }

                    nextToken = data.Substring(tempIndex, nextIndex - tempIndex).Trim();
                    tokenType = tempTokenType;
                    index = nextIndex;
                }
            }

            return nextToken;
        }

        private string Normalize(string data)
        {
            // normalize string
            data = data.Replace("\r\n", "\n");
            data = data.Replace("\t", " ");
            
            while (data.IndexOf("\n\n") >= 0)
            {
                data = data.Replace("\n\n", "\n");
            }
            
            while (data.IndexOf("  ") >= 0) {
                data = data.Replace("  ", " ");
            }

            List<string> newLines = new List<string>();
            string[] lines = data.Split('\n');
            for (int x = 0; x < lines.Length; x++)
            {
                lines[x] = lines[x].Trim();
                if (!String.IsNullOrWhiteSpace(lines[x]))
                {
                    decimal priceCandidate = (decimal)0.0;
                    if (Decimal.TryParse(lines[x], out priceCandidate))
                    {
                        if (priceCandidate > 0 && priceCandidate < 1800 /*avoid year collision*/)
                        {
                            lines[x] = "$" + lines[x];
                        }
                    }

                    if (!(lines[x].StartsWith("[") || lines[x].StartsWith("$")))
                    {
                        lines[x] = "[N]" + lines[x];
                    }

                    newLines.Add(lines[x]);
                }
            }

            string normalized = String.Join("", newLines.ToArray()).Trim();
            return normalized;
        }

        private enum TokenType
        {
            None,
            Category,
            PlateName,
            PlateDescription,
            PlatePrice
        }
    }
}