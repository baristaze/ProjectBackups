using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class FacebookUser : UserInfo, IDBEntity
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

        // this is limited to FacebookUser table only. It is being used for Insert and Update. [User] part is not considered
        protected List<SqlParameter> GetSqlParameters()
        {
            return new List<SqlParameter>(new SqlParameter[]{
                Database.SqlParam("@UserId", this.Id),
                Database.SqlParam("@FacebookId", this.FacebookId),
                Database.SqlParam("@FirstName", this.FirstName),
                Database.SqlParam("@LastName", this.LastName),
                Database.SqlParam("@FullName", this.FullName),
                Database.SqlParam("@EmailAddress", this.EmailAddress),
                Database.SqlParam("@BirthDay", this.BirthDay),
                Database.SqlParam("@Gender", (byte)this.Gender),
                Database.SqlParam("@FacebookLink", this.FacebookLink),
                Database.SqlParam("@FacebookUserName", this.FacebookUserName),
                Database.SqlParam("@Hometown", this.Hometown),
                Database.SqlParam("@HometownId", this.HometownId),
                Database.SqlParam("@TimeZoneOffset", this.TimeZoneOffset),
                Database.SqlParam("@CurrentLocation", this.CurrentLocation),
                Database.SqlParam("@CurrentLocationId", this.CurrentLocationId),
                Database.SqlParam("@ISOLocale", this.ISOLocale),
                Database.SqlParam("@IsVerified", this.IsVerified),
                Database.SqlParam("@PhotoUrl", this.PhotoUrl)
            });
        }

        public int DeleteFromDBase(SqlConnection conn, SqlTransaction trans)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteFacebookUser]",
                Database.SqlParam("@UserId", this.Id),
                Database.SqlParam("@FacebookId", this.FacebookId));
        }
        
        // includes extra fields from [User] table, too
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

        // includes extra fields from [User] table, too
        protected static List<FacebookUser> ReadAllFromDBaseByID(SqlConnection conn, SqlTransaction trans, string concatenatedUserIds)
        {
            return Database.ExecSProc<FacebookUser>(
                conn,
                trans,
                "[dbo].[GetFacebookUsersByID]",
                Database.SqlParam("@concatenatedUserIdsAsText", concatenatedUserIds));
        }

        // includes extra fields from [User] table, too
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int colIndex = 0;

            this.Id = reader.GetInt64(colIndex++);
            this.FacebookId = reader.GetInt64(colIndex++);
            this.UserType = (UserType)reader.GetByte(colIndex++);
            this.UserStatus = (UserStatus)reader.GetByte(colIndex++);
            this.SplitId = Database.GetInt32OrDefault(reader, ref colIndex);

            this.FirstName = Database.GetStringOrDefault(reader, ref colIndex);
            this.LastName = Database.GetStringOrDefault(reader, ref colIndex);
            this.FullName = Database.GetStringOrDefault(reader, ref colIndex);
            this.EmailAddress = Database.GetStringOrDefault(reader, ref colIndex);
            this.BirthDay = Database.GetDateTimeOrDefault(reader, ref colIndex);
            this.Gender = (Gender)Database.GetByteOrDefault(reader, ref colIndex);

            this.FacebookLink = Database.GetStringOrDefault(reader, ref colIndex);
            this.FacebookUserName = Database.GetStringOrDefault(reader, ref colIndex);
            this.Hometown = Database.GetStringOrDefault(reader, ref colIndex);
            this.HometownId = Database.GetInt64OrDefault(reader, ref colIndex);
            this.TimeZoneOffset = Database.GetInt32OrDefault(reader, ref colIndex);
            this.CurrentLocation = Database.GetStringOrDefault(reader, ref colIndex);
            this.CurrentLocationId = Database.GetInt64OrDefault(reader, ref colIndex);
            this.ISOLocale = Database.GetStringOrDefault(reader, ref colIndex);
            
            this.IsVerified = reader.GetBoolean(colIndex++);
            this.CreateTimeUTC = reader.GetDateTime(colIndex++);
            this.LastUpdateTimeUTC = reader.GetDateTime(colIndex++);

            this.PhotoUrl = Database.GetStringOrDefault(reader, ref colIndex);
        }
    }
}