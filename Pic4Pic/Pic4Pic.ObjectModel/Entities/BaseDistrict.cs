using System;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public abstract class BaseDistrict : NameIntegerIdPair
    {
        protected List<String> alternateNames = new List<string>();

        public List<String> AlternateNames { get { return this.alternateNames; } }

        public override string ToString()
        {
            return this.Name;
        }

        public void AddToAlternateNames(string commaSeparatedNames)
        {
            if (String.IsNullOrWhiteSpace(commaSeparatedNames))
            {
                return;
            }

            string[] tokens = commaSeparatedNames.Split(',');
            foreach (String token in tokens)
            {
                if (!String.IsNullOrWhiteSpace(token.Trim()))
                {
                    this.alternateNames.Add(token.Trim());
                }
            }
        }

        public static Dictionary<int, T> MapItemsById<T>(List<T> items, bool ignoreWildCard) where T : BaseDistrict
        {
            Dictionary<int, T> map = new Dictionary<int, T>();
            foreach (T item in items)
            {
                if (ignoreWildCard && item.Name.Equals("*"))
                {
                    ; // ignore
                }
                else 
                {
                    if (!map.ContainsKey(item.Id))
                    {
                        map.Add(item.Id, item);
                    }                    
                }                
            }

            return map;
        }

        public static Dictionary<string, T> MapItemsByNameAndCode<T>(List<T> items, bool keyToLowerCase, bool ignoreWildCard, bool includeAlternateNames) where T : BaseDistrict
        {
            Dictionary<string, T> map = new Dictionary<string, T>();
            foreach (T item in items)
            {
                if (ignoreWildCard && item.Name.Equals("*"))
                {
                    ; // ignore
                }
                else
                {
                    if (!map.ContainsKey(item.Name))
                    {
                        map.Add((keyToLowerCase ? item.Name.ToLower() : item.Name), item);
                    }
                }

                if (item is Country)
                {
                    string code = (item as Country).Code;
                    code = (keyToLowerCase ? code.ToLower() : code);
                    if (!map.ContainsKey(code))
                    {
                        map.Add(code, item);
                    }
                }
                else if (item is Region)
                {
                    string code = (item as Region).Code;
                    code = (keyToLowerCase ? code.ToLower() : code);
                    if (!map.ContainsKey(code))
                    {
                        map.Add(code, item);
                    }                    
                }
            }

            // this needs to be outside of the first for loop
            if (includeAlternateNames)
            {
                foreach (T item in items)
                {
                    if (ignoreWildCard && item.Name.Equals("*"))
                    {
                        ; // ignore
                    }
                    else
                    {
                        foreach (String alternate in item.AlternateNames)
                        {
                            if (!map.ContainsKey(alternate))
                            {
                                map.Add((keyToLowerCase ? alternate.ToLower() : alternate), item);
                            }
                        }
                    }
                }
            }            

            return map;
        }
    }
}
