using HtmlAgilityPack;
using OneNotePageSearcher;
using OneSearch.Extensibility.Core.Data;
using OneSearch.Extensibility.Core.Log;
using OneSearch.Extensibility.Core.Services;
using OneSearch.Plugin.OneNote.Data;
using OneSearch.Plugin.OneNote.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace OneSearch.Plugin.OneNote
{
    public static class OneNotePluginExtensions
    {
        public static void AddOneNotePlugin(this IServiceCollection collection)
        {
            collection.AddSingleton<IOneNotePlugin, OneNotePlugin>();
            collection.AddDataSection<AppSettings, OneNotePluginSettings>();
        }
    }

    internal class OneNotePlugin : IOneNotePlugin
    {
        private ITraceLogger<OneNotePlugin> _logger;
        private ITraceLogger _log2;
        private IDataSection<OneNotePluginSettings> _option;

        public string Name => "OneSearch.OneNotePlugin";

        // public OneNotePlugin(ITraceLogger<OneNotePlugin> logger, ITraceLoggerFactory fact, IDataSection<OneNotePluginSettings> option)
        public OneNotePlugin(ITraceLogger<OneNotePlugin> logger, ITraceLoggerFactory fact, IDataSection<OneNotePluginSettings> option)
        {
            _logger = logger;
            _log2 = fact.CreateLogger("TEST");
            _option = option;
        }

        public void Execute()
        {

            var app = new OneNoteApplication();
            var books = app.GetNotebooks();
            

            var content = app.GetPageContent("{856A3B3D-8FE0-4DAB-AC7D-3CBE9E97634E}{1}{E19553127889834409498820176487684344330369831}");


            var outlines = content.Items.Where(x => x is Outline).Cast<Outline>();

            var sw = new Stopwatch();
            sw.Start();

            var contents = string.Empty;
            foreach(var outline in outlines)
            {
                foreach (var child in outline.OEChildren)
                {
                    contents += GetText(child);
                }
            }

            sw.Stop();

            Console.WriteLine(contents);
            Console.WriteLine("elapsed : " + sw.ElapsedMilliseconds + " ms");

            var man = new OneNoteManager(false);
            var content2 = man.IndexByDocument2(app._interopApplication, "{856A3B3D-8FE0-4DAB-AC7D-3CBE9E97634E}{1}{E19553127889834409498820176487684344330369831}");

            foreach (var noteBook in books.Notebook)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(noteBook.name);
            }

            _logger.Log(TraceLogLevel.Debug, "aaaaaaaaaa");
            _log2.Log(TraceLogLevel.Debug, "ADSDASD");


            _logger.Log(TraceLogLevel.Debug, _option.Value.Name);

            _option.Value.Name = "newName";
            _option.Save();


        }


        private string GetText(OEChildren children)
        {
            StringBuilder sb = new StringBuilder("");
            var outlineElements = children.Items.Where(x => x is OE).Cast<OE>();

            foreach (var te in outlineElements)
            {
                var texts = te.Items.Where(x => x is TextRange).Cast<TextRange>().Select(x => x.Value);


                var newTexts = texts.Select(x => RemoveHtmlTags(x));

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

            input = input.Replace("&nbsp;", " ");
            var document = new HtmlDocument();
            document.LoadHtml(input);

            var acceptableTags = new String[] { };
            try
            {
                var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    var parentNode = node.ParentNode;

                    if (acceptableTags.Contains(node.Name) || node.Name == "#text") continue;
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);
                }

                return System.Net.WebUtility.HtmlDecode(document.DocumentNode.InnerHtml);
            }
            // Some text is unable to be parsed by htmlpack
            catch (ArgumentNullException)
            {
                return input;
            }
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

            // (?s)フラグを使用して改行を含む任意の文字列にマッチさせる
            string pattern = "(?s)<.*?>";
            var text2 = Regex.Replace(input, pattern, string.Empty);

            return System.Net.WebUtility.HtmlDecode(text2);
        }

        public void Initialize()
        {
            Execute();
        }

        public void Shutdown()
        {
            Console.WriteLine("OneSearchPlugin.finished");
        }
    }
}
