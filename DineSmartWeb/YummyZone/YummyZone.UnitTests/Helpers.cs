using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.Net;
using System.IO;
using System.Globalization;
using System.Xml;

namespace YummyZone.UnitTests
{
    public class Helpers
    {
        public static int CompareSqlDateTime(DateTime time1, DateTime time2)
        {
            SqlDateTime t1 = new SqlDateTime(time1);
            SqlDateTime t2 = new SqlDateTime(time2);
            return t1.CompareTo(t2);
        }

        public static Dictionary<string, string> WrapAsDictionary(params string[] pairs)
        {
            Dictionary<string, string> dict = new Dictionary<string,string>();
            if (pairs != null)
            {
                for (int x = 0; x < pairs.Length-1; x = x + 2)
                {
                    dict.Add(pairs[x], pairs[x + 1]);
                }
            }

            return dict;
        }

        public static void VerifyOperationResult(string xmlContent)
        {
            VerifyPListIntKeyVal(xmlContent, "ErrorCode", 0);
        }

        public static void VerifyPListIntKeyVal(string xmlContent, string key, int val)
        {
            int intVal = GetPListIntKeyVal(xmlContent, key);

            if (intVal != val)
            {
                throw new YummyTestException("Unexpected result for " + key + ": " + intVal.ToString());
            }
        }

        public static void VerifyPListStringKeyVal(string xmlContent, string key, string val)
        {
            string readVal = GetPListStringKeyVal(xmlContent, key);

            if (string.Compare(readVal, val, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new YummyTestException("Unexpected value for " + key + ": " + readVal);
            }
        }

        public static int GetPListIntKeyVal(string xmlContent, string key)
        {
            XmlNode node = SearchXml(xmlContent, "key", key);
            if (node == null)
            {
                throw new YummyTestException(key + " key couldn't be found in the xml");
            }

            if (node.NextSibling == null)
            {
                throw new YummyTestException("No content for " + key + " key in the xml");
            }

            if (string.Compare("integer", node.NextSibling.Name, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new YummyTestException("Unexpected value type for " + key + ": " + node.NextSibling.Name);
            }

            int intVal = Int32.MinValue;
            if (!Int32.TryParse(node.NextSibling.InnerText, out intVal))
            {
                throw new YummyTestException("Unexpected value for " + key + ": " + node.NextSibling.InnerText);
            }

            return intVal;
        }

        public static string GetPListStringKeyVal(string xmlContent, string key)
        {
            XmlNode node = SearchXml(xmlContent, "key", key);
            if (node == null)
            {
                throw new YummyTestException(key + " key couldn't be found in the xml");
            }

            if (node.NextSibling == null)
            {
                throw new YummyTestException("No content for " + key + " key in the xml");
            }

            if (string.Compare("string", node.NextSibling.Name, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new YummyTestException("Unexpected value type for " + key + ": " + node.NextSibling.Name);
            }

            return node.NextSibling.InnerText;
        }

        public static XmlNode SearchXml(string xmlContent, string tag, string value)
        {
            List<string> values = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            XmlElement root = doc.DocumentElement;
            if (root != null)
            {
                XmlNodeList list = root.GetElementsByTagName(tag);
                if (list != null)
                {
                    foreach (XmlNode node in list)
                    {
                        if (String.Compare(node.InnerText, value, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        public static string GetMethodUrl(string root, string method)
        {
            return Constants.WebSvcUrl + "/" + method + "?format=plist";
        }

        public static string PostRequest(string url, string content)
        {
            return PostRequest(url, content, null);
        }

        public static string PostRequest(string url, string content, Dictionary<string, string> headers)
        {
            try
            {
                return _PostRequest(url, content, headers);
            }
            catch (Exception ex)
            {
                string message = "POST request to '{0}' has failed.";
                message = String.Format(CultureInfo.InvariantCulture, message, url);
                throw new YummyTestException(message, ex);
            }
        }

        private static string _PostRequest(string url, string content, Dictionary<string, string> headers)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.AllowWriteStreamBuffering = true;
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";
            webRequest.ContentLength = content.Length;

            if (headers != null && headers.Count > 0)
            { 
                var enumerator = headers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    webRequest.Headers.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }

            using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
            {
                writer.Write(content);
                writer.Flush();
                writer.Close();
            }

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                reader.Close();
                return result;
            }
        }

        public static string GetRequest(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers, string userName, string password)
        {
            try
            {
                return _GetRequest(url, parameters, headers, userName, password);
            }
            catch (Exception ex)
            {
                string message = "GET request to '{0}' has failed.";
                message = String.Format(CultureInfo.InvariantCulture, message, url);
                throw new YummyTestException(message, ex);
            }
        }

        private static string _GetRequest(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers, string userName, string password)
        {
            if (parameters != null && parameters.Count > 0)
            {
                var enumerator = parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    url += "&" + enumerator.Current.Key + "=" + enumerator.Current.Value;
                }
            }

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "GET";

            if (headers != null && headers.Count > 0)
            {
                var enumerator = headers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    webRequest.Headers.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }

            if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(password))
            {
                webRequest.Credentials = new NetworkCredential(userName, password);
            }

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                reader.Close();
                return result;
            }
        }
    }
}