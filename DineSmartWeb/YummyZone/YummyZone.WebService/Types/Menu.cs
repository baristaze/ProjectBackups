using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class Menu : BaseResponse
    {
        [DataMember()]
        public Guid VenueId { get; set; }

        [DataMember()]
        public List<MenuCategory> MenuCategories { get { return this.menuCategories; } }
        private List<MenuCategory> menuCategories = new List<MenuCategory>();

        public override string ToString()
        {
            return this.menuCategories.Count.ToString();
        }
    }
}