using System;

namespace Shebeke.ObjectModel
{
    public class ActionLogView
    {
        public long UserId { get; set; }
        public int UserSplit { get; set; }
        public long FacebookId { get; set; }
        public string FacebookLink { get; set; }
        public string PhotoUrl { get; set; }
        public string Profile { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string ActionTime { get; set; }
        public string ActionData { get; set; }
        public string LinkToData { get; set; }
        public string ActionTypeColorClass { get; set; }

        public ActionLogView(UserAuthInfo user, ActionLog log, string rootUrl, string urlPostFix, string referenceTimeZone, string localTimeFormat, int maxDataLength)
        {
            this.UserId = log.UserId;
            this.UserSplit = log.UserSplit;
            this.FacebookId = log.FacebookId;
            this.FacebookLink = log.FacebookLink;
            if (String.IsNullOrWhiteSpace(log.PhotoUrl))
            {
                if (log.FacebookId > 0)
                {
                    this.PhotoUrl = "https://graph.facebook.com/" + log.FacebookId.ToString() + "/picture?type=small";
                }
                else
                {
                    this.PhotoUrl = rootUrl + "/Images/user2.png";
                }
            }
            else
            {
                this.PhotoUrl = log.PhotoUrl;
            }

            this.Profile = this.GetProfileString(log);
            if (String.IsNullOrWhiteSpace(log.FirstName) && String.IsNullOrWhiteSpace(log.LastName))
            {
                this.Name = "Misafir";
            }
            else
            {
                this.Name = log.FirstName + " " + log.LastName;
            }

            this.Action = GetActionString(log);

            TimeZoneInfo ourTimeZone = TimeZoneInfo.FindSystemTimeZoneById(referenceTimeZone);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(log.ActionTimeUTC, ourTimeZone);
            this.ActionTime = localTime.ToString(localTimeFormat);
            this.ActionData = GetActionData(log, maxDataLength);
            this.LinkToData = GetLinkToData(user, log, rootUrl, urlPostFix);
            this.ActionTypeColorClass = GetColorClass(log);
        }

        protected virtual string GetColorClass(ActionLog log)
        {
            if (log.Action == ObjectModel.Action.Signup)
            {
                return "success";
            }
            else if (log.Action == ObjectModel.Action.Signin)
            {
                return "success";
            }
            else if (log.Action == ObjectModel.Action.Create)
            {
                return "info";
            }
            else if (log.Action == ObjectModel.Action.Vote)
            {
                return "warning";
            }
            else if (log.Action == ObjectModel.Action.React)
            {
                return "warning";
            }
            else if (log.Action == ObjectModel.Action.Share)
            {
                return "warning";
            }
            else if (log.Action == ObjectModel.Action.Invite)
            {
                return "error";
            }

            return "nocolorcode";
        }

        protected virtual string GetLinkToData(UserAuthInfo user, ActionLog log, string rootUrl, string urlPostFix)
        {
            string link = String.Empty;

            if (log.AssetType == AssetType.Topic || log.AssetType == AssetType.Entry)
            {
                if (log.TopicId > 0 && !String.IsNullOrWhiteSpace(log.TopicTitle))
                {
                    link = rootUrl + "/" + SpecialCharUtils.GetSeoLink(log.TopicTitle);

                    string delim = "?";
                    if (log.EntryId > 0)
                    {
                        link += delim + "e=" + log.EntryId.ToString();
                        delim = "&";
                    }

                    if (!String.IsNullOrWhiteSpace(urlPostFix))
                    {
                        link += delim + urlPostFix;
                        delim = "&";
                    }
                }
            }

            return link;
        }

        protected virtual string GetActionData(ActionLog log, int maxDataLength)
        {
            string data = String.Empty;
            if (!String.IsNullOrWhiteSpace(log.EntryContent))
            {
                data = log.EntryContent;
            }
            else if (!String.IsNullOrWhiteSpace(log.TopicTitle))
            {
                data = log.TopicTitle;
            }
            else if (!String.IsNullOrWhiteSpace(log.ActionValue))
            {
                data = log.ActionValue;
            }

            if (!String.IsNullOrWhiteSpace(data))
            {
                if (data.Length > maxDataLength)
                {
                    data = data.Substring(0, maxDataLength) + "...";
                }
            }

            return data;
        }

        protected virtual string GetProfileString(ActionLog log)
        {
            string str = String.Empty;
            if (log.Gender == Gender.Male)
            {
                str += "Erkek";
            }
            else if (log.Gender == Gender.Female)
            {
                str += "Kadın";
            }

            if (!String.IsNullOrWhiteSpace(log.Hometown))
            {
                if (!String.IsNullOrWhiteSpace(str))
                {
                    str += ", ";
                }

                str += "yaşadığı yer: " + log.Hometown;
            }

            return str;
        }

        protected virtual string GetActionString(ActionLog log)
        {
            if (log.Action == ObjectModel.Action.Signup)
            {
                return "Kayıt Oldu";
            }
            else if (log.Action == ObjectModel.Action.Signin)
            {
                return "Bağlandı";
            }
            else if (log.Action == ObjectModel.Action.Create)
            {
                if (log.AssetType == AssetType.Topic)
                {
                    return "Yeni Başlık Ekledi";
                }
                else if (log.AssetType == AssetType.Entry)
                {
                    return "Yeni İçerik Ekledi";
                }
            }
            else if (log.Action == ObjectModel.Action.Share)
            {
                int channel = 0;
                Int32.TryParse(log.ActionValue, out channel);

                string str = String.Empty;
                if (channel == (int)SocialChannel.Facebook)
                {
                    str = "FB'de ";
                }
                else if (channel == (int)SocialChannel.Twitter)
                {
                    str = "TW'de ";
                }

                str += "Paylaştı";
                return str;
            }
            else if (log.Action == ObjectModel.Action.Invite)
            {
                int inviteeCount = 0;
                Int32.TryParse(log.ActionValue, out inviteeCount);

                if (inviteeCount > 0)
                {
                    return "Davet Gönderdi (" + inviteeCount.ToString() + ")";
                }
                else
                {
                    return "Davet Gönderdi (?)";
                }
            }
            else if (log.Action == ObjectModel.Action.Vote)
            {
                int vote = 0;
                Int32.TryParse(log.ActionValue, out vote);

                if (vote > 0)
                {
                    return "Oyladı (+1)";
                }
                else if (vote < 0)
                {
                    return "Oyladı (-1)";
                }
                else
                {
                    return "Oyladı";
                }
            }
            else if (log.Action == ObjectModel.Action.React)
            {
                long react = 0;
                Int64.TryParse(log.ActionValue, out react);
                Reaction reaction = Reaction.GetById(react);
                if (reaction != null)
                {
                    return "Kısa Yorum Yaptı (" + reaction.Name + ")";
                }
                else
                {
                    return "Kısa Yorum Yaptı";
                }
            }

            return String.Empty;
        }
    }

