using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class DinerSettingsItem : IEditable
    {
        public Guid DinerId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public DinerSettingsItem() : this(Guid.Empty, null, null)
        { 
        }

        public DinerSettingsItem(Guid dinerId, string name, string value)
        {
            this.DinerId = dinerId;
            this.Name = name;
            this.Value = value;

            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }
    }

    public partial class DinerSettings : List<DinerSettingsItem>
    {
        public string this[string name]
        {
            get
            {
                foreach (DinerSettingsItem item in this)
                {
                    if (String.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return item.Value;
                    }
                }

                return null;
            }
        }
    }
}
