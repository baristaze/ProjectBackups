using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class MenuItem
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public string Name { get; set; }

        [DataMember()]
        public string Description { get; set; }

        [DataMember()]
        public decimal Price { get; set; }

        [DataMember()]
        public string ImageUrl { get; set; }
        
        [DataMember()]
        public byte SelfRating { get; set; }

        [DataMember()]
        public decimal AverageRating { get; set; }

        [DataMember()]
        public bool IsMostLikedItem { get; set; }

        public MenuItem() { }

        public MenuItem(OM.MenuItem menuItem, string imageUrlTemplate)
        {
            this.Id = menuItem.Id;
            this.Name = menuItem.Name;
            this.Description = String.IsNullOrWhiteSpace(menuItem.Description) ? String.Empty : menuItem.Description;
            this.Price = menuItem.Price.HasValue ? menuItem.Price.Value : (decimal)0.0;

            if (menuItem.ImageId.HasValue)
            {
                this.ImageUrl = String.Format(CultureInfo.InvariantCulture, imageUrlTemplate, menuItem.ImageId.Value);
            }
            else
            {
                this.ImageUrl = String.Empty;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static int CompareByAverageRating(MenuItem m1, MenuItem m2)
        {
            return m1.AverageRating.CompareTo(m2.AverageRating);
        }
    }
}