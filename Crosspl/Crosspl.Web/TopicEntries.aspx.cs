using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Globalization;

using Crosspl.ObjectModel;

namespace Crosspl.Web
{
    public partial class TopicEntries : CrossplWebPage
    {
        public class EntryEx
        {
            public Entry Entry { get; set; }
            public string TopicTitle { get; set; }

            public EntryEx(Entry entry, string topicTitle)
            {
                this.Entry = entry;
                this.TopicTitle = topicTitle;
            }
        }

        public string PageUrlForFacebook = String.Empty;
        public string PageTitleForFacebook = "Vizibuzz";
        public string PageDescriptionForFacebook = String.Empty;

        protected override string GetSplitSectionFilters()
        {
             // 2: 'entries'
             // 3, 'signup-encourage-pg'
             // 4, 'signup-encourage-title'
             // 5, 'compare-entries'
             // 6, 'select-friends'
             // return "2,3,4,5,6";
            return "3";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Config config = new Config();
            config.Init();

            UserAuthInfo user = this.GetUserInfoFromContent(config.AES_Key, config.AES_IV, false);

            try
            {
                this.ProcessRequest(config, user);
            }
            catch (Exception ex)
            {
                // no need to re-throw
                string errorLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                    "TopicEntries.aspx.cs",
                    "Page_Load",
                    "ProcessRequest",
                    user.UserId,
                    user.SplitId,
                    ex.ToString());

                System.Diagnostics.Trace.WriteLine(errorLog, LogCategory.Error);
            }
        }

        protected void ProcessRequest(Config config, UserAuthInfo user)
        {
            using (SqlConnection connection = new SqlConnection(config.DBaseConnectionString))
            {
                connection.Open();

                // it checks the cache first
                int defaultSplitId = Splitter.GetDefaultSplitId(connection, null);
                if (this.CheckFacebookRedirect(connection, null, user, defaultSplitId))
                {
                    return; // redirected...
                }

                Topic topic = null;
                
                if (this.GetReferencedTopicOrRandom(connection, null, out topic))
                {
                    // it is random... we need to re-direct to make sure that the URL is updated
                    string delim = "?";
                    string link = this.Request.ApplicationPath.TrimEnd('/') + "/" + topic.SeoLink;
                    link = Splitter.AppendSplitId(user, defaultSplitId, -1, link, this.Request.QueryString, ref delim);
                    // don't specify experiment id here since the experiment might have been stopped. 
                    // The client-script is capable to detect it but server-side is not capable to do so.
                    // we will have one more re-direct and it is OK.
                    // link = Splitter.AppendExperimentId(link, this.Request.QueryString, true, "70062152-6", ref delim);
                    link = Splitter.AppendExperimentId(link, this.Request.QueryString, false, null, ref delim);

                    this.Response.Redirect(link);
                    return; 
                }

                this.AdjustSplitTestData(connection, null, user);
                this.SetApplicationPath();
                this.SetIsAuthenticatedFlag(user);
                this.SetCurrentUserInfo(user);
                
                if (topic != null)
                {
                    topic.Title = StringHelpers.ToTitleCase2(topic.Title);
                    bool canCurrentUserDeleteThisTopicIfNoEntry = (user.UserType > UserType.Regular) || (topic.CreatedBy == user.UserId);
                    topic.CanDelete = canCurrentUserDeleteThisTopicIfNoEntry && (topic.EntryCount == 0);

                    long entryId = this.LogPageVisit(user, topic);
                    Entry entry = this.UpdateMetaTagsForFacebook(connection, null, config, user, topic, entryId);
                    this.UpdateTopicDataControls(user, topic);

                    List<EntryEx> randomEntries = this.GetRandomEntries(connection, null, user, topic, entry);
                    if (randomEntries.Count > 0)
                    {
                        this.UpdateRandomEntry(randomEntries[0], true);
                        if (randomEntries.Count > 1)
                        {
                            this.UpdateRandomEntry(randomEntries[1], false);
                            this.entryComparisonDialogContentIsValid.InnerText = "1";
                        }
                    }

                    this.UpdateGenericHelperDataControls(connection, null);
                }
            }
        }

