using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class WebParser
    {
        public class LinkChain : NameValue
        {
            public String ParentName { get; set; }
        }

        public List<LinkChain> ExtractLinksAndNames(string parentName, String content, String pre, String post, params char[] pairSeparators)
        {
            int endIndex = 1;
            List<LinkChain> pairs = new List<LinkChain>();
            while (endIndex > 0)
            {
                String line = this.GetBetween(content, pre, post, ref endIndex);
                if (endIndex > 0)
                {
                    content = content.Substring(endIndex);
                }

                if (!String.IsNullOrWhiteSpace(line))
                {
                    String[] tokens = line.Split(pairSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 2)
                    {
                        String link = tokens[0].Trim(pairSeparators);
                        String name = tokens[1].Trim(pairSeparators);
                        if (!String.IsNullOrWhiteSpace(link) && !String.IsNullOrWhiteSpace(name))
                        {
                            LinkChain pair = new LinkChain();
                            pair.ParentName = parentName;
                            pair.Name = HttpUtility.HtmlDecode(name);
                            pair.Value = HttpUtility.HtmlDecode(link);
                            pairs.Add(pair);
                        }
                    }
                }
            }

            return pairs;
        }

        public string GetBetween(String source, String pre, String post, ref int endIndex)
        {
            endIndex = -1;
            int index = source.IndexOf(pre);
            if (index < 0)
            {
                return null;
            }

            endIndex = index + pre.Length;
            source = source.Substring(index + pre.Length).Trim();
            index = source.IndexOf(post);
            if (index < 0)
            {
                endIndex = -1;
                return null;
            }

            endIndex += index + post.Length;
            source = source.Substring(0, index).Trim();
            return source;
        }

        public string DoHttpGet(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            using (Stream stream = webRequest.GetResponse().GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
