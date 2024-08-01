using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using OneSearch.Plugin.OneNote;

namespace OneNotePageSearcher
{
    internal class OneNoteManager
    {
        private static readonly XNamespace One = "http://schemas.microsoft.com/office/onenote/2013/onenote";

        private Application oneNote = null;

        public double progressRate = 0;
        double count = 0;
        double totalCount = 0;

        public bool canceled = false;
        public bool isDebug = false;

        public string currentPageTitle = "";
        public string currentNotebookTitle = "";

        public bool isIndexing = false;

        private Dictionary<string, NotebookMetaInfo> notebookMetaInfo;
        private Dictionary<string, PageMetaInfo> pageMetaInfo;

        public OneNoteManager(bool isDebug)
        {
            this.isDebug = isDebug;
        }

        private void BuildMetaInfo()
        {
            string outputXML;
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            var notebookList = XDocument.Parse(outputXML).Descendants(One + "Notebook");

            notebookMetaInfo = new Dictionary<string, NotebookMetaInfo>();
            pageMetaInfo = new Dictionary<string, PageMetaInfo>();

            foreach (var n in notebookList)
            {
                var notebookInfo = new NotebookMetaInfo(n);

                var page4book = n.Descendants(One + "Page");
                foreach (var nn in page4book)
                {
                    var pageInfo = new PageMetaInfo(nn);
                    notebookInfo.pageIDSet.Add(pageInfo.id);
                    pageMetaInfo.Add(pageInfo.id, pageInfo);
                }

                notebookMetaInfo.Add(notebookInfo.id, notebookInfo);
            }
        }

        /// <summary>
        /// Add or update index in lucene.
        /// </summary>
        /// <param name="useCache">Indicate use previous cache or do a clean index.</param>
        public void BuildIndex(bool useCache, string indexMode)
        {
            progressRate = 0;
            if (useCache)
            {
                AddIndexByTime(indexMode);
            }
            else
            {
                AddAllIndex(indexMode);
            }
        }

        /// <summary>
        /// Just update lucene index.
        /// </summary>
        private void AddIndexByTime(string indexMode)
        {
            GetUpdateIndexID(out HashSet<string> deletedID, out HashSet<string> updateID);
            if (isDebug) Console.WriteLine("Begining Index Process...");
            foreach (var id in deletedID)
            {
                if (isDebug) Console.WriteLine("Deleting: " + id);
            }
            AddIndexFromID(updateID, indexMode);
            var currentTime = String.Format("{0:u}", DateTime.UtcNow);
        }

        /// <summary>
        /// Add all index and invalidates cache.
        /// </summary>
        private void AddAllIndex(string indexMode)
        {
            HashSet<String> updateID = new HashSet<string>();
            foreach (var n in pageMetaInfo.Values)
            {
                updateID.Add(n.id);
            }
            AddIndexFromID(updateID, indexMode);
        }

        /// <summary>
        /// Update/Create current lucene index by paragraph id. Due to out-of-date
        /// </summary>
        /// <param name="updateID">An iterable contains all paragraph id should be updated</param>
        private void AddIndexFromID(HashSet<String> updateID, string indexMode)
        {
            isIndexing = true;
            totalCount = updateID.Count();
            if (totalCount == 0) progressRate = 1;
            else
            {
                foreach (var id in updateID)
                {
                    //if (isDebug) Console.WriteLine("Adding: " + id);
                    count += 1;
                    progressRate = count / totalCount;
                    try
                    {
                        IndexByDocument(id);
                    }
                    catch (System.Runtime.InteropServices.COMException e)
                    {
                        int code = e.HResult;
                        if (isDebug) Console.WriteLine("Exception: Error Code is " + code);
                    }
                }
            }
            isIndexing = false;
        }

        /// <summary>
        /// Create index by content of paragraph in page.
        /// </summary>
        /// <param name="pageID"></param>
        private void IndexByParagraph(string pageID)
        {
            string xmlString;
            oneNote.GetPageContent(pageID, out xmlString);

            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            currentPageTitle = GetPageTitle(pageID);
            currentNotebookTitle = GetPageNotebookTitle(pageID);
            if (isDebug) Console.WriteLine("\t" + des.Count() + " Paragraphs");

            if (des.Count() > 0)
            {
                foreach (var el in des)
                {
                    var paraID = el.Attribute("objectID").Value;
                    var text = GetTextFromNode(el);
                    if (text == null) continue;
                    var paragraphText = RemoveUnwantedTags(text);
                }
            }
        }

