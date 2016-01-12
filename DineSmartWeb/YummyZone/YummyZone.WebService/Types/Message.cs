using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.WebService
{
    [DataContract()]
    public class MessageToDiner
    {
        [DataMember()]
        public Guid Id { get; set; }

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
    public class MessageList : BaseResponse
    {
        [DataMember()]
        public List<MessageToDiner> Messages { get { return this.messages; } }
        private List<MessageToDiner> messages = new List<MessageToDiner>();

        [DataMember()]
        public bool HasMoreMessageOnServer { get; set; }

        [DataMember()]
        public string HintForNextPage { get; set; }
    }
}