using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public class SocialShare : IPrintable
    {
        public DateTime ShareTimeUTC { get; set; }
        public SocialChannel Channel { get; set; }
        public AssetType AssetType { get; set; }
        public ShareType ShareType { get; set; }
        public int ShareCount { get; set; }
        public long UserId { get; set; }
        public int Split { get; set; }

        public virtual List<string> GetPropertyNames() 
        {
            return new List<string>(propNames);
        }

        private static readonly string[] propNames = new string[] { "ShareTimeUTC", "Channel", "AssetType", "ShareType", "ShareCount", "UserId", "Split" };

        public override string ToString()
        {
            return this.ToPrintString("\t");
        }

        public virtual string ToPrintString(string delim)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                this.ShareTimeUTC.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                delim,
                this.Channel,
                delim,
                this.AssetType,
                delim,
                this.ShareType,
                delim,
                this.ShareCount,
                delim,
                this.UserId,
                delim,
                this.Split);
        }
    }

    public class TopicSocialShare : SocialShare
    {
        public long TopicId { get; set; }

        public TopicSocialShare() { }

        public TopicSocialShare(SocialShare copy)
        {
            this.ShareTimeUTC = copy.ShareTimeUTC;
            this.Channel = copy.Channel;
            this.AssetType = copy.AssetType;
            this.ShareType = copy.ShareType;
            this.ShareCount = copy.ShareCount;
            this.UserId = copy.UserId;
            this.Split = copy.Split;
        }

        public override List<string> GetPropertyNames()
        {
            List<string> names = base.GetPropertyNames();
            names.Add("TopicId");
            return names;
        }

        public override string ToPrintString(string delim)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                base.ToPrintString(delim),
                delim,
                this.TopicId);
        }
    }

    public class EntrySocialShare : TopicSocialShare
    {
        public long EntryId { get; set; }

        public EntrySocialShare() { }

        public EntrySocialShare(TopicSocialShare copy) : base(copy)
        {
            this.TopicId = copy.TopicId;
        }

        public override List<string> GetPropertyNames()
        {
            List<string> names = base.GetPropertyNames();
            names.Add("EntryId");
            return names;
        }

        public override string ToPrintString(string delim)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                base.ToPrintString(delim),
                delim,
                this.EntryId);
        }
    }

    public class ReactionSocialShare : EntrySocialShare
    {
        public long ReactionId { get; set; }

        public ReactionSocialShare() { }

        public ReactionSocialShare(EntrySocialShare copy) : base(copy)
        {
            this.EntryId = copy.EntryId;
        }

        public override List<string> GetPropertyNames()
        {
            List<string> names = base.GetPropertyNames();
            names.Add("ReactionId");
            return names;
        }

        public override string ToPrintString(string delim)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                base.ToPrintString(delim),
                delim,
                this.ReactionId);
        }
    }

    public class VoteSocialShare : EntrySocialShare
    {
        public int Vote { get; set; }

        public VoteSocialShare() { }

        public VoteSocialShare(EntrySocialShare copy) : base(copy)
        {
            this.EntryId = copy.EntryId;
        }

        public override List<string> GetPropertyNames()
        {
            List<string> names = base.GetPropertyNames();
            names.Add("Vote");
            return names;
        }

        public override string ToPrintString(string delim)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                base.ToPrintString(delim),
                delim,
                this.Vote);
        }
    }
}
