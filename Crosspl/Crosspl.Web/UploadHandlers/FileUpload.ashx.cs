using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Data.SqlClient;

using Crosspl.Web.Services;

namespace Crosspl.Web
{
    /// <summary>
    /// File Upload httphandler to receive files and save them to the server.
    /// </summary>
    public class FileUpload : CrossplHttpHandlerJson<UploadedFile>
    {
        protected override object ProcessWebRequest(HttpContext context, out UserAuthInfo user)
        {
            // throw if the user is not authenticated
            Config config = new Config();
            config.Init();

            user = GetUserInfoFromContent(config.AES_Key, config.AES_IV);

            bool isLocalFile = this.IsFromLocalFile(context);

            UploadedFile result = null;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    result = this.ProcessFileUpload(context, isLocalFile, memoryStream, user);
                }
            }
            finally 
            {
                GC.Collect();
            }

            return result;
        }

        private bool IsFromLocalFile(HttpContext context)
        {
            bool isLocalFile = false;
            string isLocalFileStr = context.Request.Form["IsLocalFile"];
            if (String.IsNullOrWhiteSpace(isLocalFileStr))
            {
                throw new CrossplArgumentException("Invalid flag", "IsLocalFile");
            }
            else
            {
                int isLocalFileInt = -1;
                isLocalFileStr = isLocalFileStr.Trim();
                if (!Int32.TryParse(isLocalFileStr, out isLocalFileInt))
                {
                    throw new CrossplArgumentException("Invalid flag", "IsLocalFile");
                }

                if (isLocalFileInt != 0 && isLocalFileInt != 1)
                {
                    throw new CrossplArgumentException("Invalid flag", "IsLocalFile");
                }

                isLocalFile = (isLocalFileInt == 1);
            }

            return isLocalFile;
        }

        private UploadedFile ProcessFileUpload(HttpContext context, bool isLocalFile, MemoryStream memoryStream, UserAuthInfo user)
        {
            ImageFile imageFile = null;
            if (isLocalFile && context.Request.Files.Count > 0)
            {
                this.GetFileStreamFromPost(context, memoryStream, out imageFile);
            }

            if (!isLocalFile)
            {
                string fileFromWeb = context.Request.Form["WebFileUrl"];
                if (!String.IsNullOrWhiteSpace(fileFromWeb))
                {
                    this.GetFileStreamFromWeb(context, fileFromWeb, memoryStream, out imageFile);
                }
            }

            if (imageFile == null || imageFile.ContentLength <= 0 && memoryStream.Length <= 0)
            {
                throw new CrossplException("There is no file info");
            }
            
            // set a cloud id to the image
            imageFile.CloudId = Guid.NewGuid();
            imageFile.AssetType = AssetType.Entry;
            imageFile.CreatedBy = user.UserId;

            // get width and height of the image
            this.SetImageDimensions(memoryStream, imageFile);

            // get config
            // no need to catch
            Config config = new Config();
            config.Init();

            // write to the cloud
            // no need to catch
            memoryStream.Position = 0;
            imageFile.WriteToCloud(config.BlobStorage, memoryStream);
            
            // write meta-data to the table
            Exception exceptionDuringDBaseOp = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                {
                    conn.Open();
                    imageFile.CreateOnDBase(conn, null);
                }
            }
            catch (Exception ex)
            {
                exceptionDuringDBaseOp = ex;
            }
            finally
            {
                if (exceptionDuringDBaseOp != null)
                {
                    try
                    {
                        // clean up safely since dbase op failed
                        ImageFile.DeleteFromCloud(config.BlobStorage, imageFile.CloudUrl);
                    }
                    catch{}

                    throw exceptionDuringDBaseOp;
                }
            }

            // return
            UploadedFile result = new UploadedFile();
            result.FileInfo = imageFile;
            return result;
        }

        private void SetImageDimensions(MemoryStream memoryStream, ImageFile imageFile)
        {
            try
            {
                using (Image image = new Bitmap(memoryStream))
                {
                    imageFile.Width = image.Width;
                    imageFile.Height = image.Height;
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw new CrossplException("Image couldn't be processed as bitmap", ex);
            }
        }

        private void GetFileStreamFromPost(HttpContext context, MemoryStream memoryStream, out ImageFile imageFile)
        {
            HttpPostedFile httpPost = context.Request.Files[0];
            if (httpPost.ContentLength <= 0)
            {
                throw new CrossplArgumentException("File is empty", "LocalFile");
            }

            string ext = Path.GetExtension(httpPost.FileName);
            if (String.IsNullOrWhiteSpace(ext))
            {
                throw new CrossplArgumentException("File type is undefined", "LocalFile");
            }

            ext = ext.Trim(' ', '.', '*').ToLower();
            if (!ImageFile.AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                throw new CrossplArgumentException("File type is not allowed", "LocalFile");
            }

            imageFile = new ImageFile();
            imageFile.OriginalUrl = httpPost.FileName;
            imageFile.ContentType = httpPost.ContentType;
            imageFile.ContentLength = httpPost.ContentLength;
            httpPost.InputStream.CopyTo(memoryStream);
        }

        private void GetFileStreamFromWeb(HttpContext context, string fileFromWeb, MemoryStream memoryStream, out ImageFile imageFile)
        {
            fileFromWeb = fileFromWeb.Trim();
            WebResponse response = null;
            try
            {
                WebRequest request = WebRequest.Create(fileFromWeb);
                response = request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new CrossplException("The provided URL doesn't point to an image", ex);
            }

            if (response.ContentLength <= 0)
            {
                throw new CrossplArgumentException("The content pointed by the provided URL is empty");
            }

            if (response.ContentType == null || !response.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new CrossplArgumentException("The content pointed by the provided URL is not an image");
            }

            imageFile = new ImageFile();
            imageFile.OriginalUrl = fileFromWeb;
            imageFile.ContentType = response.ContentType;
            imageFile.ContentLength = (int)response.ContentLength;
            response.GetResponseStream().CopyTo(memoryStream);
        }
    }

    [DataContract]
    public class UploadedFile : BaseResponse
    {
        [DataMember]
        public ImageFile FileInfo { get; set; }
    }
}