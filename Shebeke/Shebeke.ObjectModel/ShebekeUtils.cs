using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class ShebekeUtils
    {
        public delegate long IdRetriever<T>(T input);

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

        public static T Search<T>(IEnumerable<T> items, IdRetriever<T> idRetriever, long id)
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

        public static T Search<T>(IEnumerable<T> items, long id) where T : Identifiable
        {
            return Search(items, (i) => { return i.Id; }, id);
        }

        public static List<long> TokenizeIDs(string concatenated) 
        {
            List<long> ids = new List<long>();
            string[] tokens = concatenated.Split(new char[] { ',', ' ', ';', '+', '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string t in tokens)
            {
                long l = -1;
                if (long.TryParse(t, out l))
                {
                    ids.Add(l);
                }
            }

            return ids;
        }
    }
}
