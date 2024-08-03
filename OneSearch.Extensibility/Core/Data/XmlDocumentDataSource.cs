using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OneSearch.Extensibility.Core.Data
{
    public class XmlDocumentDataSource : IDataSource
    {
        private XmlDocument _file;
        private string _path;

        private XmlDocumentDataSource(string path)
        {
            _file = new XmlDocument();
            _path = path;
        }

        public T GetSection<T>() where T : new()
        {
            if (!File.Exists(_path))
            {
                return new T();
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            XmlNode node = _file.DocumentElement.SelectSingleNode(elementName);

            if (node != null)
            {
                string nodeString = node.OuterXml;
                return XmlHelper.XMLDeserialize<T>(nodeString);
            }
            else
            {
                return new T();
            }
        }

        public void SetSection<T>(T section) where T : new()
        {
            if (_file.DocumentElement == null)
            {
                XmlElement root = _file.CreateElement("Root");
                _file.AppendChild(root);
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            string xmlString = XmlHelper.XMLSerialize(section);

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

        public static XmlDocumentDataSource Load(string path)
        {
            var source = new XmlDocumentDataSource(path);

            source.Load();

            return source;
        }
    }
}
