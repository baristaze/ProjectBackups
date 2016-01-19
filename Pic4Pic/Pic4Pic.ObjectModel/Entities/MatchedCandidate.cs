using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class MatchedCandidate
    {
        [DataMember()]
        public FriendProfile CandidateProfile { get; set; }

        [DataMember()]
        public PicturePair ProfilePics { get; set; }

        [DataMember()]
        public DateTime LastViewTimeUTC { get; set; }

        [DataMember()]
        public DateTime LastLikeTimeUTC { get; set; }

        [DataMember()]
        public List<PicturePair> OtherPictures { get { return this.otherPictures; } }
        protected List<PicturePair> otherPictures = new List<PicturePair>();

        /// <summary>
        /// From candidate to me
        /// </summary>
        [DataMember()]
        public List<PicForPic> SentPic4PicsByCandidate { get { return this.sentByCandidate; } }
        protected List<PicForPic> sentByCandidate = new List<PicForPic>();
        
        /// <summary>
        /// From me to candidate
        /// </summary>
        [DataMember()]
        public List<PicForPic> SentPic4PicsToCandidate { get { return this.sentToCandidate; } }
        protected List<PicForPic> sentToCandidate = new List<PicForPic>();

        public MatchedCandidate()
        { 
        }

        public override string ToString()
        {
            return this.CandidateProfile != null ? this.CandidateProfile.ToString() : "";
        }
    }
}
