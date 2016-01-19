using System;

namespace Shebeke.Web.Services
{
    public class ReactionRequest
    {
        public long TopicId { get; set; }
        public long EntryId { get; set; }
        public long ReactionId { get; set; }
    }
}