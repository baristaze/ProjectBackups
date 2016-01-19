﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Crosspl.ObjectModel;

namespace Crosspl.Web.Services
{
    [DataContract]
    public class EntryListResponse : BaseResponse
    {
        [DataMember]
        public List<Entry> Entries { get { return this.entries; } }
        private List<Entry> entries = new List<Entry>();
    }
}