using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public class MapCategoryToMenuItem : RelationMap
    {
        public MapCategoryToMenuItem() : this(Guid.Empty, Guid.Empty, Guid.Empty, Byte.MaxValue) { }

        public MapCategoryToMenuItem(Guid groupId, Guid menuCategoryId, Guid menuItemId, byte orderIndex)
            : base("[dbo].[MenuCategoryAndMenuItemMap]", "[MenuCategoryId]", "[MenuItemId]")
        {
            this.GroupId = groupId;
            this.MenuCategoryId = menuCategoryId;
            this.MenuItemId = menuItemId;
            this.OrderIndex = orderIndex;
        }

        public Guid MenuCategoryId
        {
            get { return this.firstEntityId; }
            set { this.firstEntityId = value; }
        }

        public Guid MenuItemId
        {
            get { return this.secondEntityId; }
            set { this.secondEntityId = value; }
        }
    }

    public class MapListCategoryToMenuItem : List<MapCategoryToMenuItem> 
    {
        public MapCategoryToMenuItem Search(Guid catId, Guid menuItemId)
        {
            foreach (MapCategoryToMenuItem map in this)
            {
                if (map.MenuCategoryId == catId && map.MenuItemId == menuItemId)
                {
                    return map;
                }
            }

            return null;
        }
    }
}
