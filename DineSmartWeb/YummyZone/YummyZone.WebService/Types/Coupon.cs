using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class Coupon
    {
        [DataMember()]
        public Guid Id { get; set; }

        internal Guid SenderChainId { get; set; }

        [DataMember()]
        public string Sender { get; set; }

        [DataMember()]
        public string Title { get; set; }

        [DataMember()]
        public string Content { get; set; }

        [DataMember()]
        public DateTime SentTimeUTC { get; set; }

        [DataMember()]
        public bool IsRead { get; set; }
    }

    [DataContract()]
    public class CouponList : BaseResponse
    {
        [DataMember()]
        public List<Coupon> Coupons { get { return this.coupons; } }
        private List<Coupon> coupons = new List<Coupon>();

        [DataMember()]
        public bool HasMoreCouponOnServer { get; set; }

        [DataMember()]
        public string HintForNextPage { get; set; }

    }
}