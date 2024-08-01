using OneNotePageSearcher;
using OneSearch.Plugin.OneNote.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace OneSearch.Plugin.OneNote
{
    public class OneNotePlugin
    {
        public void Execute()
        {

            var app = new OneNoteApplication();
            var books = app.GetNotebooks();
            

            var content = app.GetPageContent("{856A3B3D-8FE0-4DAB-AC7D-3CBE9E97634E}{1}{E19553127889834409498820176487684344330369831}");


            var outlines = content.Items.Where(x => x is Outline).Cast<Outline>();

            var contents = string.Empty;
            foreach(var outline in outlines)
            {
                foreach (var child in outline.OEChildren)
                {
                    contents += GetText(child);
                }
            }

            Console.WriteLine(contents);

            var man = new OneNoteManager(false);
            var content2 = man.IndexByDocument2(app._interopApplication, "{856A3B3D-8FE0-4DAB-AC7D-3CBE9E97634E}{1}{E19553127889834409498820176487684344330369831}");

            foreach (var noteBook in books.Notebook)
            {
                Console.WriteLine(noteBook.name);
            }

        }


        private string GetText(OEChildren children)
        {
            StringBuilder sb = new StringBuilder("");
            var outlineElements = children.Items.Where(x => x is OE).Cast<OE>();

            foreach (var te in outlineElements)
            {
                var texts = te.Items.Where(x => x is TextRange).Cast<TextRange>().Select(x => x.Value);


                var newTexts = texts.Select(x => RemoveHtmlTags2(x));

                foreach(var text in newTexts)
                {
                    sb.AppendLine(text);
                }

                if (te.OEChildren == null) continue;

                foreach (var child in te.OEChildren)
                {
                    sb.AppendLine(GetText(child));
                }
                
            }

            return sb.ToString();
        }


        public static string RemoveHtmlTags2(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            XmlDocument doc = new XmlDocument();
            input2 = FixHtmlAttributes(input);
            doc.LoadXml($"<root>{input2}</root>");

            return ExtractText(doc);
            return ExtractText(doc);
        }
        static string FixHtmlAttributes(string html)
        {
            // 属性値を引用符で囲む
            string pattern = @"(\w+)=([^\s>]+)";
            string replacement = "$1=\"$2\"";
            return Regex.Replace(html, pattern, replacement);
        }

        static string ExtractText(XmlNode node)
        {
            if (node == null)
                return string.Empty;

            if (node.NodeType == XmlNodeType.Text)
                return node.Value;

            string text = string.Empty;

            foreach (XmlNode child in node.ChildNodes)
            {
                text += ExtractText(child);
            }

            return text;
        }


        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            //// 改行と不完全な属性値を修正
            //input = Regex.Replace(input, @"(?<=<[^>]*?)\r\n", "");

            //// HTMLタグを削除
            //return Regex.Replace(input, "<.*?>", string.Empty);

            

            // (?s)フラグを使用して改行を含む任意の文字列にマッチさせる
            string pattern = "(?s)<.*?>";
            var text2 = Regex.Replace(input, pattern, string.Empty);

            return System.Net.WebUtility.HtmlDecode(text2);
        }
    }
}
