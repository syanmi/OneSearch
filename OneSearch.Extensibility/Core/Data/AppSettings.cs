using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSearch.Extensibility.Core.Data
{
    public class AppSettings : IDataSource
    {
        private string _path;

        public AppSettings()
        {
            _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public T GetSection<T>() where T : new()
        {
            if(typeof(T) == typeof(AppSettingSectionA))
            {
                return (T)(object)new AppSettingSectionA();
            }

            return default;
        }
    }

    public class AppSettingSectionA
    {
        public string Name => "AppSetinngTestA";
    }
}
