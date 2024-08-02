using OneSearch.Extensibility.Core.Log;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneSearch
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new SimpleServiceCollection();
            Configure(services);

            var provider = services.BuildServiceProvider();
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
            // services.AddSingleton<ITraceLoggerFactory>();
        }
    }
}
