using System;

namespace YummyZone.ObjectModel
{
    public partial class MenuItemImage : AssetImage
    {
        public MenuItemImage() : base("[dbo].[MenuItemImage]", "[MenuItemId]")
        {
        }

        public Guid MenuItemId 
        {
            get
            {
                return this.AssetId;
            }
            set
            {
                this.AssetId = value;
            }
        }
    }
}
