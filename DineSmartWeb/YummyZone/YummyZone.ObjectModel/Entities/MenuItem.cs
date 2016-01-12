using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public partial class MenuItem : YummyZoneEntity, IEditable
    {
        public const decimal MaxPrice = 10 * 1000;
        public const int MaxNameLength = 50;
        public const int MaxDescriptionLength = 1000;

        public MenuItem() : base() { }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public Nullable<Decimal> Price { get; set; }

        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public Nullable<Guid> ImageId { get; set; }

        public String LegalNotice { get; set; }
        public DietType DietTypeFlags { get; set; }
        
        public override string ToString()
        {
            return this.Name;
        }
    }

    public class MenuItemList : List<MenuItem> 
    {
        public MenuItem this[Guid id]
        {
            get
            {
                foreach (MenuItem item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public MenuItem this[string name]
        {
            get
            {
                foreach (MenuItem item in this)
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
