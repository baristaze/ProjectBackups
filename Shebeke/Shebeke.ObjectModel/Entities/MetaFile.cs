using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class MetaFile : Identifiable
    {
        public MetaFile()
        {
            this.CreateTimeUTC = DateTime.UtcNow;
        }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid CloudId { get; set; }

        [DataMember]
        public long CreatedBy { get; set; }

        [DataMember]
        public long AssetId { get; set; }

        [DataMember]
        public AssetType AssetType { get; set; }

        [DataMember]
        public string ContentType { get; set; }
        
        [DataMember]
        public int ContentLength { get; set; }

        [DataMember]
        public string OriginalUrl { get; set; }

        [DataMember]
        public string CloudUrl { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

    }
}
