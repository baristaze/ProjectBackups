using System;
using System.Data.SqlClient;
using System.Globalization;

namespace YummyZone.ObjectModel
{
    public partial class AssetImage : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            if (this.AssetId == Guid.Empty)
            {
                throw new ArgumentException("Unknown AssetId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"
                    DELETE FROM {0} WHERE {1} = @AssetId AND [GroupId] = @GroupId;

                    INSERT INTO {0}
                               ([GroupId]
                               ,{1}
                               ,[ContentType]
                               ,[ContentLength]
                               ,[InitialFileNameOrUrl]                               
                               ,[CreateTimeUTC]
                               ,[Data])
                         VALUES
                               (
                               @GroupId,
                               @AssetId,
                               @ContentType,
                               @ContentLength,
                               @InitialFileNameOrUrl,                               
                               @CreateTimeUTC,
                               @Data);";

            query = String.Format(CultureInfo.InvariantCulture, query, this.imageTableName, this.assetIdColumnName);

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            throw new NotSupportedException();
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.AssetId == Guid.Empty)
            {
                throw new ArgumentException("Unknown AssetId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            command.Parameters.AddWithValue("@GroupId", this.GroupId);
            command.Parameters.AddWithValue("@AssetId", this.AssetId);
            
            if (operation == DBaseOperation.InsertOrUpdate)
            {   
                command.Parameters.AddWithValue("@ContentType", this.ContentType);
                command.Parameters.AddWithValue("@ContentLength", this.ContentLength);
                command.Parameters.AddWithValue("@InitialFileNameOrUrl", this.InitialFileNameOrUrl);
                command.Parameters.AddWithValue("@CreateTimeUTC", this.CreateTimeUTC);
                command.Parameters.AddWithValue("@Data", this.Data);
            }
        }

        public string SelectQuery()
        {
            if (this.AssetId == Guid.Empty)
            {
                throw new ArgumentException("Unknown AssetId");
            }

            if (this.GroupId == Guid.Empty)
            {
                throw new ArgumentException("Unknown GroupId");
            }

            string query = @"SELECT [GroupId],
                                    {0},
                                    [ContentType],
                                    [ContentLength],
                                    [InitialFileNameOrUrl],
                                    [CreateTimeUTC],
                                    [Data]
                               FROM {1}
                               WHERE {0} = '{2}' AND [GroupId] = '{3}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.assetIdColumnName, this.imageTableName, this.AssetId, this.GroupId);
        }

        public string SelectQuery(Guid assetId)
        {
            if (assetId == Guid.Empty)
            {
                throw new ArgumentException("Unknown File Id");
            }

            string query = @"SELECT [GroupId],
                                    {0},
                                    [ContentType],
                                    [ContentLength],
                                    [InitialFileNameOrUrl],
                                    [CreateTimeUTC],
                                    [Data]
                               FROM {1}
                               WHERE {0} = '{2}'";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.assetIdColumnName, this.imageTableName, assetId);
        }

        public string ExistQuery(Guid assetId)
        {
            if (assetId == Guid.Empty)
            {
                throw new ArgumentException("Unknown File Id");
            }

            string query = @"SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'";
            return String.Format(CultureInfo.InvariantCulture, query, this.imageTableName, this.assetIdColumnName, assetId);
        }

        public string SelectAllQuery(Guid groupId)
        {
            throw new NotSupportedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.GroupId = reader.GetGuid(colIndex++);
            this.AssetId = reader.GetGuid(colIndex++);            
            this.ContentType = reader.GetString(colIndex++);
            this.ContentLength = reader.GetInt32(colIndex++);
            if (!reader.IsDBNull(colIndex))
            {
                this.InitialFileNameOrUrl = reader.GetString(colIndex);
            }
            colIndex++;
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);

            int readBytesSoFar = 0;
            this.Data = new byte[this.ContentLength];
            while (readBytesSoFar < this.Data.Length)            
            {
                int remaining = this.Data.Length - readBytesSoFar;
                readBytesSoFar += (int)reader.GetBytes(colIndex, 0, this.Data, readBytesSoFar, remaining);
            }
        }
    }
}
