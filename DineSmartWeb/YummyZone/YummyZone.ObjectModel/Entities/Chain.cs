using System;
using System.Collections.Generic;
using System.Runtime.Serialization; 

namespace YummyZone.ObjectModel 
{
    [Flags]
    public enum VenueType
    {
        Unspecified = 0,
    }

    [Flags]
    public enum CuisineType
    {
        Unspecified = 0,
    }

    public partial class Chain : YummyZoneEntity, IEditable
    {
        public VenueStatus Status { get; set; }
        public string Name { get; set; }
        public VenueType VenueTypeFlags { get; set; }
        public CuisineType CuisineTypeFlags { get; set; }
        public string WebURL { get; set; }
        public string LogoURL { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
