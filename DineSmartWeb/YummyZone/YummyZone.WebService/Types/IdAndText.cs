using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using OM = YummyZone.ObjectModel;

namespace YummyZone.WebService
{
    [DataContract()]
    public class IdAndText
    {
        [DataMember()]
        public Guid Id { get; set; }

        [DataMember()]
        public string Text { get; set; }

        public IdAndText() : this(Guid.Empty, null) { }

        public IdAndText(Guid id, string text)
        {
            this.Id = id;
            this.Text = text;
        }

        public IdAndText(OM.Question question)
        {
            this.Id = question.Id;
            this.Text = question.Wording;
        }

        public IdAndText(OM.Choice choice)
        {
            this.Id = choice.Id;
            this.Text = choice.Wording;
        }
    }
}