using System.Collections.Generic;

namespace OpenWeb.Core
{
    public interface ISearchDomain
    {
        string GenerateRootUrl(string question);
        string ParseSearchUrl(string url);
    }
}
