using System;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class RatedItem
    {
        internal Guid itemImageId { get; set; }

        [DataMember()]
        public string MenuItemName { get; set; }

        [DataMember()]
        public byte MenuItemRate { get; set; }

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
    public class HistoryItem
    {
        internal Guid logoImageId { get; set; }

        [DataMember()]
        public string VenueName { get; set; }

        [DataMember()]
        public DateTime CheckinTimeUTC { get; set; }

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

        [DataMember()]
        public List<RatedItem> RatedItems { get { return this.ratedItems; } }
        private List<RatedItem> ratedItems = new List<RatedItem>();
    }

    [DataContract()]
    public class History : BaseResponse
    {
        [DataMember()]
        public List<HistoryItem> Checkins { get { return this.checkins; } }
        private List<HistoryItem> checkins = new List<HistoryItem>();
    }
}