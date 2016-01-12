using System;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// LogoDownload httphandler to serve files
    /// </summary>
    public class LogoDownload : YummyZoneHttpHandler
    {
        public override void ProcessRequest(HttpContext context)
        {
            bool fileProvided = false;

            try
            {
                TenantIdentity identity = LoginHelper.GetIdentityFromAuth(context.Request, true);

                string fid = context.Request.Params["fid"];
                if (!String.IsNullOrWhiteSpace(fid))
                {
                    Guid fileId = Guid.Empty;
                    if (Guid.TryParse(fid, out fileId))
                    {
                        if (fileId != Guid.Empty)
                        {
                            LogoImage image = new LogoImage();
                            image.ChainId = fileId;
                            image.GroupId = identity.GroupId;

                            if (Database.Select(image, Helpers.ConnectionString, Database.TimeoutSecsForFile))
                            {
                                context.Response.ContentType = image.ContentType;
                                context.Response.AddHeader("Content-Length", image.ContentLength.ToString());
                                context.Response.BinaryWrite(image.Data);
                                fileProvided = true;
                                context.Response.End();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!fileProvided)
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                }
            }
        }

        protected override object ProcessWebRequest(HttpContext context)
        {
            throw new NotSupportedException();
        }
    }
}