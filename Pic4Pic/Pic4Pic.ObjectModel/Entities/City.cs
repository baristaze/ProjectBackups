using System;

namespace Pic4Pic.ObjectModel
{
    public partial class City : BaseDistrict, IDBEntity
    {
        public int CountryId { get; set; }

        public int RegionId { get; set; }

        public int SubRegionId { get; set; }

        public int OrderIndex { get; set; }

        public int WeightIndex { get; set; }
    }
}
