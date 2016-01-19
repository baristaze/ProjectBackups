using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Pic4Pic.ObjectModel
{
    public class ConfigOnAzure : ConfigBase
    {
        public override bool Init()
        {
            return true;
        }

        protected override string GetFromStore(string key)
        {
            return RoleEnvironment.GetConfigurationSettingValue(key);
        }
    }
}
