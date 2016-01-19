using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public partial class ImageFile : Identifiable, IDBEntity
    {
        public const int MaxSizeInBytes = 10 * 1024 * 1024; // 10 MB
        public static readonly string[] AllowedExtensions = new string[] { "jpg", "jpeg", "png", "bmp", "gif", "tiff", "tif" };

        public ImageFile() : this(Guid.Empty) { }

        public ImageFile(Guid id)
        {
            this.Id = id;
            this.CreateTimeUTC = DateTime.UtcNow;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid GroupingId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        internal AssetState Status { get; set; }

        [DataMember]
        public string ContentType { get; set; }
        
        [DataMember]
        public int ContentLength { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public string CloudUrl { get; set; }

        [DataMember]
        public bool IsBlurred { get; set; }

        [DataMember]
        public bool IsThumbnail { get; set; }

        [DataMember]
        public bool IsProfilePicture { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }

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
