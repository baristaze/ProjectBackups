using System;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class Entry : IDBEntity, Identifiable
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public AssetStatus Status { get; set; }

        [DataMember]
        public long TopicId { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public int FormatVersion { get; set; }

        [DataMember]
        public long CreatedBy { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

        [DataMember]
        public DateTime LastUpdateTimeUTC { get; set; }

        [DataMember]
        public VotingSummary VotingSummary { get; set; }

        [DataMember]
        public ReactionSummary ReactionSummary { get; set; }

        [DataMember]
        public string ContentAsEncodedHtml { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public AuthorInfo AuthorInfo { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
