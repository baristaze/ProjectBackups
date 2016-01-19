using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public class ImageIdDetector
    {
        // positive long can be 19 digit at max. that is why we have 18 here
        public const string DefaultRegexPattern = @"\[photo\-[1-9][0-9]{0,18}\]";

        public const string DefaultIdWrapperLeft = "[photo-";
        public const string DefaultIdWrapperRight = "]";

        public string RegexPattern { get; private set; }
        public string IdWrapperLeft { get; private set; }
        public string IdWrapperRight { get; private set; }

        public List<long> DetectedIDs { get { return this.detectedIDs; } }
        private List<long> detectedIDs = new List<long>();

        public ImageIdDetector(string regexPattern, string idWrapperLeft, string idWrapperRight)
        {
            this.RegexPattern = regexPattern;
            this.IdWrapperLeft = idWrapperLeft;
            this.IdWrapperRight = idWrapperRight;
        }
        
        public List<long> Detect(string text)
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(this.RegexPattern, options);

            this.DetectedIDs.Clear();
            regex.Replace(text, this.MatchEvaluator);

            return this.DetectedIDs;
        }

        protected virtual string MatchEvaluator(Match match)
        {
            string val = match.Value;
            if (val.Length >= this.IdWrapperLeft.Length)
            {
                val = val.Substring(this.IdWrapperLeft.Length, val.Length - this.IdWrapperLeft.Length);
            }

            if (val.Length >= this.IdWrapperRight.Length)
            {
                val = val.Substring(0, val.Length - this.IdWrapperRight.Length);
            }

            long id = -1;
            if (long.TryParse(val, out id))
            {
                if (id > 0)
                {
                    this.DetectedIDs.Add(id);

                    // clear the text
                    return String.Empty;
                }
            }

            // do not replace
            return match.Value;
        }
    }
}
