using System;
using System.Collections.Generic;

namespace Shebeke.ObjectModel
{
    public interface IPrintable
    {
        List<string> GetPropertyNames();
        string ToPrintString(string delim);
    }
}
