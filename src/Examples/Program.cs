using System;

using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;
using OpenWeb.Core.SearchDomains;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new GoogleSearchDomain();
            var extractor = new NlpFrequencyExtractor();

            string ws = new WebSearch(client, extractor)
                .Invoke("who is bob ross?")
                .SelectHigestScoredSentence();

            Console.WriteLine(ws);
            Console.Read();
        }
    }
}
