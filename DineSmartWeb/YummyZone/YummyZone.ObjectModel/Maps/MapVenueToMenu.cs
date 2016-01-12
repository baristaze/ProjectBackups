using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public class MapVenueToMenu : RelationMap
    {
        public MapVenueToMenu() : this(Guid.Empty, Guid.Empty, Guid.Empty) { }
        public MapVenueToMenu(Guid groupId, Guid venueId, Guid menuId) : this(groupId, venueId, menuId, Byte.MaxValue) { }
        public MapVenueToMenu(Guid groupId, Guid venueId, Guid menuId, byte orderIndex)
            : base("[dbo].[VenueAndMenuMap]", "[VenueId]", "[MenuId]")
        {
            this.GroupId = groupId;
            this.VenueId = venueId;
            this.MenuId = menuId;
            this.OrderIndex = orderIndex;
        }

        public Guid VenueId
        {
            get { return this.firstEntityId; }
            set { this.firstEntityId = value; }
        }

        public Guid MenuId
        {
            get { return this.secondEntityId; }
            set { this.secondEntityId = value; }
        }
    }

    public class MapListVenueToMenu : List<MapVenueToMenu> { }
}
