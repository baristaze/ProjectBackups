using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace YummyZone.VenueAdmin.Web
{
    public class Helpers
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            }
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

        public string FullyQualifiedApplicationPath(HttpContext context)
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

        public static bool EMail_UseSSL
        {
            get
            {
                string valAsText = ConfigurationManager.AppSettings["EMail_UseSSL"];
                return ConvertToBoolTrue(valAsText);
            }
        }

        public static int EMail_SmtpPort
        {
            get
            {
                int val = 0;
                string valAsText = ConfigurationManager.AppSettings["EMail_SmtpPort"];
                Int32.TryParse(valAsText, out val);
                return val;
            }
        }

        public static string EMail_SmtpHost
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_SmtpHost"];
            }
        }

        public static string EMail_SenderName
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_SenderName"];
            }
        }

        public static string EMail_SenderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_SenderEmail"];
            }
        }

        public static string EMail_SenderPswd
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_SenderPswd"];
            }
        }

        public static string EMail_ToList
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_ToList"];
            }
        }

        public static string EMail_CCList
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_CCList"];
            }
        }

        public static string EMail_BCCList
        {
            get
            {
                return ConfigurationManager.AppSettings["EMail_BCCList"];
            }
        }
    }
}