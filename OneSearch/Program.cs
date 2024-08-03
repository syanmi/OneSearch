﻿using OneSearch.Extensibility.Core.Data;
using OneSearch.Extensibility.Core.Log;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote;
using System;
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
            var services = new SimpleServiceCollection();
            Configure(services);
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

            services.Add<AppSettings>((provider) => (AppSettings.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OneSearch")), ServiceLifeTime.Singleton);
            services.MapDataSrouce<AppSettings, AppSettingSectionA>();
        }
    }
}
