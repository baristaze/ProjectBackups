using System;
using System.Data.SqlClient;
using System.Globalization;

namespace Crosspl.ObjectModel
{
    public partial class UserToken : IEditable
    {
        public string InsertOrUpdateQuery()
        {
            // insert only
            string query = @"
            IF NOT EXISTS (SELECT * FROM [dbo].[UserToken] WHERE [UserId] = @UserId AND [OAuthProvider] = @OAuthProvider)
            BEGIN
                INSERT INTO [dbo].[UserToken]
                       ([UserId]
                       ,[OAuthProvider]
                       ,[OAuthUserId]
                       ,[OAuthAccessToken]
                       ,[CreateTimeUTC]
                       ,[ExpireTimeUTC])
                 VALUES
                       (@UserId
                       ,@OAuthProvider
                       ,@OAuthUserId
                       ,@OAuthAccessToken
                       ,@CreateTimeUTC
                       ,@ExpireTimeUTC);
            END
            ELSE
            BEGIN
                UPDATE [dbo].[UserToken]
                   SET [OAuthUserId] = @OAuthUserId
                      ,[OAuthAccessToken] = @OAuthAccessToken
                      ,[ExpireTimeUTC] = @ExpireTimeUTC
                 WHERE [UserId] = @UserId AND [OAuthProvider] = @OAuthProvider;
            END";

            return Database.ShortenQuery(query);
        }

        public string DeleteQuery()
        {
            return "DELETE FROM [dbo].[UserToken] WHERE [UserId] = @UserId AND [OAuthProvider] = @OAuthProvider;";
        }

        public void AddSqlParameters(SqlCommand command, DBaseOperation operation)
        {
            if (this.UserId <= 0)
            {
                throw new ArgumentException("Unknown UserId");
            }

            command.Parameters.AddWithValue("@UserId", this.UserId);
            command.Parameters.AddWithValue("@OAuthProvider", (byte)this.OAuthProvider);

            if (operation == DBaseOperation.InsertOrUpdate)
            {
                Database.AddSqlParamOrNull(command, "@OAuthUserId", this.OAuthUserId);
                Database.AddSqlParamOrNull(command, "@OAuthAccessToken", this.OAuthAccessToken);
                Database.AddSqlParamOrNull(command, "@ExpireTimeUTC", this.ExpireTimeUTC);
                Database.AddSqlParamOrNull(command, "@CreateTimeUTC", this.CreateTimeUTC);
            }
        }

        public string SelectQuery()
        {
            if (this.UserId <= 0)
            {
                throw new ArgumentException("Unknown UserId");
            }

            string query = @"    SELECT [UserId]
                                       ,[OAuthProvider]
                                       ,[OAuthUserId]
                                       ,[OAuthAccessToken]
                                       ,[CreateTimeUTC]
                                       ,[ExpireTimeUTC]
                                  FROM [dbo].[UserToken]
                                  WHERE [UserId] = {0} AND [OAuthProvider] = {1};";

            query = Database.ShortenQuery(query);
            return String.Format(CultureInfo.InvariantCulture, query, this.UserId, (int)this.OAuthProvider);
        }

        public string SelectAllQuery(object filter)
        {
            throw new NotSupportedException();
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.UserId = reader.GetInt64(colIndex++);
            this.OAuthProvider = (OAuthProvider)reader.GetByte(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.OAuthUserId = reader.GetString(colIndex);
            }
            colIndex++;

            if (!reader.IsDBNull(colIndex))
            {
                this.OAuthAccessToken = reader.GetString(colIndex);
            }
            colIndex++;

            this.CreateTimeUTC = reader.GetDateTime(colIndex++);

            if (!reader.IsDBNull(colIndex))
            {
                this.ExpireTimeUTC = reader.GetDateTime(colIndex);
            }
            colIndex++;
        }
    }
}
