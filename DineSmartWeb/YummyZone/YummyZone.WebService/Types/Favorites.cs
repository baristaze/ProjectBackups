using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class FavoriteMenuItem
    {
        internal Guid itemImageId { get; set; }

        [DataMember()]
        public string MenuItemName { get; set; }

        [DataMember()]
        public string VenueName { get; set; }

        [DataMember()]
        public decimal MenuItemAverageRate { get; set; }

        [DataMember()]
        public string ImageUrl
        {
            get
            {
                if (itemImageId != Guid.Empty)
                {
                    return String.Format(CultureInfo.InvariantCulture, Helpers.ImageFileUrlTemplate, itemImageId);
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                // do nothing
            }
        }
    }

    [DataContract()]
    public class FavoriteVenue
    {
        internal Guid logoImageId { get; set; }

        [DataMember()]
        public string VenueName { get; set; }

        [DataMember()]
        public decimal VenueAverageRate { get; set; }

        [DataMember()]
        public string ImageUrl
        {
            get
            {
                if (logoImageId != Guid.Empty)
                {
                    return String.Format(CultureInfo.InvariantCulture, Helpers.LogoFileUrlTemplate, logoImageId);
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                // do nothing
            }
        }
    }

    [DataContract()]
    public class Favorites : BaseResponse
    {
        [DataMember()]
        public List<FavoriteMenuItem> FavoriteMenuItems { get { return this.favoriteMenuItems; } }
        private List<FavoriteMenuItem> favoriteMenuItems = new List<FavoriteMenuItem>();

        [DataMember()]
        public List<FavoriteVenue> FavoriteVenues { get { return this.favoriteVenues; } }
        private List<FavoriteVenue> favoriteVenues = new List<FavoriteVenue>();
    }
}