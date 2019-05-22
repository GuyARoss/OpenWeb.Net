using System;
using System.Linq;
using System.Collections.Generic;

using Summarizer.Core;

using OpenWeb.Core.Utilities;

namespace OpenWeb.Core
{
    public class WebSearch
    {
        private static int _maxLinks = 2;
        private static int _maxParagraphs = 2;

        public readonly SearchSettingsType SearchSettings;
        public readonly ISearchDomain SearchDomain;
        public readonly IKeywordExtractor KeywordExtractor;

        public WebSearch(ISearchDomain searchDomain, IKeywordExtractor keywordExtractor)
        {
            SearchDomain = searchDomain;
            KeywordExtractor = keywordExtractor;

            SearchSettings.MaxLinks = _maxLinks;
            SearchSettings.MaxParagraphs = _maxParagraphs;
        }

        public WebSearch(ISearchDomain searchDomain, IKeywordExtractor keywordExtractor, SearchSettingsType settings) : this(searchDomain, keywordExtractor)
        {
            SearchSettings = settings;            
        }

        
        /// <summary>
        /// Invokes the vectorization/ web search with a question.
        /// </summary>
        /// <param name="question">Question being asked</param>
        /// <returns>Vectorized answers</returns>
        public Dictionary<string, double> Invoke(string question)
        {
            string domainRootUrl = SearchDomain.GenerateRootUrl(question);
            var searchDomainDocument = new DocumentNodeSelector(domainRootUrl);

            var documentLinks = searchDomainDocument
                .FindAllLinks()
                .Select(link => SearchDomain.ParseSearchUrl(link))
                .Where(link => link != null)
                .Distinct()
                .ToList();

            documentLinks  // resize the links
                .RemoveRange(SearchSettings.MaxLinks, documentLinks.Count - SearchSettings.MaxLinks);

            var originalKeywords = WebSearchHelper.SplitSenteceToKeywords(question);

            var paragraphs = _generateParagraphsFromLinks(documentLinks);
            var scoredKeywords = _createScoredKeywords(paragraphs, originalKeywords);

            return _createScoredStatements(paragraphs, scoredKeywords);
        }

        public IEnumerable<string> _generateParagraphsFromLinks(IEnumerable<string> documentLinks)
        {
            var siteData = new List<string>();

            foreach (string documentLink in documentLinks)
            {
                var document = new DocumentNodeSelector(documentLink);

                if (!document.IsDocumentValid) continue;

                try
                {
                    var paragraphs = document
                            .FindAllParagraphs()
                            .Where(paragraph => paragraph != null)
                            .ToList();
                    paragraphs.RemoveRange(SearchSettings.MaxParagraphs, paragraphs.Count - SearchSettings.MaxParagraphs);

                    siteData.AddRange(paragraphs);
                }
                catch (ArgumentNullException)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return siteData;
        }
        private Dictionary<string, double> _createScoredKeywords(IEnumerable<string> siteParagraphs, List<string> originalKeywords)
        {
            var scoredKeywords = new Dictionary<string, double>();

            var paragraphKeywords = _generateKeywordsFromParagraph(siteParagraphs);

            foreach (var keyword in paragraphKeywords)
            {
                var lowerKeyword = keyword.Key.ToLower();

                double score = (scoredKeywords.ContainsKey(keyword.Key)) ? keyword.Value + scoredKeywords[keyword.Key] : keyword.Value;
                
                if (originalKeywords.Contains(lowerKeyword))
                {
                    score *= 1.3;
                }
                else
                {
                    score -= (score / 5);
                }

                if (scoredKeywords.ContainsKey(lowerKeyword))
                {
                    scoredKeywords[lowerKeyword] = score;
                }
                else
                {
                    scoredKeywords.Add(lowerKeyword, score);
                }
            }

            return scoredKeywords;
        }
        private Dictionary<string, double> _createScoredStatements(IEnumerable<string> paragraphs, Dictionary<string, double> keywords)
        {
            var statements = new Dictionary<string, double>();

            foreach (string paragraph in paragraphs)
            {
                double paragraphScore = 0.0;
                string[] words = paragraph.Split(' ');

                if (string.IsNullOrWhiteSpace(paragraph) || string.IsNullOrEmpty(paragraph)) continue;

                foreach (string word in words)
                {
                    if (keywords.ContainsKey(word.ToLower()))
                    {
                        paragraphScore += keywords[word.ToLower()];
                    }
                }

                if (statements.ContainsKey(paragraph))
                {
                    statements[paragraph] += paragraphScore;
                }
                else
                {
                    statements.Add(paragraph, paragraphScore);
                }
            }

            return statements;
        }
        private Dictionary<string, double> _generateKeywordsFromParagraph(IEnumerable<string> paragraphs) 
        {
            if (paragraphs != null)
            {
                string joinedParagraph = string.Join(" ", paragraphs);
                return KeywordExtractor.Invoke(joinedParagraph);
            }

            return new Dictionary<string, double>();
        }        
    }
}
