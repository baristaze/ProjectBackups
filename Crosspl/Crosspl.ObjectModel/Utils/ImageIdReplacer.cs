using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public class ImageIdReplacer : ImageIdDetector
    {
        public const string DefaultReplacerTemplate = @" <img class='entry-img-preview' alt='...' src='{0}'/>";

        // an html which contains {0} to be replaced
        public string ReplacerTemplate { get; private set; }

        // caller will set this after Regex.Replace
        public string ReplaceResult { get; private set; }

        public List<ImageFile> AllocatedImages { get { return this.allocatedImages; } }
        private List<ImageFile> allocatedImages = new List<ImageFile>();

        public delegate string ImageSourceGetter(ImageFile imageFile);

        private ImageSourceGetter imageSourceConverter = (ImageFile imageFile) => { return imageFile.CloudUrl; };
        public ImageSourceGetter ImageSourceConverter
        {
            get { return this.imageSourceConverter; }
            set { this.imageSourceConverter = value; }
        }

        protected bool replaceMode;

        // constructor
        public ImageIdReplacer(
            string regexPattern,
            string idWrapperLeft,
            string idWrapperRight,
            string templateForMatched, 
            List<ImageFile> existingImages)
            : base(regexPattern, idWrapperLeft, idWrapperRight)
        {
            this.ReplacerTemplate = templateForMatched;
            if (existingImages != null && existingImages.Count > 0)
            {
                this.AllocatedImages.AddRange(existingImages);
            }
        }
        
        // core method
        public string Replace(string text)
        {
            this.ReplaceResult = null;
            this.replaceMode = true;
            this.DetectedIDs.Clear();

            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(this.RegexPattern, options);
            this.ReplaceResult = regex.Replace(text, this.MatchEvaluator);

            // reset this to make sure that Detect function is not affected
            this.replaceMode = false;
            return this.ReplaceResult;
        }

        protected override string MatchEvaluator(Match match)
        {
            string baseResult = base.MatchEvaluator(match);
            if (!this.replaceMode)
            {
                // ImageIdReplacer.Detect has been called instead of ImageIdReplacer.Replace
                // base method has done the job already. return the result
                return baseResult;
            }

            // else = replace mode
            if (String.IsNullOrEmpty(baseResult) && this.DetectedIDs.Count > 0)
            {
                // we could parse the id
                // check to see if the Id is an existing image
                long lastDetectedId = this.DetectedIDs[this.DetectedIDs.Count-1];
                ImageFile image = null;
                foreach (ImageFile i in this.allocatedImages)
                {
                    if (i.Id == lastDetectedId)
                    {
                        image = i;
                        break;
                    }
                }

                // image found?
                if (image != null)
                {
                    string replaceValue = String.Format(
                        CultureInfo.InvariantCulture,
                        this.ReplacerTemplate,
                        this.ImageSourceConverter(image));

                    return replaceValue;
                }
                else
                {
                    // id is there but there is not such an image. do not make any change
                    return match.Value;
                }
            }
            else
            {
                // we couldn't parse id after the match. do not make any change
                return match.Value;
            }
        }
    }
}
