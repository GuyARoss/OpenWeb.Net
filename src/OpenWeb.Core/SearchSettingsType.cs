namespace OpenWeb.Core
{
    public struct SearchSettingsType
    {
        public int MaxLinks;
        public int MaxParagraphs;

        public SearchSettingsType(int maxLinks, int maxParagraphs)
        {
            MaxLinks = maxLinks;
            MaxParagraphs = maxParagraphs;
        }
    }
}
