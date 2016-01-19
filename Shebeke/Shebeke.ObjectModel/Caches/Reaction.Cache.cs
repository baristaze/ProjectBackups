using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace Shebeke.ObjectModel
{
    public partial class Reaction : NameIdPair
    {
        public static List<Reaction> ReadAll(string dbConnectionString)
        {
            return _ReadAll(dbConnectionString, null, null);
        }
                
        public static List<Reaction> ReadAll(SqlConnection connection, SqlTransaction transaction)
        {
            return _ReadAll(null, connection, transaction);
        }
        
        private static List<Reaction> _ReadAll(string dbConnectionString, SqlConnection connection, SqlTransaction transaction)
        {
            List<Reaction> reactions = new List<Reaction>();
            List<NameIdPair> pairs = new List<NameIdPair>();
            if (!CacheHelper.GetObj<List<NameIdPair>>(CacheHelper.CacheName_StaticResources, "ReactionList", ref pairs))
            {
                // get from database
                if (connection == null)
                {
                    reactions = Reaction._ReadFromDBase(dbConnectionString);
                }
                else
                {
                    reactions = Reaction.ReadAllFromDBase(connection, transaction);
                }

                if (reactions != null && reactions.Count > 0)
                {
                    // convert it to pairs, which are serializeable
                    foreach (Reaction react in reactions)
                    {
                        pairs.Add(react.GetAsNameIdPair());
                    }

                    // cache it
                    CacheHelper.Put(CacheHelper.CacheName_StaticResources, "ReactionList", pairs);
                }
            }
            else
            {
                // convert pairs to reactions
                foreach (NameIdPair pair in pairs)
                {
                    reactions.Add(new Reaction(pair.Id, pair.Name));
                }
            }

            return reactions;

        }

        private static List<Reaction> _ReadFromDBase(string dbConnectionString)
        {
            List<Reaction> reactions = new List<Reaction>();

            // get from database
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    // open db connection
                    connection.Open();

                    // read all
                    reactions = Reaction.ReadAllFromDBase(connection, null);
                }
            }
            catch (Exception ex)
            {
                string msg = "Reaction types couldn't be read from database. Exception: " + ex.ToString();

                string errorLog = String.Format(
                            CultureInfo.InvariantCulture,
                            "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                            "Reaction.Cache.cs",
                            "ReadAll",
                            "ReadAllFromDBase",
                            0,
                            0,
                            msg);

                Trace.WriteLine(errorLog, LogCategory.Error);
            }

            return reactions;
        }
    }
}
