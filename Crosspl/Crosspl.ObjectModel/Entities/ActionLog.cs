using System;

namespace Crosspl.ObjectModel
{
    public partial class ActionLog : IDBEntity
    {
        public long UserId { get; set; }
        public UserType UserType { get; set; }
        public int UserSplit { get; set; }
        public long FacebookId { get; set; }
        public string PhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string FacebookLink { get; set; }
        public string Hometown { get; set; }
        public AssetType AssetType { get; set; }
        public Action Action { get; set; }
        public DateTime ActionTimeUTC { get; set; }
        public string ActionValue { get; set; }
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public long EntryId { get; set; }
        public string EntryContent { get; set; }
    }
}
