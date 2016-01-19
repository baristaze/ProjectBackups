using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace Pic4Pic.ObjectModel
{
    public class MultiImageUploader
    {
        private MemoryStream clearImageData;
        private BlobStorageAccount blobStorage;
        private int thumbnailWidth;
        private int thumbnailHeight;
        private int pixelizationRatio;        

        public MultiImageUploader(
            MemoryStream clearImageData, 
            BlobStorageAccount blobStorage, 
            int thumbnailWidth,
            int thumbnailHeight,
            int pixelizationRatio)
        {
            this.clearImageData = clearImageData;
            this.blobStorage = blobStorage;
            this.thumbnailWidth = thumbnailWidth;
            this.thumbnailHeight = thumbnailHeight;
            this.pixelizationRatio = pixelizationRatio;            
        }

        private Task<ImageFile> CreateTask(bool blurize, bool resize) 
        {
            // Copy the original data into a new memory stream to isolate image operations.
            // Otherwise, same memory is used by different threads and race conditions occurs.
            // Bottom line is that every thread should have its own bitmap<->memory stream
            this.clearImageData.Position = 0;
            MemoryStream memoryStream = new MemoryStream();
            this.clearImageData.CopyTo(memoryStream);

            // created memory stream will be disposed by the thread within SafeUploadSingle() method            
            Task<ImageFile> task = new Task<ImageFile>(() => SafeUploadSingle(blurize, resize, memoryStream));

            // don't start the new task; i.e. thread, yet.
            return task;
        }

        public ImageFile[] SafeUploadAllOrNone()
        {
            // create tasks to be executed in parallel
            Task<ImageFile> t1 = this.CreateTask(false, false);
            Task<ImageFile> t2 = this.CreateTask(true, false);
            Task<ImageFile> t3 = this.CreateTask(false, true);
            Task<ImageFile> t4 = this.CreateTask(true, true);

            // run them in parallel
            List<Task<ImageFile>> tasks = new List<Task<ImageFile>>() { t1, t2, t3, t4 };
            foreach (Task<ImageFile> t in tasks)
            {
                t.Start();
            }

            // collect results... Task.Result holds the main thread; i.e. it acts as a 'Join'
            ImageFile[] images = new ImageFile[] { t1.Result, t2.Result, t3.Result, t4.Result };

            // all results are retrieved... check if they all are good or not.
            bool atLeastOneFailure = false;
            foreach(ImageFile imageFile in images) 
            {
                if (imageFile == null) 
                {
                    atLeastOneFailure = true;
                    break;
                }
            }

            if (atLeastOneFailure)
            {
                foreach (ImageFile imageFile in images)
                {
                    if (imageFile != null)
                    {
                        ImageFile.SafeDeleteFromCloud(this.blobStorage, imageFile.CloudUrl);
                    }
                }

                images = null;
            }
            else
            {
                Guid groupId = images[3].Id; // ID of blurized, thumbnail image
                foreach (ImageFile image in images)
                {
                    image.GroupingId = groupId;
                }
            }

            return images;
        }

        // this is executed in a parallel thread
        private ImageFile SafeUploadSingle(bool blurize, bool resize, MemoryStream memoryStream)
        {
            try
            {
                using (Bitmap clearImage = new Bitmap(memoryStream))
                {
                    return this.UploadSingle(blurize, resize, clearImage);
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                memoryStream.Dispose();
            }
        }

        // this is executed in a parallel thread
        private ImageFile UploadSingle(bool blurize, bool resize, Bitmap clearImage)
        {
            List<Bitmap> resourcesToDispose = new List<Bitmap>();

            try
            {
                // don't add this to the resource to be cleaned. it will be cleaned by the caller
                Bitmap target = clearImage;

                if (resize)
                {
                    // change the size
                    target = ImageHelper.FixedSize(target, this.thumbnailWidth, this.thumbnailHeight, true);

                    // above line creates a new bitmap. therefore we need to dispose it at the end
                    resourcesToDispose.Add(target);
                }

                if (blurize)
                {
                    // blurize
                    int pixelationSize = Math.Max(10, (Math.Min(target.Width, target.Height) / this.pixelizationRatio));
                    target = ImageHelper.Pixelate(target, pixelationSize);

                    // above line creates a new bitmap. therefore we need to dispose it at the end
                    resourcesToDispose.Add(target);
                }

                // create regular image
                ImageFile metaFile = new ImageFile();
                metaFile.Id = Guid.NewGuid();
                metaFile.Status = AssetState.New;
                metaFile.Width = target.Width;
                metaFile.Height = target.Height;
                metaFile.IsBlurred = blurize;
                metaFile.IsThumbnail = resize;

                // write to cloud
                using (MemoryStream rawData = new MemoryStream())
                {
                    // convert bitmap to stream
                    target.Save(rawData, System.Drawing.Imaging.ImageFormat.Jpeg);
                    
                    // update content propertied
                    metaFile.ContentLength = (int)rawData.Length;
                    metaFile.ContentType = "image/jpeg";

                    // write to cloud
                    rawData.Position = 0;
                    metaFile.WriteToCloud(this.blobStorage, rawData);
                }

                // return
                return metaFile;
            }
            finally 
            {
                foreach (Bitmap bmp in resourcesToDispose)
                {
                    bmp.Dispose();
                }
            }
        }
    }
}
