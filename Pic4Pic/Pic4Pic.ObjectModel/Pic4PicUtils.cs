using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Pic4Pic.ObjectModel
{
    public class Pic4PicUtils
    {
        public delegate Guid IdRetriever<T>(T input);

        public static string ConcatenateIDs<T>(IList<T> items, IdRetriever<T> idRetriever)
        {
            string s = String.Empty;
            for (int x = 0; x < items.Count; x++)
            {
                s += idRetriever(items[x]).ToString();
                if (x != items.Count - 1)
                {
                    s += ","; // don't put extra space
                }
            }

            return s;
        }

        public static string ConcatenateIDs<T>(IList<T> items) where T : Identifiable
        {
            return ConcatenateIDs(items, (i) => { return i.Id; });
        }

        public static T Search<T>(IEnumerable<T> items, IdRetriever<T> idRetriever, Guid id)
        {
            foreach (T item in items)
            {
                if (idRetriever(item) == id)
                {
                    return item;
                }
            }

            return default(T);
        }

        public static T Search<T>(IEnumerable<T> items, Guid id) where T : Identifiable
        {
            return Search(items, (i) => { return i.Id; }, id);
        }

        public static List<Guid> TokenizeIDs(string concatenated) 
        {
            List<Guid> ids = new List<Guid>();
            string[] tokens = concatenated.Split(new char[] { ',', ' ', ';', '+', '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string t in tokens)
            {
                Guid g = Guid.Empty;
                if (Guid.TryParse(t, out g))
                {
                    ids.Add(g);
                }
            }

            return ids;
        }

        public static string SerializeObjectToJSON<T>(T obj) where T : class
        {
            using (MemoryStream writerStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(writerStream, obj);
                writerStream.Position = 0;
                using (StreamReader readerStream = new StreamReader(writerStream))
                {
                    return readerStream.ReadToEnd();
                }
            }
        }

        public static T DeserializeObjectFromJSON<T>(string blob) where T : class
        {
            using (MemoryStream writerStream = new MemoryStream())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(blob);
                writerStream.Write(bytes, 0, bytes.Length);
                writerStream.Position = 0;

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                object o = serializer.ReadObject(writerStream);
                return (o as T);
            }
        }
    }
}
