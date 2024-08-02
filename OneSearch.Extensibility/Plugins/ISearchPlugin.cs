using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSearch.Extensibility.Plugins
{
    public class ISearchPlugin
    {
        Guid Id { get; }
        string Name { get; }

    }
}