        public void IndexByParagraph2(Application oneNoteApp, string pageID)
        {
            string xmlString;
            oneNoteApp.GetPageContent(pageID, out xmlString);

            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            currentPageTitle = GetPageTitle(pageID);
            currentNotebookTitle = GetPageNotebookTitle(pageID);
            if (isDebug) Console.WriteLine("\t" + des.Count() + " Paragraphs");

            if (des.Count() > 0)
            {
                foreach (var el in des)
                {
                    var paraID = el.Attribute("objectID").Value;
                    var text = GetTextFromNode(el);
                    if (text == null) continue;
                    var paragraphText = RemoveUnwantedTags(text);
                }
            }
        }
        public Page IndexByDocument2(Application oneNoteApp, string pageID)
        {
            string xmlString;
            oneNoteApp.GetPageContent(pageID, out xmlString);
            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            StringBuilder sb = new StringBuilder("");

            return XMLDeserialize<Page>(xmlString);
            //foreach (var el in des)
            //{
            //    var text = GetTextFromNode(el);
            //    if (text == null) continue;
            //    var paragraphText = RemoveUnwantedTags(text);
            //    if (isDebug)
            //    {
            //        using (StreamWriter w = File.AppendText("log.txt"))
            //        {
            //            Log(paragraphText, w);
            //        }
            //    }
            //    sb.AppendLine(paragraphText);
            //}
        }
        public static T XMLDeserialize<T>(string input)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(new XmlTextReader(new StringReader(input)));
        }

        /// <summary>
        /// Create index by whole content in page.
        /// </summary>
        /// <param name="pageID"></param>
        private void IndexByDocument(string pageID)
        {
            string xmlString;
            oneNote.GetPageContent(pageID, out xmlString);
            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            StringBuilder sb = new StringBuilder("");

            foreach (var el in des)
            {
                var text = GetTextFromNode(el);
                if (text == null) continue;
                var paragraphText = RemoveUnwantedTags(text);
                if (isDebug)
                {
                    using (StreamWriter w = File.AppendText("log.txt"))
                    {
                        Log(paragraphText, w);
                    }
                }
                sb.AppendLine(paragraphText);
            }
        }

        private string GetTextFromNode(XElement el)
        {
            var textNode = el.Element(One + "T");
            string text;
            if (textNode == null)
            {
                var image = el.Element(One + "Image");
                if (image == null) return null;
                var altAttr = image.Attribute("alt");
                if (altAttr == null) return null;
                text = altAttr.Value;
            }
            else
            {
                text = textNode.Value;
            }
            return text.ToString();
        }


        /// <summary>
        /// Send page id(from index), open requested page.
        /// </summary>
        /// <param name="id"></param>
        public bool OpenPage(string pageID, string paraID = "NULL")
        {
            try
            {
                if (paraID == "NULL")
                {
                    oneNote.NavigateTo(pageID);
                }
                else
                {
                    oneNote.NavigateTo(pageID, paraID);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send page id(from index), get page title.
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns>page title</returns>
        public string GetPageTitle(string pageID)
        {
            if(pageMetaInfo.ContainsKey(pageID))
            {
                return pageMetaInfo[pageID].name;
            }
            else
            {
                return "Default Title";
            }
        }

        public string GetPageNotebookTitle(string pageID)
        {
            foreach(var notebook in notebookMetaInfo.Values)
            {
                if (notebook.pageIDSet.Contains(pageID)) return notebook.name;
            }
            return "Defaule Title";
        }

        /// <summary>
        /// Get a list of page id from current onenote content.
        /// </summary>
        /// <returns></returns>
        public HashSet<String> GetAllPageId()
        {
            return new HashSet<string>(pageMetaInfo.Keys.ToArray());
        }

        // If t1 is older than t2, return true, otherwise return false
        private bool CompareTimeByString(string t1, string t2)
        {
            return DateTime.Parse(t1) >= DateTime.Parse(t2);
        }

        public void GetUpdateIndexID(out HashSet<String> indexIDToDelete, out HashSet<String> indexIDToCreate)
        {
            indexIDToDelete = null;
            indexIDToCreate = null;
        }

        public void setIndexDirectory(string indexDir)
        {

        }

        public void Main()
        {
            string outputXML = System.IO.File.ReadAllText(@"test.xml");
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            StreamWriter sw = new StreamWriter("D:\\Sample.xml");
            sw.WriteLine(outputXML);
            sw.Close();
        }

        internal static string RemoveUnwantedTags(string data)
        {
            return data;
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine(logMessage);
        }
    }

    class BaseMetaInfo
    {
        public string name;
        public string id;
        public string lastModifiedTime;

        public BaseMetaInfo(XElement node)
        {
            name = node.Attribute("name").Value;
            id = node.Attribute("ID").Value;
            lastModifiedTime = node.Attribute("lastModifiedTime").Value;
        }
    }

    class NotebookMetaInfo: BaseMetaInfo
    {
        public HashSet<string> pageIDSet;

        public NotebookMetaInfo(XElement node): base(node)
        {
            pageIDSet = new HashSet<string>();
        }
    }

    class PageMetaInfo: BaseMetaInfo
    {
        public PageMetaInfo(XElement node): base(node) { }
    }


}
