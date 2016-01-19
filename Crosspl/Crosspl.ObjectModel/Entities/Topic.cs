using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class Topic : IDBEntity, Identifiable
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
        public DateTime CreateTimeUTC { get; set; }

        [DataMember]
        public DateTime LastUpdateTimeUTC { get; set; }

        [DataMember]
        public int EntryCount { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        [DataMember]
        public string SeoLink
        {
            get
            {
                return StringHelpers.GetSeoLink(this.Title);
            }
            set
            {
                // we can't throw since cache mechanism is using set for sure.
                // throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        [DataMember]
        public List<Category> Categories
        {
            get
            {
                return this.categories;
            }
            set
            {
                this.categories = value;
                if (this.categories == null)
                {
                    this.categories = new List<Category>();
                }
            }
        }
        private List<Category> categories = new List<Category>();

        public override string ToString()
        {
            return this.Title;
        }
    }
}
