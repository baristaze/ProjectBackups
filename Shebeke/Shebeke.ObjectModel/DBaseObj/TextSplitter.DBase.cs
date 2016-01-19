using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class TextSplitter : Splitter, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            base.InitFromSqlReader(reader, ref colIndex);

            this.SideToApply = (Side)Database.GetByteOrDefault(reader, ref colIndex);
            this.JQSelector = Database.GetStringOrDefault(reader, ref colIndex);
        }

        protected static TextSplitterList ReadFromDBase(SqlConnection conn, SqlTransaction trans, int splitId, string sectionFilters)
        {
            List<TextSplitter> items = Database.ExecSProc<TextSplitter>(
                conn,
                trans,
                "[dbo].[GetSplitInfo]",
                Database.SqlParam("@SplitType", 1), // text
                Database.SqlParam("@SplitId", splitId),
                Database.SqlParam("@SectionFilters", sectionFilters));

            TextSplitterList list = new TextSplitterList();
            list.AddRange(items);
            return list;
        }
    }
}
