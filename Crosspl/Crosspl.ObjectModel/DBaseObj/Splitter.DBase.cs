using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace Crosspl.ObjectModel
{
    public partial class Splitter // : IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader, ref int index)
        {
            this.Id = Database.GetInt32OrDefault(reader, ref index);
            this.IsEnabled = Database.GetBoolOrDefault(reader, ref index);
            this.SectionId = Database.GetInt32OrDefault(reader, ref index);
            this.VariationId = Database.GetInt32OrDefault(reader, ref index);
            this.FriendlyName = Database.GetStringOrDefault(reader, ref index);
            this.Value = Database.GetStringOrDefault(reader, ref index);
            this.CreateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            this.LastUpdateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }
        
        protected static int GetDefaultSplitIdFromDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecScalar<int>(
                conn,
                trans,
                "[dbo].[GetDefaultSplitId]");
        }
    }
}
