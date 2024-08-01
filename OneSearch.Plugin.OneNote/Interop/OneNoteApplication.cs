using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Office.Interop.OneNote;

namespace OneSearch.Plugin.OneNote.Interop
{
    class OneNoteApplication
    {
        public Application _interopApplication = new Application();

        public Notebooks GetNotebooks()
        {
            // XXX: Figure out how to deal with performance implictions of this.
            // Maybe make this a paramater.
            return XMLDeserialize<Notebooks>(GetHierarchy("", HierarchyScope.hsPages));
        }

        public string GetHierarchy(string root, HierarchyScope hsScope)
        {
            string output;
            _interopApplication.GetHierarchy(root, hsScope, out output);
            return output;
        }

        public Page GetPageContent(string PageId)
        {
            string pageContent;
            _interopApplication.GetPageContent(PageId, out pageContent);
            return XMLDeserialize<Page>(pageContent);
        }

        public static T XMLDeserialize<T>(string input)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(new XmlTextReader(new StringReader(input)));
        }
    }
}
