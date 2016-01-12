using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public class MapMenuToCategory : RelationMap
    {
        public MapMenuToCategory() : this(Guid.Empty, Guid.Empty, Guid.Empty, Byte.MaxValue) { }

        public MapMenuToCategory(Guid groupId, Guid menuId, Guid menuCategoryId, byte orderIndex)
            : base("[dbo].[MenuAndMenuCategoryMap]", "[MenuId]", "[MenuCategoryId]")
        {
            this.GroupId = groupId;
            this.MenuId = menuId;
            this.MenuCategoryId = menuCategoryId;
            this.OrderIndex = orderIndex;
        }

        public Guid MenuId
        {
            get { return this.firstEntityId; }
            set { this.firstEntityId = value; }
        }

        public Guid MenuCategoryId
        {
            get { return this.secondEntityId; }
            set { this.secondEntityId = value; }
        }
    }

    public class MapMenuToCategoryEx : MapMenuToCategory
    {
        public string MenuName { get; set; }
        public string MenuCategoryName { get; set; }
    }

    public class MapListMenuToCategory : List<MapMenuToCategory> { }

    public class MapListMenuToCategoryEx : List<MapMenuToCategoryEx> 
    {
        public MapMenuToCategoryEx SearchByMenuCategoryId(Guid menuCategoryId)
        {
            foreach (MapMenuToCategoryEx item in this)
            {
                if (item.MenuCategoryId == menuCategoryId)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
