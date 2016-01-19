using System;
using System.Collections.Generic;

namespace Crosspl.ObjectModel
{
    public interface IPrintable
    {
        List<string> GetPropertyNames();
        string ToPrintString(string delim);
    }
}
