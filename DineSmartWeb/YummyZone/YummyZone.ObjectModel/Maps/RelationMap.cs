using System;
using System.Globalization;
using System.Collections.Generic;

namespace YummyZone.ObjectModel
{
    public abstract partial class RelationMap : IEditable
    {        
        protected Guid firstEntityId;
        protected Guid secondEntityId;

        public Guid GroupId { get; set; }
        public Byte OrderIndex { get; set; }
        public Status Status { get; set; }

        private string table;
        private string firstEntityColName;
        private string secondEntityColName;

        private RelationMap() : this(null, null, null) { }
        protected RelationMap(string table, string firstEntityColName, string secondEntityColName) 
        {
            this.table = table;
            this.firstEntityColName = firstEntityColName;
            this.secondEntityColName = secondEntityColName;

            this.OrderIndex = Byte.MaxValue;
        }
    }

    public class RelationMapList : List<RelationMap> { }
}
