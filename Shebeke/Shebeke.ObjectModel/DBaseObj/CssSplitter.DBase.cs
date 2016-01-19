using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public partial class CssSplitter : Splitter, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            base.InitFromSqlReader(reader, ref colIndex);

            this.CssClass = Database.GetStringOrDefault(reader, ref colIndex);
            this.CssTemplate = Database.GetStringOrDefault(reader, ref colIndex);
        }

        protected static CssSplitterList ReadFromDBase(SqlConnection conn, SqlTransaction trans, int splitId, string sectionFilters)
        {
            List<CssSplitter> items = Database.ExecSProc<CssSplitter>(
                conn,
                trans,
                "[dbo].[GetSplitInfo]",
                Database.SqlParam("@SplitType", 0), // css
                Database.SqlParam("@SplitId", splitId),
                Database.SqlParam("@SectionFilters", sectionFilters));

            CssSplitterList list = new CssSplitterList();
            list.AddRange(items);
            return list;
        }
    }
}
