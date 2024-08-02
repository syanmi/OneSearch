using System.Collections.Generic;

namespace OneSearch.Extensibility.Core.Services
{
    public interface IServiceProvider
    {
        TService GetService<TService>();
    }
}
