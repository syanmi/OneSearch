using OneSearch.Extensibility.Core.Data;
using OneSearch.Extensibility.Core.Log;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OneSearch
{
    [XmlRoot("YourElementName")]
    public class MyElement
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }

    [XmlRoot("YourElementNameA")]
    public class MyElementA
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {

            var source = XmlFileDataSource.Load(@"E:\repository\test.xml");
#if true
            var elm = source.GetSection<MyElement>();
            var elm2 = source.GetSection<MyElementA>();
#else
            var elm = new MyElement();
            elm.Name = "1212";
            elm.Value = "3434";
            source.SetSection(elm);

            var elmA = new MyElementA();
            elmA.Name = "56";
            elmA.Value = "78";
            source.SetSection(elmA);
            source.Save();
#endif

            while (true) { }


            var services = new SimpleServiceCollection();
            Configure(services);

            // 直接データでinjectionされたいものは以下のようにとる
            services.MapDataSrouce<AppSettings, AppSettingSectionA>();


            var provider = services.BuildServiceProvider();

            var setting = provider.GetService<AppSettingSectionA>();
            Console.WriteLine(setting.Name);
            
            var plugin = provider.GetService<OneNotePlugin>();
            plugin.Execute();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Console.WriteLine("finished.");
            while (true)
            {
                ;
            }
        }

        private static void Configure(IServiceCollection services)
        {
            services.AddOneSearch();
            services.AddOneNotePlugin();
        }

        private static void AddOneSearch(this IServiceCollection services)
        {
            services.Add(typeof(ITraceLogger<>), typeof(SimpleTraceLogger<>), ServiceLifeTime.Singleton);
            services.Add(typeof(ITraceLoggerFactory), typeof(TraceLoggerFactory), ServiceLifeTime.Singleton);
            services.Add(typeof(AppSettings), typeof(AppSettings), ServiceLifeTime.Singleton);
        }
    }

    public static class TestExtensions
    {
        public static void MapDataSrouce<TSource, TSection>(this IServiceCollection services) where TSource : IDataSource where TSection : class, new()
        {
            services.Add<TSection>((provider) => 
            {
                var source = provider.GetService<TSource>();
                return source.GetSection<TSection>();
            }, ServiceLifeTime.Singleton);

        }
    }
}
