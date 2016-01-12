using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Globalization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class MenuItemRate
    {
        [DataMember()]
        public Guid MenuItemId { get; set; }

        [DataMember()]
        public byte Rate { get; set; }

        public override string ToString()
        {
            return this.Rate.ToString();
        }

        public OM.MenuItemRate ToMenuItemRate()
        {
            OM.MenuItemRate mir = new OM.MenuItemRate();
            mir.MenuItemId = this.MenuItemId;
            mir.Rate = this.Rate;
            return mir;
        }
    }

    [DataContract()]
    public abstract class UserAnswer
    {
        [DataMember()]
        public Guid QuestionId { get; set; }

        public abstract OM.Answer ToAnswer();
    }

    [DataContract()]
    public class RateQuestionAnswer : UserAnswer
    {
        [DataMember()]
        public byte Rate { get; set; }

        public override string ToString()
        {
            return this.Rate.ToString();
        }

        public override OM.Answer ToAnswer()
        {
            OM.Answer answer = new OM.Answer();
            answer.QuestionId = this.QuestionId;
            answer.AnswerRate = this.Rate;
            return answer;
        }
    }

    [DataContract()]
    public class YesNoQuestionAnswer : UserAnswer
    {
        [DataMember()]
        public int Answer { get; set; }

        public override string ToString()
        {
            return this.Answer > 0 ? "Yes" : "No";
        }

        public override OM.Answer ToAnswer()
        {
            OM.Answer answer = new OM.Answer();
            answer.QuestionId = this.QuestionId;
            answer.AnswerYesNo = this.Answer > 0;
            return answer;
        }
    }

    [DataContract()]
    public class MultiChoiceQuestionAnswer : UserAnswer
    {
        [DataMember()]
        public Guid AnswerId { get; set; }

        public override string ToString()
        {
            return this.AnswerId.ToString();
        }

        public override OM.Answer ToAnswer()
        {
            OM.Answer answer = new OM.Answer();
            answer.QuestionId = this.QuestionId;
            answer.AnswerChoiceId = this.AnswerId;
            return answer;
        }
    }

    [DataContract()]
    public class OpenEndedQuestionAnswer : UserAnswer
    {
        [DataMember()]
        public string Answer { get; set; }

        public override string ToString()
        {
            return this.Answer;
        }

        public override OM.Answer ToAnswer()
        {
            OM.Answer answer = new OM.Answer();
            answer.QuestionId = this.QuestionId;
            answer.AnswerFreeText = this.Answer;
            return answer;
        }
    }

    [DataContract()]
    public class Coordinate
    {
        [DataMember()]
        public double Latitude { get; set; }

        [DataMember()]
        public double Longitude { get; set; }

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "Lat: {0}, Long: {1}",
                this.Latitude,
                this.Longitude);
        }
    }

    [DataContract()]
    public class Feedback
    {
        private List<MenuItemRate> menuItemRates = new List<MenuItemRate>();
        private List<RateQuestionAnswer> rateQuestionAnswers = new List<RateQuestionAnswer>();
        private List<YesNoQuestionAnswer> yesNoQuestionAnswers = new List<YesNoQuestionAnswer>();
        private List<MultiChoiceQuestionAnswer> multiChoiceQuestionAnswers = new List<MultiChoiceQuestionAnswer>();
        private List<OpenEndedQuestionAnswer> openEndedQuestionAnswers = new List<OpenEndedQuestionAnswer>();

        [DataMember()]
        public Guid VenueId { get; set; }

        [DataMember()]
        public Coordinate CurrentLocation { get; set; }

        [DataMember()]
        public List<MenuItemRate> MenuItemRates { get { return this.menuItemRates; } }

        [DataMember()]
        public List<RateQuestionAnswer> RateQuestionAnswers { get { return this.rateQuestionAnswers; } }

        [DataMember()]
        public List<YesNoQuestionAnswer> YesNoQuestionAnswers { get { return this.yesNoQuestionAnswers; } }

        [DataMember()]
        public List<MultiChoiceQuestionAnswer> MultiChoiceQuestionAnswers { get { return this.multiChoiceQuestionAnswers; } }

        [DataMember()]
        public List<OpenEndedQuestionAnswer> OpenEndedQuestionAnswers { get { return this.openEndedQuestionAnswers; } }

        public OM.AnswerList ToAnswerList(Guid checkinId)
        {
            OM.AnswerList answerList = new OM.AnswerList();

            foreach (RateQuestionAnswer a in this.rateQuestionAnswers)
            {
                if (a.Rate > 0)
                {
                    OM.Answer answer = a.ToAnswer();
                    answer.CheckInId = checkinId;
                    answerList.Add(answer);
                }
            }

            foreach (UserAnswer a in this.yesNoQuestionAnswers)
            {
                OM.Answer answer = a.ToAnswer();
                answer.CheckInId = checkinId;
                answerList.Add(answer);
            }

            foreach (UserAnswer a in this.multiChoiceQuestionAnswers)
            {
                OM.Answer answer = a.ToAnswer();
                answer.CheckInId = checkinId;
                answerList.Add(answer);
            }

            foreach (OpenEndedQuestionAnswer a in this.openEndedQuestionAnswers)
            {
                if (!String.IsNullOrWhiteSpace(a.Answer))
                {
                    a.Answer = a.Answer.Trim();
                    OM.Answer answer = a.ToAnswer();
                    answer.CheckInId = checkinId;
                    answerList.Add(answer);
                }
            }

            return answerList;
        }

        public OM.MenuItemRateList ToMenuItemRateList(Guid checkinId)
        {
            OM.MenuItemRateList menuItemRateList = new OM.MenuItemRateList();

            foreach (MenuItemRate mir in this.menuItemRates)
            {
                OM.MenuItemRate menuItemRate = mir.ToMenuItemRate();
                menuItemRate.CheckInId = checkinId;
                menuItemRateList.Add(menuItemRate);
            }

            return menuItemRateList;
        }
    }
}