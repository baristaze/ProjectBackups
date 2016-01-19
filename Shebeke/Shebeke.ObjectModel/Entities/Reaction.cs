using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace Shebeke.ObjectModel
{
    public partial class Reaction : NameIdPair
    {
        protected Reaction() : this(0, null) { }

        protected Reaction(long id, string name) : base(id, name)
        {
        }

        public NameIdPair GetAsNameIdPair()
        {
            return new NameIdPair(this.Id, this.Name);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Reaction react = obj as Reaction;
            if ((object)react == null)
            {
                return false;
            }

            return this.Id == react.Id;
        }

        public bool Equals(Reaction reaction)
        {
            if (reaction == null) 
            {
                return false;
            }

            return this.Id == reaction.Id;
        }

        public static bool operator == (Reaction first, Reaction second)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(first, second))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.Id == second.Id;
        }

        public static bool operator != (Reaction first, Reaction second)
        {
            return !(first == second);
        }

        private static Reaction[] all = null;
        private static object reactionLock = new object();

        public static Reaction[] All { get { return all; } }

        public static Reaction[] InitAll(string dbConnectionString)
        {
            if (all == null)
            {
                lock (reactionLock)
                {
                    if (all == null)
                    {
                        Reaction[] temp = ReadAll(dbConnectionString).ToArray();
                        if (temp != null && temp.Length > 0)
                        {
                            all = temp;
                        }
                    }
                }
            }

            return all;
        }
                
        public static Reaction[] InitAll(SqlConnection connection, SqlTransaction transaction)
        {
            if (all == null)
            {
                lock (reactionLock)
                {
                    if (all == null)
                    {
                        Reaction[] temp = ReadAll(connection, transaction).ToArray();
                        if (temp != null && temp.Length > 0)
                        {
                            all = temp;
                        }
                    }
                }
            }

            return all;
        }

        public static Reaction GetById(long id)
        {
            if (all != null)
            {
                foreach (Reaction r in all)
                {
                    if (r.Id == id)
                    {
                        return r;
                    }
                }
            }

            return null;
        }        
    }
}
