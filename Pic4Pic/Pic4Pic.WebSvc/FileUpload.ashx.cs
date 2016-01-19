using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    /// <summary>
    /// File Upload httphandler to receive files and save them to the server.
    /// http://pic4pic-web-svc.cloudapp.net/file/upload?blurSize=20&profile=1&thumbx=200&thumby=200
    /// </summary>
    public class FileUpload : Pic4PicHttpHandlerJson<UploadResult>
    {
        protected override object ProcessWebRequest(HttpContext context, out UserAuthInfo user)
        {   
            // throw if the user is not authenticated
            Config config = new Config();
            config.Init();

            // Get user from security content. It doesn't have to be an authenticated user.
            user = GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            // get guid
            Guid clientId = Guid.Empty;
            string clientIdText = HttpContext.Current.Request.Headers["ClientId"];
            if (!String.IsNullOrWhiteSpace(clientIdText)) {
                Guid.TryParse(clientIdText, out clientId);
            }

            // get parameters
            int pixelRatio = this.GetIntOrDefault(context, "blurSize", "Pixelization Ratio", 10, 40, 20, Source.Url);
            int profile = this.GetIntOrDefault(context, "profile", "Is Profile Picture Flag", 0, 1, 0, Source.Url);
            int thumbSizeX = this.GetIntOrDefault(context, "thumbx", "Thumbnail Image Width", 100, 300, 200, Source.Url);
            int thumbSizeY = this.GetIntOrDefault(context, "thumby", "Thumbnail Image Height", 100, 300, thumbSizeX, Source.Url);

            UploadResult result = null;
            
            try
            {
                // below method throws exception on error
                ImageFile[] imageMetaFiles = this.ProcessAndUploadFiles(context, config, user.UserId, thumbSizeX, thumbSizeY, pixelRatio, (profile == 1));

                // below method throws exception on error
                this.SaveMetaFilesOrFixCloud(config, imageMetaFiles);

                // prepare result
                result = PrepareResultEnvelope(config, imageMetaFiles);
            }
            finally 
            {
                GC.Collect();
            }

            return result;            
        }

        private ImageFile[] ProcessAndUploadFiles(HttpContext context, Config config, Guid userId, int thumbnailSizeX, int thumbnailSizeY, int pixelizationRatio, bool isProfilePic)
        {
            // get image meta data
            using (MemoryStream clearImageData = new MemoryStream())
            {
                // fill out source memory stream
                if (context.Request.Files.Count > 0)
                {
                    this.GetStreamFromFile(context, clearImageData);
                }
                else 
                {
                    this.GetStreamFromInputStream(context, clearImageData);
                }                

                // create image processer & uploader
                MultiImageUploader uploader = new MultiImageUploader(
                    clearImageData, config.BlobStorage, thumbnailSizeX, thumbnailSizeY, pixelizationRatio);

                // process and upload images
                ImageFile[] imageMetaFiles = uploader.SafeUploadAllOrNone();

                // check result
                if (imageMetaFiles == null || imageMetaFiles.Length != 4)
                {
                    throw new Pic4PicException("Images couldn't be saved");
                }

                // fix ownership & flags
                for (int x = 0; x < imageMetaFiles.Length; x++)
                {
                    // set ownerships
                    imageMetaFiles[x].UserId = userId;

                    // set flags
                    imageMetaFiles[x].IsProfilePicture = isProfilePic;
                }

                return imageMetaFiles;

            } // using memory stream            
        }

        private void GetStreamFromFile(HttpContext context, MemoryStream clearImageData)
        {
            if (context.Request.Files.Count <= 0)
            {
                throw new Pic4PicArgumentException("No image file has been posted", "File");
            }

            HttpPostedFile httpPost = context.Request.Files[0];
            if (httpPost.ContentLength <= 0)
            {
                throw new Pic4PicArgumentException("File is empty", "File");
            }

            string ext = Path.GetExtension(httpPost.FileName);
            if (String.IsNullOrWhiteSpace(ext))
            {
                throw new Pic4PicArgumentException("File type is undefined", "File");
            }

            ext = ext.Trim(' ', '.', '*').ToLower();
            if (!ImageFile.AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                throw new Pic4PicArgumentException("File type is not allowed", "File");
            }

            httpPost.InputStream.CopyTo(clearImageData);
            if (clearImageData.Length <= 0)
            {
                throw new Pic4PicException("There is no file info");
            }

        } // method

        private void GetStreamFromInputStream(HttpContext context, MemoryStream clearImageData)
        {
            if (context.Request.InputStream == null || context.Request.InputStream.Length <= 0)
            {
                throw new Pic4PicArgumentException("No image file has been posted", "File");
            }

            context.Request.InputStream.CopyTo(clearImageData);
            if (clearImageData.Length <= 0)
            {
                throw new Pic4PicException("There is no file info");
            }

        } // method

        private void SaveMetaFilesOrFixCloud(Config config, ImageFile[] imageMetaFiles)
        {
            // save all meta files or delete from cloud
            try
            {
                using (SqlConnection conn = new SqlConnection(config.DBaseConnectionString))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (ImageFile imf in imageMetaFiles)
                            {
                                imf.CreateOnDBase(conn, trans);
                            }

                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                foreach (ImageFile imf in imageMetaFiles)
                {
                    ImageFile.SafeDeleteFromCloud(config.BlobStorage, imf.CloudUrl);
                }
                throw;
            }
        }

        private UploadResult PrepareResultEnvelope(Config config, ImageFile[] imageMetaFiles)
        {                        
            // prepare reference string
            string concatenatedImageIds = String.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}:{2}:{3}:padding:{4}:{5}:{6}",
                imageMetaFiles[0].Id,
                imageMetaFiles[1].Id,
                imageMetaFiles[2].Id,
                imageMetaFiles[3].Id,
                Guid.NewGuid(), // extra padding
                Guid.NewGuid(), // extra padding
                Guid.NewGuid()  // extra padding
                );

            // encrypt reference string
            string encryptedReference = Crypto.EncryptAES(concatenatedImageIds, config.AES_Key, config.AES_IV);

            // prepare result
            UserProfilePics images = new UserProfilePics();
            images.FullSizeClear = imageMetaFiles[0];
            images.FullSizeBlurred = imageMetaFiles[1];
            images.ThumbnailClear = imageMetaFiles[2];
            images.ThumbnailBlurred = imageMetaFiles[3];

            UploadResult result = new UploadResult();
            result.Images = images;
            result.UploadReference = encryptedReference;

            // return
            return result;
        }

    } // class

    [DataContract]
    public class UploadResult : BaseResponse
    {
        [DataMember]
        public UserProfilePics Images { get; set; }

        [DataMember]
        public String UploadReference { get; set; }
    }
}