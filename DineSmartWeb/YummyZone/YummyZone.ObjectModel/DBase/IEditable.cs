using System;
using System.Data.SqlClient;

namespace YummyZone.ObjectModel
{
    public enum DBaseOperation
    {
        InsertOrUpdate,
        Delete,
    }

    public interface IEditable
    {
        string SelectQuery();
        string SelectAllQuery(Guid groupId);
        void InitFromSqlReader(SqlDataReader reader);

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
