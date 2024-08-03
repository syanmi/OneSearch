using System.IO;
using System.Xml.Linq;

namespace OneSearch.Extensibility.Core.Data
{
    public class XDocumentDataSource : IDataSource
    {
        private XDocument _file;
        private string _path;

        protected XDocumentDataSource(string path)
        {
            _file = null;
            _path = path;
        }

        public T GetSection<T>() where T : new()
        {
            if (!File.Exists(_path))
            {
                return new T();
            }

            _file = _file ?? XDocument.Load(_path);

            string elementName = XmlHelper.GetElementName<T>();
            XElement element = _file.Root.Element(elementName);

            if (element != null)
            {
                return XmlHelper.XMLDeserialize<T>(element.ToString());
            }
            else
            {
                return new T();
            }
        }

        public void SetSection<T>(T section) where T : new()
        {
            if (_file == null)
            {
                _file = (File.Exists(_path)) ? 
                    XDocument.Load(_path) : 
                    new XDocument(new XElement("Root"));
            }

            string elementName = XmlHelper.GetElementName<T>();
            string xmlString = XmlHelper.XMLSerialize(section);

            XElement newElement = XElement.Parse(xmlString);
            XElement existingElement = _file.Root.Element(elementName);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(newElement);
            }
            else
            {
                _file.Root.Add(newElement);
            }
        }

        public void Save()
        {
            _file?.Save(_path);
        }

        private void Load()
        {
            if (File.Exists(_path))
            {
                _file = XDocument.Load(_path);
            }
        }

        public static XDocumentDataSource Load(string path)
        {
            return new XDocumentDataSource(path);
        }
    }
}
