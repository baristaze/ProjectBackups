using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Web;

namespace Shebeke.ObjectModel
{
    public static class StringHelpers
    {
        private const string emailPattern = "^((\"[\\w-+\\s]+\")|([\\w-+]+(?:\\.[\\w-+]+)*)|(\"[\\w-+\\s]+\")([\\w-+]+(?:\\.[\\w-+]+)*))" +
                                             "(@((?:[\\w-+]+\\.)*\\w[\\w-+]{0,66})\\.([a-z]{2,6}(?:\\.[a-z]{2})?)$)|(@\\[?((25[0-5]\\." +
                                             "|2[0-4][\\d]\\.|1[\\d]{2}\\.|[\\d]{1,2}\\.))((25[0-5]|2[0-4][\\d]|1[\\d]{2}|[\\d]{1,2})\\.)" +
                                             "{2}(25[0-5]|2[0-4][\\d]|1[\\d]{2}|[\\d]{1,2})\\]?$)";

        private const string phonePattern = "^(?:\\+?1\\s*(?:[.-]\\s*)?)?(?:\\(\\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\\s*\\)" +
                                             "|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\\s*(?:[.-]\\s*)?([2-9]1[02-9]|[2-9][02-9]1|" +
                                             "[2-9][02-9]{2})\\s*(?:[.-]\\s*)?([0-9]{4})(?:\\s*(?:#|x\\.?|ext\\.?|extension)\\s*(\\d+))?$";

        private const string zipCodePattern = "^\\d{5}$|^\\d{5}-\\d{4}$";


        public static bool IsValidEmail(string email) 
        {
            Regex pattern = new Regex(emailPattern);
            return pattern.IsMatch(email.ToLowerInvariant());
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            Regex pattern = new Regex(phonePattern);
            return pattern.IsMatch(phone);
        }

        public static bool IsValidZipCode(string zipCode)
        {
            Regex pattern = new Regex(zipCodePattern);
            return pattern.IsMatch(zipCode);
        }

        public static string ToTitleCase(string text)
        {
            string titleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLower());
            string[] preposition = new string[] { " By ", " And ", " The ", " At ", " In ", " On ", " A ", " An ", " With ", " Of ", " My ", " Our ", " His ", " Her ", " Their ", " Your " };
            foreach(string pre in preposition)
            {
                titleCase = titleCase.Replace(pre, pre.ToLower());
            }

            return titleCase;
        }

        private static bool IsAllUpper(string text)
        {
            foreach (char c in text)
            {
                if (c >= 'a' && c <= 'z')
                {
                    return false;
                }
            }

            return true;
        }

        private static readonly string[] prepositions = new string[] { "by", "and", "the", "at", "in", "on", "a", "an", "with", "of", "my", "our", "his", "her", "is", "are" };

