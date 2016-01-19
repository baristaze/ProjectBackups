using System;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class VotingSummary : IDBEntity
    {
        [DataMember]
        public int MyVote { get; set; }

        [DataMember]
        public int UpVoteCount { get; set; }

        [DataMember]
        public int DownVoteCount { get; set; }

        [DataMember]
        public int UpVoteSum { get; set; }

        [DataMember]
        public int DownVoteSum { get; set; }

        [DataMember]
        public int NetVoteSum { get; set; }

        [DataMember]
        public string UpvotePercentageAsText
        {
            get
            {
                int totalVoteSum = this.UpVoteSum - this.DownVoteSum; // since the downVoteSum is negative
                return StringHelpers.PercentageAsString(this.UpVoteSum, totalVoteSum, "upvote!", true);
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }
    }
}
