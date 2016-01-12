using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public static class StringHelpers
    {
        private static string emailPattern = "^((\"[\\w-+\\s]+\")|([\\w-+]+(?:\\.[\\w-+]+)*)|(\"[\\w-+\\s]+\")([\\w-+]+(?:\\.[\\w-+]+)*))" +
                                             "(@((?:[\\w-+]+\\.)*\\w[\\w-+]{0,66})\\.([a-z]{2,6}(?:\\.[a-z]{2})?)$)|(@\\[?((25[0-5]\\." +
                                             "|2[0-4][\\d]\\.|1[\\d]{2}\\.|[\\d]{1,2}\\.))((25[0-5]|2[0-4][\\d]|1[\\d]{2}|[\\d]{1,2})\\.)" +
                                             "{2}(25[0-5]|2[0-4][\\d]|1[\\d]{2}|[\\d]{1,2})\\]?$)";

        private static string phonePattern = "^(?:\\+?1\\s*(?:[.-]\\s*)?)?(?:\\(\\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\\s*\\)" +
                                             "|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\\s*(?:[.-]\\s*)?([2-9]1[02-9]|[2-9][02-9]1|" +
                                             "[2-9][02-9]{2})\\s*(?:[.-]\\s*)?([0-9]{4})(?:\\s*(?:#|x\\.?|ext\\.?|extension)\\s*(\\d+))?$";

        private static string zipCodePattern = "^\\d{5}$|^\\d{5}-\\d{4}$";


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

        public static bool IsValidState(string stateAbbrv)
        {
            return USAState.USStateIndexByAbbrv(stateAbbrv) >= 0;
        }

        public static string ToTitleCase(string text)
        {
            string titleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLower());
            string[] preposition = new string[] { " By ", " And ", " The ", " At ", " In ", " On ", " A ", " An ", " With ", " Of " };
            foreach(string pre in preposition)
            {
                titleCase = titleCase.Replace(pre, pre.ToLower());
            }

            return titleCase;
        }
    }
}
