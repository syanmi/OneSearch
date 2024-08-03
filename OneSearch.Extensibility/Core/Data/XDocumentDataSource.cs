using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OneSearch.Extensibility.Core.Data
{
    public class XDocumentDataSource : IDataSource
    {
        private XmlDocument _file;
        private string _path;

        private XDocumentDataSource(string path)
        {
            _file = new XmlDocument();
            _path = path;
        }

        public T GetSection<T>() where T : new()
        {
            string filePath = _path;
            if (!File.Exists(filePath))
            {
                return new T(); // ファイルが存在しない場合、新しいインスタンスを返す
            }

            XDocument doc = XDocument.Load(filePath);

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            XElement element = doc.Root.Element(elementName);

            if (element != null)
            {
                return XmlHelper.XMLDeserialize<T>(element.ToString());
            }
            else
            {
                return new T(); // 要素が存在しない場合、新しいインスタンスを返す
            }
        }

        public void SetSection<T>(T section) where T : new()
        {
            string filePath = _path;
            XDocument doc;

            if (File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = new XDocument(new XElement("Root"));
            }

            string elementName = typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false) is XmlRootAttribute[] attributes && attributes.Length > 0 ? attributes[0].ElementName : typeof(T).Name;
            string xmlString = XmlHelper.XMLSerialize(section);

            XElement newElement = XElement.Parse(xmlString);
            XElement existingElement = doc.Root.Element(elementName);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(newElement);
            }
            else
            {
                doc.Root.Add(newElement);
            }

            doc.Save(filePath);
            Console.WriteLine($"XML saved successfully for {typeof(T).Name}.");
        }

        public void Save()
        {
            // _file.Save(_path);
        }

        private void Load()
        {
            if (File.Exists(_path))
            {
                _file.Load(_path);
            }
        }

        public static XDocumentDataSource Load(string path)
        {
            var source = new XDocumentDataSource(path);

            source.Load();

            return source;
        }
    }
}
