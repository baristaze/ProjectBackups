using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace Shebeke.ObjectModel
{
    public class LinkReplacer
    {
        // default regex pattern
        public const string DefaultPattern = @"((((https?|ftps?)\:\/\/|www\.)[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3})|([a-zA-Z0-9\-\.]+\.(com|net|org|info|co|us|me|tv|mil)(\.tr)?))((\/|\?)\S*)?";

        // placeholders for initial phase
        public Dictionary<Guid, string> WebLinkPlaceHolder { get { return this.webLinkPlaceHolder; } }
        private Dictionary<Guid, string> webLinkPlaceHolder = new Dictionary<Guid, string>();

        // let us know how to detect links
        public string RegexPattern { get; private set; }

        // an html which contains {0} to be replaced
        public string ReplacerTemplate { get; private set; }

        // should we replace directly or replace with placeholders
        public bool UsePlaceholders { get; private set; }

        // should we html encode the matched string?
        public bool EncodeForHtml { get; private set; }

        // caller will set this after Regex.Replace
        public string ReplaceResult { get; private set; }

        // constructor
        public LinkReplacer(string regexPattern, string templateForMatched, bool usePlaceHolders)
            : this(regexPattern, templateForMatched, usePlaceHolders, true)
        {
        }

        // constructor
        public LinkReplacer(string regexPattern, string templateForMatched, bool usePlaceHolders, bool doHtmlEncode)
        {
            this.RegexPattern = regexPattern;
            this.ReplacerTemplate = templateForMatched;
            this.UsePlaceholders = usePlaceHolders;
            this.EncodeForHtml = doHtmlEncode;
        }

        // core method
        public string Replace(string text)
        {
            this.ReplaceResult = null;
            this.WebLinkPlaceHolder.Clear();

            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(this.RegexPattern, options);
            this.ReplaceResult = regex.Replace(text, this.ReplaceLink);
            return this.ReplaceResult;
        }

        // places the place-holders into the given text
        public string ApplyPlaceHolders(string text)
        {
            foreach (Guid key in this.webLinkPlaceHolder.Keys)
            {
                string val = this.webLinkPlaceHolder[key];
                text = text.Replace(key.ToString("N"), val);
            }

            return text;
        }

        // match evaluator that will be called by the Regex
        protected virtual string ReplaceLink(Match match)
        {
            string matchedValue = match.Value;

            /*
            // client will take care of this... user will see this extra http:// if we add it here; which is not a good idea
            if (!matchedValue.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ||
                !matchedValue.StartsWith("ftp", StringComparison.InvariantCultureIgnoreCase))
            {
                matchedValue = "http://" + matchedValue;
            }
            */

            // escape-url
            // string fullLink = HttpUtility.UrlEncode(matchedValue); 
            // string fullLink = matchedValue.Replace('\'', '\"');
            string fullLink = HttpUtility.HtmlEncode(matchedValue);

            // trim
            string trimmed = matchedValue;
            if (trimmed.Length > 35)
            {
                trimmed = trimmed.Substring(0, 32) + "...";
            }

            // encode
            if (this.EncodeForHtml)
            {
                trimmed = HttpUtility.HtmlEncode(trimmed);
            }

            string replaceValue = String.Format(
                CultureInfo.InvariantCulture,
                this.ReplacerTemplate,
                trimmed,
                fullLink);

            if (this.UsePlaceholders)
            {
                Guid placeholder = Guid.NewGuid();
                this.WebLinkPlaceHolder.Add(placeholder, replaceValue);
                return placeholder.ToString("N");
            }
            else
            {
                return replaceValue;
            }
        }
    }
}
