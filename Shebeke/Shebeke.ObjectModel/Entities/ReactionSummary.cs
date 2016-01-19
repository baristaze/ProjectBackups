using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class ReactionSummary : IDBEntity
    {
        [DataMember]
        public long Top1ReactionId { get; set; }

        [DataMember]
        public long Top2ReactionId { get; set; }

        [DataMember]
        public long Top3ReactionId { get; set; }

        [DataMember]
        public int Top1ReactionCount { get; set; }

        [DataMember]
        public int Top2ReactionCount { get; set; }

        [DataMember]
        public int Top3ReactionCount { get; set; }

        [DataMember]
        public int TotalReactionCount { get; set; }

        [DataMember]
        public List<long> MyReactions { get { return this.myReactions; } }
        private List<long> myReactions = new List<long>();

        [DataMember]
        public string Top1ReactionResult 
        {
            get
            {
                if (this.Top1ReactionId > 0)
                {
                    Reaction reaction = Reaction.GetById(this.Top1ReactionId);
                    string item = reaction == null ? "???" : reaction.Name;
                    return StringHelpers.PercentageAsString(this.Top1ReactionCount, this.TotalReactionCount, item, true);
                }

                return String.Empty;
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        [DataMember]
        public string Top2ReactionResult
        {
            get
            {
                if (this.Top2ReactionId > 0)
                {
                    Reaction reaction = Reaction.GetById(this.Top2ReactionId);
                    string item = reaction == null ? "???" : reaction.Name;
                    return StringHelpers.PercentageAsString(this.Top2ReactionCount, this.TotalReactionCount, item, true);
                }

                return String.Empty;
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        [DataMember]
        public string Top3ReactionResult
        {
            get
            {
                if (this.Top3ReactionId > 0)
                {
                    Reaction reaction = Reaction.GetById(this.Top3ReactionId);
                    string item = reaction == null ? "???" : reaction.Name;
                    return StringHelpers.PercentageAsString(this.Top3ReactionCount, this.TotalReactionCount, item, true);
                }

                return String.Empty;
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        [DataMember]
        public string TopReactionsAsText
        {
            get
            {
                string result = String.Empty;
                string temp = this.Top1ReactionResult;
                if (!String.IsNullOrWhiteSpace(temp)) 
                {
                    result += temp;
                }

                temp = this.Top2ReactionResult;
                if (!String.IsNullOrWhiteSpace(temp))
                {
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        result += ", ";
                    }

                    result += temp;
                }

                temp = this.Top3ReactionResult;
                if (!String.IsNullOrWhiteSpace(temp))
                {
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        result += ", ";
                    }

                    result += temp;
                }

                return result;
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }

        [DataMember]
        public string MyReactionsAsText
        {
            get
            {
                string my = String.Empty;

                for (int x = 0; x < this.MyReactions.Count; x++)
                {
                    Reaction reaction = Reaction.GetById(this.MyReactions[x]);
                    if (reaction != null)
                    {
                        my += reaction.Name;
                        if (x != this.MyReactions.Count - 1)
                        {
                            my += ", ";
                        }
                    }
                }

                if (!String.IsNullOrWhiteSpace(my))
                {
                    my = "Yorumlarınız: " + my;
                }

                return my;
            }
            set
            {
                throw new NotSupportedException("Not supported! Enabled for the sake of fulfilling DataContract Serialization");
            }
        }
    }
}
