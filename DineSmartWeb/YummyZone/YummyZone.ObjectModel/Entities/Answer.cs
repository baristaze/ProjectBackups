using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class Answer : IEditable
    {
        public Guid CheckInId { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionText { get; private set; }

        public bool? AnswerYesNo { get; set; }
        public byte? AnswerRate { get; set; }
        public Guid? AnswerChoiceId { get; set; }
        public string AnswerChoiceText { get; private set; }
        public string AnswerFreeText { get; set; }

        public DateTime CreateTimeUTC { get; set; }
        public DateTime LastUpdateTimeUTC { get; set; }

        public Answer()
        {
            DateTime utc = DateTime.Now;
            this.CreateTimeUTC = utc;
            this.LastUpdateTimeUTC = utc;
        }

        public override string ToString()
        {
            return this.ConvertToNameAndValuePair().ToString();
        }

        public NameAndValue ConvertToNameAndValuePair()
        {
            NameAndValue pair = new NameAndValue();
            pair.Name = this.QuestionText;

            if (this.AnswerRate.HasValue)
            {
                pair.Value = this.AnswerRate.Value.ToString();
            }
            else if (this.AnswerYesNo.HasValue)
            {
                pair.Value = this.AnswerYesNo.Value ? "Yes" : "No";
            }
            else if (this.AnswerChoiceId.HasValue)
            {
                pair.Value = this.AnswerChoiceText;
            }
            else if (this.AnswerFreeText != null)
            {
                pair.Value = this.AnswerFreeText;
            }
            else
            {
                pair.Value = String.Empty;
            }

            return pair;
        }

        private int GetTypeAsInteger()
        {
            if (this.AnswerRate.HasValue)
            {
                return 1;
            }
            else if (this.AnswerYesNo.HasValue)
            {
                return 2;
            }
            else if (this.AnswerChoiceId.HasValue)
            {
                return 3;
            }
            else if (this.AnswerFreeText != null)
            {
                return 4;
            }
            else
            {
                return Int32.MaxValue;
            }
        }

        public static int CompareByType(Answer first, Answer second)
        {
            int a = first.GetTypeAsInteger();
            int b = second.GetTypeAsInteger();

            if (a < b)
            {
                return -1;
            }
            else if (a > b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public class AnswerList : List<Answer> 
    {
        public AnswerList FilterBy(Guid checkinId)
        {
            AnswerList filtered = new AnswerList();
            foreach (Answer item in this)
            {
                if (item.CheckInId == checkinId)
                {
                    filtered.Add(item);
                }
            }

            return filtered;
        }

        public Answer GetBy(Guid questionId)
        {
            foreach (Answer item in this)
            {
                if (item.QuestionId == questionId)
                {
                    return item;
                }
            }

            return null;
        } 

        public AnswerList RateAnswers
        {
            get
            {
                AnswerList filtered = new AnswerList();
                foreach (Answer item in this)
                {
                    if (item.AnswerRate.HasValue)
                    {
                        filtered.Add(item);
                    }
                }

                return filtered;
            }
        }

        public AnswerList YesNoAnswers
        {
            get
            {
                AnswerList filtered = new AnswerList();
                foreach (Answer item in this)
                {
                    if (item.AnswerYesNo.HasValue)
                    {
                        filtered.Add(item);
                    }
                }

                return filtered;
            }
        }

        public AnswerList MultiChoiceAnswers
        {
            get
            {
                AnswerList filtered = new AnswerList();
                foreach (Answer item in this)
                {
                    if (item.AnswerChoiceId.HasValue)
                    {
                        filtered.Add(item);
                    }
                }

                return filtered;
            }
        }

        public AnswerList FreeFormAnswers
        {
            get
            {
                AnswerList filtered = new AnswerList();
                foreach (Answer item in this)
                {
                    if (item.AnswerFreeText != null)
                    {
                        filtered.Add(item);
                    }
                }

                return filtered;
            }
        }        
    }
}   