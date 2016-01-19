﻿using System;
using System.IO;
using System.Runtime.Serialization;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Crosspl.ObjectModel
{
    public partial class ImageFile : MetaFile, IDBEntity
    {
        public string WriteToCloud(BlobStorageAccount blobStorage, Stream data)
        {
            try
            {
                CloudBlobContainer blobContainer = blobStorage.GetBlobContainer(true);

                string fileName = "img_" + this.CloudId.ToString("N") + this.CalculateExtension();
                CloudBlob blob = blobContainer.GetBlobReference(fileName);
                this.CloudUrl = blobStorage.RootBlobUri + "/" + blobStorage.ContainerName + "/" + fileName;

                // blob.Properties.BlobType = BlobType.BlockBlob; // read-only
                blob.Properties.ContentType = this.ContentType;
                blob.Properties.CacheControl = "public";
                blob.UploadFromStream(data);

                return this.CloudUrl;
            }
            catch (Exception ex)
            {
                throw new CrossplException("File couldn't be uploaded to the cloud storage", ex);
            }
        }

        public static bool DeleteFromCloud(BlobStorageAccount blobStorage, string absoluteUri) 
        {
            try
            {
                CloudBlobContainer blobContainer = blobStorage.GetBlobContainer(false);
                CloudBlob blob = blobContainer.GetBlobReference(absoluteUri);
                return blob.DeleteIfExists();
            }
            catch (Exception ex)
            {
                throw new CrossplException("File couldn't be deleted from the cloud storage", ex);
            }
        }
    }
}
