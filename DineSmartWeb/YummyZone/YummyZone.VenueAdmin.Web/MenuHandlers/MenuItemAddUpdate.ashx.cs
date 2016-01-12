using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// MenuItemAddUpdate httphandler to receive files and save them to the server.
    /// </summary>
    public class MenuItemAddUpdate : YummyZoneHttpHandler
    {   
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            MenuItem menuItem = new MenuItem();
            menuItem.GroupId = identity.GroupId;

            List<IEditable> entities = new List<IEditable>();
            entities.Add(menuItem);

            // get menu item id if there is any
            string menuItemId = context.Request.Form["MenuItemId"];
            if (!String.IsNullOrWhiteSpace(menuItemId))
            {
                // if we have a valid ID, then this is an update case
                menuItem.Id = this.GetMandatoryGuid(context, "MenuItemId", "Menu Item Id", Source.Form);

                // get the menu item from database
                if (!Database.Select(menuItem, Helpers.ConnectionString, Database.TimeoutSecs))
                {
                    string msg = "There is not a plate with the given Id: " + menuItem.Id.ToString();
                    throw new YummyZoneArgumentException(msg, "MenuItemId");
                }

                // update the time
                menuItem.LastUpdateTimeUTC = DateTime.UtcNow;
            }
            else
            {
                MapCategoryToMenuItem map = new MapCategoryToMenuItem();
                map.GroupId = identity.GroupId;
                map.MenuCategoryId = this.GetMandatoryGuid(context, "MenuCategoryId", "Menu Category Id", Source.Form);
                map.MenuItemId = menuItem.Id;

                entities.Add(map);
            }
            
            // get name
            menuItem.Name = this.GetMandatoryString(context, "MenuItemName", "Item Name", MenuItem.MaxNameLength, Source.Form);
            menuItem.Name = StringHelpers.ToTitleCase(menuItem.Name);

            // get price if there is any
            string menuItemPriceStr = context.Request.Form["MenuItemPrice"];
            if (!String.IsNullOrWhiteSpace(menuItemPriceStr))
            {
                decimal tempPrice = -1;
                if (!Decimal.TryParse(menuItemPriceStr, out tempPrice))
                {
                    throw new YummyZoneArgumentException("Item Price is invalid", "MenuItemPrice");
                }

                if (tempPrice < 0)
                {
                    throw new YummyZoneArgumentException("Item Price may not be negative", "MenuItemPrice");
                }

                if (tempPrice > MenuItem.MaxPrice)
                {
                    throw new YummyZoneArgumentException("Item Price is too big", "MenuItemPrice");
                }

                menuItem.Price = tempPrice;
            }
            else
            {
                menuItem.Price = null;
            }

            // get description if there is any
            string menuItemDesc = context.Request.Form["MenuItemDescription"];
            if (!String.IsNullOrWhiteSpace(menuItemDesc))
            {
                menuItemDesc = menuItemDesc.Trim();
                if (menuItemDesc.Length > MenuItem.MaxDescriptionLength)
                {
                    throw new YummyZoneArgumentException("Item Description is too long", "MenuItemDescription");
                }

                menuItem.Description = menuItemDesc;
            }
            else
            {
                menuItem.Description = null;
            }

            bool isFromLocal = false;
            string isImageFromLocal = context.Request.Form["IsMenuItemImageFromLocal"];
            if (!String.IsNullOrWhiteSpace(isImageFromLocal))
            {
                isImageFromLocal = isImageFromLocal.Trim();
                if (!Boolean.TryParse(isImageFromLocal, out isFromLocal))
                {
                    throw new YummyZoneArgumentException("Invalid flag", "IsMenuItemImageFromLocal");
                }
            }

            MenuItemImage menuItemImage = null;
            if (isFromLocal && context.Request.Files.Count > 0)
            {
                HttpPostedFile uploadedfile = context.Request.Files[0];
                if (uploadedfile.ContentLength <= 0)
                {
                    throw new YummyZoneArgumentException("File is empty", "ImageFromLocal");
                }

                string ext = Path.GetExtension(uploadedfile.FileName);
                if (String.IsNullOrWhiteSpace(ext))
                {
                    throw new YummyZoneArgumentException("File type is undefined", "ImageFromLocal");
                }

                ext = ext.Trim(' ', '.', '*').ToLower();
                if (!MenuItemImage.AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    throw new YummyZoneArgumentException("File type is not allowed", "ImageFromLocal");
                }

                menuItemImage = new MenuItemImage();
                menuItemImage.GroupId = identity.GroupId;
                menuItemImage.MenuItemId = menuItem.Id;                
                menuItemImage.InitialFileNameOrUrl = uploadedfile.FileName;
                menuItemImage.ContentType = uploadedfile.ContentType;
                menuItemImage.ContentLength = uploadedfile.ContentLength;
                menuItemImage.Data = Helpers.StreamToByteArray(uploadedfile.InputStream, uploadedfile.ContentLength);
            }

            if (!isFromLocal)
            {
                string menuItemImageFromWeb = context.Request.Form["MenuItemImageFromURL"];
                if (!String.IsNullOrWhiteSpace(menuItemImageFromWeb))
                {
                    menuItemImageFromWeb = menuItemImageFromWeb.Trim();
                    WebRequest request = WebRequest.Create(menuItemImageFromWeb);
                    WebResponse response = request.GetResponse();

                    if (response.ContentLength <= 0)
                    {
                        throw new YummyZoneArgumentException("The content pointed by URL is empty", "ImageFromURL");
                    }

                    if (response.ContentType == null || !response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new YummyZoneArgumentException("The content pointed by URL is not an image", "ImageFromURL");
                    }

                    menuItemImage = new MenuItemImage();
                    menuItemImage.GroupId = identity.GroupId;
                    menuItemImage.MenuItemId = menuItem.Id;
                    menuItemImage.InitialFileNameOrUrl = menuItemImageFromWeb;
                    menuItemImage.ContentType = response.ContentType;
                    menuItemImage.ContentLength = (int)response.ContentLength;
                    menuItemImage.Data = Helpers.StreamToByteArray(response.GetResponseStream(), menuItemImage.ContentLength);
                }
            }

            
            if (menuItemImage != null && menuItemImage.Data != null && menuItemImage.Data.Length > 0)
            {   
                entities.Add(menuItemImage);
            }

            // no need to catch... 
            Database.InsertOrUpdate(entities, Helpers.ConnectionString);

            return menuItem.Id.ToString("N");
        }
    }
}