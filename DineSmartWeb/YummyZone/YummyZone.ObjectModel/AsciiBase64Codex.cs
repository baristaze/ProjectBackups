using System;
using System.Text;

namespace YummyZone.ObjectModel
{
    public class AsciiBase64Codex
    {
        public static string Encode(string plainText)
        {
            byte[] encodedBytes = new ASCIIEncoding().GetBytes(plainText);
            return Convert.ToBase64String(encodedBytes);
        }

        public static string Decode(string encodedText)
        {
            byte[] decodedBytes = Convert.FromBase64String(encodedText);
            return new ASCIIEncoding().GetString(decodedBytes);
        }
    }
}
