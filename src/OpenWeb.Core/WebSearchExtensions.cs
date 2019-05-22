using System.Linq;
using System.Collections.Generic;

namespace OpenWeb.Core
{
    public static class WebSearchExtensions
    {
        /// <summary>
        /// Selects the head item from a set of paragraphs
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static string SelectHead(this IEnumerable<KeyValuePair<string, double>> paragraphs)
        {
            return paragraphs
                .Select(x => x.Key)
				.First();
        }

        /// <summary>
        /// Orders a set of paragraphs based on highest values.
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, double>> OrderByHighest(this Dictionary<string, double> paragraphs)
        {
            return paragraphs
                .OrderBy(statement => statement.Value)
                .Reverse();
        }

        /// <summary>
        /// Orders a set of paragraphs based on lowest values.
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, double>> OrderByLowest(this Dictionary<string, double> paragraphs)
        {
            return paragraphs
                .OrderBy(statement => statement.Value);                
        }

        /// <summary>
        /// Generates a correctness score based on a set of keywords.
        /// </summary>
        /// <param name="scoredStatements"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
		public static double ScoreFromKeywords(this string answer, IEnumerable<string> keywords)
        {
            var answerKeywords = WebSearchHelper.SplitSenteceToKeywords(answer);

            double amountCorrect = 0;
            foreach(string keyword in keywords)
            {
                if (answerKeywords.Contains(keyword))
                {
                    amountCorrect++;
                }
            }

            return (amountCorrect / keywords.Count());
        }
    }
}
