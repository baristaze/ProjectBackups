using System;

namespace Pic4Pic.ObjectModel
{
    public partial class Region : BaseDistrict, IDBEntity
    {
        public string Code { get; set; }
        public int CountryId { get; set; }
    }
}
