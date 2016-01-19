using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shebeke.ObjectModel
{
    public class SocialShareMetricParser : MetricParser<SocialShare>
    {
        public override SocialShare Parse(WADLogsTable log)
        {
            if (log == null || String.IsNullOrWhiteSpace(log.Message))
            {
                return default(SocialShare);
            }

            if (!log.Message.StartsWith(MetricParser<SocialShare>.MetricPreamble))
            {
                return default(SocialShare);
            }

            string message = this.ConsumePreamble(log.Message);

            int version = 0;
            message = this.ConsumeVersion(message, out version);
            if (version <= 0)
            {
                return default(SocialShare);
            }

            string metricName = null;
            message = this.ConsumeMetricName(message, out metricName);

            SocialChannel channel = SocialChannel.None;
            if (metricName == "FacebookShare") 
            {
                channel = SocialChannel.Facebook;
            }
            else if (metricName == "TwitterShare")
            {
                channel = SocialChannel.Twitter;
            }
            else
            {
                return default(SocialShare);
            }

            NameValuePair<string> pair = null;

            SocialShare share = new SocialShare();
            share.ShareTimeUTC = log.Timestamp;
            share.Channel = channel;

            pair = null;
            message = this.ParseNextAndValidateName(message, out pair, "Asset");
            if(pair.Value == "Topic")
            {
                share.AssetType = AssetType.Topic;
            }
            else if (pair.Value == "Entry")
            {
                share.AssetType = AssetType.Entry;
            }
            else
            {
                throw new ShebekeException("Unknown asset type in logs");
            }

            pair = null;
            message = this.ParseNextAndValidateName(message, out pair, "ShareType");
            if (pair.Value == "New")
            {
                share.ShareType = ShareType.New;
            }
            else if (pair.Value == "AppReqInvite")
            {
                share.ShareType = ShareType.AppReqInvite;
            }
            else if (pair.Value == "Reaction")
            {
                share.ShareType = ShareType.Reaction;
            }
            else if (pair.Value == "Vote")
            {
                share.ShareType = ShareType.Vote;
            }
            else if (pair.Value == "ClientSideShare")
            {
                share.ShareType = ShareType.ClientSideShare;
            }
            else
            {
                throw new ShebekeException("Unknown social share type in logs");
            }

            NameValuePair<int> pairInt = null;
            message = this.ParseNextAndValidateName(message, out pairInt, "ShareCount", 0, Int32.MaxValue);
            share.ShareCount = pairInt.Value;

            NameValuePair<long> pairLong = null;
            message = this.ParseNextAndValidateName(message, out pairLong, "UserId", -1, long.MaxValue);
            share.UserId = pairLong.Value;

            try
            {
                pairLong = null;
                message = this.ParseNextAndValidateName(message, out pairLong, "TopicId", -1, long.MaxValue);

                TopicSocialShare tShare = new TopicSocialShare(share);
                tShare.TopicId = pairLong.Value;
                share = tShare;
            }
            catch { }

            if (share is TopicSocialShare)
            {
                try
                {
                    pairLong = null;
                    message = this.ParseNextAndValidateName(message, out pairLong, "EntryId", -1, long.MaxValue);

                    if (pairLong.Value > 0)
                    {
                        EntrySocialShare eShare = new EntrySocialShare(share as TopicSocialShare);
                        eShare.EntryId = pairLong.Value;
                        share = eShare;
                    }
                }
                catch { }
            }

            if (share is EntrySocialShare)
            {
                try
                {
                    pairLong = null;
                    message = this.ParseNextAndValidateName(message, out pairLong, "ReactionId", 0, long.MaxValue);

                    ReactionSocialShare rShare = new ReactionSocialShare(share as EntrySocialShare);
                    rShare.ReactionId = pairLong.Value;
                    share = rShare;
                }
                catch { }
            }

            if ((share is EntrySocialShare) && !(share is ReactionSocialShare))
            {
                try
                {
                    pairLong = null;
                    message = this.ParseNextAndValidateName(message, out pairInt, "Vote", int.MinValue, int.MaxValue);

                    VoteSocialShare vShare = new VoteSocialShare(share as EntrySocialShare);
                    vShare.Vote = pairInt.Value;
                    share = vShare;
                }
                catch { }
            }

            if (version > 1)
            {
                pairInt = null;
                message = this.ParseNextAndValidateName(message, out pairInt, "Split", 0, int.MaxValue);
                share.Split = pairInt.Value;
            }

            return share;
        }
    }
}
