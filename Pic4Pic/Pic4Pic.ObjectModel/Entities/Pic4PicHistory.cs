using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class Pic4PicHistory
    {
        private List<PicForPic> sentByCandidate = new List<PicForPic>();
        private List<PicForPic> sentToCandidate = new List<PicForPic>();

        [DataMember()]
        public List<PicForPic> SentByCandidate { get { return this.sentByCandidate; } }

        [DataMember()]
        public List<PicForPic> SentToCandidate { get { return this.sentToCandidate; } }

        public Familiarity GetFamiliarity() 
        {
            foreach (PicForPic p in this.sentToCandidate) 
            {
                if (p.IsAccepted()) 
                {
                    return Familiarity.Familiar;
                }
            }

            foreach (PicForPic p in this.sentByCandidate)
            {
                if (p.IsAccepted())
                {
                    return Familiarity.Familiar;
                }
            }

            return Familiarity.Stranger;
        }

        public static Pic4PicHistory From(List<PicForPic> all, Guid senderId, Guid receiverId)
        {
            Pic4PicHistory history = new Pic4PicHistory();
            foreach (PicForPic p in all)
            {
                if (p.UserId1 == senderId && p.UserId2 == receiverId)
                {
                    history.SentToCandidate.Add(p);
                }
                else if (p.UserId1 == receiverId && p.UserId2 == senderId)
                {
                    history.SentByCandidate.Add(p);
                }
            }

            return history;
        }
    }
}
