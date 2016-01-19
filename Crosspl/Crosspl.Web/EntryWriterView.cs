using System;

namespace Crosspl.Web
{
    public class EntryWriterView
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
