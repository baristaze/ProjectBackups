using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public partial class ImageFile : MetaFile, IDBEntity
    {
        public void InitFromSqlReader(SqlDataReader reader)
        {
            int index = 0;
            this.Id = reader.GetInt64(index++);
            this.CloudId = reader.GetGuid(index++);

            this.CreatedBy = reader.GetInt64(index++);

            if (!reader.IsDBNull(index))
            {
                this.AssetId = reader.GetInt64(index);
            }
            index++;

            this.AssetType = (AssetType)reader.GetByte(index++);
            this.ContentType = reader.GetString(index++);
            this.ContentLength = reader.GetInt32(index++);

            if (!reader.IsDBNull(index))
            {
                this.OriginalUrl = reader.GetString(index);
            }
            index++;

            if (!reader.IsDBNull(index))
            {
                this.CloudUrl = reader.GetString(index);
            }
            index++;

            if (!reader.IsDBNull(index))
            {
                this.Width = reader.GetInt32(index);
            }
            index++;

            if (!reader.IsDBNull(index))
            {
                this.Height = reader.GetInt32(index);
            }
            index++;

            this.CreateTimeUTC = reader.GetDateTime(index++);
        }

        public long CreateOnDBase(SqlConnection conn, SqlTransaction trans)
        {
            this.Id = Database.ExecScalar<long>(
                conn,
                trans,
                "[dbo].[CreateNewImageMetaFile]",
                Database.SqlParam("@CloudId", this.CloudId),
                Database.SqlParam("@CreatedBy", this.CreatedBy),
                Database.SqlParam("@AssetId", this.AssetId),
                Database.SqlParam("@AssetType", (byte)this.AssetType),
                Database.SqlParam("@ContentType", this.ContentType),
                Database.SqlParam("@ContentLength", this.ContentLength),
                Database.SqlParam("@OriginalUrl", this.OriginalUrl),
                Database.SqlParam("@CloudUrl", this.CloudUrl),
                Database.SqlParam("@Width", this.Width),
                Database.SqlParam("@Height", this.Height));

            return this.Id;
        }

        public int DeleteFromDBase(SqlConnection conn, SqlTransaction trans, bool checkUser, long userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[DeleteImageMetaFile]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));
        }

        public int AssociateImageToAsset(SqlConnection conn, SqlTransaction trans, long userId)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[AssociateImageToAsset]",
                Database.SqlParam("@Id", this.Id),
                Database.SqlParam("@AssetId", this.AssetId),
                Database.SqlParam("@AssetType", (byte)this.AssetType),
                Database.SqlParam("@CheckUser", true),
                Database.SqlParam("@UserId", userId));
        }

        public static ImageFile ReadFromDBase(SqlConnection conn, SqlTransaction trans, long imageId, long userId)
        {
            return ReadFromDBase(conn, trans, imageId, true, userId);
        }

        public static ImageFile ReadFromDBase(SqlConnection conn, SqlTransaction trans, long imageId, bool checkUser, long userId)
        {
            List<ImageFile> images = Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetImageMetaFile]",
                Database.SqlParam("@Id", imageId),
                Database.SqlParam("@CheckUser", checkUser),
                Database.SqlParam("@UserId", userId));

            if (images.Count > 0)
            {
                return images[0];
            }

            return null;
        }

        public static List<ImageFile> ReadFromDBaseByAssetIDs(SqlConnection conn, SqlTransaction trans, AssetType assetType, string assetIdListAsText)
        {
            return Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetImageMetaFilesByAssetIDs]",
                Database.SqlParam("@AssetType", (byte)assetType),
                Database.SqlParam("@AssetIdListAsText", assetIdListAsText));
        }

        public static List<ImageFile> ReadUnassignedImagesFromDBase(
            SqlConnection conn, SqlTransaction trans, long userId, AssetType assetType)
        {
            return Database.ExecSProc<ImageFile>(
                conn,
                trans,
                "[dbo].[GetUnAssociatedImages]",
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@AssetType", (byte)assetType));
        }
    }
}
