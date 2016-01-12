using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class Menu : YummyZoneEntity, IEditable
    {
        public const int MaxNameLength = 25;

        public Menu() { }
        public Menu(Guid groupId, string name, short serviceStartTime, short serviceEndTime) : base(groupId)
        {
            this.Name = name;
            this.ServiceStartTime = serviceStartTime;
            this.ServiceEndTime = serviceEndTime;
        }

        public String Name { get; set; }
        public short? ServiceStartTime { get; set; }
        public short? ServiceEndTime { get; set; }
        
        public override string ToString()
        {
            return this.Name;
        }

        internal MenuCategoryList Categories { get { return this.categories; } }
        private MenuCategoryList categories = new MenuCategoryList();
    }

    public class MenuList : List<Menu> 
    {
        public Menu this[Guid id]
        {
            get
            {
                foreach (Menu item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public MenuCategory SearchByCategoryId(Guid categoryId)
        {
            foreach (Menu menu in this)
            {
                MenuCategory cat = menu.Categories[categoryId];
                if (cat != null)
                {
                    return cat;
                }
            }

            return null;
        }
    }
}