        protected bool GetReferencedTopicOrRandom(SqlConnection connection, SqlTransaction trans, out Topic topic)
        {
            topic = null;

            long referencedTopicId = -1;
            string topicIdText = this.Request.QueryString["topicId"];
            long.TryParse(topicIdText, out referencedTopicId);

            string seoLink = this.Request.QueryString["seoLink"];

            if (referencedTopicId > 0)
            {
                // it checks the cache first
                topic = Topic.ReadById(connection, trans, referencedTopicId, true, AssetStatus.New);

                if (topic == null && !String.IsNullOrWhiteSpace(seoLink))
                {
                    // seoLink = seoLink.Trim().ToLowerInvariant();
                    topic = Topic.ReadBySeoLink(connection, trans, seoLink, true, AssetStatus.New);
                }
            }
            else if (!String.IsNullOrWhiteSpace(seoLink))
            {
                // seoLink = seoLink.Trim().ToLowerInvariant();
                topic = Topic.ReadBySeoLink(connection, trans, seoLink, true, AssetStatus.New);
            }
            else
            {
                // it checks the cache first
                List<Topic> latestTopics = Topic.ReadLatestTopics(connection, trans, 100, true, AssetStatus.New);
                if (latestTopics.Count > 0)
                {
                    System.Diagnostics.Trace.WriteLine("TopicEntries - Redirecting to a random topic", LogCategory.Info);

                    Random rand = new Random((int)DateTime.Now.Ticks);
                    int randomIndex = rand.Next(0, latestTopics.Count); /*upper bound is exclusive*/
                    topic = latestTopics[randomIndex];
                    // seoLink = topic.SeoLink;
                    return true;
                }
            }

            return false;
        }

        protected long LogPageVisit(UserAuthInfo user, Topic topic)
        {
            long entryId = -1;
            bool isFromFacebook = false;
            string entryIdText = this.Request.QueryString["e"];
            if (long.TryParse(entryIdText, out entryId))
            {
                isFromFacebook = true;
            }

            String metricLog = String.Format(
                CultureInfo.InvariantCulture,
                // "[Version=1];[MetricName=PageVisit];[Page=TopicEntries];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}]",
                "[Version=2];[MetricName=PageVisit];[Page=TopicEntries];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}];[Split={4}]",
                (isFromFacebook ? "Facebook" : "Web"),
                user.UserId,
                topic.Id,
                entryId,
                user.SplitId);

            System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);

