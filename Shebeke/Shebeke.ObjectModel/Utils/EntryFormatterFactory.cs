using System;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public interface IEntryFormatter
    {
        List<long> DetectImageIds(string text);
        string GetEncodedHtml(string text, List<ImageFile> allocatedImages, params object[] otherParams);
    }

    public class DummyEntryFormatter : IEntryFormatter
    {
        public List<long> DetectImageIds(string text)
        {
            return new List<long>();
        }

        public string GetEncodedHtml(string text, List<ImageFile> allocatedImages, params object[] otherParams)
        {
            return text;
        }
    }

    public class EntryFormatterAbstractFactory
    {
        public const int CurrentFormatVersion = 1;

        public static IEntryFormatter CreateForCurrent(string applicationRoot) 
        {
            return Create(CurrentFormatVersion, applicationRoot);
        }

        public static IEntryFormatter Create(int version, string applicationRoot) 
        {
            if (version == 1)
            {
                return new EntryFormatter(applicationRoot);
            }

            return new DummyEntryFormatter();
        }
    }
}
