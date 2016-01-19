using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data.SqlClient;

namespace Crosspl.ObjectModel
{
    public class Reaction : NameIdPair
    {
        protected Reaction() : this(0, null) { }

        protected Reaction(long id, string name) : base(id, name)
        {
        }

        public NameIdPair GetAsNameIdPair()
        {
            return new NameIdPair(this.Id, this.Name);
        }

        public static readonly Reaction YAY = new Reaction(1 << 0, "YAY");
        public static readonly Reaction LOL = new Reaction(1 << 1, "LOL");
        public static readonly Reaction WIN = new Reaction(1 << 2, "WIN");
        public static readonly Reaction OMFG = new Reaction(1 << 3, "OMFG");
        public static readonly Reaction GR8 = new Reaction(1 << 4, "GR8");
        public static readonly Reaction WTF = new Reaction(1 << 5, "WTF");
        public static readonly Reaction EPIC = new Reaction(1 << 6, "EPIC");
        public static readonly Reaction FAIL = new Reaction(1 << 7, "FAIL");
        public static readonly Reaction NERDS = new Reaction(1 << 8, "NERDS");
        public static readonly Reaction UGH = new Reaction(1 << 9, "UGH");
        public static readonly Reaction MEH = new Reaction(1 << 10, "MEH");
        public static readonly Reaction YUCK = new Reaction(1 << 11, "YUCK");
        public static readonly Reaction BOOO = new Reaction(1 << 12, "BOOO");

        private static readonly Reaction[] hardcodedList = new Reaction[] 
        { 
            Reaction.YAY,
            Reaction.LOL,
            Reaction.WIN,
            Reaction.OMFG,
            Reaction.GR8,
            Reaction.WTF,
            Reaction.EPIC,
            Reaction.FAIL,
            Reaction.NERDS,
            Reaction.UGH,
            Reaction.MEH,
            Reaction.YUCK,
            Reaction.BOOO,
        };

        public static Reaction[] All
        {
            get
            {
                //if (all == null)
                //{
                    return hardcodedList;
                //}
                //else
                //{
                //    return all;
                //}
            }
        }

        public static Reaction GetById(long id)
        {
            foreach (Reaction r in All)
            {
                if (r.Id == id)
                {
                    return r;
                }
            }

            return null;
        }

        private static Reaction[] all = null;
        private static object reactionLock = new object();

        public static Reaction[] ReadAllFromDBase(SqlConnection connection, SqlTransaction transaction)
        {
            if (all == null)
            {
                lock (reactionLock)
                {
                    if (all == null)
                    {
                        List<Reaction> reactions = new List<Reaction>();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT [Id], [Text] FROM [dbo].[ReactionType] WHERE [IsEnabled] = 1 ORDER BY [Order] ASC";
                            command.Transaction = transaction;
                            command.CommandTimeout = Database.TimeoutSecs;

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader != null)
                                {
                                    while (reader.Read())
                                    {
                                        Reaction item = new Reaction();
                                        item.Id = reader.GetInt64(0);
                                        item.Name = reader.GetString(1);
                                        reactions.Add(item);
                                    }
                                }
                            }
                        }

                        all = reactions.ToArray();
                    }
                }
            }

            return all;
        }
    }
}
