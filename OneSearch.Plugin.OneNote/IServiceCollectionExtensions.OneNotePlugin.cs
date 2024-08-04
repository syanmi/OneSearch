using OneSearch.Extensibility.Core.Data;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote.Data;

namespace OneSearch.Plugin.OneNote
{
    public static class ServiceCollectionExtensionsOneNotePlugin
    {
        public static void AddOneNotePlugin(this IServiceCollection collection)
        {
            collection.AddSingleton<IOneNotePlugin, OneNotePlugin>();
            collection.AddDataSection<AppSettings, OneNotePluginSettings>();
        }
    }
}
