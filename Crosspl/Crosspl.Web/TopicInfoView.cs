using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;

using Crosspl.ObjectModel;

namespace Crosspl.Web
{
    public class TopicInfoView
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string SeoLink { get; set; }
        public string SeoLinkPlain { get; set; }
        public string EntryCountText { get; set; }
        public string SocialNumbers { get; set; }
        public string CreaterPhotoUrl { get; set; }
        public string CreaterActivityLink { get; set; }
        public List<EntryWriterView> TopWriters { get { return this.topWriters; } }
        private List<EntryWriterView> topWriters = new List<EntryWriterView>();

        private UserAuthInfo User;
        private string RootWebUrl;
        private int DefaultSplitId;
        private NameValueCollection QueryString;

        public TopicInfoView(TopicInfo topic, string rootWebUrl, UserAuthInfo user, int defaultSplitId, NameValueCollection queryString)
        {
            this.Id = topic.Id;
            this.SeoLinkPlain = topic.SeoLink;
            this.SeoLink = topic.SeoLink; // this will be updated later
            this.Title = StringHelpers.ToTitleCase2(topic.Title);
            this.EntryCountText = String.Format(
                CultureInfo.InvariantCulture,
                "({0} entries)",
                topic.EntryCount);

            this.SocialNumbers = String.Format(
                CultureInfo.InvariantCulture,
                "{0} shares, {1} votes, {2} reactions",
                (topic.ShareCount + topic.InvitationCount),
                topic.VoteCount,
                topic.ReactionCount);

            if (topic.Creator != null)
            {
                this.CreaterPhotoUrl = topic.Creator.GetPhotoUrlOrDefault(rootWebUrl);
            }
            else
            {
                this.CreaterPhotoUrl = rootWebUrl + "/Images/user2.png";
            }

            foreach (FacebookUser writer in topic.TopWriters)
            {
                EntryWriterView contributor = new EntryWriterView();
                contributor.Id = writer.Id;
                contributor.Name = writer.FirstName;
                this.TopWriters.Add(contributor);
            }

            // update the full SeoLink
            string delim = "?";
            string topicUrl = rootWebUrl + "/" + this.SeoLinkPlain;
            topicUrl = Splitter.AppendSplitId(user, defaultSplitId, -1, topicUrl, queryString, ref delim);
            topicUrl = Splitter.AppendExperimentId(topicUrl, queryString, false, null, ref delim);
            this.SeoLink = topicUrl;

            delim = "?";
            string linkToUser = rootWebUrl + "/user/" + topic.CreatedBy;
            linkToUser = Splitter.AppendSplitId(user, defaultSplitId, -1, linkToUser, queryString, ref delim);
            linkToUser = Splitter.AppendExperimentId(linkToUser, queryString, false, null, ref delim);
            this.CreaterActivityLink = linkToUser;

            this.User = user;
            this.RootWebUrl = rootWebUrl;
            this.DefaultSplitId = defaultSplitId;
            this.QueryString = queryString;
        }

        public override string ToString()
        {
            return this.Title;
        }

        public string TopWriterNames
        {
            get
            {
                if (topWriters.Count > 0)
                {
                    string writers = String.Empty;
                    for (int x = 0; x < this.topWriters.Count; x++)
                    {
                        string delim = "?";
                        string linkToUser = this.RootWebUrl + "/user/" + this.topWriters[x].Id;
                        linkToUser = Splitter.AppendSplitId(this.User, this.DefaultSplitId, -1, linkToUser, this.QueryString, ref delim);
                        linkToUser = Splitter.AppendExperimentId(linkToUser, this.QueryString, false, null, ref delim);

                        writers += String.Format(
                            CultureInfo.InvariantCulture,
                            "<a href=\"{0}\" class='entry-writer'>{1}</a>",
                            linkToUser,
                            this.topWriters[x].Name);

                        if (x != this.topWriters.Count - 1)
                        {
                            writers += "<span class='entry-writer-sep'>, </span>";
                        }
                    }

                    // interpunc middle point/ char 183 / &#183; / &middot;
                    return "<span class='entry-writer-label'> · Top Writers: </span>" + writers;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
