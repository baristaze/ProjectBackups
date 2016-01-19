using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class ImageFile : MetaFile, IDBEntity
    {
        public const int MaxSizeInBytes = 10 * 1024 * 1024; // 10 MB
        public static readonly string[] AllowedExtensions = new string[] { "jpg", "jpeg", "png", "bmp", "gif", "tiff", "tif" };

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        protected virtual string CalculateExtension()
        {
            foreach (string ext in AllowedExtensions)
            {
                if (this.ContentType.IndexOf(ext, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return "." + ext;
                }
            }

            return String.Empty;
        }
    }
}
