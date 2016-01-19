using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class User : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.InitFromSqlReader(reader, ref colIndex);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int colIndex)
        {
            this.Id = reader.GetGuid(colIndex++);
            this.UserType = (UserType)reader.GetByte(colIndex++);
            this.UserStatus = (UserStatus)reader.GetByte(colIndex++);
            this.SplitId = Database.GetInt32OrDefault(reader, ref colIndex);
            this.Username = Database.GetStringOrDefault(reader, ref colIndex);
            this.Password = Database.GetStringOrDefault(reader, ref colIndex);
            this.Description = Database.GetStringOrDefault(reader, ref colIndex); // read-only as it is being joined from another table
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);
        }

        protected List<SqlParameter> GetSqlParameters()
        {
            return new List<SqlParameter>(new SqlParameter[]{
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@Type", (byte)this.UserType),
                Database.SqlParam("@Status", (byte)this.UserStatus),
                Database.SqlParam("@SplitId", this.SplitId),
                Database.SqlParam("@Username", this.Username),
                Database.SqlParam("@Password", this.Password)
            });
        }

        public int CreateNewOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[CreateNewUser]",
                this.GetSqlParameters().ToArray());
        }

        public static int ActivateUserOnDBase(SqlConnection conn, SqlTransaction trans, Guid userId, bool ifOnlyPartial)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[ActivateUser]",
                Database.SqlParam("@Id", userId),
                Database.SqlParam("@OnlyIfPartial", ifOnlyPartial));
        }

        public static int AddOrUpdateUserDetailsOnDBase(SqlConnection conn, SqlTransaction trans, Guid userId, String description)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[AddOrUpdateUserDetails]",
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@Description", description));
        }

        public static List<User> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, string concatenatedUserIds)
        {
            return Database.ExecSProc<User>(
                conn,
                trans,
                "[dbo].[GetUsersByID]",
                Database.SqlParam("@concatenatedIdsAsText", concatenatedUserIds));
        }

        public static List<User> ReadAllFromDBaseWithFamiliarity(
            SqlConnection conn, SqlTransaction trans, string concatenatedUserIds, Guid userIdToCompare, ref List<Familiarity> familarities)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                Database.SqlParam("@concatenatedIdsAsText", concatenatedUserIds),
                Database.SqlParam("@userIdToCheckPic4Pics", userIdToCompare)
            };
            

            List<User> list = new List<User>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "[dbo].[GetUsersByIDWithPic4PicCounts]";
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int colIndex = 0;
                        User item = new User();
                        item.InitFromSqlReader(reader, ref colIndex);
                        list.Add(item);

                        Familiarity familiarity = Familiarity.Stranger;
                        int acceptedPic4PicCount = Database.GetInt32OrDefault(reader, ref colIndex);
                        if (acceptedPic4PicCount > 0) 
                        {
                            familiarity = Familiarity.Familiar;
                        }

                        familarities.Add(familiarity);
                    }
                }
            }

            return list;
        }

        public static List<User> ReadAllFromDBaseWithFamiliaritiesAndHash(
            SqlConnection conn, SqlTransaction trans, string concatenatedUserIds, Guid userIdToCompare, ref Dictionary<Guid, Familiarity> familiarityMaps)
        {
            List<Familiarity> familarities = new List<Familiarity>();
            List<User> matchesAsUsers = User.ReadAllFromDBaseWithFamiliarity(conn, trans, concatenatedUserIds, userIdToCompare, ref familarities);

            for (int x = 0; x < matchesAsUsers.Count; x++)
            {
                // build the map
                User user = matchesAsUsers[x];
                familiarityMaps.Add(user.Id, familarities[x]);
            }

            return matchesAsUsers;
        }

        public static User ReadFromDBase(SqlConnection conn, SqlTransaction trans, string username, string password)
        {
            bool usernameExists = false;
            return ReadFromDBase(conn, trans, username, password, out usernameExists);
        }

        public static User ReadFromDBaseById(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            List<User> list = Database.ExecSProc<User>(
                conn,
                trans,
                "[dbo].[GetUserByID]",
                Database.SqlParam("@UserId", userId));

            if (list.Count > 0)
            {
                if (list.Count > 1)
                {
                    throw new Pic4PicException("Unexpected error!");
                }

                return list[0];
            }

            return null;
        }

        public static User ReadFromDBaseWithFamiliarity(
            SqlConnection conn, SqlTransaction trans, Guid userId, Guid userIdToCompare, ref Familiarity familarity)
        { 
            List<Familiarity> temp = new List<Familiarity>();
            List<User> users = ReadAllFromDBaseWithFamiliarity(conn, trans, userId.ToString(), userIdToCompare, ref temp);
            if (users.Count > 0) 
            {
                if (temp.Count > 0)
                {
                    familarity = temp[0];
                }

                return users[0];
            }

            return null;
        }

        public static User ReadFromDBase(SqlConnection conn, SqlTransaction trans, string username, string password, out bool usernameExists) 
        {
            usernameExists = false;
            User user = _ReadFromDBase(conn, trans, username);
            if (user == null) 
            {
                return null;
            }

            usernameExists = true;
            if (user.Password != password) 
            {
                return null;
            }

            user.Password = "***";
            return user;
        }

        protected static User _ReadFromDBase(SqlConnection conn, SqlTransaction trans, string username)
        {
            List<User> list = Database.ExecSProc<User>(
                conn,
                trans,
                "[dbo].[GetUserByUsername]",
                Database.SqlParam("@Username", username));

            if (list.Count > 0)
            {
                if (list.Count > 1)
                {
                    throw new Pic4PicException("Unexpected error!");
                }

                return list[0];
            }

            return null;
        }

        public static int SaveTemporarySplitId(SqlConnection conn, SqlTransaction trans, Guid clientId, int splitId) 
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[AddOrUpdateTempSplitId]",
                Database.SqlParam("@ClientId", clientId),
                Database.SqlParam("@SplitId", splitId));
        }

        public static int ReadTemporarySplitId(SqlConnection conn, SqlTransaction trans, Guid clientId) {

            return Database.ExecScalar<int>(
                conn,
                trans,
                "[dbo].[GetTempSplitId]",
                Database.SqlParam("@ClientId", clientId));
        }
    }
}
