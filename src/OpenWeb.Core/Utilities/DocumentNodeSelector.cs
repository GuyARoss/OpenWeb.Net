using System.Linq;
using System.Collections.Generic;

using HtmlAgilityPack;

namespace OpenWeb.Core.Utilities
{
    public class DocumentNodeSelector
    {        
        public readonly HtmlDocument LoadedDocument;
        public readonly string WebUrl;
        public readonly bool IsDocumentValid;

        private static HtmlWeb _agilityWeb = new HtmlWeb(); 

        public DocumentNodeSelector(string webUrl)
        {
            if (webUrl.StartsWith("http"))
            {
                LoadedDocument = _agilityWeb.Load(webUrl);
                WebUrl = webUrl;              

                IsDocumentValid = LoadedDocument.DocumentNode.FirstChild != null; // we don't care to use the loaded doc if it doesn't have a first child.
            }
            else
            {
                IsDocumentValid = false;
            }
        }

        public IEnumerable<string> FindAllLinks()
        {
            return LoadedDocument.DocumentNode.SelectNodes("//*//a")
                ?.Select(node => node.Attributes["href"].Value);
        }

        public IEnumerable<string> FindAllParagraphs()
        {
            return LoadedDocument.DocumentNode.SelectNodes("//*//p")
                ?.Select(node => node.InnerText);
        }
    }
}
