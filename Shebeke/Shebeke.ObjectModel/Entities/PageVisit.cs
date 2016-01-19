using System;
using System.Collections.Generic;
using System.Globalization;

namespace Shebeke.ObjectModel
{
    public class PageVisit : IPrintable
    {
        public DateTime ShareTimeUTC { get; set; }
        public string Page { get; set; }
        public string Referrer { get; set; }
        public long UserId { get; set; }
        public int Split { get; set; }
        public long TopicId { get; set; }
        public long EntryId { get; set; }
        
        public virtual List<string> GetPropertyNames()
        {
            return new List<string>(propNames);
        }

        private static readonly string[] propNames = new string[] { "ShareTimeUTC", "Page", "Referrer", "UserId", "Split", "TopicId", "EntryId" };

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
                this.Page,
                delim,
                this.Referrer,
                delim,
                this.UserId,
                delim,
                this.Split,
                delim,
                this.TopicId,
                delim,
                this.EntryId);
        }
    }
}
