using System;

namespace Shebeke.Web
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
