using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public abstract partial class RelationMap : IEditable, IOrderable
    {
        public string ReorderQuery()
        {
            if (this.firstEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ParentEntityId");
            }

            if (this.secondEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChildEntityId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "UPDATE {0} SET [OrderIndex] = {1} WHERE {2} = '{3}' AND {4} = '{5}' AND [GroupId] = '{6}';";

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.table,
                this.OrderIndex,
                this.firstEntityColName,
                this.firstEntityId,
                this.secondEntityColName,
                this.secondEntityId,
                this.GroupId);
        }

        public string ReorderForAllQuery()
        {   
            if (this.secondEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChildEntityId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "UPDATE {0} SET [OrderIndex] = {1} WHERE {2} = '{3}' AND [GroupId] = '{4}';";

            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.table,
                this.OrderIndex,
                this.secondEntityColName,
                this.secondEntityId,
                this.GroupId);
        }

        public string InsertQuery()
        {
            if (this.firstEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ParentEntityId");
            }

            if (this.secondEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChildEntityId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "INSERT INTO {0} ([GroupId], {1}, {2}, [OrderIndex], [Status]) VALUES ('{3}', '{4}', '{5}', {6}, {7});";
            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.table,
                this.firstEntityColName,                
                this.secondEntityColName,
                this.GroupId,
                this.firstEntityId,
                this.secondEntityId,
                this.OrderIndex,
                (int)this.Status);
        }

        public string SelectAllQuery(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "SELECT [GroupId], {0}, {1}, [OrderIndex], [Status] FROM {2} WHERE [GroupId] = '{3}' ORDER BY [Status] ASC, [OrderIndex] ASC;";
            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.firstEntityColName,
                this.secondEntityColName,
                this.table,
                groupId);
        }

        public string SelectAllOfFirst(Guid groupId, Guid firstEntityId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "SELECT [GroupId], {0}, {1}, [OrderIndex], [Status] FROM {2} WHERE [GroupId] = '{3}' AND {0} = '{4}' ORDER BY [Status] ASC, [OrderIndex] ASC;";
            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.firstEntityColName,
                this.secondEntityColName,
                this.table,
                groupId,
                firstEntityId);
        }

        public string SelectAllOfSecond(Guid groupId, Guid seciondEntityId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "SELECT [GroupId], {0}, {1}, [OrderIndex], [Status] FROM {2} WHERE [GroupId] = '{3}' AND {1} = '{4}' ORDER BY [Status] ASC, [OrderIndex] ASC;";
            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.firstEntityColName,
                this.secondEntityColName,
                this.table,
                groupId,
                seciondEntityId);
        }

        public string SelectQuery()
        {
            if (this.firstEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ParentEntityId");
            }

            if (this.secondEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChildEntityId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = "SELECT [GroupId], {0}, {1}, [OrderIndex], [Status] FROM {2} WHERE {0} = '{3}' AND {1} = '{4}' AND [GroupId] = '{5}';";
            return String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.firstEntityColName,
                this.secondEntityColName,
                this.table,
                this.firstEntityId,
                this.secondEntityId,
                this.GroupId);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.GroupId = reader.GetGuid(colIndex++);
            this.firstEntityId = reader.GetGuid(colIndex++);
            this.secondEntityId = reader.GetGuid(colIndex++);
            this.OrderIndex = reader.GetByte(colIndex++);
            this.Status = (Status)reader.GetByte(colIndex++);
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();
        }

        public string InsertOrUpdateQuery()
        {
            if (this.firstEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ParentEntityId");
            }

            if (this.secondEntityId == Guid.Empty)
            {
                throw new ArgumentException("Unknown ChildEntityId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"   declare @count int;
                                set @count = (SELECT COUNT(*) FROM {0} WHERE {1} = '{3}' AND {2} = '{4}' AND [GroupId] = '{5}');
                                IF(@count > 0)
                                BEGIN
	                                UPDATE {0} SET [OrderIndex] = {6}, [Status] = {7}
		                                WHERE {1} = '{3}' AND {2} = '{4}' AND [GroupId] = '{5}';
                                END
                                ELSE
                                BEGIN
	                                INSERT INTO {0} ([GroupId], {1}, {2}, [OrderIndex], [Status]) 
                                        VALUES ('{5}', '{3}', '{4}', {6}, {7});
                                END;";

            query = Database.ShortenQuery(query);

            query = String.Format(
                CultureInfo.InvariantCulture,
                query,
                this.table,
                this.firstEntityColName,
                this.secondEntityColName,                
                this.firstEntityId,
                this.secondEntityId,
                this.GroupId,
                this.OrderIndex,
                (int)this.Status);

            return query;
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            // do nothing
        }
    }
}
