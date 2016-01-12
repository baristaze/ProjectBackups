using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    /// <summary>
    /// Summary description for State
    /// </summary>
    [Serializable]
    public class USAState
    {
        public USAState() : this("") { }
        public USAState(string abbrv) : this("", abbrv) { }
        public USAState(string name, string abbrv)
        {
            this.name = name;
            this.abbrv = abbrv;
        }

        public object Clone()
        {
            return new USAState(name, abbrv);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string name = "";

        public string Abbrv
        {
            get { return abbrv; }
            set { abbrv = value; }
        }
        private string abbrv = "";

        public override string ToString()
        {
            return abbrv;
        }

        public static int USStateIndexByAbbrv(string abbrv)
        {
            for (int x = 0; x < USStates.Length; x++)
            {
                if (0 == String.Compare(abbrv, USStates[x].abbrv, StringComparison.InvariantCultureIgnoreCase))
                {
                    return x;
                }
            }

            return -1;
        }
        
        public static readonly USAState[] USStates = new USAState[]{
            new USAState("Alabama", "AL"), 
            new USAState("Alaska", "AK"), 
            new USAState("American Samoa", "AS"), 
            new USAState("Arizona", "AZ"), 
            new USAState("Arkansas", "AR"), 
            new USAState("California", "CA"), 
            new USAState("Colorado", "CO"), 
            new USAState("Connecticut", "CT"), 
            new USAState("Delaware", "DE"), 
            new USAState("District of Columbia", "DC"), 
            new USAState("F. S. of Micronesia", "FM"), 
            new USAState("Florida", "FL"), 
            new USAState("Georgia", "GA"), 
            new USAState("Guam", "GU"), 
            new USAState("Hawaii", "HI"), 
            new USAState("Idaho", "ID"), 
            new USAState("Illinois", "IL"), 
            new USAState("Indiana", "IN"), 
            new USAState("Iowa", "IA"), 
            new USAState("Kansas", "KS"), 
            new USAState("Kentucky", "KY"), 
            new USAState("Louisiana", "LA"), 
            new USAState("Maine", "ME"), 
            new USAState("Marshall Islands", "MH"), 
            new USAState("Maryland", "MD"), 
            new USAState("Massachusetts", "MA"), 
            new USAState("Michigan", "MI"), 
            new USAState("Minnesota", "MN"), 
            new USAState("Mississippi", "MS"), 
            new USAState("Missouri", "MO"), 
            new USAState("Montana", "MT"), 
            new USAState("Nebraska", "NE"), 
            new USAState("Nevada", "NV"), 
            new USAState("New Hampshire", "NH"), 
            new USAState("New Jersey", "NJ"), 
            new USAState("New Mexico", "NM"), 
            new USAState("New York", "NY"), 
            new USAState("North Carolina", "NC"), 
            new USAState("North Dakota", "ND"), 
            new USAState("N. Mariana Islands", "MP"), 
            new USAState("Ohio", "OH"), 
            new USAState("Oklahoma", "OK"), 
            new USAState("Oregon", "OR"), 
            new USAState("Palau", "PW"), 
            new USAState("Pennsylvania", "PA"), 
            new USAState("Puerto Rico", "PR"), 
            new USAState("Rhode Island", "RI"), 
            new USAState("South Carolina", "SC"), 
            new USAState("South Dakota", "SD"), 
            new USAState("Tennessee", "TN"), 
            new USAState("Texas", "TX"), 
            new USAState("Utah", "UT"), 
            new USAState("Vermont", "VT"), 
            new USAState("Virgin Islands", "VI"), 
            new USAState("Virginia", "VA"), 
            new USAState("Washington", "WA"), 
            new USAState("West Virginia", "WV"), 
            new USAState("Wisconsin", "WI"), 
            new USAState("Wyoming", "WY")
        };
    }
}