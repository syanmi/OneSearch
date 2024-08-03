using System.Xml.Serialization;

namespace OneSearch.Plugin.OneNote.Data
{
    public class OneNotePluginSettings
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }
}
