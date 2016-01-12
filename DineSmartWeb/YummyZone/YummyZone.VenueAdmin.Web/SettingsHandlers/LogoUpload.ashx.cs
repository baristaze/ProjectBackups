using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// LogoUpload httphandler to receive files and save them to the server.
    /// </summary>
    public class LogoUpload : YummyZoneHttpHandler
    {   
        protected override object ProcessWebRequest(HttpContext context)
        {
            TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);
            
            bool isFromLocal = false;
            string isImageFromLocal = context.Request.Form["IsLogoImageFromLocal"];
            if (!String.IsNullOrWhiteSpace(isImageFromLocal))
            {
                isImageFromLocal = isImageFromLocal.Trim();
                if (!Boolean.TryParse(isImageFromLocal, out isFromLocal))
                {
                    throw new YummyZoneArgumentException("Invalid flag", "IsLogoImageFromLocal");
                }
            }

            LogoImage logoImage = null;
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
                if (!LogoImage.AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    throw new YummyZoneArgumentException("File type is not allowed", "ImageFromLocal");
                }

                logoImage = new LogoImage();
                logoImage.GroupId = identity.GroupId;
                logoImage.ChainId = identity.ChainId;
                logoImage.InitialFileNameOrUrl = uploadedfile.FileName;
                logoImage.ContentType = uploadedfile.ContentType;
                logoImage.ContentLength = uploadedfile.ContentLength;
                logoImage.Data = Helpers.StreamToByteArray(uploadedfile.InputStream, uploadedfile.ContentLength);
            }

            if (!isFromLocal)
            {
                string logoImageFromWeb = context.Request.Form["LogoImageFromURL"];
                if (!String.IsNullOrWhiteSpace(logoImageFromWeb))
                {
                    logoImageFromWeb = logoImageFromWeb.Trim();
                    WebRequest request = WebRequest.Create(logoImageFromWeb);
                    WebResponse response = request.GetResponse();

                    if (response.ContentLength <= 0)
                    {
                        throw new YummyZoneArgumentException("The content pointed by URL is empty", "ImageFromURL");
                    }

                    if (response.ContentType == null || !response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new YummyZoneArgumentException("The content pointed by URL is not an image", "ImageFromURL");
                    }

                    logoImage = new LogoImage();
                    logoImage.GroupId = identity.GroupId;
                    logoImage.ChainId = identity.ChainId;
                    logoImage.InitialFileNameOrUrl = logoImageFromWeb;
                    logoImage.ContentType = response.ContentType;
                    logoImage.ContentLength = (int)response.ContentLength;
                    logoImage.Data = Helpers.StreamToByteArray(response.GetResponseStream(), logoImage.ContentLength);
                }
            }


            if (logoImage != null && logoImage.Data != null && logoImage.Data.Length > 0)
            {
                // no need to catch... 
                Database.InsertOrUpdate(logoImage, Helpers.ConnectionString);
            }

            return logoImage.ChainId.ToString("N");
        }
    }
}