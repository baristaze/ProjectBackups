using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public class CraigsListParser : WebParser
    {
        public static string RootUrl = "http://seattle.craigslist.org/";

        public List<LinkChain> ExtractAllSubDomains(string url)
        {
            List<LinkChain> states = this.ExtractStatesAndURLs(url, "US");
            List<LinkChain> metros = this.ExtractMetroCitiesAndURLs(url, "US States");
            List<LinkChain> cities = new List<LinkChain>();
            foreach (LinkChain metro in metros)
            {
                List<LinkChain> temp = this.ExtractNearByCitiesAndURLs(metro.Value, metro.Name);
                cities.AddRange(temp);
            }

            List<LinkChain> all = new List<LinkChain>();
            all.AddRange(states);
            all.AddRange(metros);
            all.AddRange(cities);

            return all;
        }

        public List<LinkChain> ExtractStatesAndURLs(string url, string parentName)
        {
            List<LinkChain> pairs = this.ExtractExpandableMenuItems(url, "us states", "canada", parentName);
            if (pairs.Count > 0 && pairs[pairs.Count - 1].Value == "//geo.craigslist.org/iso/us") // "more..."
            {
                pairs.RemoveAt(pairs.Count - 1);
            }

            return pairs;
        }

        public List<LinkChain> ExtractMetroCitiesAndURLs(string url, string parentName)
        {
            List<LinkChain> pairs = this.ExtractExpandableMenuItems(url, "us cities", "us states", parentName);
            if (pairs.Count > 0 && pairs[pairs.Count - 1].Value == "//geo.craigslist.org/iso/us") // "more..."
            {
                pairs.RemoveAt(pairs.Count - 1);
            }

            return pairs;
        }

        public List<LinkChain> ExtractNearByCitiesAndURLs(string url, string parentName)
        {
            return this.ExtractExpandableMenuItems(url, "nearby cl", "us cities", parentName);
        }

        public List<LinkChain> ExtractExpandableMenuItems(string url, String pre, String post, string parentName)
        {
            List<LinkChain> pairs = new List<LinkChain>();
            String pageContent = this.DoHttpGet(url);
            if (String.IsNullOrWhiteSpace(pageContent))
            {
                return pairs;
            }

            int endIndex = -1;
            String block = this.GetBetween(pageContent, pre, post, ref endIndex);
            if (String.IsNullOrWhiteSpace(block))
            {
                return pairs;
            }

            return this.ExtractLinksAndNames(parentName, block, "<a href=\"", "</a>", '>', '\"');
        }
    }
}
