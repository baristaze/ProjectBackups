using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;

namespace Crosspl.ObjectModel
{
    public class EntryFormatter : IEntryFormatter
    {
        private string applicationRoot;
        public EntryFormatter(string applicationRoot)
        {
            this.applicationRoot = applicationRoot;
        }
        
        public List<long> DetectImageIds(string text)
        {
            ImageIdDetector detector = new ImageIdDetector(
                ImageIdDetector.DefaultRegexPattern, 
                ImageIdDetector.DefaultIdWrapperLeft, 
                ImageIdDetector.DefaultIdWrapperRight);

            return detector.Detect(text);
        }

        public string GetEncodedHtml(string text, List<ImageFile> allocatedImages, params object[] otherParams)
        {
            string linkTemplate = @" <a class='entry-detected-link' target='_blank' href='javascript:void(0);'>{0}<span class='link-data hidden'>{1}</span></a><img class='entry-detected-link-img' src='" + this.applicationRoot + @"/Images/link.png' alt='.' />";

            // detect links and extract            
            LinkReplacer linkReplacer = new LinkReplacer(
                LinkReplacer.DefaultPattern,
                linkTemplate, 
                true, 
                true);

            // replace links with placeholders
            text = linkReplacer.Replace(text);
            
            // html-encode
            text = HttpUtility.HtmlEncode(text);

            // replace images with img tags that have cloud URLs as src
            ImageIdReplacer imageReplacer = new ImageIdReplacer(
                ImageIdReplacer.DefaultRegexPattern, 
                ImageIdReplacer.DefaultIdWrapperLeft, 
                ImageIdReplacer.DefaultIdWrapperRight,
                ImageIdReplacer.DefaultReplacerTemplate,
                allocatedImages);

            // replace images
            text = imageReplacer.Replace(text);

            // BOLD
            text = (new SimpleTagReplacer('*', "<b>", "</b>")).MatchAndReplaceAll(text);
            
            // UNDELINED
            text = (new SimpleTagReplacer('_', "<u>", "</u>")).MatchAndReplaceAll(text);

            // escape http:// and https:// ftp and file
            string protocolPlaceHolder = Guid.NewGuid().ToString("N");
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(@"(https?|ftps?|file)\:\/\/", options);
            text = regex.Replace(text, (Match match) => { return match.Value.Substring(0, match.Length - 2) + protocolPlaceHolder; });

            // handle italic staff
            text = (new SimpleTagReplacer('/', "<i>", "</i>")).MatchAndReplaceAll(text);

            // replace the // which were after the protocol strings like http, https, ftp, ftps, file
            text = text.Replace(protocolPlaceHolder, "//");

            // UPPER CASE
            text = (new SimpleTagReplacer('|', " <span class='upperCase'>", "</span>")).MatchAndReplaceAll(text);

            // apply link placeholders
            text = linkReplacer.ApplyPlaceHolders(text);

            // trim
            text = text.Trim();

            // replace new lines
            text = text.Replace("\n", " <br/>");

            return "<p class='entry-paragraph'>" + text + "</p>";
        }
    }
}
