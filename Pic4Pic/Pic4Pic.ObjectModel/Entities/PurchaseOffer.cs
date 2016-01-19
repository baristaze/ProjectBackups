using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pic4Pic.ObjectModel
{
    [DataContract()]
    public class PurchaseOffer
    {
        [DataMember()]
        public int InternalItemId { get; set; }
	
        [DataMember()]
        public AppStoreType AppStoreId { get; set; }
	
	    [DataMember()]
        public String AppStoreItemId { get; set; }
	
	    [DataMember()]
        public String InternalItemName { get; set; }

        [DataMember()]
        public String InternalItemDescription { get; set; }
	
	    [DataMember()]
        public int ItemPriceInCents { get; set; }

        [DataMember()]
        public int CreditValue { get; set; }

        private static PurchaseOffer createGooglePlayOffer(int id, int cost, int credit, int freeCredit)
        {
            PurchaseOffer offer = new PurchaseOffer();
            offer.AppStoreId = AppStoreType.GooglePlay;
            offer.AppStoreItemId = String.Format("buy_{0}_credit_test_version_01", credit);
            offer.ItemPriceInCents = cost;
            offer.CreditValue = credit;
            offer.InternalItemId = id;
            offer.InternalItemName = String.Format(CultureInfo.InvariantCulture, "Buy {0} Credit", credit);

            if (freeCredit > 0)
            {
                offer.InternalItemDescription = String.Format(
                    CultureInfo.InvariantCulture, 
                    "Get extra {0:n0} credit for free", // "You will be receiving {1:n0} credit in total.",
                    freeCredit); // , (credit + freeCredit)
            }

            return offer;
        }

        /*
            $0.99       10 credit    X DO NOT OFFER THIS
            $2.99       30 credit
            $4.99       50 credit
            $9.99      110 credit    Save 10% (get 10 credit free)
            $19.99      250 credit    Save 20% (get 50 credit free)
            $29.99      400 credit    Save 25% (get 100 credit free)
            $49.99      750 credit    Save 33% (get 250 credit free)
            $99.99    2,000 credit    Save 50% (get 1,000 credit free)
         */
        private static readonly PurchaseOffer GooglePlayOffer001 = createGooglePlayOffer(1, 99, 10, 0);
        private static readonly PurchaseOffer GooglePlayOffer003 = createGooglePlayOffer(3, 299, 30, 0);
        private static readonly PurchaseOffer GooglePlayOffer005 = createGooglePlayOffer(5, 499, 50, 0);
        private static readonly PurchaseOffer GooglePlayOffer010 = createGooglePlayOffer(10, 999, 100, 10);
        private static readonly PurchaseOffer GooglePlayOffer020 = createGooglePlayOffer(20, 1999, 200, 50);
        private static readonly PurchaseOffer GooglePlayOffer030 = createGooglePlayOffer(30, 2999, 300, 100);
        private static readonly PurchaseOffer GooglePlayOffer050 = createGooglePlayOffer(50, 4999, 5000, 250);
        private static readonly PurchaseOffer GooglePlayOffer100 = createGooglePlayOffer(100, 9999, 1000, 1000);

        private static object allOffersGuard = new object();
        private static Dictionary<int, PurchaseOffer> allOffers = null;

        public static PurchaseOffer GetByInternalId(int id)
        {
            if (allOffers == null)
            {
                lock (allOffersGuard)
                {
                    if (allOffers == null)
                    {
                        allOffers = new Dictionary<int, PurchaseOffer>();
                        allOffers.Add(GooglePlayOffer001.InternalItemId, GooglePlayOffer001);
                        allOffers.Add(GooglePlayOffer003.InternalItemId, GooglePlayOffer003);
                        allOffers.Add(GooglePlayOffer005.InternalItemId, GooglePlayOffer005);
                        allOffers.Add(GooglePlayOffer010.InternalItemId, GooglePlayOffer010);
                        allOffers.Add(GooglePlayOffer020.InternalItemId, GooglePlayOffer020);
                        allOffers.Add(GooglePlayOffer030.InternalItemId, GooglePlayOffer030);
                        allOffers.Add(GooglePlayOffer050.InternalItemId, GooglePlayOffer050);
                        allOffers.Add(GooglePlayOffer100.InternalItemId, GooglePlayOffer100);
                    }
                }
            }

            if (!allOffers.ContainsKey(id)) 
            {
                return null;
            }

            return allOffers[id];
        }

        /// <summary>
        /// Supports 4 different offer packages.
        /// </summary>
        /// <param name="splitId">1, 2, 3, 4</param>
        /// <returns></returns>
        public static List<PurchaseOffer> GetGooglePlayOffers(int splitId)
        {
            if (splitId == 2) 
            {
                return new List<PurchaseOffer>(new PurchaseOffer[] { GooglePlayOffer003, GooglePlayOffer010, GooglePlayOffer020, GooglePlayOffer030 });
            }

            if (splitId == 3)
            {
                return new List<PurchaseOffer>(new PurchaseOffer[] { GooglePlayOffer005, GooglePlayOffer010, GooglePlayOffer030, GooglePlayOffer050 });
            }

            if (splitId == 4)
            {
                return new List<PurchaseOffer>(new PurchaseOffer[] { GooglePlayOffer005, GooglePlayOffer020, GooglePlayOffer050, GooglePlayOffer100 });
            }
            // if split == 1 // or default
            return new List<PurchaseOffer>(new PurchaseOffer[] { GooglePlayOffer003, GooglePlayOffer005, GooglePlayOffer010, GooglePlayOffer020 });
        }

        public static List<int> GetGooglePlayOfferIDs(int splitId)
        {
            List<int> ids = new List<int>();
            List<PurchaseOffer> offers = GetGooglePlayOffers(splitId);
            foreach (PurchaseOffer offer in offers)
            {
                ids.Add(offer.InternalItemId);
            }

            return ids;
        }
    }
}
