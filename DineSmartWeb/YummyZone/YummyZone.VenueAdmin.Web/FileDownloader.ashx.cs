using System;
using System.IO;
using System.Web;

using YummyZone.ObjectModel;

namespace YummyZone.VenueAdmin.Web
{
    /// <summary>
    /// FileDownload httphandler to serve files
    /// </summary>
    public class FileDownloader : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            bool fileProvided = false;

            try
            {
                string fileName = null;
                string mime = null;
                int fileId = this.GetFileId(context, 1, 3);
                if (fileId == 1)
                {
                    fileName = "flyer.pdf";
                    mime = "application/pdf";
                }
                else if (fileId == 2)
                {
                    fileName = "bizcard-front.png";
                    mime = "image/png";
                }
                else if (fileId == 3)
                {
                    fileName = "bizcard-back.png";
                    mime = "image/png";
                }

                string root = context.Server.MapPath(String.Empty);
                string filePath = Path.Combine(root, fileName);
                FileInfo fifo = new FileInfo(filePath);

                context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                context.Response.AddHeader("Accept-Ranges", "bytes");
                context.Response.AddHeader("Content-Encoding", "none");
                context.Response.AddHeader("Content-Type", mime);
                context.Response.AddHeader("Content-Length", fifo.Length.ToString());
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                context.Response.AddHeader("Last-Modified", fifo.LastWriteTime.ToString());

                context.Response.WriteFile(filePath);
                
                fileProvided = true;

                context.Response.Flush();
                context.Response.End();
            }
            catch (Exception)
            {
                // 
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

        protected int GetFileId(HttpContext context, int min, int max)
        {
            string integerValueAsString = context.Request.Params["fid"];
            if (String.IsNullOrWhiteSpace(integerValueAsString))
            {
                throw new YummyZoneArgumentException("File ID hasn't been specified.");
            }

            int fileId = 0;
            integerValueAsString = integerValueAsString.Trim();
            if (!Int32.TryParse(integerValueAsString, out fileId))
            {
                throw new YummyZoneArgumentException("Invalid File ID.");
            }

            if (fileId < min || fileId > max)
            {
                throw new YummyZoneArgumentException("Unknown File ID.");
            }

            return fileId;
        }
    }
}