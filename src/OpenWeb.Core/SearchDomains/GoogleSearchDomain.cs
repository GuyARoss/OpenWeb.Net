using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenWeb.Core.SearchDomains
{
    public class GoogleSearchDomain : ISearchDomain
    {
        private readonly string _baseAddress = "https://www.google.com/search?q=";
        private readonly List<string> _badSubLinks = new List<string>
            {
                "google", "youtube"
            };

        public string GenerateRootUrl(string question)
        {
            string formattedQuestion = question.Replace(" ", "+");
            return string.Format("{0}{1}", _baseAddress, formattedQuestion);
        }
        public string ParseSearchUrl(string url)
        {
            if (url.Contains("http") && _isLinkValid(url))
            {
                string cleanLink = url.Split(new string[] { "&amp" }, StringSplitOptions.None)[0];
                cleanLink = cleanLink.Replace("/search?q=related:", "");
                return cleanLink.Replace("/url?q=", "");                
            }

            return null;
        }
        private bool _isLinkValid(string link)
        {
            return !_badSubLinks
                .Any(subLink => link.Contains(subLink));
        }
    }
}
