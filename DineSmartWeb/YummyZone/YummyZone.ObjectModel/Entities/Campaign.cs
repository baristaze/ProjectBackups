using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public enum CampaignStatus
    {
        Draft = 0,
        Active,
        Disabled,
        Removed
    }

    public enum CampaignType
    {
        Unspecified = 0,
        Manual,
        AutoCouponOnSignup,
        MassDistribution
    }

    public enum Repeatition
    {
        None = 0,
        Daily,
        Weekly,
        Biweekly,
        Monthly
    }

    public partial class Campaign : IEditable
    {
        public Guid GroupId { get; set; }
        public Guid ChainId { get; set; }
        public Guid CreatorId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CampaignStatus Status { get; set; }
        public int Priority { get; set; }
        public CampaignType CampaignType { get; set; }
        public CouponType CouponType { get; set; }
        public decimal? FaceValue { get; set; }
        public int CouponCount { get; set; }
        public int DeliveredCouponCount { get; set; }
        public Repeatition Repeatition { get; set; }
        public int ExpiryDays { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishTimeUTC { get; set; }
        public DateTime? RevocationTimeUTC { get; set; }
        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public bool CanCreateCoupon()
        {
            if (this.Status != CampaignStatus.Active)
            {
                return false;
            }

            if(this.CouponCount <= 0)
            {
                return false;
            }

            if(this.PublishTimeUTC > DateTime.UtcNow)
            {
                return false;
            }

            if(this.RevocationTimeUTC.HasValue && this.RevocationTimeUTC.Value < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public Coupon CreateCoupon(Guid receiverId)
        {
            if(!this.CanCreateCoupon())
            {
                return null;
            }

            DateTime utcNow = DateTime.UtcNow;

            Coupon cpn = new Coupon();
            cpn.GroupId = this.GroupId;
            cpn.ChainId = this.ChainId;
            cpn.CampaignId = this.Id;
            cpn.SenderId = this.CreatorId;
            cpn.ReceiverId = receiverId;
            cpn.CouponType = this.CouponType;
            cpn.Title = this.Title;
            cpn.Content = this.Content;            
            cpn.ExpiryDateUTC = utcNow.AddDays(this.ExpiryDays);
            cpn.FaceValue = this.FaceValue;            
            cpn.QueueTimeUTC = utcNow;

            return cpn;
        }
    }

    public class CampaignList : List<Campaign> { }
}