            return entryId;
        }

        protected Entry UpdateMetaTagsForFacebook(SqlConnection connection, SqlTransaction trans, Config config, UserAuthInfo user, Topic topic, long entryId)
        {
            Entry entry = null;
            string pageDescForFB = string.Empty;

            if (entryId > 0)
            {
                try
                {
                    entry = Entry.ReadFromDBase(connection, trans, entryId, true, AssetStatus.New);
                    if (entry != null)
                    {
                        if (entry.TopicId == topic.Id)
                        {
                            List<ImageFile> entryImages = ImageFile.ReadFromDBaseByAssetIDs(connection, trans, AssetType.Entry, entry.Id.ToString());
                            EntryPlainTextFormatter plainTextFormatter = new EntryPlainTextFormatter();
                            pageDescForFB = plainTextFormatter.GetEncodedHtml(entry.Content, entryImages, config.RootWebUrl);
                        }
                        else
                        {
                            // fake entry id is passed
                            entry = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // no need to re-throw
                    string errorLog = String.Format(
                        CultureInfo.InvariantCulture,
                        // "[Version=1];[File={0}];[Method={1}];[Action={2}];[User={3}];[Details={4}]",
                        "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                        "TopicEntries.aspx.cs",
                        "UpdateDataControls",
                        "ReadFromDBase|ReadFromDBaseByAssetIDs|GetEncodedHtml",
                        user.UserId,
                        user.SplitId,
                        ex.ToString());

                    System.Diagnostics.Trace.WriteLine(errorLog, LogCategory.Error);
                }
            }

            // escape special chars
            this.PageTitleForFacebook = HttpUtility.HtmlEncode(topic.Title);
            this.PageDescriptionForFacebook = HttpUtility.HtmlEncode(pageDescForFB);
            return entry;
        }

        protected void UpdateTopicDataControls(UserAuthInfo user, Topic topic)
        {
            this.topicId.InnerText = topic.Id.ToString();
            this.topicTitle.InnerText = topic.Title;
            string pageTitleText = topic.Title;
            this.pageTitle.InnerText = pageTitleText;

            if (!topic.CanDelete)
            {
                this.topicDelete.Style.Add("display", "none");
            }

            bool canCurrentUserDeleteThisTopicIfNoEntry = (user.UserType > UserType.Regular) || (topic.CreatedBy == user.UserId);
            this.canDeleteIfNoEntry.InnerText = canCurrentUserDeleteThisTopicIfNoEntry ? "1" : "0";

            this.categoryRepeater.DataSource = topic.Categories;
            this.categoryRepeater.DataBind();

            if (topic.Categories.Count > 0)
            {
                this.categoriesTitleLabel.InnerText = "Categories: ";
            }
        }

        protected List<EntryEx> GetRandomEntries(SqlConnection connection, SqlTransaction trans, UserAuthInfo user, Topic topic, Entry entry)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            List<Entry> entries = new List<Entry>();
            List<EntryEx> entriesEx = new List<EntryEx>();
            
            if (entry != null)
            {
                // first topic is OK, one of its entry is OK
                entries.Add(entry);
                entriesEx.Add(new EntryEx(entry, topic.Title));
            }
            else if (topic.EntryCount > 0)
            {
                // first topic is OK, but the referenced entry couldn't be found.
                // topic has still some entries. let's fetch one of them
                int randomIndex = rand.Next(0, topic.EntryCount); /*upper bound is exclusive*/
                List <Entry> temp = Entry.GetLatestActiveEntries(
                    connection, trans, topic.Id, user.UserId, 1, randomIndex, true, AssetStatus.New, 0);
                if (temp.Count > 0)
                {
                    // first random entry of topic 1
                    entries.Add(temp[0]);
                    entriesEx.Add(new EntryEx(temp[0], topic.Title));
                }
            }

            // get random topics... (latest topics indeed)
            // eliminate topics with no entry & topic that is equal to current
            // it checks the cache first
            List<Topic> topics = Topic.ReadLatestTopics(connection, trans, 100, true, AssetStatus.New);
            for (int x = 0; x < topics.Count; x++)
            {
                if (topics[x].Id == topic.Id || topics[x].EntryCount == 0)
                {
                    topics.RemoveAt(x);
                    x--;
                }
            }

            // get more random topic
            while (entriesEx.Count < 2 && topics.Count > 0)
            {
                // get a random topic
                int randTopicIndex = rand.Next(0, topics.Count); /*upper bound is exclusive*/
                Topic randomTopic = topics[randTopicIndex];
                topics.RemoveAt(randTopicIndex);

                int randEntryIndex = rand.Next(0, randomTopic.EntryCount); /*upper bound is exclusive*/

                List<Entry> tempEntryList = Entry.GetLatestActiveEntries(
                    connection, trans, randomTopic.Id, user.UserId, 1, randEntryIndex, true, AssetStatus.New, 0);

                if (tempEntryList.Count > 0)
                {
                    // first random entry of topic 1
                    entries.Add(tempEntryList[0]);
                    entriesEx.Add(new EntryEx(tempEntryList[0], randomTopic.Title));
                }
            }

            string appPath = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            EntryFormatHelper.FormatEntriesAndAttachImages(connection, trans, user, entries, appPath, false);
            return entriesEx;
        }

        protected void UpdateRandomEntry(EntryEx entry, bool isFirst)
        {
            if (isFirst)
            {
                this.randomTopicName1.InnerText = entry.TopicTitle;
                this.randomTopicId1.InnerText = entry.Entry.TopicId.ToString();
                this.randomEntryId1.InnerText = entry.Entry.Id.ToString();
                this.randomEntryContent1.InnerHtml = entry.Entry.ContentAsEncodedHtml;
            }
            else
            {
                this.randomTopicName2.InnerText = entry.TopicTitle;
                this.randomTopicId2.InnerText = entry.Entry.TopicId.ToString();
                this.randomEntryId2.InnerText = entry.Entry.Id.ToString();
                this.randomEntryContent2.InnerHtml = entry.Entry.ContentAsEncodedHtml;
            }
        }

        protected void UpdateGenericHelperDataControls(SqlConnection connection, SqlTransaction trans)
        {
            Reaction[] allReactions = Reaction.ReadAllFromDBase(connection, trans);
            this.reactionTypeRepeater.DataSource = allReactions;
            this.reactionTypeRepeater.DataBind();
        }
    }
}