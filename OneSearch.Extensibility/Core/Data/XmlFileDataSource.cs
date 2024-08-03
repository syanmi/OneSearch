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

        public T GetSection<T>()
        {
            if(_file.BaseURI != _path)
            {
                _file.Load(_path);
            }

            XmlNode node = _file.DocumentElement;
            if (node != null)
            {
                string nodeString = node.OuterXml;
                var myElement = XMLDeserialize<T>(nodeString);
                return myElement;
            }

            return default;
        }

        public void SetSection<T>(T section)
        {
            string filePath = _path;
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(filePath))
            {
                doc.Load(filePath);
            }
            else
            {
                XmlElement root = doc.CreateElement("Root");
                doc.AppendChild(root);
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            string xmlString = XMLSerialize(section);

            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(xmlString);
            XmlNode newNode = tempDoc.DocumentElement;

            XmlNode existingNode = doc.DocumentElement.SelectSingleNode(elementName);
            if (existingNode != null)
            {
                doc.DocumentElement.ReplaceChild(doc.ImportNode(newNode, true), existingNode);
            }
            else
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(newNode, true));
            }

            doc.Save(filePath);
            Console.WriteLine($"XML saved successfully for {typeof(T).Name}.");
        }
        static void SaveOrUpdateNode(XmlDocument doc, string elementName, string xmlString)
        {
            XmlNode existingNode = doc.DocumentElement.SelectSingleNode(elementName);
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(xmlString);
            XmlNode newNode = tempDoc.DocumentElement;

            if (existingNode != null)
            {
                doc.DocumentElement.ReplaceChild(doc.ImportNode(newNode, true), existingNode);
            }
            else
            {
                doc.DocumentElement.AppendChild(doc.ImportNode(newNode, true));
            }
        }

        public void Save()
        {
           // _file.Save(_path);
        }

        public static XmlFileDataSource Load(string path)
        {
            return new XmlFileDataSource(path);
        }

        public static string XMLSerialize<T>(T obj)
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
