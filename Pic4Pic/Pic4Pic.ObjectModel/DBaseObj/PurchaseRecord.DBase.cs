using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections.Generic;

namespace Pic4Pic.ObjectModel
{
    public partial class PurchaseRecord :  IDBEntity, IVerifiable
    {
        public void InitFromSqlReader(SqlDataReader reader, ref int index, ref int earnedCredit, ref string offeredItemIDs)
        {
            this.PurchaseReferenceToken = Database.GetStringOrDefault(reader, ref index);   // [ExternalId]
            this.UserId = Database.GetGuidOrDefault(reader, ref index);
            offeredItemIDs = Database.GetStringOrDefault(reader, ref index);                // [ItemIDsInOfferredPackage]
            this.InternalOfferId = Database.GetInt32OrDefault(reader, ref index);
            this.AppStoreId = (AppStoreType)Database.GetByteOrDefault(reader, ref index);
            this.PurchaseTimeUTC = Database.GetDateTimeOrDefault(reader, ref index);
            earnedCredit = Database.GetInt32OrDefault(reader, ref index);                   // [EarnedCredit]
            this.PurchaseInstanceId = Database.GetStringOrDefault(reader, ref index);       // [ExtraData1]            
            this.InternalPurchasePayLoad = Database.GetStringOrDefault(reader, ref index);  // [ExtraData2]
        }

        public void InitFromSqlReader(SqlDataReader reader, ref int earnedCredit, ref string offeredItemIDs)
        {
            int index = 0;
            this.InitFromSqlReader(reader, ref index, ref earnedCredit, ref offeredItemIDs);
        }

        public void InitFromSqlReader(SqlDataReader reader)
        {
            int earnedCredit = 0;
            string offeredItemIDs = String.Empty;
            this.InitFromSqlReader(reader, ref earnedCredit, ref offeredItemIDs);
        }

        public int CreateOnDBase(SqlConnection conn, SqlTransaction trans, PurchaseOffer offer, string itemIDsInOfferredPackage)
        {
            return Database.ExecNonQuery(
                conn,
                trans,
                "[dbo].[SavePurchaseRecord]",
                Database.SqlParam("@ExternalId", this.PurchaseReferenceToken),
                Database.SqlParam("@UserId", this.UserId),
                Database.SqlParam("@ItemIDsInOfferredPackage", itemIDsInOfferredPackage),
                Database.SqlParam("@InternalOfferId", this.InternalOfferId),
                Database.SqlParam("@AppStoreId", (byte)this.AppStoreId),
                Database.SqlParam("@PurchaseTimeUTC", this.PurchaseTimeUTC),
                Database.SqlParam("@EarnedCredit", offer.CreditValue),
                Database.SqlParam("@ExtraData1", this.PurchaseInstanceId),
                Database.SqlParam("@ExtraData2", this.InternalPurchasePayLoad));
        }

        public static PurchaseRecord ReadFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId, string externalId)
        {
            List<PurchaseRecord> result = Database.ExecSProc<PurchaseRecord>(
                conn,
                trans,
                "[dbo].[GetPurchaseRecordByExternalId]",
                Database.SqlParam("@UserId", userId),
                Database.SqlParam("@ExternalId", externalId));

            if (result.Count > 0) 
            {
                return result[0];
            }

            return null;
        }

        public static List<PurchaseRecord> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, Guid userId)
        {
            return Database.ExecSProc<PurchaseRecord>(
                conn,
                trans,
                "[dbo].[GetPurchaseRecordsByUserId]",
                Database.SqlParam("@UserId", userId));
        }
        public static List<PurchaseRecord> ReadAllFromDBase(SqlConnection conn, SqlTransaction trans, int maxCount, int cutOffAsMinutes)
        {
            return Database.ExecSProc<PurchaseRecord>(
                conn,
                trans,
                "[dbo].[GetAllPurchaseRecords]",
                Database.SqlParam("@MaxCount", maxCount),
                Database.SqlParam("@CutOffMinutes", cutOffAsMinutes));
        }
    }
}
