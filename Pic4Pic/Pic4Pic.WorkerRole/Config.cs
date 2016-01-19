using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsAzure.ServiceRuntime;

using Pic4Pic.ObjectModel;

namespace Pic4Pic.WorkerRole
{
    public class Config : ConfigBase
    {
        public string DBaseConnectionString { get; set; }

        public int ConfigRefreshPeriodInSec { get; set; }

        public int EnvironmentId { get; set; }

        public override bool Init()
        {
            this.DBaseConnectionString = this.Get("DBConnectionString");
            this.ConfigRefreshPeriodInSec = this.GetAsInt("ConfigRefreshPeriodInSec", 600, 0, 86400);
            this.EnvironmentId = this.GetAsInt("EnvironmentId", 0, 0, 1);

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
