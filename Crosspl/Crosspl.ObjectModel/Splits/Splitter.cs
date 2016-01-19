using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{   
    /*
Editor // Image picker, Text editor
Viewer // Plain value, Path to a File
Data   // text, file, css, css-value
DataTemplate // null, css-template, null, css-template
FriendlyName // Brand Text Color

Text: fname, value, text editor, code-behind-inject, save to db
Plain-CSS: fname, value, text editor, [css-template=null], dynamic-css, save to db
CSS-val: fname, value, text editor, css-template, dynamic-css, save to db
File: fname, value, file-picker, css-template, dynamic-css, save to storage, save to db
     */

    [DataContract]
    public partial class Splitter // : IDBEntity
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public int SectionId { get; set; }

        [DataMember]
        public int VariationId { get; set; }

        [DataMember]
        public string FriendlyName { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public DateTime CreateTimeUTC { get; set; }

        [DataMember]
        public DateTime LastUpdateTimeUTC { get; set; }

        public Splitter() : this(null, null) { }

        public Splitter(string name, string val)
        {
            this.FriendlyName = name;
            this.Value = val;

            DateTime utcNow = DateTime.UtcNow;
            this.CreateTimeUTC = utcNow;
            this.LastUpdateTimeUTC = utcNow;
        }

        public override string ToString()
        {
            return FriendlyName + " = " + Value;
        }

        public static string SanitizeSectionFilters(string sections)
        {
            if (String.IsNullOrWhiteSpace(sections))
            {
                sections = "0";
            }
            else
            {
                List<long> ids = CrossplUtils.TokenizeIDs(sections);
                if (ids.Count > 0)
                {
                    sections = String.Join(",", ids);
                }
                else
                {
                    sections = "0";
                }
            }

            return sections;
        }

        public static int DetermineSplitId(UserAuthInfo user, int senderSplitId, string splitText)
        {
            int theSplitId = 0;
            if (user != null && user.SplitId > 0)
            {
                theSplitId = user.SplitId;
            }
            else if (senderSplitId > 0)
            {
                theSplitId = senderSplitId;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(splitText))
                {
                    int tempSplitId = 0;
                    if (Int32.TryParse(splitText, out tempSplitId))
                    {
                        if (tempSplitId > 0)
                        {
                            // this may not exist in reality. Though, the database will return default records
                            theSplitId = tempSplitId;
                        }
                    }
                }
            }

            return theSplitId;
        }

        public static string AppendSplitId(
            UserAuthInfo user,
            int defaultSplitId,
            int senderSplitId,
            string link,
            NameValueCollection queryString,
            ref string queryParamDelim)
        {
            string splitText = String.Empty;
            if (queryString != null)
            {
                splitText = queryString["split"];
            }

            int theSplitId = Splitter.DetermineSplitId(user, senderSplitId, splitText);
            if (theSplitId > 0 && theSplitId != defaultSplitId)
            {
                link += queryParamDelim + "split=" + theSplitId;
                queryParamDelim = "&";
            }

            return link;
        }

        public static string AppendExperimentId(
            string link,
            NameValueCollection queryString,
            bool forceAddition,
            string predefinedExperimentId,
            ref string queryParamDelim)
        {
            string experimentId = String.Empty;
            if (queryString != null)
            {
                experimentId = queryString["utm_expid"];
            }

            if (!String.IsNullOrWhiteSpace(experimentId))
            {
                link += queryParamDelim + "utm_expid=" + experimentId;
                queryParamDelim = "&";
            }
            else if (forceAddition && !String.IsNullOrWhiteSpace(predefinedExperimentId))
            {
                link += queryParamDelim + "utm_expid=" + predefinedExperimentId;
                queryParamDelim = "&";
            }

            return link;
        }
    }

    [DataContract]
    public class SplitterList<T> : List<T> where T : Splitter
    {
        public SplitterList() : base() { }

        public SplitterList(IEnumerable<T> items) : base(items) { }

        public T this[string name]
        {
            get
            {
                for (int x = 0; x < this.Count; x++)
                {
                    if (0 == String.Compare(this[x].FriendlyName, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return this[x];
                    }
                }

                return null;
            }
        }
    }
}