        public static string ToTitleCase2(string text)
        {
            if(String.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            string[] tokens = text.Split(' ');
            for (int x = 0; x < tokens.Length; x++)
            {
                string lower = tokens[x].ToLower();
                /*
                if (prepositions.Contains(lower))
                {
                    tokens[x] = lower;
                }
                */
                if (!IsAllUpper(tokens[x]))
                {
                    tokens[x] = UppercaseFirst(lower);
                }
            }

            string title = String.Join(" ", tokens);
            return UppercaseFirst(title);
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static byte[] StreamToByteArray(Stream stream, int length)
        {
            byte[] all = new byte[length];

            int totalReadSoFar = 0;
            int bufferSize = Math.Min(length, 10 * 1000);
            while (totalReadSoFar < length)
            {
                int remaining = length - totalReadSoFar;
                int nextRead = Math.Min(remaining, bufferSize);
                int actualRead = stream.Read(all, totalReadSoFar, nextRead);
                totalReadSoFar += actualRead;
            }

            return all;
        }

        public static string FullyQualifiedDomainUrl(HttpContext context)
        {
            string appPath = string.Format("{0}://{1}{2}",
                                    context.Request.Url.Scheme,
                                    context.Request.Url.Host,
                                    context.Request.Url.Port == 80 ? string.Empty : ":" + context.Request.Url.Port);

            if (!appPath.EndsWith("/"))
            {
                appPath += "/";
            }

            return appPath;
        }

        public static string FullyQualifiedApplicationPath(HttpContext context)
        {
            string appPath = string.Format("{0}://{1}{2}{3}",
                                    context.Request.Url.Scheme,
                                    context.Request.Url.Host,
                                    context.Request.Url.Port == 80 ? string.Empty : ":" + context.Request.Url.Port,
                                    context.Request.ApplicationPath);

            if (!appPath.EndsWith("/"))
            {
                appPath += "/";
            }

            return appPath;
        }

        public static bool ConvertToBoolTrue(string valAsText)
        {
            bool val = false;
            if (valAsText != null)
            {
                valAsText = valAsText.Trim();
                valAsText.ToLower();
                if (valAsText == "1" || valAsText == "true" || valAsText == "yes")
                {
                    val = true;
                }
            }

            return val;
        }

        public static string PercentageAsString(int observation, int total, string item, bool emptyOnZeroTotal)
        {
            string percentage = String.Empty;
            if (total <= 0)
            {
                if (emptyOnZeroTotal)
                {
                    return String.Empty;
                }
                else
                {
                    percentage = "0%";
                }
            }
            else
            {
                double d = ((double)observation / (double)total) * (double)100.0;
                percentage = ((int)d).ToString() + "%";
            }

            if (String.IsNullOrEmpty(item))
            {
                return percentage;
            }
            else
            {
                return percentage + " " + item;
            }
        }

        public static string GetSeoLink(string text)
        {
            /*
            text =text.ToLowerInvariant();
            StringBuilder url = new StringBuilder();
            foreach (char c in text)
            {
                if ((int)c >= (int)'a' && (int)c <= (int)'z')
                { 
                    url.Append(c);
                }
                else if ((int)c >= (int)'0' && (int)c <= (int)'9')
                {
                    url.Append(c);
                }
                else // if (c == '-')
                {
                    url.Append('-');
                }
            }

            return url.ToString();
            */

            return SpecialCharUtils.GetSeoLink(text);
        }

        public static List<string> SubSentences(string sentence)
        {
            List<string> result = new List<string>();
            if (String.IsNullOrWhiteSpace(sentence))
            {
                return result;
            }

            string specials = " `~!@#$%^&*()-_=+[]{}\\|;:'\",.<>/?*\n\r";
            string[] tokens = sentence.Split(specials.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (String.IsNullOrWhiteSpace(sentence))
            {
                return result;
            }
            else if (tokens.Length == 1)
            {
                result.Add(sentence);
                return result;
            }

            for (int start = 0; start < tokens.Length; start++)
            {
                if (tokens[start].Length == 1)
                {
                    continue;
                }

                for (int end = start; end < tokens.Length; end++)
                {
                    if (tokens[end].Length == 1)
                    {
                        continue;
                    }

                    string sub = String.Join(" ", tokens, start, end - start + 1);
                    result.Add(sub);
                }
            }

            result.Sort((a, b) => { return a.Length.CompareTo(b.Length); });
            result.Reverse();

            return result;
        }

        public static string GetRootUrl(Uri uri, bool ignoreservicePart)
        {
            string url = "http";
            if (uri.Scheme == Uri.UriSchemeHttps)
                url += "s";

            url += "://";
            url += uri.Host;
            if (ignoreservicePart)
            {
                string svcTag = "svc.";
                int index = url.IndexOf(svcTag, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    url = url.Remove(index, svcTag.Length);
                }
            }

            int port = HttpContext.Current.Request.Url.Port;
            if (port != 80 && port != 443)
            {
                url += ":";
                url += port.ToString();
            }

            return url;
        }
    }
}
