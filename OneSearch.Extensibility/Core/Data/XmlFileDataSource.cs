using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OneSearch.Extensibility.Core.Data
{
    public class XmlFileDataSource : IFileDataSource
    {
        private XmlDocument _file;
        private string _path;

        private XmlFileDataSource(string path)
        {
            _file = new XmlDocument();
            _path = path;
        }

        public T GetSection<T>() where T : new()
        {
            string filePath = _path;

            if (!File.Exists(filePath))
            {
                return new T();
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            XmlNode node = _file.DocumentElement.SelectSingleNode(elementName);


            if (node != null)
            {
                string nodeString = node.OuterXml;
                return XMLDeserialize<T>(nodeString);
            }
            else
            {
                return new T(); // 要素が存在しない場合、新しいインスタンスを返す
            }
        }

        public void SetSection<T>(T section)
        {
            string filePath = _path;

            if (_file.DocumentElement == null)
            {
                XmlElement root = _file.CreateElement("Root");
                _file.AppendChild(root);
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            string xmlString = XMLSerialize(section);

            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(xmlString);
            XmlNode newNode = tempDoc.DocumentElement;

            XmlNode existingNode = _file.DocumentElement.SelectSingleNode(elementName);
            if (existingNode != null)
            {
                _file.DocumentElement.ReplaceChild(_file.ImportNode(newNode, true), existingNode);
            }
            else
            {
                _file.DocumentElement.AppendChild(_file.ImportNode(newNode, true));
            }

            Console.WriteLine($"XML saved successfully for {typeof(T).Name}.");
        }

        public void Save()
        {
            _file.Save(_path);
        }

        private void Load()
        {
            if (File.Exists(_path))
            {
                _file.Load(_path);
            }
        }

        public static XmlFileDataSource Load(string path)
        {
            var source = new XmlFileDataSource(path);

            source.Load();

            return source;
        }

        private static string XMLSerialize<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        private static T XMLDeserialize<T>(string input)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(new XmlTextReader(new StringReader(input)));
        }
    }
}
