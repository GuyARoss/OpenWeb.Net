using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenWeb.Core
{
    public static class WebSearchHelper
    {
        public static List<string> SplitSenteceToKeywords(string sentence)
        {
            string cleanedQuestion = Regex.Replace(sentence , @"[^0-9A-Za-z ,]", "");

            return cleanedQuestion
                .Split(' ')
                .ToList();
        }
    }
}
