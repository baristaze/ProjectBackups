using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public enum CouponType
    {
        Unspecified = 0,
        DiscountAsPercentage,
        DiscountAsValue,
        FreeItem,
        FreesubItem,
        BuyXGetYFree
    }

    public partial class Coupon : IEditable
    {
        public const int MaxLengthTitle = 140;
        public const int MaxLengthContent = 2000;
        public const int MaxCouponPerCheckin = 1;
        public const int MaxCouponPerWeek = 2;
        public const int DefaultExpireDays = 30;

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid? CampaignId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChainId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? CheckInId { get; set; }
        public CouponType CouponType { get; set; }
        public decimal? FaceValue { get; set; }
        public DateTime ExpiryDateUTC { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime QueueTimeUTC { get; set; }
        public DateTime? PushTimeUTC { get; set; }
        public DateTime? ReadTimeUTC { get; set; }
        public Guid? RedeemCheckInId { get; set; }
        public decimal? RedeemedValue { get; set; }
        public DateTime? RedeemTimeUTC { get; set; }
        public DateTime? DeleteTimeUTC { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public int ReceiverCheckinCountInTotal { get; set; }

        public Coupon()
        {
            this.Id = Guid.NewGuid();
            this.QueueTimeUTC = DateTime.UtcNow;
            this.ExpiryDateUTC = this.QueueTimeUTC.AddDays(DefaultExpireDays);
        }

        public override string ToString()
        {
            return this.Content;
        }
    }

    public class CouponList : List<Coupon> { }

    [DataContract]
    public class SimpleCoupon
    {
        public SimpleCoupon(Coupon cpn)
        {
            this.Id = cpn.Id;
            this.Title = cpn.Title;
            this.DinerType = Diner.ToString(Diner.ConvertToDinerType(cpn.ReceiverCheckinCountInTotal), true).ToLowerInvariant();
            this.SenderFullName = cpn.SenderFirstName + " " + cpn.SenderLastName;
            this.SentDateTimeUTC = cpn.QueueTimeUTC;
            this.IsRedeemed = cpn.RedeemTimeUTC.HasValue;
            this.RedeemDateTimeUTC = cpn.RedeemTimeUTC.GetValueOrDefault();
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string DinerType { get; set; }

        [DataMember]
        public string SenderFullName { get; set; }

        // [DataMember]
        public DateTime SentDateTimeUTC { get; set; }

        [DataMember]
        public bool IsRedeemed { get; set; }

        // [DataMember]
        public DateTime RedeemDateTimeUTC { get; set; }

        [DataMember]
        public string SentDateTimeUtcAsText
        {
            get
            {
                return this.SentDateTimeUTC.ToString();
            }
            set
            {
                // json serializer needs a setter, too, to honor a property during serialization
            }
        }

        [DataMember]
        public string RedeemDateTimeUtcAsText
        {
            get
            {
                return this.RedeemDateTimeUTC.ToString();
            }
            set
            {
                // json serializer needs a setter, too, to honor a property during serialization
            }
        }
    }

    public class SimpleCouponList : List<SimpleCoupon> { }
}
