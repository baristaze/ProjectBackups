using System;

namespace YummyZone.ObjectModel
{
    public partial class Message : IEditable
    {
        public const int MaxLengthTitle = 140;
        public const int MaxLengthContent = 2000;
        public const int MaxMessagePerCheckin = 3;
        public const int MaxMessagePerWeek = 5;

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChainId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid? CheckInId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime QueueTimeUTC { get; set; }
        public DateTime? PushTimeUTC { get; set; }
        public DateTime? ReadTimeUTC { get; set; }
        public DateTime? DeleteTimeUTC { get; set; }

        public Message()
        {
            this.Id = Guid.NewGuid();
            this.QueueTimeUTC = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
