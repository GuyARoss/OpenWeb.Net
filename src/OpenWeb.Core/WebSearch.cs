using System.Linq;
using System.Collections.Generic;

using Summarizer.Core;

using OpenWeb.Core.Utilities;
using System;

namespace OpenWeb.Core
{
    public class WebSearch
    {
        public readonly ISearchDomain SearchSettings;
        public readonly IKeywordExtractor KeywordExtractor;

        public WebSearch(ISearchDomain searchSettings, IKeywordExtractor keywordExtractor)
        {
            SearchSettings = searchSettings;
            KeywordExtractor = keywordExtractor;
        }

        /// <summary>
        /// Invokes the vectorization/ web search with a question.
        /// </summary>
        /// <param name="question">Question being asked</param>
        /// <returns>Vectorized answers</returns>
        public Dictionary<string, double> Invoke(string question)
        {
            string domainRootUrl = SearchSettings.GenerateRootUrl(question);
            var searchDomainDocument = new DocumentNodeSelector(domainRootUrl);

            var documentLinks = searchDomainDocument
                .FindAllLinks()
                .Select(link => SearchSettings.ParseSearchUrl(link))
                .Where(link => link != null)
                .Distinct()
                .ToList();

            var scoredKeywords = _createScoredKeywordsFromLinks(documentLinks);
            
            foreach (string questionWord in SummarizationHelper.ConvertStatementToSentences(question))
            {
                if (scoredKeywords.ContainsKey(questionWord)) scoredKeywords[questionWord] *= 2;               
                else scoredKeywords.Add(questionWord, 100);
            }

            return _createScoredStatements(documentLinks, scoredKeywords);
        }       

        private Dictionary<string, double> _createScoredKeywordsFromLinks(IEnumerable<string> documentLinks)
        {
            var keywords = new Dictionary<string, double>();
            foreach (string documentLink in documentLinks)
            {
                var document = new DocumentNodeSelector(documentLink);

                if (!document.IsDocumentValid) continue;
                
                var paragraphs = document
                    .FindAllParagraphs()
                    .Where(paragraph => paragraph != null)
                    .ToList();

                var paragraphKeywords = _generateKeywordsFromParagraph(paragraphs);

                foreach (var keyword in paragraphKeywords)
                {
                    if (string.IsNullOrWhiteSpace(keyword.Key) || string.IsNullOrEmpty(keyword.Key)) continue;

                    if (keywords.ContainsKey(keyword.Key))
                    {
                        keywords[keyword.Key] += keyword.Value;
                    }
                    else
                    {
                        keywords.Add(keyword.Key, keyword.Value);
                    }
                }
            }

            return keywords;
        }
        private Dictionary<string, double> _createScoredStatements(IEnumerable<string> documentLinks, Dictionary<string, double> keywords)
        {
            var statements = new Dictionary<string, double>();

            foreach (string link in documentLinks)
            {
                var paragraphs = new DocumentNodeSelector(link)                    
                    .FindAllParagraphs()                   
                    ?.ToList();

                if (paragraphs == null) continue;

                foreach (string paragraph in paragraphs)
                {
                    var sentences = SummarizationHelper.ConvertStatementToSentences(paragraph);

                    foreach (string sentence in sentences)
                    {
                        double sentenceScore = 0.0;
                        string[] words = paragraph.Split(' ');

                        if (string.IsNullOrWhiteSpace(sentence) || string.IsNullOrEmpty(sentence)) continue;

                        foreach (string word in words)
                        {
                            if (keywords.ContainsKey(word))
                            {
                                sentenceScore += keywords[word];
                            }
                        }

                        if (statements.ContainsKey(sentence))
                        {
                            statements[sentence] += sentenceScore;
                        }
                        else
                        {
                            statements.Add(sentence, sentenceScore);
                        }
                    }                    
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
