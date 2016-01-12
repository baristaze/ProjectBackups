using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public partial class Choice : YummyZoneEntity, IEditable
    {
        public const int MaxWording = 120;

        public Choice() : this(null) { }
        public Choice(string w) : this(Guid.Empty, w) { }
        public Choice(Guid groupId, string w) : base(groupId) { this.Wording = w; }
        
        public string Wording { get; set; }

        public override string ToString()
        {
            return this.Wording;
        }
    }

    public class ChoiceList : List<Choice>
    {
        public Choice this[Guid id]
        {
            get
            {
                foreach (Choice item in this)
                {
                    if (item.Id == id)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public ChoiceList Filter(MapListQuestionToChoice choiceMaps)
        {
            ChoiceList filteredItems = new ChoiceList();
            foreach (MapQuestionToChoice choiceMap in choiceMaps)
            {
                Choice choice = this[choiceMap.ChoiceId];
                if (choice != null)
                {
                    filteredItems.Add(choice);
                }
            }

            return filteredItems;
        }
    }
}