    public class ActionLogViewLight : ActionLogView
    {
        public ActionLogViewLight(
            UserAuthInfo user, ActionLog log, string rootUrl, string urlPostFix, string referenceTimeZone, string localTimeFormat, int maxDataLength)
            : base(user, log, rootUrl, urlPostFix, referenceTimeZone, localTimeFormat, maxDataLength)
        {
        }

        protected override string GetColorClass(ActionLog log)
        {
            /*
            if (log.Action == ObjectModel.Action.Signup)
            {
                return "success";
            }
            else if (log.Action == ObjectModel.Action.Create)
            {
                return "warning";
            }
            */
            return "nocolorcode";
        }

        protected override string GetActionString(ActionLog log)
        {
            if (log.Action == ObjectModel.Action.Signup)
            {
                return "Kayıt Oldu";
            }
            else if (log.Action == ObjectModel.Action.Signin)
            {
                return "Bağlandı";
            }
            else if (log.Action == ObjectModel.Action.Create)
            {
                if (log.AssetType == AssetType.Topic)
                {
                    return "Yeni Başlık Ekledi";
                }
                else if (log.AssetType == AssetType.Entry)
                {
                    return "Yeni İçerik Ekledi";
                }
            }
            else if (log.Action == ObjectModel.Action.Share)
            {
                int channel = 0;
                Int32.TryParse(log.ActionValue, out channel);

                string str = String.Empty;
                if (channel == (int)SocialChannel.Facebook)
                {
                    str = "Facebook'ta ";
                }
                else if (channel == (int)SocialChannel.Twitter)
                {
                    str = "Twitter'da ";
                }

                str += "Paylaştı";
                return str;
            }
            else if (log.Action == ObjectModel.Action.Invite)
            {
                int inviteeCount = 0;
                Int32.TryParse(log.ActionValue, out inviteeCount);

                if (inviteeCount > 0)
                {
                    return inviteeCount.ToString() + " arkadaşını davet etti";
                }
                else
                {
                    return "Arkadaşlarını davet etti";
                }
            }
            else if (log.Action == ObjectModel.Action.Vote)
            {
                int vote = 0;
                Int32.TryParse(log.ActionValue, out vote);

                if (vote > 0)
                {
                    return "Oyladı (+1)";
                }
                else if (vote < 0)
                {
                    return "Oyladı (-1)";
                }
                else
                {
                    return "Oyladı";
                }
            }
            else if (log.Action == ObjectModel.Action.React)
            {
                long react = 0;
                Int64.TryParse(log.ActionValue, out react);
                Reaction reaction = Reaction.GetById(react);
                if (reaction != null)
                {
                    return "Kısa Yorum Yaptı (" + reaction.Name + ")";
                }
                else
                {
                    return "Kısa Yorum Yaptı";
                }
            }

            return String.Empty;
        }
    }
}
