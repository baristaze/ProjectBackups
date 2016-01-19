using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using Shebeke.ObjectModel;
using System.Globalization;

namespace Shebeke.Web
{
    public class Config : ConfigBase
    {
        public string DBaseConnectionString { get; set; }
        public BlobStorageAccount BlobStorage { get; set; }
        public string RootWebUrl { get; set; }
        public string ReferenceTimeZone { get; set; }
        public string LocalTimeFormat { get; set; }
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }

        public int UserActivity_LastXDays { get; set; }
        public int RecentPopularTopics_LastXDays { get; set; }
        public int RecentPopularTopics_MaxTopicCount { get; set; }

        public string AES_Key { get; set; }
        public string AES_IV { get; set; }

        public override bool Init()
        {
            this.DBaseConnectionString = this.Get("DBConnectionString");

            this.BlobStorage = new BlobStorageAccount();
            this.BlobStorage.AccountName = this.Get("CloudAccountName");
            this.BlobStorage.AccountKey = this.Get("CloudAccountKey");
            this.BlobStorage.ContainerName = this.Get("CloudBlobEntryImageContainer");
            this.BlobStorage.UriTemplate = this.Get("CloudBlobUriTemplate");
            
            this.RootWebUrl = this.Get("RootWebUrl");
            this.ReferenceTimeZone = this.Get("ReferenceTimeZone");
            this.LocalTimeFormat = this.Get("LocalTimeFormat");
            this.FacebookAppId = this.Get("FacebookAppId");
            this.FacebookAppSecret = this.Get("FacebookAppSecret");

            this.UserActivity_LastXDays = this.GetAsInt("UserActivity_LastXDays", 1, 3650);
            this.RecentPopularTopics_LastXDays = this.GetAsInt("RecentPopularTopics_LastXDays", 1, 3650);
            this.RecentPopularTopics_MaxTopicCount = this.GetAsInt("RecentPopularTopics_MaxTopicCount", 1, 500);

            this.AES_Key = this.Get("AES_Key");
            this.AES_IV = this.Get("AES_IV");
            
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
                throw new ShebekeException("Config key " + key + " couldn't be read from environment", ex);
            }
        }
    }
}