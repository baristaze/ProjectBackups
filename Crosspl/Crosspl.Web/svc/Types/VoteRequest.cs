using System;

namespace Crosspl.Web.Services
{
    public class VoteRequest
    {
        public long TopicId { get; set; }
        public long EntryId { get; set; }
        public int Vote { get; set; }
    }
}