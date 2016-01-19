using System;

namespace Pic4Pic.ObjectModel
{
    public partial class SubRegion : BaseDistrict, IDBEntity
    {
        public int CountryId { get; set; }

        public int RegionId { get; set; }

        public int OrderIndex { get; set; }
    }
}
