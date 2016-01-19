using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public enum Side
    {
        [EnumMember]
        Client = 0,

        [EnumMember]
        Server
    }

    [DataContract]
    public partial class TextSplitter : Splitter, IDBEntity
    {
        [DataMember]
        public Side SideToApply { get; set; }

        [DataMember]
        public string JQSelector { get; set; }

        public TextSplitter() : this(Side.Client, null, null, null) { }

        public TextSplitter(Side sideToApply, string name, string jqSelector, string val)
            : base(name, val)
        {
            this.JQSelector = jqSelector;
            this.SideToApply = sideToApply;
        }

        public void ReplacePlaceholders(string placeholder, string actualVal)
        {
            if (this.Value != null)
            {
                this.Value = this.Value.Replace(placeholder, actualVal);
            }

            /*
            if (this.JQSelector != null)
            {
                this.JQSelector = this.JQSelector.Replace(placeholder, actualVal);
            }
            */
        }
    }

    [DataContract]
    public class TextSplitterList : SplitterList<TextSplitter>
    {
        public TextSplitterList() : base() { }
        public TextSplitterList(IEnumerable<TextSplitter> items) : base(items) { }
    }
}