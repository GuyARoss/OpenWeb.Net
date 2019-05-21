using System.Collections.Generic;
using System.Linq;

namespace OpenWeb.Core
{
    public static class WebSearchExtensions
    {
		public static string SelectHigestScoredSentence(this Dictionary<string, double> scoredStatements)
        {
            return scoredStatements
                .OrderBy(statement => statement.Value)
                .Reverse()
                .First()
                .Key;
        }

		public static double ScoreSentenceFromVectorizedKeywords(this string scoredStatement)
        {
            throw new System.NotImplementedException();
        }
    }
}
