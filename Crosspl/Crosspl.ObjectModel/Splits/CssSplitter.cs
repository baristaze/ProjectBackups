using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class CssSplitter : Splitter, IDBEntity
    {
        // split-logo-wrp
        [DataMember]
        public string CssClass { get; set; }

        // color:{0}; -or- filter: alpha(opacity=30); opacity: 0.3;
        [DataMember]
        public string CssTemplate { get; set; }

        public CssSplitter() : this(null, null, null, null) { }

        public CssSplitter(string name, string cssClass, string val, string cssTemplate)
            : base(name, val)
        {
            this.CssClass = cssClass;
            this.CssTemplate = cssTemplate;
        }

        public bool IsFullCss
        {
            get 
            {
                return String.IsNullOrWhiteSpace(this.CssTemplate);
            }
        }

        public bool IsCssAttribute
        {
            get 
            {
                return !this.IsFullCss;
            }
        }

        public string ComputedValue
        {
            get
            {
                if (this.IsCssAttribute)
                {
                    return String.Format(CultureInfo.InvariantCulture, this.CssTemplate, this.Value).Trim(' ', ';');
                }
                else
                {
                    return this.Value.Trim(' ', ';');
                }
            }
        }

        public override string ToString()
        {
            return FriendlyName + ComputedValue;
        }

        public void ReplacePlaceholders(string placeholder, string actualVal)
        {
            if (this.Value != null)
            {
                this.Value = this.Value.Replace(placeholder, actualVal);
            }

            /*
            if (this.CssClass != null)
            {
                this.CssClass = this.CssClass.Replace(placeholder, actualVal);
            }
            */

            if (this.CssTemplate != null)
            {
                this.CssTemplate = this.CssTemplate.Replace(placeholder, actualVal);
            }
        }
    }

    [DataContract]
    public class CssSplitterList : SplitterList<CssSplitter>
    {
        public CssSplitterList() : base() { }
        public CssSplitterList(IEnumerable<CssSplitter> items) : base(items) { }

        public CssSplitterList GetByCssClassName(string cssClassName)
        {
            CssSplitterList list = new CssSplitterList();

            for (int x = 0; x < this.Count; x++)
            {
                if (0 == String.Compare(this[x].CssClass, cssClassName, StringComparison.InvariantCultureIgnoreCase))
                {
                    list.Add(this[x]);
                }
            }

            return list;
        }

        public IEnumerable<string> GetUniqueCssClassNames()
        {
            Dictionary<string, string> cssClassList = new Dictionary<string, string>();

            for (int x = 0; x < this.Count; x++)
            {
                if (!cssClassList.ContainsKey(this[x].CssClass))
                {
                    cssClassList.Add(this[x].CssClass, "1");
                }
            }

            return cssClassList.Keys;
        }
    }
}