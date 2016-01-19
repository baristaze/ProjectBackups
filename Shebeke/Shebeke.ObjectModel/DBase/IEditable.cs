using System;
using System.Data.SqlClient;

namespace Shebeke.ObjectModel
{
    public enum DBaseOperation
    {
        InsertOrUpdate,
        Delete,
    }

    public interface IDBEntity
    {
        void InitFromSqlReader(SqlDataReader reader);
    }

    public interface IEditable : IDBEntity
    {
        string SelectQuery();
        string SelectAllQuery(object filter);

        string DeleteQuery();
        string InsertOrUpdateQuery();
        void AddSqlParameters(SqlCommand command, DBaseOperation operation);
    }

    public interface IOrderable
    {
        string ReorderQuery();
        string ReorderForAllQuery();
    }
}
