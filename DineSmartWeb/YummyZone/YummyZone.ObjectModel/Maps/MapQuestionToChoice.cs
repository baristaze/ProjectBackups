using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public class MapQuestionToChoice : RelationMap
    {
        public MapQuestionToChoice() : this(Guid.Empty, Guid.Empty, Guid.Empty) { }
        public MapQuestionToChoice(Guid groupId, Guid questionId, Guid choiceId) : this(groupId, questionId, choiceId, Byte.MaxValue) { }
        public MapQuestionToChoice(Guid groupId, Guid questionId, Guid choiceId, byte orderIndex)
            : base("[dbo].[QuestionAndChoiceMap]", "[QuestionId]", "[ChoiceId]")
        {
            this.GroupId = groupId;
            this.QuestionId = questionId;
            this.ChoiceId = choiceId;
            this.OrderIndex = orderIndex;
        }

        public Guid QuestionId
        {
            get { return this.firstEntityId; }
            set { this.firstEntityId = value; }
        }

        public Guid ChoiceId
        {
            get { return this.secondEntityId; }
            set { this.secondEntityId = value; }
        }
    }

    public class MapListQuestionToChoice : List<MapQuestionToChoice> 
    {
        public MapListQuestionToChoice Filter(Guid questionId, Status status)
        {
            MapListQuestionToChoice filteredItems = new MapListQuestionToChoice();
            foreach (MapQuestionToChoice choiceMap in this)
            {
                if (choiceMap.QuestionId == questionId && status == choiceMap.Status)
                {
                    filteredItems.Add(choiceMap);
                }
            }

            return filteredItems;
        }
    }
}