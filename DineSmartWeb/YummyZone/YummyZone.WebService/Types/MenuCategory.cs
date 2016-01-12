using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class MenuCategory
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public List<MenuItem> MenuItems { get { return this.menuItems; } }
        private List<MenuItem> menuItems = new List<MenuItem>();

        public MenuItem this[Guid id]
        {
            get
            {
                foreach (MenuItem item in this.menuItems)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}