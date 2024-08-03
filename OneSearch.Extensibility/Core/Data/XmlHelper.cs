using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OneSearch.Extensibility.Core.Data
{
    public static class XmlHelper
    {
        public static string XMLSerialize<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        public static T XMLDeserialize<T>(string input)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(new XmlTextReader(new StringReader(input)));
        }
    }
}
