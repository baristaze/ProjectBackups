using System;

namespace YummyZone.ObjectModel
{
    public partial class LogoImage : AssetImage
    {
        public LogoImage() : base("[dbo].[LogoImage]", "[ChainId]")
        {
        }

        public Guid ChainId 
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