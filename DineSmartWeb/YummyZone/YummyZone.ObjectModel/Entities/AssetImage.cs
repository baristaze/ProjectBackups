using System;

namespace YummyZone.ObjectModel
{
    public partial class AssetImage : IEditable
    {
        public const int MaxImageSizeInBytes = 10 * 1024 * 1024; // 10 MB
        public static readonly string[] AllowedExtensions = new string[] { "jpeg", "jpg", "png", "bmp", "gif", "tif", "tiff" };

        protected string imageTableName { get; private set; }
        protected string assetIdColumnName { get; private set; }

        public AssetImage(string imageTableName, string assetIdColumnName)
        {
            this.imageTableName = imageTableName;
            this.assetIdColumnName = assetIdColumnName;

            DateTime utc = DateTime.Now;
            CreateTimeUTC = utc;
        }

        protected Guid AssetId { get; set; }
        public Guid GroupId { get; set; }
        public String ContentType { get; set; }
        public int ContentLength { get; set; }
        public String InitialFileNameOrUrl { get; set; }
        public Byte[] Data { get; set; }
        public DateTime CreateTimeUTC { get; set; }
    }
}
