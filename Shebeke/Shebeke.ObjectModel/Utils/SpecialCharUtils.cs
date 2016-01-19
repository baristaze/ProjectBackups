using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class SpecialCharUtils
    {
        private const string SeoLinkInputxAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ÇçĞğŞşİıÖöÜüÂâÊêÎîÔôÛûÉé";
        private const string SeoLinkOutputAlphabet = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789ccggssiioouuaaeeiioouuee";
        private const string SpecialChars = "ÇçĞğŞşİıÖöÜüÂâÊêÎîÔôÛûÉé";

        public static string GetSeoLink(string text)
        {
            StringBuilder url = new StringBuilder();
            foreach (char c in text)
            {
                int index = SeoLinkInputxAlphabet.IndexOf(c);
                if (index >= 0)
                {
                    url.Append(SeoLinkOutputAlphabet[index]);
                }
                else
                {
                    url.Append('-');
                }
            }

            return url.ToString();
        }

        public static string ReplaceSpecials(string text, char replacer)
        {
            {
                StringBuilder query = new StringBuilder();
                foreach (char c in text)
                {
                    int index = SpecialChars.IndexOf(c);
                    if (index >= 0)
                    {
                        query.Append(replacer);
                    }
                    else
                    {
                        query.Append(c);
                    }
                }

                return query.ToString();
            }
        }
    }
}
