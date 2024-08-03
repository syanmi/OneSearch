using System.Xml.Serialization;

namespace OneSearch.Plugin.OneNote.Data
{
    internal class OneNotePluginSettings
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }
}
