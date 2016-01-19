using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class Reaction : NameIdPair
    {
        protected static List<Reaction> ReadAllFromDBase(SqlConnection connection, SqlTransaction transaction)
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

            return reactions;
        }
    }
}
