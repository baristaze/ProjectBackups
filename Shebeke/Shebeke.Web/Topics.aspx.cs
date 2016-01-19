﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

using Shebeke.ObjectModel;

namespace Shebeke.Web
{
    public partial class Topics : ShebekeWebPage
    {
        protected override string GetSplitSectionFilters()
        {
            // 10: 'topics'
            return "10";
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
            catch(Exception ex)
            {
                // no need to re-throw
                string errorLog = String.Format(
                    CultureInfo.InvariantCulture,
                    "[Version=2];[File={0}];[Method={1}];[Action={2}];[User={3}];[Split={4}];[Details={5}]",
                    "Topics.aspx.cs",
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

                if (CheckFacebookRedirect(connection, null, user, defaultSplitId))
                {
                    return;
                }

                Topic topic = this.GetReferencedTopic(connection, null);
                if (topic != null)
                {
                    // topic has been specified
                    string delim = "?";
                    string link = this.Request.ApplicationPath.TrimEnd('/') + "/" + topic.SeoLink;
                    link = Splitter.AppendSplitId(user, defaultSplitId, -1, link, this.Request.QueryString, ref delim);
                    // don't specify experiment id here since the experiment might have been stopped. 
                    // The client-script is capable to detect it but server-side is not capable to do so.
                    // we will have one more re-direct and it is OK.
                    // link = Splitter.AppendExperimentId(link, this.Request.QueryString, true, "76168981-0", ref delim);
                    link = Splitter.AppendExperimentId(link, this.Request.QueryString, false, null, ref delim);

                    this.Response.Redirect(link);
                    return;
                }

                this.LogPageVisit(user);
                this.AdjustSplitTestData(connection, null, user);
                this.SetApplicationPath();
                this.SetIsAuthenticatedFlag(user);
                this.SetCurrentUserInfo(user);

                DateTime endTimeUtc = DateTime.UtcNow;
                DateTime startTimeUtc = endTimeUtc.AddDays(config.RecentPopularTopics_LastXDays * -1);

                // it checks the cache first...
                List<TopicInfo> topics = TopicInfo.GetRecentPopularTopics(
                    connection,
                    null, 
                    startTimeUtc,
                    endTimeUtc,
                    config.RecentPopularTopics_MaxTopicCount,
                    true,
                    AssetStatus.New,
                    5);

                // prepare the view items.
                List<TopicInfoView> items = new List<TopicInfoView>();
                foreach (TopicInfo tpx in topics)
                {
                    TopicInfoView tiv = new TopicInfoView(tpx, config.RootWebUrl, user, defaultSplitId, this.Request.QueryString);
                    items.Add(tiv);
                }

                this.recentPopularTopicsRepeater.DataSource = items;
                this.recentPopularTopicsRepeater.DataBind();

                // it checks the cache first
                List<Topic> promotedTopics = Topic.ReadPromotedTopics(
                    connection, null, user, true, AssetStatus.New, true, true, true);
                
                this.promotedTopicsRepeater.DataSource = promotedTopics;
                this.promotedTopicsRepeater.DataBind();

                // this is to boost SEO links only... content is hidden
                this.LoadLatestTopics(connection, null, config, user);
            }
        }

        protected void LoadLatestTopics(SqlConnection conn, SqlTransaction trans, Config config, UserAuthInfo user)
        {
            string topicLinks = String.Empty;
            // int defaultSplitId = Splitter.GetDefaultSplitId(conn, trans);
            // it checks the cache first
            List<Topic> topics = Topic.ReadLatestTopics(conn, trans, 100, true, AssetStatus.New);
            foreach (Topic topic in topics)
            {
                string topicUrl = config.RootWebUrl + "/" + topic.SeoLink;
                // we don't add split and experiment id since this is for SEO only
                //string delim = "?";
                //topicUrl = Splitter.AppendSplitId(user, defaultSplitId, -1, topicUrl, this.Request.QueryString, ref delim);
                //topicUrl = Splitter.AppendExperimentId(topicUrl, this.Request.QueryString, false, null, ref delim);
                String topicLinkElement = String.Format(
                    CultureInfo.InvariantCulture,
                    "<a href='{0}'>{1}</a><br/>",
                    topicUrl,
                    topic.Title);

                topicLinks += topicLinkElement;
            }

            this.latestTopics.InnerHtml = topicLinks;
        }

        protected Topic GetReferencedTopic(SqlConnection connection, SqlTransaction trans)
        {
            long referencedTopicId = -1;
            string topicIdText = this.Request.QueryString["topicId"];
            if (long.TryParse(topicIdText, out referencedTopicId) && referencedTopicId > 0)
            {
                // it checks the cache first
                return Topic.ReadById(connection, trans, referencedTopicId, true, AssetStatus.New);
            }

            return null;
        }

        protected void LogPageVisit(UserAuthInfo user)
        {
            String metricLog = String.Format(
                CultureInfo.InvariantCulture,
                // "[Version=1];[MetricName=PageVisit];[Page=TopicEntries];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}]",
                "[Version=2];[MetricName=PageVisit];[Page=Topics];[Referrer={0}];[UserId={1}];[TopicId={2}];[EntryId={3}];[Split={4}]",
                "Web",
                user.UserId,
                0,
                0,
                user.SplitId);

            System.Diagnostics.Trace.WriteLine(metricLog, LogCategory.Metric);
        }
    }
}