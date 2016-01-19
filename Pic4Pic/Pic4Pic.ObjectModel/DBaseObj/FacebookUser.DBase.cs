using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class FacebookUser : IDBEntity
    {
        public int CreateNewOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[CreateNewFacebookUser]",
                this.GetSqlParameters().ToArray());
        }

        public int UpdateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[UpdateFacebookUser]",
                this.GetSqlParameters().ToArray());
        }

        // this is limited to FacebookUser table only. It is being used for Insert and Update.
        protected List<SqlParameter> GetSqlParameters()
        {
            return new List<SqlParameter>(new SqlParameter[]{
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@FacebookId", this.FacebookId),
                Database.SqlParam("@FirstName", this.FirstName),
                Database.SqlParam("@LastName", this.LastName),
                Database.SqlParam("@FullName", this.FullName),
                Database.SqlParam("@EmailAddress", this.EmailAddress),
                Database.SqlParam("@BirthDay", this.BirthDay),
                Database.SqlParam("@Gender", (byte)this.Gender),
                Database.SqlParam("@MaritalStatus", (byte)this.MaritalStatus),
                Database.SqlParam("@MaritalStatusAsText", this.MaritalStatusAsText),
                Database.SqlParam("@Profession", this.Profession),
                Database.SqlParam("@EducationLevel", (byte)this.EducationLevel),
                Database.SqlParam("@FacebookLink", this.FacebookLink),
                Database.SqlParam("@FacebookUserName", this.FacebookUserName),
                Database.SqlParam("@HometownCity", this.HometownCity),
                Database.SqlParam("@HometownState", this.HometownState),
                Database.SqlParam("@HometownId", this.HometownId),
                Database.SqlParam("@TimeZoneOffset", this.TimeZoneOffset),
                Database.SqlParam("@CurrentLocationCity", this.CurrentLocationCity),
                Database.SqlParam("@CurrentLocationState", this.CurrentLocationState),
                Database.SqlParam("@CurrentLocationId", this.CurrentLocationId),
                Database.SqlParam("@ISOLocale", this.ISOLocale),
                Database.SqlParam("@IsVerified", this.IsVerified),
                Database.SqlParam("@PhotoUrl", this.PhotoUrl),
            });
        }

        public int DeleteFromDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteFacebookUser]",
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@FacebookId", this.FacebookId));
        }
        
        public static FacebookUser ReadFromDBase(SqlConnection conn, SqlTransaction trans, long facebookId)
        {
            List<FacebookUser> list = Database.ExecSProc<FacebookUser>(
                conn,
                trans,
                "[dbo].[GetFacebookUser]",
                Database.SqlParam("@FacebookId", facebookId));

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        public static FacebookUser ReadFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            List<FacebookUser> list = ReadAllFromDBaseByID(conn, trans, userId.ToString());

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
        }

        public static List<FacebookUser> ReadAllFromDBaseByID(SqlConnection conn, SqlTransaction trans, string concatenatedUserIds)
        {
            return Database.ExecSProc<FacebookUser>(
                conn,
                trans,
                "[dbo].[GetFacebookUsersByID]",
                Database.SqlParam("@concatenatedUserIdsAsText", concatenatedUserIds));
        }

        public static Dictionary<Guid, FacebookUser> ReadAllFromDBaseWithFamiliarity(
            SqlConnection conn, 
            SqlTransaction trans, 
            string concatenatedUserIds, 
            Guid userIdToCompare, 
            ref Dictionary<Guid, Familiarity> familarities,
            ref Dictionary<Guid, UserType> userTypes)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                Database.SqlParam("@concatenatedUserIdsAsText", concatenatedUserIds),
                Database.SqlParam("@userIdToCheckPic4Pics", userIdToCompare)
            };


            Dictionary<Guid, FacebookUser> list = new Dictionary<Guid, FacebookUser>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "[dbo].[GetFacebookUsersByUserIDsWithExtensions]";
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

                        // read core FB properties
                        FacebookUser item = new FacebookUser();
                        item.InitFromSqlReader(reader, ref colIndex);

                        // read username
                        string username = Database.GetStringOrDefault(reader, ref colIndex);

                        // set username
                        item.Username = username;

                        // read user type
                        UserType userType = (UserType)Database.GetByteOrDefault(reader, ref colIndex);

                        // add to the list
                        list.Add(item.UserId, item);

                        // get familiarity
                        Familiarity familiarity = Familiarity.Stranger;
                        int acceptedPic4PicCount = Database.GetInt32OrDefault(reader, ref colIndex);
                        if (acceptedPic4PicCount > 0)
                        {
                            familiarity = Familiarity.Familiar;
                        }

                        // add to the familiarity
                        familarities.Add(item.UserId, familiarity);

                        // add to user types
                        userTypes.Add(item.UserId, userType);
                    }
                }
            }

            return list;
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;
            this.InitFromSqlReader(reader, ref colIndex);
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int colIndex)
        {
            this.UserId = reader.GetGuid(colIndex++);
            this.FacebookId = reader.GetInt64(colIndex++);
            this.FirstName = Database.GetStringOrDefault(reader, ref colIndex);
            this.LastName = Database.GetStringOrDefault(reader, ref colIndex);
            this.FullName = Database.GetStringOrDefault(reader, ref colIndex);
            this.EmailAddress = Database.GetStringOrDefault(reader, ref colIndex);
            this.BirthDay = Database.GetDateTimeOrDefault(reader, ref colIndex);
            this.Gender = (Gender)Database.GetByteOrDefault(reader, ref colIndex);
            this.MaritalStatus = (MaritalStatus)Database.GetByteOrDefault(reader, ref colIndex);
            this.MaritalStatusAsText = Database.GetStringOrDefault(reader, ref colIndex);
            this.Profession = Database.GetStringOrDefault(reader, ref colIndex);
            this.EducationLevel = (EducationLevel)Database.GetByteOrDefault(reader, ref colIndex);
            this.FacebookLink = Database.GetStringOrDefault(reader, ref colIndex);
            this.FacebookUserName = Database.GetStringOrDefault(reader, ref colIndex);
            this.HometownCity = Database.GetStringOrDefault(reader, ref colIndex);
            this.HometownState = Database.GetStringOrDefault(reader, ref colIndex);
            this.HometownId = Database.GetInt64OrDefault(reader, ref colIndex);
            this.TimeZoneOffset = Database.GetInt32OrDefault(reader, ref colIndex);
            this.CurrentLocationCity = Database.GetStringOrDefault(reader, ref colIndex);
            this.CurrentLocationState = Database.GetStringOrDefault(reader, ref colIndex);
            this.CurrentLocationId = Database.GetInt64OrDefault(reader, ref colIndex);
            this.ISOLocale = Database.GetStringOrDefault(reader, ref colIndex);
            this.IsVerified = Database.GetBoolOrDefault(reader, ref colIndex);
            this.PhotoUrl = Database.GetStringOrDefault(reader, ref colIndex);
            this.Description = Database.GetStringOrDefault(reader, ref colIndex); // read-only as it is being joined from another table
            this.CreateTimeUTC = Database.GetDateTimeOrDefault(reader, ref colIndex);
            this.LastUpdateTimeUTC = Database.GetDateTimeOrDefault(reader, ref colIndex);
        }
    }
}