using System;

namespace OneSearch.Extensibility.Core.Data
{
    public class AppSettings : XmlDocumentDataSource
    {
        private AppSettings(string path) : base(path)
        {
        }

        public AppSettingSectionA SectionA
        {
            get => GetSection<AppSettingSectionA>();
            set => SetSection(value);
        }

        public new static AppSettings Load(string path) 
        {
            return new AppSettings(path);
        }
    }

    public class AppSettingSectionA
    {
        public string Name => "AppSetinngTestA";
    }
}
