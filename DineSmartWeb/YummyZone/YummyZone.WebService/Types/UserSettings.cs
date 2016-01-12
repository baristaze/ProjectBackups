using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class UserSettings : BaseResponse
    {
        [DataMember()]
        public UserProfile Settings { get; set; }

        public UserSettings() : this(null) { }

        public UserSettings(UserProfile data)
        {
            this.Settings = data;
        }
    }

    [DataContract()]
    public class UserProfile
    {
        private const string PropertyVoid = "0";
        private const string PropertyEnableFacebook = "EnableFacebook";
        private const string PropertyEnableTwitter = "EnableTwitter";

        private static readonly string[] PropertyNames = new string[] 
        {
            PropertyVoid,
            PropertyEnableFacebook,
            PropertyEnableTwitter,
        };

        [DataMember()]
        public byte EnableFacebook { get; set; }

        [DataMember()]
        public byte EnableTwitter { get; set; }

        public UserProfile()
        {
        }

        internal UserProfile(OM.DinerSettings settings)
        {
            this.Init(settings);
        }

        internal void Init(OM.DinerSettings settings)
        {
            foreach (OM.DinerSettingsItem item in settings)
            {
                this.Update(item.Name, item.Value);
            }
        }

        internal bool Update(string name, string value)
        {
            if (String.Compare(PropertyVoid, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (String.Compare(PropertyEnableFacebook, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (Helpers.ConvertToBoolTrue(value))
                {
                    this.EnableFacebook = 1;
                    return true;
                }
                else if (Helpers.ConvertToBoolFalse(value))
                {
                    this.EnableFacebook = 0;
                    return true;
                }
            }

            if (String.Compare(PropertyEnableTwitter, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (Helpers.ConvertibleToBool(value))
                {
                    this.EnableTwitter = 1;
                }
                else if (Helpers.ConvertToBoolFalse(value))
                {
                    this.EnableTwitter = 0;
                    return true;
                }
            }

            return false;
        }

        internal static bool IsValidName(string name)
        {
            foreach (string prop in PropertyNames)
            {
                if (String.Compare(prop, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsValidValue(string name, string value)
        {
            if (String.Compare(PropertyVoid, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (String.Compare(PropertyEnableFacebook, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Helpers.ConvertibleToBool(value);
            }

            if (String.Compare(PropertyEnableTwitter, name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Helpers.ConvertibleToBool(value);
            }

            return false;
        }
    }
}