using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class ImageFile : Identifiable, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.Id = Database.GetGuidOrDefault(reader, ref index);
            this.GroupingId = Database.GetGuidOrDefault(reader, ref index);
            this.UserId = Database.GetGuidOrDefault(reader, ref index);
            this.Status = (AssetState)Database.GetByteOrDefault(reader, ref index);
            this.ContentType = Database.GetStringOrDefault(reader, ref index);
            this.ContentLength = Database.GetInt32OrDefault(reader, ref index);
            this.Width = Database.GetInt32OrDefault(reader, ref index);
            this.Height = Database.GetInt32OrDefault(reader, ref index);
            this.CloudUrl = Database.GetStringOrDefault(reader, ref index);
            this.IsBlurred = Database.GetBoolOrDefault(reader, ref index);
            this.IsThumbnail = Database.GetBoolOrDefault(reader, ref index);
            this.IsProfilePicture = Database.GetBoolOrDefault(reader, ref index);
            this.CreateTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
             return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[CreateNewImageMetaFile]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@GroupingId", this.GroupingId),
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@Status", this.Status),
                Database.SqlParam("@ContentType", this.ContentType),
                Database.SqlParam("@ContentLength", this.ContentLength),
                Database.SqlParam("@Width", this.Width),
                Database.SqlParam("@Height", this.Height),
                Database.SqlParam("@CloudUrl", this.CloudUrl),
                Database.SqlParam("@IsBlurred", this.IsBlurred),
                Database.SqlParam("@IsThumbnail", this.IsThumbnail),
                Database.SqlParam("@IsProfilePicture", this.IsProfilePicture));
        }

        public int MarkAsDeletedOnDBase(SqlConnection conn, SqlTransaction trans, bool checkUser, Guid userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[MarkImageMetaFileAsDeleted]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public int DisableOnDBase(SqlConnection conn, SqlTransaction trans, bool checkUser, Guid userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DisableImageMetaFile]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public int UpdateImageOwnership(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            int result = Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[UpdateImageOwner]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@UserId", userId));

            if (result > 0) {
                this.UserId = userId;
            }

            return result;
        }

        public static int UpdateAllImageOwnerships(SqlConnection conn, SqlTransaction trans, Guid userId, string concatenatedImageIds)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[UpdateAllImageOwners]",
                Database.SqlParam("@concatenatedImageIds", concatenatedImageIds),
                Database.SqlParam("@UserId", userId));
        }

        public static int ResetProfileFlags(SqlConnection conn, SqlTransaction trans, Guid userId, string concatenatedImageIds)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[ResetProfileFlags]",
                Database.SqlParam("@concatenatedImageIds", concatenatedImageIds),
                Database.SqlParam("@UserId", userId));
        }

        public int DeleteFromDBase(SqlConnection conn, SqlTransaction trans, bool checkUser, Guid userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteImageMetaFile]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public static ImageFile ReadFromDBase(SqlConnection conn, SqlTransaction trans, Guid imageId, Guid userId)
        {
            return ReadFromDBase(conn, trans, imageId, true, userId);
        }

        public static ImageFile ReadFromDBase(SqlConnection conn, SqlTransaction trans, Guid imageId, bool checkUser, Guid userId)
        {
            List<ImageFile> images = Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetImageMetaFileById]",
                Database.SqlParam("@Id", imageId),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));

            if (images.Count > 0)
            {
                return images[0];
            }

            return null;
        }

        public static List<ImageFile> ReadAllFromDBaseByUserId(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            return Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetImageMetaFilesByUserId]",
                Database.SqlParam("@UserId", userId));
        }

        public static List<ImageFile> ReadAllFromDBaseByUserIDs(SqlConnection conn, SqlTransaction trans, string concatenatedUserIds)
        {
            return Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetImageMetaFilesByUserIDs]",
                Database.SqlParam("@concatenatedIdsAsText", concatenatedUserIds));
        }

        public static Dictionary<Guid, List<ImageFile>> ReadAllFromDBaseByUserIDsAndHash(
            SqlConnection conn, SqlTransaction trans, string concatenatedUserIds) 
        {
            List<ImageFile> images = ImageFile.ReadAllFromDBaseByUserIDs(conn, trans, concatenatedUserIds);
            Dictionary<Guid, List<ImageFile>> imageMaps = new Dictionary<Guid, List<ImageFile>>();
            foreach (ImageFile image in images)
            {
                if (!imageMaps.ContainsKey(image.UserId))
                {
                    imageMaps.Add(image.UserId, new List<ImageFile>());
                }

                imageMaps[image.UserId].Add(image);
            }

            return imageMaps;
        }
    }
}
