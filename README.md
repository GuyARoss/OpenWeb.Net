## OpenWeb .Net
OpenWeb .Net is a Library that provides high-level access to finding answers to your questions on the web. That includes functionality for fact checking, as well a finding answers to more general questions.

Extracted for public use from [Project Orva](https://www.ross-cdn.com/orva).

## Installation

#### Basic Usage Installation
Import all of the OpenWeb Dlls and external dependencies from the [distribute directory](./distribute) into your .net project solution. 
- Summarizer.Core
- Summarizer.Infrastructure
- Newtonsoft.Json
- HtmlAgilityPack
- OpenNLP

#### Nlp Resources
Open Web utilities OpenNlp, to use, extract the `NLP` contents within the [project resources](./resources/NLP) directory and place it within a `nlp` directory within your project's bin.

## Examples

#### Searching for an answer
Utilizing the web for finding questions requires an instance of [ISearchDomain](#Available-Instances-of-ISearchDomain). As well as an instance of [IKeywordExtractor](https://github.com/GuyARoss/Summarizer.Net) from the Summarizer .Net project.

Additionally, the web search invocation returns a dictionary of ranked answers utilizing a variant of the [Page Rank Algorithm](https://en.wikipedia.org/wiki/PageRank).

```c#
using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;

// ...

string question; // some question

ISearchDomain searchDomain; // set search domain instance
IKeywordExtractor keywordExtractor; // set keyword extractor instance

var webSearch = new WebSearch(searchDomain, keywordExtractor);

Dictionary<string, double> rankedAnswers = webSearch.Invoke(question); // ranked keywords
```

### Extension Methods for Ranked Answers
OpenWeb provides high level extensions for the ranked answers returned from the web search invocation. 

#### Ordering by Highest Score

```c#
using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;

// ...
Dictionary<string, double> rankedAnswers = webSearch.Invoke(question); // ranked keywords

Dictionary<string, double> orderedAnswers = rankedAnswers.OrderByHighest();
```

#### Ordering by Lowest Score

```c#
using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;

// ...
Dictionary<string, double> rankedAnswers = webSearch.Invoke(question); // ranked keywords

Dictionary<string, double> orderedAnswers = rankedAnswers.OrderByLowest();
```

#### Selecting the Top Answer

```c#
using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;

// ...
Dictionary<string, double> rankedAnswers = webSearch.Invoke(question); // ranked keywords

string topAnswer = rankedAnswers.SelectHead();
```

#### Compare Against Keyword Set
Implications of the comparison includes fact checking an already defined answer against the answer that would be provided from the web search.

```c#
using Summarizer.Core.KeywordExtractors;

using OpenWeb.Core;

// ...
Dictionary<string, double> rankedAnswers = webSearch.Invoke(question); // ranked keywords

IEnumerable<string> keywords; // keyword data set to compare against
Dictionary<string, double> orderedAnswers = rankedAnswers.ScoreFromKeyword(keywords);
```

#### Available Instances of `ISearchDomain`
Any instance of `ISearchDomain` dictates from what search engine the initial question query will originate from.

- `GoogleSearchDomain`


## Contributing
Feel free to contribute by opening a Pull Request or an issue thread.

Contributions are always appreciated! 

## License 
OpenWeb .Net is [MIT licensed](./LICENSE)