using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Crosspl.ObjectModel
{
    [DataContract]
    public partial class Category : NameIdPair
    {
        public Category() : this(0, null) { }
        public Category(long id, string name) : base(id, name) { }

        public static readonly Category[] All = new Category[] { 
            new Category(1, "General"),
            new Category(2, "Definition"),
            new Category(3, "News"),
            new Category(4, "TV"),
            new Category(5, "Pop Culture"),
        };

        public static Category GetById(long id)
        {
            foreach (Category c in All)
            {
                if (c.Id == id)
                {
                    return c;
                }
            }

            return null;
        }
    }
}
