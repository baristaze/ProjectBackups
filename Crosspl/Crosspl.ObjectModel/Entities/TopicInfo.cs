using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class TopicInfo : IDBEntity, Identifiable
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public AssetStatus Status { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public long CreatedBy { get; set; }

        [DataMember]
        public int EntryCount { get; set; }

        [DataMember]
        public int VoteCount { get; set; }

        [DataMember]
        public int ReactionCount { get; set; }

        [DataMember]
        public int ShareCount { get; set; }

        [DataMember]
        public int InvitationCount { get; set; }

        [DataMember]
        public double SocialScore { get; set; }

        [DataMember]
        public List<long> TopWriterIDs 
        { 
            get 
            {
                return this.topWriterIDs; 
            }
            set
            {
                this.topWriterIDs = value;
                if (this.topWriterIDs == null)
                {
                    this.topWriterIDs = new List<long>();
                }
            }

        }
        private List<long> topWriterIDs = new List<long>();

        [DataMember]
        public string SeoLink
        {
            get
            {
                return StringHelpers.GetSeoLink(this.Title);
            }
            set
            {
                // we can't throw since cache serializes this
                // throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        // additional properties
        [DataMember]
        public FacebookUser Creator { get; set; }

        [DataMember]
        public List<FacebookUser> TopWriters 
        {
            get
            {
                return this.topWriters;
            }
            set
            {
                this.topWriters = value;
                if (this.topWriters == null)
                {
                    this.topWriters = new List<FacebookUser>();
                }
            }
        }
        private List<FacebookUser> topWriters = new List<FacebookUser>();

        public override string ToString()
        {
            return this.Title;
        }
    }
}
