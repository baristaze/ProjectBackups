using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    [DataContract]
    public partial class Feedback
    {
        [DataMember]
        public Guid CheckInId { get; set; }

        [DataMember]
        public DateTime CheckInTimeUTC { get; set; }

        /// <summary>
        /// this is convenient since there is not a date format in javascript
        /// </summary>
        [DataMember]
        public string CheckInTimeUtcAsText 
        { 
            get 
            { 
                return this.CheckInTimeUTC.ToString(); 
            }
            set
            {
                // json serializer needs a setter, too, to honor a property during serialization
            }
        }

        [DataMember]
        public string DinerType { get; set; }

        [DataMember]
        public string CustomerType { get; set; }

        [DataMember]
        public bool IsRead { get; set; }

        [DataMember]
        public bool IsReplied { get; set; }

        [DataMember]
        public bool IsCouponSent { get; set; }

        [DataMember]
        public NameAndValueList RateItems { get { return this.rateItems; } }
        private NameAndValueList rateItems = new NameAndValueList();

        [DataMember]
        public NameAndValueList YesNoItems { get { return this.yesNoItems; } }
        private NameAndValueList yesNoItems = new NameAndValueList();

        [DataMember]
        public NameAndValueList MultiChoiceItems { get { return this.multiChoiceItems; } }
        private NameAndValueList multiChoiceItems = new NameAndValueList();

        [DataMember]
        public NameAndValueList FreeFormItems { get { return this.freeFormItems; } }
        private NameAndValueList freeFormItems = new NameAndValueList();

        public bool HasAnyComment
        {
            get
            {
                if (this.rateItems.Count > 0)
                {
                    return true;
                }

                if (this.yesNoItems.Count > 0)
                {
                    return true;
                }

                if (this.multiChoiceItems.Count > 0)
                {
                    return true;
                }

                if (this.freeFormItems.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }
    }
        
    public class FeedbackList : List<Feedback> { }
}
