using System;

namespace Pic4Pic.ObjectModel
{
    public partial class Country : BaseDistrict, IDBEntity
    {
        public string Code { get; set; }
    }
}
