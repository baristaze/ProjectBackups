using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shebeke.ObjectModel
{
    [DataContract]
    public partial class Category : NameIdPair
    {
        public Category() : this(0, null) { }
        public Category(long id, string name) : base(id, name) { }

        public static readonly Category[] All = new Category[] { 
            new Category(1, "Genel"),
            new Category(2, "Tanım"),
            new Category(3, "Haber"),
            new Category(4, "TV"),
            new Category(5, "Popüler Kültür"),
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
