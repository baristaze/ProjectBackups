using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    /// <summary>
    /// DataContractJsonSerializer requires this 'DataContract' flag
    /// </summary>
    [DataContract]
    public partial class MenuCategory : YummyZoneEntity, IEditable
    {
        public const int MaxNameLength = 40;

        public MenuCategory() : this(Guid.Empty, null) { }
        public MenuCategory(Guid groupId, string name) : base(groupId)
        {
            this.Name = name;
        }

        /// <summary>
        /// This needs to be serialized into JSON; therefore it has 'DataMember'
        /// </summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>
        /// This needs to be serialized into JSON; therefore it has 'DataMember'.
        /// </summary>
        [DataMember]
        public MenuItemList Items { get { return this.items; } }
        private MenuItemList items = new MenuItemList();

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class MenuCategoryList : List<MenuCategory> 
    {
        public MenuCategory this[Guid id]
        {
            get
            {
                foreach (MenuCategory item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public MenuCategory this[string name]
        {
            get
            {
                foreach (MenuCategory item in this)
                {
                    if (String.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }
}
