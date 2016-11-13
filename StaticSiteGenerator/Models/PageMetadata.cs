namespace StaticSiteGenerator.Models
{
    internal class PageMetadata : IPageMetadata
    {
        public LayoutType LayoutType { get; set; }
        public string Title { get; set; }
        public string Markdown { get; set; }
        public bool IsHeaderPage { get; set; }
    }
}
