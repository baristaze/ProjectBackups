using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public class NameIdPair<T> where T:struct
    {
        [DataMember()]
        public T Id { get; set; }

        [DataMember()]
        public string Name { get; set; }

        public NameIdPair() : this(default(T), null) { }

        public NameIdPair(T id, string name)
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
            if (obj is NameIdPair<T>)
            {
                return this.Equals(obj as NameIdPair<T>);
            }

            return base.Equals(obj);
        }

        public virtual bool Equals(NameIdPair<T> pair)
        {
            if (null == pair)
            {
                return false;
            }

            return this.Id.Equals(pair.Id);
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
    }

    [DataContract]
    public class NameIntegerIdPair : NameIdPair<int>
    {
        public NameIntegerIdPair() : base() { }
        public NameIntegerIdPair(int id, string name) : base(id, name) { }
    }

    [DataContract]
    public class NameLongIdPair : NameIdPair<long> 
    {
        public NameLongIdPair() : base() { }
        public NameLongIdPair(long id, string name) : base(id, name) { }
    }

    [DataContract]
    public class NameGuidIdPair : NameIdPair<Guid>, Identifiable 
    {
        public NameGuidIdPair() : base() { }
        public NameGuidIdPair(Guid id, string name) : base(id, name) { }
    }

    public class NameIdList<T1, T2> : List<T1> where T1 : NameIdPair<T2> where T2 : struct
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

    public class NameIntegerIdPairList : NameIdList<NameIntegerIdPair, int> { }

    public class NameLongIdPairList : NameIdList<NameLongIdPair, long> { }
    public class NameGuidIdPairList : NameIdList<NameGuidIdPair, Guid> { }
}
