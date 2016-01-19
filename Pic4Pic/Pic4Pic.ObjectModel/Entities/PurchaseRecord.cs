using System;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract]
    public partial class PurchaseRecord : IDBEntity, IVerifiable
    {
        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public int InternalOfferId { get; set; }

        [DataMember()]
        public AppStoreType AppStoreId { get; set; }

        [DataMember()]
        public DateTime PurchaseTimeUTC { get; set; }

        [DataMember()]
        public String PurchaseInstanceId { get; set; }

        [DataMember()]
        public String PurchaseReferenceToken { get; set; }

        [DataMember()]
        public String InternalPurchasePayLoad { get; set; }

        [DataMember()]
        public String OriginalData { get; set; }

        [DataMember()]
        public String DataSignature { get; set; }

        public PurchaseOffer GetPurchaseOffer()
        {
            return PurchaseOffer.GetByInternalId(this.InternalOfferId);
        }

        public void Validate()
        {
            if (PurchaseOffer.GetByInternalId(this.InternalOfferId) == null)
            {
                throw new Pic4PicException("Invalid Offer Id: " + this.InternalOfferId);
            }

            if (this.AppStoreId != AppStoreType.GooglePlay)
            {
                throw new Pic4PicException("Unknown AppStore Id: " + (int)this.AppStoreId);
            }

            if (this.PurchaseTimeUTC.Equals(default(DateTime)))
            {
                throw new Pic4PicException("PurchaseTimeUTC hasn't been specified");
            }

            if (String.IsNullOrWhiteSpace(this.PurchaseInstanceId))
            {
                throw new Pic4PicException("PurchaseInstanceId hasn't been specified");
            }

            if (String.IsNullOrWhiteSpace(this.PurchaseReferenceToken))
            {
                throw new Pic4PicException("PurchaseReferenceToken hasn't been specified");
            }

            /*
            if (String.IsNullOrWhiteSpace(this.InternalPurchasePayLoad))
            {
                throw new Pic4PicException("InternalPurchasePayLoad hasn't been specified");
            }
            */

            if (String.IsNullOrWhiteSpace(this.OriginalData))
            {
                throw new Pic4PicException("OriginalData hasn't been included into the PurchaseRecord");
            }

            if (String.IsNullOrWhiteSpace(this.DataSignature))
            {
                throw new Pic4PicException("DataSignature hasn't been included into the PurchaseRecord");
            }
        }
    }

    [DataContract]
    public class GooglePlayInappPurchaseRecord
    {
        [DataMember()]
        public string orderId { get; set; }

        [DataMember()]
        public string packageName { get; set; }

        [DataMember()]
        public string productId { get; set; }

        [DataMember()]
        public long purchaseTime { get; set; }

        private DateTime purchaseTimeUTC 
        {
            get 
            {
                long ticks = 621355968000000000L + this.purchaseTime * 10000;
                return new DateTime(ticks); 
            } 
        }

        [DataMember()]
        public int purchaseState { get; set; }

        
        [DataMember()]
        public string developerPayload { get; set; }

        [DataMember()]
        public string purchaseToken { get; set; }

        public bool Compare(PurchaseRecord purchase)
        {
            if (this.orderId != purchase.PurchaseInstanceId) {
                return false;
            }

            if (this.purchaseToken != purchase.PurchaseReferenceToken) {
                return false;
            }

            if (this.developerPayload != purchase.InternalPurchasePayLoad) {
                return false;
            }

            if (!this.purchaseTimeUTC.Equals(purchase.PurchaseTimeUTC)) {
                return false;
            }

            return true;
        }
    }
}
