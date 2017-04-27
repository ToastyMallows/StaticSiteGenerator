namespace StaticSiteGenerator.Models
{
    public interface IPageMetadata : IBasePageMetadata
    {
        string Title { get; set; }

        bool IsHeaderPage { get; set; }
    }
}
