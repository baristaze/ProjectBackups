using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public class NameIdPair : Identifiable
    {
        [DataMember()]
        public long Id { get; set; }

        [DataMember()]
        public string Name { get; set; }

        public NameIdPair() : this(0, null) { }

        public NameIdPair(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NameIdPair)
            {
                return this.Equals(obj as NameIdPair);
            }

            return base.Equals(obj);
        }

        public virtual bool Equals(NameIdPair pair)
        {
            if (null == pair)
            {
                return false;
            }

            return this.Id == pair.Id;
        }

        public override string  ToString()
        {
 	         return this.Name;
        }

        public virtual string ToPrintString(int flag)
        {
            if (flag == 0)
            {
                return this.Name;
            }
            else if (flag == 1)
            {
                return this.Id.ToString();
            }

            return String.Empty;
        }

        public static int CompareIds(NameIdPair first, NameIdPair second)
        {
            return first.Id.CompareTo(second.Id);
        }
    }

    public class NameIdList<T> : List<T> where T : NameIdPair
    {
        public string JoinNames(string delim)
        {
            return Join(0, delim);
        }

        public string JoinIds(string delim)
        {
            return Join(1, delim);
        }

        protected string Join(int flag, string delim)
        {
            StringBuilder result = new StringBuilder();
            for (int x = 0; x < this.Count; x++)
            {
                result.Append(this[x].ToPrintString(flag));

                if (x != (this.Count - 1))
                {
                    result.Append(delim);
                }
            }

            return result.ToString();
        }
    }

    public class NameIdPairList : NameIdList<NameIdPair> 
    {
    }
}
