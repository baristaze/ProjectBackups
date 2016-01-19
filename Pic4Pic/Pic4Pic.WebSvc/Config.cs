using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsAzure.ServiceRuntime;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WebSvc
{
    public class Config : ConfigBase
    {
        public string DBaseConnectionString { get; set; }
        public BlobStorageAccount BlobStorage { get; set; }
        public string RootWebUrl { get; set; }
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }

        public string AES_Key { get; set; }
        public string AES_IV { get; set; }

        public string GooglePlayBillingServicePublicKey { get; set; }

        public string GoogleAPIServerKey { get; set; }

        public override bool Init()
        {
            this.DBaseConnectionString = this.Get("DBConnectionString");

            this.BlobStorage = new BlobStorageAccount();
            this.BlobStorage.AccountName = this.Get("CloudAccountName");
            this.BlobStorage.AccountKey = this.Get("CloudAccountKey");
            this.BlobStorage.ContainerName = this.Get("CloudBlobEntryImageContainer");
            this.BlobStorage.UriTemplate = this.Get("CloudBlobUriTemplate");
            
            this.RootWebUrl = this.Get("RootWebUrl");            
            this.FacebookAppId = this.Get("FacebookAppId");
            this.FacebookAppSecret = this.Get("FacebookAppSecret");

            this.AES_Key = this.Get("AES_Key");
            this.AES_IV = this.Get("AES_IV");

            this.GooglePlayBillingServicePublicKey = this.Get("GooglePlayBillingServicePublicKey");
            this.GoogleAPIServerKey = this.Get("GoogleAPIServerKey");
            
            return true;
        }

        protected override string GetFromStore(string key)
        {
            try
            {
                return RoleEnvironment.GetConfigurationSettingValue(key);
            }
            catch (Exception ex)
            {
                throw new Pic4PicException("Config key " + key + " couldn't be read from environment", ex);
            }
        }
    }
}