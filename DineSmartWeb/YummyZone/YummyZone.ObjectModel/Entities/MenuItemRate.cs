using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class MenuItemRate : IEditable
    {
        public Guid CheckInId { get; set; }
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; private set; }
        public byte Rate { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public MenuItemRate()
        {
            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        public NameAndValue ConvertToNameAndValuePair()
        {
            NameAndValue pair = new NameAndValue();
            pair.Name = this.MenuItemName;
            pair.Value = this.Rate.ToString();
            return pair;
        }
    }

    public class MenuItemRateList : List<MenuItemRate> 
    {
        public MenuItemRateList FilterBy(Guid checkinId)
        {
            MenuItemRateList filtered = new MenuItemRateList();
            foreach (MenuItemRate item in this)
            {
                if (item.CheckInId == checkinId)
                {
                    filtered.Add(item);
                }
            }

            return filtered;
        }

        public MenuItemRate this[Guid menuItemId]
        {
            get
            {
                foreach (MenuItemRate item in this)
                {
                    if (item.MenuItemId == menuItemId)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public int IndexOf(Guid menuItemId)
        {
            int index = 0;
            foreach (MenuItemRate item in this)
            {
                if (item.MenuItemId == menuItemId)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }    
    }
}
