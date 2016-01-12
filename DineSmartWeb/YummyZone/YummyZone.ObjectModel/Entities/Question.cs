using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace YummyZone.ObjectModel
{
    public partial class Question : YummyZoneEntity, IEditable
    {
        public const int MaxWordingForRateQ = 40;
        public const int MaxWordingForYesNoQ = 120;
        public const int MaxWordingForMultiQ = 120;
        public const int MaxWordingForFreeTQ = 120;

        public Question() : base() { }

        public QuestionType QuestionType { get; set; }
        public string Wording { get; set; }

        public override string ToString()
        {
            return this.Wording;
        }

        public int GetMaxWordingLength()
        {
            switch (this.QuestionType) 
            {
                case QuestionType.Rate: return MaxWordingForRateQ;
                case QuestionType.YesNo: return MaxWordingForYesNoQ;
                case QuestionType.MultiChoice: return MaxWordingForMultiQ;
                case QuestionType.FreeText: return MaxWordingForFreeTQ;
                default: return MaxWordingForFreeTQ;
            }
        }

        private ChoiceList choices = new ChoiceList();
        public ChoiceList Choices { get { return this.choices; } }
    }

    public class QuestionList : List<Question> 
    {
        public Question this[Guid id]
        {
            get
            {
                foreach (Question item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }
    }
}
