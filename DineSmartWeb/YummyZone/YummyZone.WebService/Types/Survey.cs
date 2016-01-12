using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class SurveyQuestion<T> : IdAndText
    {
        [DataMember()]
        public bool HasAnsweredInCurrentSession { get; set; }

        [DataMember()]
        public T AnswerInCurrentSession { get; set; }

        public SurveyQuestion() : this(Guid.Empty, null, false, default(T)) { }

        public SurveyQuestion(OM.Question question, bool hasCurrentAnswer, T currentAnswer)
            : this(question.Id, question.Wording, hasCurrentAnswer, currentAnswer) { }

        public SurveyQuestion(Guid id, string text, bool hasCurrentAnswer, T currentAnswer)
            : base(id, text)
        {
            this.HasAnsweredInCurrentSession = hasCurrentAnswer;
            this.AnswerInCurrentSession = currentAnswer;
        }
    }
    
    [DataContract()]
    public class MultipleChoiceQuestion : SurveyQuestion<Guid>
    {
        [DataMember()]
        public List<IdAndText> Choices { get { return this.choices; } }
        private List<IdAndText> choices = new List<IdAndText>();

        public MultipleChoiceQuestion() { }

        public MultipleChoiceQuestion(OM.Question question, bool hasCurrentAnswer, Guid currentAnswer) 
            : base(question, hasCurrentAnswer, currentAnswer)
        {
            foreach (OM.Choice choice in question.Choices)
            {
                this.choices.Add(new IdAndText(choice));
            }
        }
    }

    [DataContract()]
    public class Survey : BaseResponse
    {
        private List<SurveyQuestion<byte>> rateQuestions = new List<SurveyQuestion<byte>>();
        private List<SurveyQuestion<bool>> yesNoQuestions = new List<SurveyQuestion<bool>>();
        private List<SurveyQuestion<string>> openEndedQuestions = new List<SurveyQuestion<string>>();
        private List<MultipleChoiceQuestion> multipleChoiceQuestions = new List<MultipleChoiceQuestion>();

        [DataMember()]
        public Guid VenueId { get; set; }

        [DataMember()]
        public List<SurveyQuestion<byte>> RateQuestions { get { return this.rateQuestions; } }

        [DataMember()]
        public List<SurveyQuestion<bool>> YesNoQuestions { get { return this.yesNoQuestions; } }

        [DataMember()]
        public List<SurveyQuestion<string>> OpenEndedQuestions { get { return this.openEndedQuestions; } }

        [DataMember()]
        public List<MultipleChoiceQuestion> MultipleChoiceQuestions { get { return this.multipleChoiceQuestions; } }

        [DataMember]
        public int MaxLengthOfAnswerForAnOpenEndedQuestion { get; set; }

        public Survey() { }

        public void Init(OM.QuestionList raters, OM.QuestionList yesNo, OM.QuestionList multiple, OM.QuestionList openEnded, OM.AnswerList answerList)
        {
            OM.AnswerList rateAnswers = answerList.RateAnswers;
            OM.AnswerList yesNoAnswers = answerList.YesNoAnswers;
            OM.AnswerList multiChoiceAnswers = answerList.MultiChoiceAnswers;
            OM.AnswerList freeFormAnswers = answerList.FreeFormAnswers;

            foreach (OM.Question q in raters)
            {
                OM.Answer lastAnswer = rateAnswers.GetBy(q.Id);
                bool hasAnswer = (lastAnswer != null);
                byte answerVal = hasAnswer ? lastAnswer.AnswerRate.Value : (byte)0;
                this.rateQuestions.Add(new SurveyQuestion<byte>(q, hasAnswer, answerVal));
            }

            foreach (OM.Question q in yesNo)
            {
                OM.Answer lastAnswer = yesNoAnswers.GetBy(q.Id);
                bool hasAnswer = (lastAnswer != null);
                bool answerVal = hasAnswer ? lastAnswer.AnswerYesNo.Value : false;
                this.yesNoQuestions.Add(new SurveyQuestion<bool>(q, hasAnswer, answerVal));
            }

            foreach (OM.Question q in openEnded)
            {
                OM.Answer lastAnswer = freeFormAnswers.GetBy(q.Id);
                bool hasAnswer = (lastAnswer != null);
                string answerVal = hasAnswer ? lastAnswer.AnswerFreeText : String.Empty;
                this.openEndedQuestions.Add(new SurveyQuestion<string>(q, hasAnswer, answerVal));
            }

            foreach (OM.Question q in multiple)
            {
                OM.Answer lastAnswer = multiChoiceAnswers.GetBy(q.Id);
                bool hasAnswer = (lastAnswer != null);
                Guid answerVal = hasAnswer ? lastAnswer.AnswerChoiceId.Value : Guid.Empty;
                this.multipleChoiceQuestions.Add(new MultipleChoiceQuestion(q, hasAnswer, answerVal));
            }
        }
    }
}