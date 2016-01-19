using System;
using System.Text.RegularExpressions;

namespace Pic4Pic.ObjectModel
{
    public class RegexUtil
    {
        public static bool IsValidEmail(string email)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public static bool IsValidEmailList(string emails, bool isEmptyOK)
        {
            string[] tokens = emails.Split(';');
            if (tokens.Length == 0)
            {
                return isEmptyOK;
            }

            foreach (string token in tokens)
            {
                if (!IsValidEmail(token))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
