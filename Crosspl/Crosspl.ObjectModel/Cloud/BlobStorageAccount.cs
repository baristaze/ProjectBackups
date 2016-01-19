using System;
using System.IO;
using System.Globalization;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Crosspl.ObjectModel
{
    public class BlobStorageAccount
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string ContainerName { get; set; }
        public string UriTemplate { get; set; }

        public string RootBlobUri
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, this.UriTemplate, this.AccountName);
            }
        }

        public CloudBlobContainer GetBlobContainer(bool createIfNotExist)
        {
            StorageCredentialsAccountAndKey cred = new StorageCredentialsAccountAndKey(this.AccountName, this.AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(cred, false);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(this.ContainerName);
            if (createIfNotExist && blobContainer.CreateIfNotExist())
            {
                BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                blobContainer.SetPermissions(containerPermissions);
            }

            return blobContainer;
        }
    }
}
