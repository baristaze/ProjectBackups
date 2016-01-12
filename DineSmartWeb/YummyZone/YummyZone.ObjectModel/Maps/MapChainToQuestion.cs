using System;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public class MapChainToQuestion : RelationMap
    {
        public MapChainToQuestion() : this(Guid.Empty, Guid.Empty, Guid.Empty) { }
        public MapChainToQuestion(Guid groupId, Guid chainId, Guid questionId) : this(groupId, chainId, questionId, Byte.MaxValue) { }
        public MapChainToQuestion(Guid groupId, Guid chainId, Guid questionId, byte orderIndex)
            : base("[dbo].[ChainAndQuestionMap]", "[ChainId]", "[QuestionId]")
        {
            this.GroupId = groupId;
            this.ChainId = chainId;
            this.QuestionId = questionId;
            this.OrderIndex = orderIndex;
        }

        public Guid ChainId
        {
            get { return this.firstEntityId; }
            set { this.firstEntityId = value; }
        }

        public Guid QuestionId
        {
            get { return this.secondEntityId; }
            set { this.secondEntityId = value; }
        }
    }

    public class MapListChainToQuestion : List<MapChainToQuestion> { }
}
