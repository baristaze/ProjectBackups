using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;

namespace Shebeke.ObjectModel
{
    public class EntryPlainTextFormatter : IEntryFormatter
    {
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
            // clear link templates
            
            /*
            string linkTemplate = @"{0}";

            // detect links and extract            
            LinkReplacer linkReplacer = new LinkReplacer(
                LinkReplacer.DefaultPattern,
                linkTemplate,
                true,
                true);

            // replace links with placeholders
            text = linkReplacer.Replace(text);
            */

            // html-encode
            // text = HttpUtility.HtmlEncode(text);

            // change the source format; we don't want to expose azure urls...
            string imageUrl = String.Empty;
            string imageReplacerFormat = String.Empty;
            ImageIdReplacer.ImageSourceGetter imgSrcConverter = null;
            if (otherParams.Length > 0)
            {
                imageReplacerFormat = "{0}";
                string rootUrl = otherParams[0].ToString();
                imageUrl = rootUrl + "/photo";
                imgSrcConverter = (ImageFile imageFile) => { return imageUrl + "/" + imageFile.Id.ToString(); };
            }

            // replace images with img tags that have cloud URLs as src
            ImageIdReplacer imageReplacer = new ImageIdReplacer(
                ImageIdReplacer.DefaultRegexPattern,
                ImageIdReplacer.DefaultIdWrapperLeft,
                ImageIdReplacer.DefaultIdWrapperRight,
                imageReplacerFormat, // put directly its link
                allocatedImages);

            if (imgSrcConverter != null)
            {
                imageReplacer.ImageSourceConverter = imgSrcConverter;
            }

            // replace images
            text = imageReplacer.Replace(text);

            // clear BOLD tags
            text = (new SimpleTagReplacer('*', String.Empty, String.Empty)).MatchAndReplaceAll(text);

            // clear UNDERLINED
            text = (new SimpleTagReplacer('_', String.Empty, String.Empty)).MatchAndReplaceAll(text);

            // escape http:// and https:// ftp and file
            string protocolPlaceHolder = Guid.NewGuid().ToString("N");
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(@"(https?|ftps?|file)\:\/\/", options);
            text = regex.Replace(text, (Match match) => { return match.Value.Substring(0, match.Length - 2) + protocolPlaceHolder; });

            // clear ITALIC tags
            text = (new SimpleTagReplacer('/', String.Empty, String.Empty)).MatchAndReplaceAll(text);

            // replace the // which were after the protocol strings like http, https, ftp, ftps, file
            text = text.Replace(protocolPlaceHolder, "//");

            // clear UPPER CASE references
            text = (new SimpleTagReplacer('|', String.Empty, String.Empty)).MatchAndReplaceAll(text);

            // apply link placeholders
            // text = linkReplacer.ApplyPlaceHolders(text);

            // trim
            text = text.Trim();

            while (text.IndexOf("\n\n") >= 0)
            {
                text = text.Replace("\n\n", "\n");
            }

            return text;
        }

    }
}
